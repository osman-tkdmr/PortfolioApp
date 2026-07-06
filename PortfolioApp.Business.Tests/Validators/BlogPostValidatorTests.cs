using PortfolioApp.Business.Validators;
using PortfolioApp.DTO.DTOs.Blog;
using Xunit;

namespace PortfolioApp.Business.Tests.Validators;

public class BlogPostCreateValidatorTests
{
    private readonly BlogPostCreateValidator _validator = new();

    [Fact]
    public void Validate_ValidDto_Passes()
    {
        var dto = new BlogPostCreateDto { Title = "Hello", Content = "Some content", BlogCategoryId = 1 };

        Assert.True(_validator.Validate(dto).IsValid);
    }

    [Fact]
    public void Validate_EmptyTitle_Fails()
    {
        var dto = new BlogPostCreateDto { Title = "", Content = "Some content", BlogCategoryId = 1 };

        Assert.False(_validator.Validate(dto).IsValid);
    }

    [Fact]
    public void Validate_EmptyContent_Fails()
    {
        var dto = new BlogPostCreateDto { Title = "Hello", Content = "", BlogCategoryId = 1 };

        Assert.False(_validator.Validate(dto).IsValid);
    }

    [Fact]
    public void Validate_NoCategorySelected_Fails()
    {
        var dto = new BlogPostCreateDto { Title = "Hello", Content = "Some content", BlogCategoryId = 0 };

        Assert.False(_validator.Validate(dto).IsValid);
    }

    [Fact]
    public void Validate_SummaryTooLong_Fails()
    {
        var dto = new BlogPostCreateDto { Title = "Hello", Content = "x", BlogCategoryId = 1, Summary = new string('a', 501) };

        Assert.False(_validator.Validate(dto).IsValid);
    }
}

public class BlogPostUpdateValidatorTests
{
    private readonly BlogPostUpdateValidator _validator = new();

    [Fact]
    public void Validate_ValidDto_Passes()
    {
        var dto = new BlogPostUpdateDto { Id = 1, Title = "Hello", Content = "Some content", BlogCategoryId = 1 };

        Assert.True(_validator.Validate(dto).IsValid);
    }

    [Fact]
    public void Validate_ZeroId_Fails()
    {
        var dto = new BlogPostUpdateDto { Id = 0, Title = "Hello", Content = "Some content", BlogCategoryId = 1 };

        Assert.False(_validator.Validate(dto).IsValid);
    }

    [Fact]
    public void Validate_NoCategorySelected_Fails()
    {
        var dto = new BlogPostUpdateDto { Id = 1, Title = "Hello", Content = "Some content", BlogCategoryId = 0 };

        Assert.False(_validator.Validate(dto).IsValid);
    }
}
