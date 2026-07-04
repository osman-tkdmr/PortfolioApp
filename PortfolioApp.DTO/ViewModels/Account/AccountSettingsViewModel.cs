using System.ComponentModel.DataAnnotations;

namespace PortfolioApp.DTO.ViewModels.Account;

public class AccountSettingsViewModel
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

    [MaxLength(150)]
    public string? Title { get; set; }

    [MaxLength(2000)]
    public string? Bio { get; set; }

    public string? ProfileImageUrl { get; set; }
    public string? CvFileUrl { get; set; }
}
