using System.Text.RegularExpressions;

namespace Account.API.Extensions;

public static partial class PhoneNumberExtension
{
    [GeneratedRegex("^\\+[1-9]\\d{1,14}$", RegexOptions.IgnoreCase, "en-US")]
    private static partial Regex PhoneNumberRegex();

    public static bool IsValidPhoneNumber(string phoneNumber)
    { 
        Regex regex = PhoneNumberRegex();
        return regex.IsMatch(phoneNumber);
    } 
}
