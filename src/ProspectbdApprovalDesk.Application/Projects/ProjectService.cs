using AutoMapper;
using FluentValidation;
using ProspectbdApprovalDesk.Application.ActivityLogs;
using ProspectbdApprovalDesk.Application.Common;
using ProspectbdApprovalDesk.Application.Exceptions;
using ProspectbdApprovalDesk.Application.Projects.Dto;
using ProspectbdApprovalDesk.Application.Security;
using ProspectbdApprovalDesk.Application.Users;
using ProspectbdApprovalDesk.Domain.Entities;
using ProspectbdApprovalDesk.Domain.Enums;

namespace ProspectbdApprovalDesk.Application.Projects;

public sealed class ProjectService : IProjectService
{
    private readonly IProjectRepository _projects;
    private readonly IUserRepository _users;
    private readonly IEncryptionService _encryption;
    private readonly ICurrentUser _currentUser;
    private readonly IActivityLogService _activity;
    private readonly IMapper _mapper;
    private readonly IValidator<CreateProjectRequest> _createValidator;
    private readonly IValidator<UpdateProjectRequest> _updateValidator;
    private readonly IValidator<AssignProjectRequest> _assignValidator;
    private readonly IValidator<UpdateProjectCredentialsRequest> _credentialsValidator;
    private readonly IValidator<UpdateProjectStatusRequest> _statusValidator;
    private readonly IValidator<UpdateProjectNotesRequest> _notesValidator;

    public ProjectService(
        IProjectRepository projects,
        IUserRepository users,
        IEncryptionService encryption,
        ICurrentUser currentUser,
        IActivityLogService activity,
        IMapper mapper,
        IValidator<CreateProjectRequest> createValidator,
        IValidator<UpdateProjectRequest> updateValidator,
        IValidator<AssignProjectRequest> assignValidator,
        IValidator<UpdateProjectCredentialsRequest> credentialsValidator,
        IValidator<UpdateProjectStatusRequest> statusValidator,
        IValidator<UpdateProjectNotesRequest> notesValidator)
    {
        _projects = projects;
        _users = users;
        _encryption = encryption;
        _currentUser = currentUser;
        _activity = activity;
        _mapper = mapper;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _assignValidator = assignValidator;
        _credentialsValidator = credentialsValidator;
        _statusValidator = statusValidator;
        _notesValidator = notesValidator;
    }

    public async Task<ProjectDto> GetByIdAsync(Guid id, CancellationToken ct)
    {
        var project = await _projects.GetByIdAsync(id, ct) ?? throw new NotFoundException("Project not found.");

        EnsureCanAccessProject(project);

        return ToProjectDto(project, includeSecrets: CanViewSecrets(project));
    }

    public async Task<PagedResult<ProjectListItemDto>> ListAsync(int page, int pageSize, string? search, Guid? assignedUserId, ProjectStatus? status, CancellationToken ct)
    {
        page = page < 1 ? 1 : page;
        pageSize = pageSize is < 1 or > 200 ? 20 : pageSize;

        var effectiveAssigned = assignedUserId;
        if (!IsAdmin())
            effectiveAssigned = _currentUser.UserId;

        var skip = (page - 1) * pageSize;

        var items = await _projects.ListAsync(skip, pageSize, search, effectiveAssigned, status, ct);
        var total = await _projects.CountAsync(search, effectiveAssigned, status, ct);

        return new PagedResult<ProjectListItemDto>
        {
            Items = items.Select(_mapper.Map<ProjectListItemDto>).ToArray(),
            Page = page,
            PageSize = pageSize,
            TotalCount = total
        };
    }

