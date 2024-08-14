using System.Text.RegularExpressions;

namespace Account.API.Extensions;

public static partial class VehicleExtension
{
    [GeneratedRegex(@"^[A-Z0-9]+$")]
    private static partial Regex LegalDataRegex();
    public static bool IsValidNumber(string value)
    {
        Regex regex = LegalDataRegex();
        return regex.IsMatch(value);
    } 
}
