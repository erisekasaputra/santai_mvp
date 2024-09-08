using System.Text.RegularExpressions;

namespace Core.Extensions;

public static partial class StringNormalizer
{
    [GeneratedRegex(@"\s+")]
    private static partial Regex CleaningSpacesAndLowering();

    public static string CleanAndLowering(this string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return string.Empty;

        return CleaningSpacesAndLowering().Replace(value, " ");
    }

    [GeneratedRegex(@"\s+")]
    private static partial Regex CleaningSpaces();
    public static string Clean(this string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return string.Empty;
        return CleaningSpaces().Replace(value, " ").Trim();
    }
}
