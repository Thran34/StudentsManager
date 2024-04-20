using FluentValidation;
using StudentsManager.Domain.Models;

namespace StudentsManager.Domain.Validators;

public class StudentValidator : AbstractValidator<Student>
{
    public StudentValidator()
    {
        RuleFor(s => s.FirstName)
            .NotEmpty().WithMessage("Student first name is required.")
            .MaximumLength(100).WithMessage("Student name must not exceed 100 characters.");

        RuleFor(s => s.Age)
            .GreaterThan(0).WithMessage("Student age must be greater than zero.");

        RuleFor(s => s.PhoneNumber)
            .NotEmpty().NotNull().WithMessage("Student phone number must be provided.");
    }
}