using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PortfolioApp.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class MultiTenantOwnership : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SeoSettings_PageSlug",
                table: "SeoSettings");

            migrationBuilder.DropIndex(
                name: "IX_Projects_Slug",
                table: "Projects");

            migrationBuilder.DropIndex(
                name: "IX_BlogPosts_Slug",
                table: "BlogPosts");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "VisitorLogs",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Testimonials",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "SocialMedias",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Skills",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "SkillCategories",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "SiteSettings",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "SeoSettings",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Projects",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "ProjectImages",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "ProjectCategories",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "MenuItems",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Languages",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "HeroSections",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "FooterContents",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Experiences",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Educations",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "ContactMessages",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "ContactInfos",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Comments",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Certificates",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "BlogTags",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "BlogCategories",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Handle",
                table: "AspNetUsers",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Achievements",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Abouts",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            // Backfill: give any pre-existing users a unique, URL-safe Handle (derived from email) before the unique index is created below.
            migrationBuilder.Sql(@"
                UPDATE u
                SET Handle = LOWER(LEFT(REPLACE(REPLACE(COALESCE(NULLIF(u.Email, ''), 'user'), '@', '-'), '.', '-'), 40)) + CAST(rn.RowNum AS nvarchar(10))
                FROM AspNetUsers u
                INNER JOIN (SELECT Id, ROW_NUMBER() OVER (ORDER BY CreatedAt ASC) AS RowNum FROM AspNetUsers) rn ON rn.Id = u.Id
                WHERE u.Handle = '';
            ");

            // Backfill: pre-existing single-tenant data (created before multi-tenancy existed) belongs to the earliest-created user (the seeded admin).
            migrationBuilder.Sql(@"
                DECLARE @adminId nvarchar(450);
                SELECT TOP 1 @adminId = Id FROM AspNetUsers ORDER BY CreatedAt ASC;

                IF @adminId IS NOT NULL
                BEGIN
                    UPDATE Projects SET UserId = @adminId WHERE UserId = '';
                    UPDATE ProjectCategories SET UserId = @adminId WHERE UserId = '';
                    UPDATE ProjectImages SET UserId = @adminId WHERE UserId = '';
                    UPDATE BlogCategories SET UserId = @adminId WHERE UserId = '';
                    UPDATE BlogTags SET UserId = @adminId WHERE UserId = '';
                    UPDATE Comments SET UserId = @adminId WHERE UserId = '';
                    UPDATE Experiences SET UserId = @adminId WHERE UserId = '';
                    UPDATE Educations SET UserId = @adminId WHERE UserId = '';
                    UPDATE Certificates SET UserId = @adminId WHERE UserId = '';
                    UPDATE SkillCategories SET UserId = @adminId WHERE UserId = '';
                    UPDATE Skills SET UserId = @adminId WHERE UserId = '';
                    UPDATE Languages SET UserId = @adminId WHERE UserId = '';
                    UPDATE Achievements SET UserId = @adminId WHERE UserId = '';
                    UPDATE Testimonials SET UserId = @adminId WHERE UserId = '';
                    UPDATE SocialMedias SET UserId = @adminId WHERE UserId = '';
                    UPDATE MenuItems SET UserId = @adminId WHERE UserId = '';
                    UPDATE FooterContents SET UserId = @adminId WHERE UserId = '';
                    UPDATE ContactMessages SET UserId = @adminId WHERE UserId = '';
                    UPDATE HeroSections SET UserId = @adminId WHERE UserId = '';
                    UPDATE Abouts SET UserId = @adminId WHERE UserId = '';
                    UPDATE SiteSettings SET UserId = @adminId WHERE UserId = '';
                    UPDATE SeoSettings SET UserId = @adminId WHERE UserId = '';
                    UPDATE ContactInfos SET UserId = @adminId WHERE UserId = '';
                    UPDATE VisitorLogs SET UserId = @adminId WHERE UserId = '';
                END
            ");

            migrationBuilder.CreateIndex(
                name: "IX_VisitorLogs_UserId_VisitedAt",
                table: "VisitorLogs",
                columns: new[] { "UserId", "VisitedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Testimonials_UserId",
                table: "Testimonials",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_SocialMedias_UserId",
                table: "SocialMedias",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Skills_UserId",
                table: "Skills",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_SkillCategories_UserId",
                table: "SkillCategories",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_SiteSettings_UserId",
                table: "SiteSettings",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SeoSettings_UserId_PageSlug",
                table: "SeoSettings",
                columns: new[] { "UserId", "PageSlug" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Projects_UserId",
                table: "Projects",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_UserId_Slug",
                table: "Projects",
                columns: new[] { "UserId", "Slug" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProjectImages_UserId",
                table: "ProjectImages",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectCategories_UserId",
                table: "ProjectCategories",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_MenuItems_UserId",
                table: "MenuItems",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Languages_UserId",
                table: "Languages",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_HeroSections_UserId",
                table: "HeroSections",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FooterContents_UserId",
                table: "FooterContents",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Experiences_UserId",
                table: "Experiences",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Educations_UserId",
                table: "Educations",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ContactMessages_UserId",
                table: "ContactMessages",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ContactInfos_UserId",
                table: "ContactInfos",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Comments_UserId",
                table: "Comments",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Certificates_UserId",
                table: "Certificates",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_BlogTags_UserId",
                table: "BlogTags",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_BlogPosts_AuthorId_Slug",
                table: "BlogPosts",
                columns: new[] { "AuthorId", "Slug" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BlogCategories_UserId",
                table: "BlogCategories",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_Handle",
                table: "AspNetUsers",
                column: "Handle",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Achievements_UserId",
                table: "Achievements",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Abouts_UserId",
                table: "Abouts",
                column: "UserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_VisitorLogs_UserId_VisitedAt",
                table: "VisitorLogs");

            migrationBuilder.DropIndex(
                name: "IX_Testimonials_UserId",
                table: "Testimonials");

            migrationBuilder.DropIndex(
                name: "IX_SocialMedias_UserId",
                table: "SocialMedias");

            migrationBuilder.DropIndex(
                name: "IX_Skills_UserId",
                table: "Skills");

            migrationBuilder.DropIndex(
                name: "IX_SkillCategories_UserId",
                table: "SkillCategories");

            migrationBuilder.DropIndex(
                name: "IX_SiteSettings_UserId",
                table: "SiteSettings");

            migrationBuilder.DropIndex(
                name: "IX_SeoSettings_UserId_PageSlug",
                table: "SeoSettings");

            migrationBuilder.DropIndex(
                name: "IX_Projects_UserId",
                table: "Projects");

            migrationBuilder.DropIndex(
                name: "IX_Projects_UserId_Slug",
                table: "Projects");

            migrationBuilder.DropIndex(
                name: "IX_ProjectImages_UserId",
                table: "ProjectImages");

            migrationBuilder.DropIndex(
                name: "IX_ProjectCategories_UserId",
                table: "ProjectCategories");

            migrationBuilder.DropIndex(
                name: "IX_MenuItems_UserId",
                table: "MenuItems");

            migrationBuilder.DropIndex(
                name: "IX_Languages_UserId",
                table: "Languages");

            migrationBuilder.DropIndex(
                name: "IX_HeroSections_UserId",
                table: "HeroSections");

            migrationBuilder.DropIndex(
                name: "IX_FooterContents_UserId",
                table: "FooterContents");

            migrationBuilder.DropIndex(
                name: "IX_Experiences_UserId",
                table: "Experiences");

            migrationBuilder.DropIndex(
                name: "IX_Educations_UserId",
                table: "Educations");

            migrationBuilder.DropIndex(
                name: "IX_ContactMessages_UserId",
                table: "ContactMessages");

            migrationBuilder.DropIndex(
                name: "IX_ContactInfos_UserId",
                table: "ContactInfos");

            migrationBuilder.DropIndex(
                name: "IX_Comments_UserId",
                table: "Comments");

            migrationBuilder.DropIndex(
                name: "IX_Certificates_UserId",
                table: "Certificates");

            migrationBuilder.DropIndex(
                name: "IX_BlogTags_UserId",
                table: "BlogTags");

            migrationBuilder.DropIndex(
                name: "IX_BlogPosts_AuthorId_Slug",
                table: "BlogPosts");

            migrationBuilder.DropIndex(
                name: "IX_BlogCategories_UserId",
                table: "BlogCategories");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_Handle",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_Achievements_UserId",
                table: "Achievements");

            migrationBuilder.DropIndex(
                name: "IX_Abouts_UserId",
                table: "Abouts");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "VisitorLogs");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Testimonials");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "SocialMedias");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Skills");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "SkillCategories");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "SiteSettings");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "SeoSettings");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "ProjectImages");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "ProjectCategories");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "MenuItems");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Languages");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "HeroSections");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "FooterContents");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Experiences");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Educations");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "ContactMessages");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "ContactInfos");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Certificates");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "BlogTags");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "BlogCategories");

            migrationBuilder.DropColumn(
                name: "Handle",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Achievements");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Abouts");

            migrationBuilder.CreateIndex(
                name: "IX_SeoSettings_PageSlug",
                table: "SeoSettings",
                column: "PageSlug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Projects_Slug",
                table: "Projects",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BlogPosts_Slug",
                table: "BlogPosts",
                column: "Slug",
                unique: true);
        }
    }
}
