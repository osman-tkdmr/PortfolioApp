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
            .Must(IsHttpUrl)
            .WithMessage("Geçerli bir URL giriniz.")
            .When(x => !string.IsNullOrEmpty(x.DemoUrl));

        RuleFor(x => x.SourceUrl)
            .Must(IsHttpUrl)
            .WithMessage("Geçerli bir URL giriniz.")
            .When(x => !string.IsNullOrEmpty(x.SourceUrl));
    }

    // Restrict to http(s) so a value like "javascript:..." can't be stored and later
    // rendered straight into an href attribute on the public project detail page.
    private static bool IsHttpUrl(string? url) =>
        Uri.TryCreate(url, UriKind.Absolute, out var uri) &&
        (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
}
