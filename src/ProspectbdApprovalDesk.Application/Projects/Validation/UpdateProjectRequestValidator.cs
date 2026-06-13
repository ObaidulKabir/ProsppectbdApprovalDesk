using FluentValidation;
using ProspectbdApprovalDesk.Application.Projects.Dto;

namespace ProspectbdApprovalDesk.Application.Projects.Validation;

public sealed class UpdateProjectRequestValidator : AbstractValidator<UpdateProjectRequest>
{
    public UpdateProjectRequestValidator()
    {
        RuleFor(x => x.ProjectName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.OwnerName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.ProjectArea).MaximumLength(200);
        RuleFor(x => x.ProjectLocation).MaximumLength(500);
        RuleFor(x => x.DriveLink).MaximumLength(2048).Must(BeValidAbsoluteUri).When(x => !string.IsNullOrWhiteSpace(x.DriveLink));
        RuleFor(x => x.ContactName).MaximumLength(200);
        RuleFor(x => x.ContactNumber).MaximumLength(30);
        RuleFor(x => x.Email).EmailAddress().When(x => !string.IsNullOrWhiteSpace(x.Email)).MaximumLength(256);
        RuleFor(x => x.EcpsAccountId).MaximumLength(100);
        RuleFor(x => x.EcpsApplicationId).MaximumLength(100);
        RuleFor(x => x.Notes).MaximumLength(4000);
    }

    private static bool BeValidAbsoluteUri(string? value) =>
        Uri.TryCreate(value, UriKind.Absolute, out _);
}

