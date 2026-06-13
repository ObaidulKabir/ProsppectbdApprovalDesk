using FluentValidation;
using ProspectbdApprovalDesk.Application.Projects.Dto;

namespace ProspectbdApprovalDesk.Application.Projects.Validation;

public sealed class UpdateProjectCredentialsRequestValidator : AbstractValidator<UpdateProjectCredentialsRequest>
{
    public UpdateProjectCredentialsRequestValidator()
    {
        RuleFor(x => x.EmailPassword).MaximumLength(200);
        RuleFor(x => x.EcpsPassword).MaximumLength(200);
    }
}

