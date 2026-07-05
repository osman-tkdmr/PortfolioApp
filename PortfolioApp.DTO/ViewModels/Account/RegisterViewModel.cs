using System.ComponentModel.DataAnnotations;

namespace PortfolioApp.DTO.ViewModels.Account;

public class RegisterViewModel
{
    [Required(ErrorMessage = "Ad zorunludur.")]
    [MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Soyad zorunludur.")]
    [MaxLength(100)]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Kullanıcı adı zorunludur.")]
    [RegularExpression("^[a-z0-9-]{3,50}$", ErrorMessage = "Kullanıcı adı yalnızca küçük harf, rakam ve tire içerebilir (3-50 karakter).")]
    public string Handle { get; set; } = string.Empty;

    [Required(ErrorMessage = "E-posta zorunludur.")]
    [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Şifre zorunludur.")]
    [DataType(DataType.Password)]
    [MinLength(8, ErrorMessage = "Şifre en az 8 karakter olmalıdır.")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Şifre tekrarı zorunludur.")]
    [DataType(DataType.Password)]
    [Compare(nameof(Password), ErrorMessage = "Şifreler eşleşmiyor.")]
    public string ConfirmPassword { get; set; } = string.Empty;
}
