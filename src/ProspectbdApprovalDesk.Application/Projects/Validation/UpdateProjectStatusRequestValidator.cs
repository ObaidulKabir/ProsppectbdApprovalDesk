using FluentValidation;
using ProspectbdApprovalDesk.Application.Projects.Dto;

namespace ProspectbdApprovalDesk.Application.Projects.Validation;

public sealed class UpdateProjectStatusRequestValidator : AbstractValidator<UpdateProjectStatusRequest>
{
    public UpdateProjectStatusRequestValidator()
    {
        RuleFor(x => x.SubmissionDate).NotNull().When(x => x.Status is Domain.Enums.ProjectStatus.ApplicationSubmitted);
        RuleFor(x => x.ApprovalDate).NotNull().When(x => x.Status is Domain.Enums.ProjectStatus.Approved or Domain.Enums.ProjectStatus.Rejected);
    }
}

