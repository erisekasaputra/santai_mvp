using System.Text.RegularExpressions;

namespace Identity.API.Extensions;

public partial class PhoneNumberValidator
{

    [GeneratedRegex("^\\+[1-9]\\d{1,14}$")]
    private static partial Regex PhoneNumberRegex();
    public static bool IsValid(string value)
    {
        Regex regex = PhoneNumberRegex();
        return regex.IsMatch(value);
    } 
}
