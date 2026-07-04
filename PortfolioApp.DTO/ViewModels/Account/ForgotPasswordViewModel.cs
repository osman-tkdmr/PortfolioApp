using System.ComponentModel.DataAnnotations;

namespace PortfolioApp.DTO.ViewModels.Account;

public class ForgotPasswordViewModel
{
    [Required(ErrorMessage = "E-posta zorunludur.")]
    [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
    public string Email { get; set; } = string.Empty;
}
