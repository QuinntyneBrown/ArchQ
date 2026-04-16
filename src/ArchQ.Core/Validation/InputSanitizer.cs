using System.Text.RegularExpressions;

namespace ArchQ.Core.Validation;

public static class InputSanitizer
{
    /// <summary>
    /// Trims whitespace, strips control characters, and normalizes internal whitespace.
    /// </summary>
    public static string SanitizeString(string? input)
    {
        if (string.IsNullOrEmpty(input))
            return string.Empty;

        // Strip control characters (except newline and tab for body fields)
        var cleaned = Regex.Replace(input, @"[\x00-\x08\x0B\x0C\x0E-\x1F\x7F]", string.Empty);

        // Normalize multiple spaces/tabs to single space
        cleaned = Regex.Replace(cleaned, @"[^\S\n]+", " ");

        return cleaned.Trim();
    }

    /// <summary>
    /// Strips all HTML tags from the input. Suitable for text-only fields like titles.
    /// </summary>
    public static string SanitizeHtml(string? input)
    {
        if (string.IsNullOrEmpty(input))
            return string.Empty;

        var stripped = Regex.Replace(input, @"<[^>]*>", string.Empty);
        return SanitizeString(stripped);
    }
}
