using FluentValidation;

namespace ArchQ.Application.Adrs.Commands.CreateAdr;

public class CreateAdrCommandValidator : AbstractValidator<CreateAdrCommand>
{
    public CreateAdrCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(200).WithMessage("Title must not exceed 200 characters.");

        RuleFor(x => x.Body)
            .NotEmpty().WithMessage("Body is required.")
            .MaximumLength(50000).WithMessage("Body must not exceed 50000 characters.");

        RuleFor(x => x.Tags)
            .Must(tags => tags.Count <= 20)
            .WithMessage("A maximum of 20 tags is allowed.");

        RuleForEach(x => x.Tags)
            .MaximumLength(50).WithMessage("Each tag must not exceed 50 characters.");
    }
}
