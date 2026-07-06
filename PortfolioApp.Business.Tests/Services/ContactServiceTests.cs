using PortfolioApp.Business.Services.Concrete;
using PortfolioApp.Business.Tests.TestSupport;
using PortfolioApp.DTO.DTOs.Site;
using PortfolioApp.Entity.Concrete;
using Xunit;

namespace PortfolioApp.Business.Tests.Services;

public class ContactServiceTests : ServiceTestBase
{
    private ContactService CreateSut() => new(Uow, Mapper, CurrentUser);

    [Fact]
    public async Task GetContactInfoAsync_ScopesToOwner()
    {
        await SeedAsync(
            new ContactInfo { UserId = TestTenants.TenantA, Email = "a@example.com", IsActive = true },
            new ContactInfo { UserId = TestTenants.TenantB, Email = "b@example.com", IsActive = true });

        var result = await CreateSut().GetContactInfoAsync(TestTenants.TenantA);

        Assert.True(result.Success);
        Assert.Equal("a@example.com", result.Data!.Email);
    }

    [Fact]
    public async Task UpdateContactInfoAsync_BelongsToDifferentTenant_ReturnsFailure()
    {
        var info = new ContactInfo { UserId = TestTenants.TenantB, Email = "b@example.com" };
        await SeedAsync(info);

        CurrentUser.UserId = TestTenants.TenantA;
        var result = await CreateSut().UpdateContactInfoAsync(new ContactInfoUpdateDto { Id = info.Id, Email = "hijacked@example.com" });

        Assert.False(result.Success);
        Assert.Equal("b@example.com", Context.ContactInfos.Single(c => c.Id == info.Id).Email);
    }

    [Fact]
    public async Task SendMessageAsync_StampsGivenOwnerId()
    {
        var dto = new ContactMessageCreateDto { SenderName = "Visitor", SenderEmail = "visitor@example.com", Subject = "Hi", Message = "Hello there!" };

        var result = await CreateSut().SendMessageAsync(TestTenants.TenantA, dto);

        Assert.True(result.Success);
        Assert.Equal(TestTenants.TenantA, Context.ContactMessages.Single().UserId);
    }

    [Fact]
    public async Task GetMessagesAsync_ScopesToCurrentUserAndExcludesSpam()
    {
        await SeedAsync(
            new ContactMessage { UserId = TestTenants.TenantA, SenderName = "A1", SenderEmail = "a1@x.com", Subject = "s", Message = "m", IsSpam = false },
            new ContactMessage { UserId = TestTenants.TenantA, SenderName = "A2-spam", SenderEmail = "a2@x.com", Subject = "s", Message = "m", IsSpam = true },
            new ContactMessage { UserId = TestTenants.TenantB, SenderName = "B1", SenderEmail = "b1@x.com", Subject = "s", Message = "m", IsSpam = false });

        CurrentUser.UserId = TestTenants.TenantA;
        var result = await CreateSut().GetMessagesAsync();

        Assert.True(result.Success);
        var msg = Assert.Single(result.Data!);
        Assert.Equal("A1", msg.SenderName);
    }

    [Fact]
    public async Task GetMessageByIdAsync_BelongsToDifferentTenant_ReturnsFailure()
    {
        var msg = new ContactMessage { UserId = TestTenants.TenantB, SenderName = "B1", SenderEmail = "b1@x.com", Subject = "s", Message = "m" };
        await SeedAsync(msg);

        CurrentUser.UserId = TestTenants.TenantA;
        var result = await CreateSut().GetMessageByIdAsync(msg.Id);

        Assert.False(result.Success);
    }

    [Fact]
    public async Task MarkAsReadAsync_BelongsToDifferentTenant_ReturnsFailure()
    {
        var msg = new ContactMessage { UserId = TestTenants.TenantB, SenderName = "B1", SenderEmail = "b1@x.com", Subject = "s", Message = "m" };
        await SeedAsync(msg);

        CurrentUser.UserId = TestTenants.TenantA;
        var result = await CreateSut().MarkAsReadAsync(msg.Id);

        Assert.False(result.Success);
        Assert.False(Context.ContactMessages.Single(m => m.Id == msg.Id).IsRead);
    }

    [Fact]
    public async Task ReplyAsync_OwnMessage_SavesReply()
    {
        var msg = new ContactMessage { UserId = TestTenants.TenantA, SenderName = "A1", SenderEmail = "a1@x.com", Subject = "s", Message = "m" };
        await SeedAsync(msg);

        CurrentUser.UserId = TestTenants.TenantA;
        var result = await CreateSut().ReplyAsync(msg.Id, "Thanks for reaching out!");

        Assert.True(result.Success);
        var stored = Context.ContactMessages.Single(m => m.Id == msg.Id);
        Assert.True(stored.IsReplied);
        Assert.Equal("Thanks for reaching out!", stored.ReplyText);
    }

    [Fact]
    public async Task DeleteMessageAsync_BelongsToDifferentTenant_ReturnsFailure()
    {
        var msg = new ContactMessage { UserId = TestTenants.TenantB, SenderName = "B1", SenderEmail = "b1@x.com", Subject = "s", Message = "m" };
        await SeedAsync(msg);

        CurrentUser.UserId = TestTenants.TenantA;
        var result = await CreateSut().DeleteMessageAsync(msg.Id);

        Assert.False(result.Success);
        Assert.False(Context.ContactMessages.Single(m => m.Id == msg.Id).IsDeleted);
    }

    [Fact]
    public async Task MarkAsSpamAsync_BelongsToDifferentTenant_ReturnsFailure()
    {
        var msg = new ContactMessage { UserId = TestTenants.TenantB, SenderName = "B1", SenderEmail = "b1@x.com", Subject = "s", Message = "m" };
        await SeedAsync(msg);

        CurrentUser.UserId = TestTenants.TenantA;
        var result = await CreateSut().MarkAsSpamAsync(msg.Id);

        Assert.False(result.Success);
        Assert.False(Context.ContactMessages.Single(m => m.Id == msg.Id).IsSpam);
    }
}
