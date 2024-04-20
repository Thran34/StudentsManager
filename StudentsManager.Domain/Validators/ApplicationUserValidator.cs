using FluentValidation;
using StudentsManager.Domain.Models;

namespace StudentsManager.Domain.Validators;

public class ApplicationUserValidator : AbstractValidator<ApplicationUser>
{
    public ApplicationUserValidator()
    {
        RuleFor(s => s.FirstName)
            .NotEmpty().WithMessage("Application user first name is required.")
            .MaximumLength(100).WithMessage("Student name must not exceed 100 characters.");

        RuleFor(s => s.LastName)
            .NotEmpty().WithMessage("Application user last name is required.")
            .MaximumLength(100).WithMessage("Student name must not exceed 100 characters.");

        RuleFor(s => s.PhoneNumber)
            .NotEmpty().NotNull().WithMessage("Application user phone number must be provided.");
    }
}