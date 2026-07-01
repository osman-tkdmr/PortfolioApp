using FluentValidation;
using PortfolioApp.DTO.DTOs.Project;

namespace PortfolioApp.Business.Validators;

public class ProjectCreateValidator : AbstractValidator<ProjectCreateDto>
{
    public ProjectCreateValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Proje adı zorunludur.")
            .MaximumLength(250).WithMessage("Proje adı en fazla 250 karakter olabilir.");

        RuleFor(x => x.ProjectCategoryId)
            .GreaterThan(0).WithMessage("Kategori seçimi zorunludur.");

        RuleFor(x => x.DemoUrl)
            .Must(url => Uri.TryCreate(url, UriKind.Absolute, out _))
            .WithMessage("Geçerli bir URL giriniz.")
            .When(x => !string.IsNullOrEmpty(x.DemoUrl));

        RuleFor(x => x.SourceUrl)
            .Must(url => Uri.TryCreate(url, UriKind.Absolute, out _))
            .WithMessage("Geçerli bir URL giriniz.")
            .When(x => !string.IsNullOrEmpty(x.SourceUrl));
    }
}
