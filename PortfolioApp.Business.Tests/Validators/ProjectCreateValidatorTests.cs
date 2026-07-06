using PortfolioApp.Business.Validators;
using PortfolioApp.DTO.DTOs.Project;
using Xunit;

namespace PortfolioApp.Business.Tests.Validators;

public class ProjectCreateValidatorTests
{
    private readonly ProjectCreateValidator _validator = new();

    [Fact]
    public void Validate_ValidDto_Passes()
    {
        var dto = new ProjectCreateDto { Title = "My Project", ProjectCategoryId = 1, DemoUrl = "https://example.com" };

        Assert.True(_validator.Validate(dto).IsValid);
    }

    [Fact]
    public void Validate_EmptyTitle_Fails()
    {
        var dto = new ProjectCreateDto { Title = "", ProjectCategoryId = 1 };

        Assert.False(_validator.Validate(dto).IsValid);
    }

    [Fact]
    public void Validate_TitleTooLong_Fails()
    {
        var dto = new ProjectCreateDto { Title = new string('a', 251), ProjectCategoryId = 1 };

        Assert.False(_validator.Validate(dto).IsValid);
    }

    [Fact]
    public void Validate_NoCategorySelected_Fails()
    {
        var dto = new ProjectCreateDto { Title = "My Project", ProjectCategoryId = 0 };

        Assert.False(_validator.Validate(dto).IsValid);
    }

    [Theory]
    [InlineData("javascript:alert(1)")]
    [InlineData("not a url")]
    [InlineData("ftp://example.com")]
    public void Validate_DemoUrlNotHttpOrHttps_Fails(string demoUrl)
    {
        var dto = new ProjectCreateDto { Title = "My Project", ProjectCategoryId = 1, DemoUrl = demoUrl };

        Assert.False(_validator.Validate(dto).IsValid);
    }

    [Fact]
    public void Validate_EmptyDemoUrl_Passes()
    {
        var dto = new ProjectCreateDto { Title = "My Project", ProjectCategoryId = 1, DemoUrl = null };

        Assert.True(_validator.Validate(dto).IsValid);
    }
}
