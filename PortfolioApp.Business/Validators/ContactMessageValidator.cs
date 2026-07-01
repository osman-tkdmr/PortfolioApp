using FluentValidation;
using PortfolioApp.DTO.DTOs.Site;

namespace PortfolioApp.Business.Validators;

public class ContactMessageCreateValidator : AbstractValidator<ContactMessageCreateDto>
{
    public ContactMessageCreateValidator()
    {
        RuleFor(x => x.SenderName)
            .NotEmpty().WithMessage("Ad Soyad zorunludur.")
            .MaximumLength(100).WithMessage("Ad Soyad en fazla 100 karakter olabilir.");

        RuleFor(x => x.SenderEmail)
            .NotEmpty().WithMessage("E-posta zorunludur.")
            .EmailAddress().WithMessage("Geçerli bir e-posta adresi giriniz.");

        RuleFor(x => x.Subject)
            .NotEmpty().WithMessage("Konu zorunludur.")
            .MaximumLength(200).WithMessage("Konu en fazla 200 karakter olabilir.");

        RuleFor(x => x.Message)
            .NotEmpty().WithMessage("Mesaj zorunludur.")
            .MinimumLength(10).WithMessage("Mesaj en az 10 karakter olmalıdır.")
            .MaximumLength(2000).WithMessage("Mesaj en fazla 2000 karakter olabilir.");

        RuleFor(x => x.SenderPhone)
            .MaximumLength(20).WithMessage("Telefon numarası en fazla 20 karakter olabilir.")
            .When(x => !string.IsNullOrEmpty(x.SenderPhone));
    }
}
