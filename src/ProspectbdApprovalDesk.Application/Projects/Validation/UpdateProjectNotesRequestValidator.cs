using FluentValidation;
using ProspectbdApprovalDesk.Application.Projects.Dto;

namespace ProspectbdApprovalDesk.Application.Projects.Validation;

public sealed class UpdateProjectNotesRequestValidator : AbstractValidator<UpdateProjectNotesRequest>
{
    public UpdateProjectNotesRequestValidator()
    {
        RuleFor(x => x.Notes).MaximumLength(4000);
    }
}

