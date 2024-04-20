using FluentValidation;
using StudentsManager.Domain.Models;

namespace StudentsManager.Domain.Validators;

public class ClassGroupValidator : AbstractValidator<ClassGroup>
{
    public ClassGroupValidator()
    {
        RuleFor(cg => cg.Name)
            .NotEmpty().WithMessage("Class group name is required.")
            .MaximumLength(50).WithMessage("Class group name must not exceed 50 characters.");

        RuleFor(cg => cg.TeacherId)
            .NotEmpty().WithMessage("A teacher must be assigned to the class group.");

        RuleForEach(cg => cg.Students)
            .NotNull().WithMessage("Student details must be provided.")
            .SetValidator(new StudentValidator());

        RuleForEach(cg => cg.LessonPlans)
            .SetValidator(new LessonPlanValidator());

        RuleFor(cg => cg.Teacher)
            .NotNull().WithMessage("Teacher details must be provided.")
            .SetValidator(new TeacherValidator());
    }
}