using FluentValidation;
using ProspectbdApprovalDesk.Application.Authentication.Dto;

namespace ProspectbdApprovalDesk.Application.Authentication.Validation;

public sealed class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(256);
        RuleFor(x => x.Password).NotEmpty().MinimumLength(6).MaximumLength(128);
    }
}

