using System.Text.RegularExpressions;

namespace Identity.API.Extensions;

public partial class PhoneNumberValidator
{

    [GeneratedRegex(@"^\+?\d+$")]
    private static partial Regex PhoneNumberRegex();
    public static bool IsValid(string value)
    {
        Regex regex = PhoneNumberRegex();
        return regex.IsMatch(value);
    } 
}
