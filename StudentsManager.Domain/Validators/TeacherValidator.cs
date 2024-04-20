using FluentValidation;
using StudentsManager.Domain.Models;

namespace StudentsManager.Domain.Validators;

public class TeacherValidator : AbstractValidator<Teacher>
{
    public TeacherValidator()
    {
        RuleFor(s => s.FirstName)
            .NotEmpty().WithMessage("Teacher first name is required.")
            .MaximumLength(100).WithMessage("Student name must not exceed 100 characters.");

        RuleFor(s => s.Age)
            .GreaterThan(0).WithMessage("Teacher age must be greater than zero.");

        RuleFor(s => s.PhoneNumber)
            .NotEmpty().NotNull().WithMessage("Teacher phone number must be provided.");
    }
}