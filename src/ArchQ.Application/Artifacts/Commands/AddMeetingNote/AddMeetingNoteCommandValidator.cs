using FluentValidation;

namespace ArchQ.Application.Artifacts.Commands.AddMeetingNote;

public class AddMeetingNoteCommandValidator : AbstractValidator<AddMeetingNoteCommand>
{
    public AddMeetingNoteCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(200).WithMessage("Title must not exceed 200 characters.");

        RuleFor(x => x.MeetingDate)
            .NotEmpty().WithMessage("Meeting date is required.");

        RuleFor(x => x.Attendees)
            .NotEmpty().WithMessage("At least one attendee is required.");

        RuleFor(x => x.Body)
            .NotEmpty().WithMessage("Body is required.")
            .MaximumLength(50000).WithMessage("Body must not exceed 50000 characters.");
    }
}