    public async Task<ProjectDto> CreateAsync(CreateProjectRequest request, CancellationToken ct)
    {
        await _createValidator.ValidateAndThrowAsync(request, ct);

        var project = new Project
        {
            Id = Guid.NewGuid(),
            ProjectCode = request.ProjectCode.Trim(),
            ProjectName = request.ProjectName.Trim(),
            OwnerName = request.OwnerName.Trim(),
            ProjectArea = request.ProjectArea?.Trim(),
            ProjectLocation = request.ProjectLocation?.Trim(),
            DriveLink = request.DriveLink?.Trim(),
            ContactName = request.ContactName?.Trim(),
            ContactNumber = request.ContactNumber?.Trim(),
            Email = request.Email?.Trim(),
            EmailPasswordEncrypted = string.IsNullOrWhiteSpace(request.EmailPassword) ? null : _encryption.EncryptToBase64(request.EmailPassword),
            EcpsAccountId = request.EcpsAccountId?.Trim(),
            EcpsPasswordEncrypted = string.IsNullOrWhiteSpace(request.EcpsPassword) ? null : _encryption.EncryptToBase64(request.EcpsPassword),
            EcpsApplicationId = request.EcpsApplicationId?.Trim(),
            Notes = request.Notes?.Trim(),
            Status = ProjectStatus.Draft,
            CreatedAt = DateTimeOffset.UtcNow
        };

        await _projects.AddAsync(project, ct);
        await _projects.SaveChangesAsync(ct);

        await _activity.LogAsync(_currentUser.UserId, "ProjectCreated", "Project", project.Id.ToString(), $"Created project {project.ProjectCode}.", ct);

        return ToProjectDto(project, includeSecrets: CanViewSecrets(project));
    }

    public async Task<ProjectDto> UpdateAsync(Guid id, UpdateProjectRequest request, CancellationToken ct)
    {
        await _updateValidator.ValidateAndThrowAsync(request, ct);

        var project = await _projects.GetByIdAsync(id, ct) ?? throw new NotFoundException("Project not found.");

        EnsureCanAccessProject(project);

        project.ProjectName = request.ProjectName.Trim();
        project.OwnerName = request.OwnerName.Trim();
        project.ProjectArea = request.ProjectArea?.Trim();
        project.ProjectLocation = request.ProjectLocation?.Trim();
        project.DriveLink = request.DriveLink?.Trim();
        project.ContactName = request.ContactName?.Trim();
        project.ContactNumber = request.ContactNumber?.Trim();
        project.Email = request.Email?.Trim();
        project.EcpsAccountId = request.EcpsAccountId?.Trim();
        project.EcpsApplicationId = request.EcpsApplicationId?.Trim();
        project.Notes = request.Notes?.Trim();
        project.UpdatedAt = DateTimeOffset.UtcNow;

        await _projects.SaveChangesAsync(ct);
        await _activity.LogAsync(_currentUser.UserId, "ProjectUpdated", "Project", project.Id.ToString(), $"Updated project {project.ProjectCode}.", ct);

        return ToProjectDto(project, includeSecrets: CanViewSecrets(project));
    }

    public async Task AssignAsync(Guid id, AssignProjectRequest request, CancellationToken ct)
    {
        await _assignValidator.ValidateAndThrowAsync(request, ct);

        if (!IsAdmin())
            throw new ForbiddenException("Only Admin can assign projects.");

        var project = await _projects.GetByIdAsync(id, ct) ?? throw new NotFoundException("Project not found.");

        if (request.AssignedUserId.HasValue)
        {
            var user = await _users.GetByIdAsync(request.AssignedUserId.Value, ct) ?? throw new NotFoundException("Assigned user not found.");
            if (!user.IsActive)
                throw new AppException("Assigned user is inactive.", 409);
        }

        project.AssignedUserId = request.AssignedUserId;
        project.UpdatedAt = DateTimeOffset.UtcNow;
        await _projects.SaveChangesAsync(ct);

        await _activity.LogAsync(_currentUser.UserId, "ProjectAssigned", "Project", project.Id.ToString(), $"Assigned project to {request.AssignedUserId}.", ct);
    }

