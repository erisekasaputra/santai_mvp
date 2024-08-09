using System.Text.RegularExpressions;

namespace Account.API.Extensions;

public static partial class StringNormalizer
{ 
    [GeneratedRegex(@"\s+")]
    private static partial Regex CleaningSpacesAndLowering();

    public static string CleanAndLowering(this string value)
    {
        return CleaningSpacesAndLowering().Replace(value, " ");
    }

    [GeneratedRegex(@"\s+")]
    private static partial Regex CleaningSpaces();
    public static string Clean(this string value)
    {
        return CleaningSpaces().Replace(value, " ").Trim(); 
    } 
}
