
using FluentValidation;
using Library.Application.DTOs;

namespace Library.Application.Validators;

public class UpdateBookRequestValidator
    : AbstractValidator<UpdateBookRequest>
{
    public UpdateBookRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Title is required.")
            .MaximumLength(200)
            .WithMessage("Title cannot exceed 200 characters.");

        RuleFor(x => x.Isbn)
            .NotEmpty()
            .WithMessage("ISBN is required.")
            .Matches(@"^\d{13}$")
            .WithMessage("ISBN must contain exactly 13 digits.");

        RuleFor(x => x.PublishedYear)
            .Must(year => year >= 1450 &&
                          year <= DateTime.UtcNow.Year)
            .WithMessage("Published year must be between 1450 and the current year.");

        RuleFor(x => x.AuthorId)
            .GreaterThan(0)
            .WithMessage("Author ID must be greater than 0.");
    }
}
