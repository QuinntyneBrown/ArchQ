using FluentValidation;

namespace ArchQ.Application.Tenants.Commands.CreateTenant;

public class CreateTenantCommandValidator : AbstractValidator<CreateTenantCommand>
{
    private static readonly string[] ReservedSlugs =
        ["_system", "_default", "admin", "api", "www", "app"];

    public CreateTenantCommandValidator()
    {
        RuleFor(x => x.DisplayName)
            .NotEmpty().WithMessage("Display name is required.")
            .MaximumLength(200).WithMessage("Display name must not exceed 200 characters.");

        RuleFor(x => x.Slug)
            .NotEmpty().WithMessage("Slug is required.")
            .MinimumLength(3).WithMessage("Slug must be 3-63 lowercase alphanumeric characters or hyphens.")
            .MaximumLength(63).WithMessage("Slug must be 3-63 lowercase alphanumeric characters or hyphens.")
            .Matches(@"^[a-z0-9][a-z0-9-]{1,61}[a-z0-9]$")
                .WithMessage("Slug must be 3-63 lowercase alphanumeric characters or hyphens.")
            .Must(slug => !ReservedSlugs.Contains(slug))
                .WithMessage("The specified slug is reserved and cannot be used.");
    }
}
