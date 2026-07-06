using PortfolioApp.Business.Validators;
using PortfolioApp.DTO.DTOs.Site;
using Xunit;

namespace PortfolioApp.Business.Tests.Validators;

public class ContactMessageCreateValidatorTests
{
    private readonly ContactMessageCreateValidator _validator = new();

    private static ContactMessageCreateDto ValidDto() => new()
    {
        SenderName = "Jane Doe",
        SenderEmail = "jane@example.com",
        Subject = "Hello",
        Message = "This is a valid message body."
    };

    [Fact]
    public void Validate_ValidDto_Passes()
    {
        Assert.True(_validator.Validate(ValidDto()).IsValid);
    }

    [Fact]
    public void Validate_EmptySenderName_Fails()
    {
        var dto = ValidDto();
        dto.SenderName = "";

        Assert.False(_validator.Validate(dto).IsValid);
    }

    [Theory]
    [InlineData("")]
    [InlineData("not-an-email")]
    public void Validate_InvalidSenderEmail_Fails(string email)
    {
        var dto = ValidDto();
        dto.SenderEmail = email;

        Assert.False(_validator.Validate(dto).IsValid);
    }

    [Fact]
    public void Validate_EmptySubject_Fails()
    {
        var dto = ValidDto();
        dto.Subject = "";

        Assert.False(_validator.Validate(dto).IsValid);
    }

    [Fact]
    public void Validate_MessageTooShort_Fails()
    {
        var dto = ValidDto();
        dto.Message = "short";

        Assert.False(_validator.Validate(dto).IsValid);
    }

    [Fact]
    public void Validate_MessageTooLong_Fails()
    {
        var dto = ValidDto();
        dto.Message = new string('a', 2001);

        Assert.False(_validator.Validate(dto).IsValid);
    }

    [Fact]
    public void Validate_PhoneTooLong_Fails()
    {
        var dto = ValidDto();
        dto.SenderPhone = new string('1', 21);

        Assert.False(_validator.Validate(dto).IsValid);
    }
}
