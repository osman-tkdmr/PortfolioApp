using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace PortfolioApp.Core.Utilities;

public static class SlugHelper
{
    public static string Slugify(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return string.Empty;

        var normalized = text.ToLowerInvariant()
            .Normalize(NormalizationForm.FormD);

        var sb = new StringBuilder();
        foreach (var c in normalized)
        {
            var category = CharUnicodeInfo.GetUnicodeCategory(c);
            if (category != UnicodeCategory.NonSpacingMark)
                sb.Append(c);
        }

        var slug = sb.ToString().Normalize(NormalizationForm.FormC);

        // Turkish character replacements
        slug = slug
            .Replace('ı', 'i')
            .Replace('ğ', 'g')
            .Replace('ş', 's')
            .Replace('ö', 'o')
            .Replace('ü', 'u')
            .Replace('ç', 'c');

        slug = Regex.Replace(slug, @"[^a-z0-9\s-]", "");
        slug = Regex.Replace(slug, @"\s+", "-");
        slug = Regex.Replace(slug, @"-+", "-");
        slug = slug.Trim('-');

        return slug;
    }

    public static string EnsureUnique(string slug, IEnumerable<string> existingSlugs)
    {
        var slugs = existingSlugs.ToHashSet();
        if (!slugs.Contains(slug))
            return slug;

        var counter = 1;
        string uniqueSlug;
        do
        {
            uniqueSlug = $"{slug}-{counter}";
            counter++;
        } while (slugs.Contains(uniqueSlug));

        return uniqueSlug;
    }
}
