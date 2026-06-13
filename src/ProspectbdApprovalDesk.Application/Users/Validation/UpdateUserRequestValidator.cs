using FluentValidation;
using ProspectbdApprovalDesk.Application.Users.Dto;

namespace ProspectbdApprovalDesk.Application.Users.Validation;

public sealed class UpdateUserRequestValidator : AbstractValidator<UpdateUserRequest>
{
    public UpdateUserRequestValidator()
    {
        RuleFor(x => x.FullName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Phone).MaximumLength(30);
    }
}

