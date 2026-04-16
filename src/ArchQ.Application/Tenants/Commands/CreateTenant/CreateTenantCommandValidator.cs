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
            .MinimumLength(3).WithMessage("Slug must be at least 3 characters.")
            .MaximumLength(63).WithMessage("Slug must not exceed 63 characters.")
            .Matches(@"^[a-z0-9][a-z0-9-]{1,61}[a-z0-9]$")
                .WithMessage("Slug must start and end with a lowercase alphanumeric character and contain only lowercase alphanumeric characters and hyphens.")
            .Must(slug => !ReservedSlugs.Contains(slug))
                .WithMessage("The specified slug is reserved and cannot be used.");
    }
}
