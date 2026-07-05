using Microsoft.EntityFrameworkCore;
using PortfolioApp.Core.Constants;
using PortfolioApp.Core.Interfaces;
using PortfolioApp.DataAccess.UnitOfWork;
using PortfolioApp.Entity.Concrete;

namespace PortfolioApp.Business.Services.Concrete;

public class UserProvisioningService : IUserProvisioningService
{
    private readonly UnitOfWork _uow;

    public UserProvisioningService(UnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task ProvisionDefaultsAsync(string userId)
    {
        await ProvisionSiteSettingsAsync(userId);
        await ProvisionSeoSettingsAsync(userId);
        await ProvisionContactInfoAsync(userId);
        await ProvisionHeroSectionAsync(userId);
        await _uow.SaveChangesAsync();
    }

    private async Task ProvisionSiteSettingsAsync(string userId)
    {
        if (await _uow.Context.SiteSettings.AnyAsync(s => s.UserId == userId))
            return;

        await _uow.Context.SiteSettings.AddAsync(new SiteSettings
        {
            UserId = userId,
            SiteName = "My Portfolio",
            SiteTitle = "Full-Stack Developer Portfolio",
            SiteDescription = "Professional portfolio showcasing projects, skills and experience.",
            CopyrightText = $"© {DateTime.Now.Year} My Portfolio. All rights reserved.",
            Language = "tr"
        });
    }

    private async Task ProvisionSeoSettingsAsync(string userId)
    {
        if (await _uow.Context.SeoSettings.AnyAsync(s => s.UserId == userId))
            return;

        var pages = new List<SeoSettings>
        {
            new() { UserId = userId, PageSlug = AppConstants.PageSlugs.Home, MetaTitle = "Home | My Portfolio", MetaDescription = "Welcome to my professional portfolio." },
            new() { UserId = userId, PageSlug = AppConstants.PageSlugs.Blog, MetaTitle = "Blog | My Portfolio", MetaDescription = "Articles about software development and technology." },
            new() { UserId = userId, PageSlug = AppConstants.PageSlugs.Projects, MetaTitle = "Projects | My Portfolio", MetaDescription = "Browse my portfolio of projects." },
            new() { UserId = userId, PageSlug = AppConstants.PageSlugs.Contact, MetaTitle = "Contact | My Portfolio", MetaDescription = "Get in touch with me." },
            new() { UserId = userId, PageSlug = AppConstants.PageSlugs.About, MetaTitle = "About | My Portfolio", MetaDescription = "Learn more about me." },
        };

        await _uow.Context.SeoSettings.AddRangeAsync(pages);
    }

    private async Task ProvisionContactInfoAsync(string userId)
    {
        if (await _uow.Context.ContactInfos.AnyAsync(c => c.UserId == userId))
            return;

        await _uow.Context.ContactInfos.AddAsync(new ContactInfo
        {
            UserId = userId,
            WorkingHours = "Mon-Fri: 09:00 - 18:00",
            IsActive = true
        });
    }

    private async Task ProvisionHeroSectionAsync(string userId)
    {
        if (await _uow.Context.HeroSections.AnyAsync(h => h.UserId == userId))
            return;

        await _uow.Context.HeroSections.AddAsync(new HeroSection
        {
            UserId = userId,
            Title = "Hi, I'm Your Name",
            Subtitle = "Full-Stack Developer",
            Description = "I build modern, scalable web applications with passion and precision.",
            TypewriterTexts = "[\"Full-Stack Developer\",\"Software Engineer\",\"UI/UX Enthusiast\"]",
            ShowTypewriter = true,
            CtaButtonText = "View My Work",
            CtaButtonUrl = "#portfolio",
            SecondaryButtonText = "Download CV",
            SecondaryButtonUrl = "#",
            IsActive = true
        });
    }
}
