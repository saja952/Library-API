
using FluentValidation;
using Library.Application.DTOs;

namespace Library.Application.Validators;

public class CreateAuthorRequestValidator
    : AbstractValidator<CreateAuthorRequest>
{
    public CreateAuthorRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Author name is required.")
            .MaximumLength(100)
            .WithMessage("Author name cannot exceed 100 characters.");
    }
}
