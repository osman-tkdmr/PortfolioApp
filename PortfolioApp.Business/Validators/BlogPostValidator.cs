using FluentValidation;
using PortfolioApp.DTO.DTOs.Blog;

namespace PortfolioApp.Business.Validators;

public class BlogPostCreateValidator : AbstractValidator<BlogPostCreateDto>
{
    public BlogPostCreateValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Başlık zorunludur.")
            .MaximumLength(250).WithMessage("Başlık en fazla 250 karakter olabilir.");

        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("İçerik zorunludur.");

        RuleFor(x => x.BlogCategoryId)
            .GreaterThan(0).WithMessage("Kategori seçimi zorunludur.");

        RuleFor(x => x.Summary)
            .MaximumLength(500).WithMessage("Özet en fazla 500 karakter olabilir.")
            .When(x => !string.IsNullOrEmpty(x.Summary));
    }
}

public class BlogPostUpdateValidator : AbstractValidator<BlogPostUpdateDto>
{
    public BlogPostUpdateValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Başlık zorunludur.")
            .MaximumLength(250);
        RuleFor(x => x.Content).NotEmpty().WithMessage("İçerik zorunludur.");
        RuleFor(x => x.BlogCategoryId).GreaterThan(0).WithMessage("Kategori zorunludur.");
    }
}
