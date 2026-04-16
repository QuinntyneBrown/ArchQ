using FluentValidation;

namespace ArchQ.Application.Artifacts.Commands.AddComment;

public class AddCommentCommandValidator : AbstractValidator<AddCommentCommand>
{
    public AddCommentCommandValidator()
    {
        RuleFor(x => x.Body)
            .NotEmpty().WithMessage("Comment body is required.")
            .MaximumLength(10000).WithMessage("Comment body must not exceed 10000 characters.");
    }
}
