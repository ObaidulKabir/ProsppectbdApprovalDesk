using FluentValidation;
using ProspectbdApprovalDesk.Application.Projects.Dto;

namespace ProspectbdApprovalDesk.Application.Projects.Validation;

public sealed class AssignProjectRequestValidator : AbstractValidator<AssignProjectRequest>
{
    public AssignProjectRequestValidator()
    {
        RuleFor(x => x.AssignedUserId).NotNull().When(x => x.AssignedUserId.HasValue);
    }
}

