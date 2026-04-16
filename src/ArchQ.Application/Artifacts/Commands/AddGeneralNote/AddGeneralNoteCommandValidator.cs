using FluentValidation;

namespace ArchQ.Application.Artifacts.Commands.AddGeneralNote;

public class AddGeneralNoteCommandValidator : AbstractValidator<AddGeneralNoteCommand>
{
    public AddGeneralNoteCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(200).WithMessage("Title must not exceed 200 characters.");

        RuleFor(x => x.Body)
            .NotEmpty().WithMessage("Body is required.")
            .MaximumLength(50000).WithMessage("Body must not exceed 50000 characters.");
    }
}
