using System.Text.RegularExpressions;

namespace Core.Validations;

public static partial class UrlValidation
{
    [GeneratedRegex(@"^(https?:\/\/)?([\w\-]+\.)+[a-z]{2,6}(:\d+)?(\/[\w\-\.\/?%&=]*)?$", RegexOptions.IgnoreCase, "en-US")]
    private static partial Regex UrlRegex();

    public static bool IsValidUrl(string url)
    {  
        Regex regex = UrlRegex();
        return regex.IsMatch(url);  
    }

    [GeneratedRegex(@"^https?:\/\/.*\.(jpg|jpeg|png)$")]
    private static partial Regex UrlImageRegex();

    public static bool IsValidImageUrl(string url)
    {
        Regex regex = UrlImageRegex();
        return regex.IsMatch(url);
    }  
}
