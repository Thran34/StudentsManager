using FluentValidation;
using StudentsManager.Domain.Models;

namespace StudentsManager.Domain.Validators;

public class LessonPlanValidator : AbstractValidator<LessonPlan>
{
    public LessonPlanValidator()
    {
        RuleFor(s => s.Date)
            .NotNull().WithMessage("Lesson plan date is required");
        RuleFor(s => s.Description)
            .NotNull().WithMessage("Lesson plan description is required");
        RuleFor(s => s.Subject)
            .NotNull().WithMessage("Lesson plan subject is required");
    }
}