    public async Task UpdateCredentialsAsync(Guid id, UpdateProjectCredentialsRequest request, CancellationToken ct)
    {
        await _credentialsValidator.ValidateAndThrowAsync(request, ct);

        var project = await _projects.GetByIdAsync(id, ct) ?? throw new NotFoundException("Project not found.");
        EnsureCanAccessProject(project);

        if (!CanViewSecrets(project))
            throw new ForbiddenException("Not authorized to update credentials.");

        if (request.EmailPassword is not null)
            project.EmailPasswordEncrypted = string.IsNullOrWhiteSpace(request.EmailPassword) ? null : _encryption.EncryptToBase64(request.EmailPassword);

        if (request.EcpsPassword is not null)
            project.EcpsPasswordEncrypted = string.IsNullOrWhiteSpace(request.EcpsPassword) ? null : _encryption.EncryptToBase64(request.EcpsPassword);

        project.UpdatedAt = DateTimeOffset.UtcNow;
        await _projects.SaveChangesAsync(ct);

        await _activity.LogAsync(_currentUser.UserId, "ProjectCredentialsUpdated", "Project", project.Id.ToString(), "Updated project credentials.", ct);
    }

    public async Task UpdateStatusAsync(Guid id, UpdateProjectStatusRequest request, CancellationToken ct)
    {
        await _statusValidator.ValidateAndThrowAsync(request, ct);

        var project = await _projects.GetByIdAsync(id, ct) ?? throw new NotFoundException("Project not found.");
        EnsureCanAccessProject(project);

        project.Status = request.Status;
        project.SubmissionDate = request.SubmissionDate;
        project.ApprovalDate = request.ApprovalDate;
        project.UpdatedAt = DateTimeOffset.UtcNow;

        await _projects.SaveChangesAsync(ct);

        await _activity.LogAsync(_currentUser.UserId, "ProjectStatusUpdated", "Project", project.Id.ToString(), $"Status changed to {request.Status}.", ct);
    }

    public async Task UpdateNotesAsync(Guid id, UpdateProjectNotesRequest request, CancellationToken ct)
    {
        await _notesValidator.ValidateAndThrowAsync(request, ct);

        var project = await _projects.GetByIdAsync(id, ct) ?? throw new NotFoundException("Project not found.");
        EnsureCanAccessProject(project);

        project.Notes = request.Notes?.Trim();
        project.UpdatedAt = DateTimeOffset.UtcNow;
        await _projects.SaveChangesAsync(ct);

        await _activity.LogAsync(_currentUser.UserId, "ProjectNotesUpdated", "Project", project.Id.ToString(), "Updated notes.", ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct)
    {
        if (!IsAdmin())
            throw new ForbiddenException("Only Admin can delete projects.");

        var project = await _projects.GetByIdAsync(id, ct) ?? throw new NotFoundException("Project not found.");
        await _projects.DeleteAsync(project, ct);
        await _projects.SaveChangesAsync(ct);

        await _activity.LogAsync(_currentUser.UserId, "ProjectDeleted", "Project", project.Id.ToString(), $"Deleted project {project.ProjectCode}.", ct);
    }

    private bool IsAdmin() => _currentUser.Role == UserRole.Admin;

    private void EnsureCanAccessProject(Project project)
    {
        if (IsAdmin()) return;
        if (!_currentUser.UserId.HasValue) throw new UnauthorizedException("Not authenticated.");
        if (project.AssignedUserId != _currentUser.UserId) throw new ForbiddenException("Not authorized to access this project.");
    }

    private bool CanViewSecrets(Project project)
    {
        if (IsAdmin()) return true;
        return _currentUser.UserId.HasValue && project.AssignedUserId == _currentUser.UserId;
    }

    private ProjectDto ToProjectDto(Project project, bool includeSecrets)
    {
        return new ProjectDto(
            project.Id,
            project.ProjectCode,
            project.ProjectName,
            project.OwnerName,
            project.ProjectArea,
            project.ProjectLocation,
            project.DriveLink,
            project.ContactName,
            project.ContactNumber,
            project.Email,
            includeSecrets && project.EmailPasswordEncrypted is not null ? _encryption.DecryptFromBase64(project.EmailPasswordEncrypted) : null,
            project.EcpsAccountId,
            includeSecrets && project.EcpsPasswordEncrypted is not null ? _encryption.DecryptFromBase64(project.EcpsPasswordEncrypted) : null,
            project.EcpsApplicationId,
            project.AssignedUserId,
            project.Status,
            project.SubmissionDate,
            project.ApprovalDate,
            project.Notes,
            project.CreatedAt,
            project.UpdatedAt);
    }
}

