namespace PortfolioApp.Core.Constants;

public static class AppConstants
{
    public static class Roles
    {
        public const string Admin = "Admin";
        public const string SuperAdmin = "SuperAdmin";
    }

    public static class CacheKeys
    {
        public const string ActiveTheme = "active_theme";
        public const string SiteSettings = "site_settings";
        public const string NavigationMenu = "navigation_menu";
        public const string SocialMedia = "social_media";
        public const string FooterContent = "footer_content";
        public const string ContactInfo = "contact_info";
    }

    public static class PageSlugs
    {
        public const string Home = "home";
        public const string Blog = "blog";
        public const string Projects = "projects";
        public const string Contact = "contact";
        public const string About = "about";
        public const string Resume = "resume";
    }

    public static class Upload
    {
        public const string ImagesPath = "uploads/images";
        public const string AvatarsPath = "uploads/avatars";
        public const string CvPath = "uploads/cv";
        public const string ThemesPath = "uploads/themes";
        public const int MaxImageSizeBytes = 5 * 1024 * 1024;
    }

    public static class Pagination
    {
        public const int DefaultPageSize = 10;
        public const int BlogPageSize = 6;
        public const int ProjectPageSize = 9;
        public const int AdminPageSize = 20;
    }

    public static class SeedData
    {
        public const string AdminEmail = "admin@portfolio.com";
        public const string AdminFirstName = "Admin";
        public const string AdminLastName = "User";
    }
}
