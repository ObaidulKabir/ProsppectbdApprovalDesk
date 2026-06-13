using FluentValidation;
using ProspectbdApprovalDesk.Application.Users.Dto;

namespace ProspectbdApprovalDesk.Application.Users.Validation;

public sealed class ResetUserPasswordRequestValidator : AbstractValidator<ResetUserPasswordRequest>
{
    public ResetUserPasswordRequestValidator()
    {
        RuleFor(x => x.NewPassword).NotEmpty().MinimumLength(8).MaximumLength(128);
    }
}

