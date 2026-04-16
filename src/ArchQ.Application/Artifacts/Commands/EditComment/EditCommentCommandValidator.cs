using FluentValidation;

namespace ArchQ.Application.Artifacts.Commands.EditComment;

public class EditCommentCommandValidator : AbstractValidator<EditCommentCommand>
{
    public EditCommentCommandValidator()
    {
        RuleFor(x => x.Body)
            .NotEmpty().WithMessage("Comment body is required.")
            .MaximumLength(10000).WithMessage("Comment body must not exceed 10000 characters.");
    }
}
