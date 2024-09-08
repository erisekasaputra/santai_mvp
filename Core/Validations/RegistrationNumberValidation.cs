using System.Text.RegularExpressions;

namespace Core.Validations;

public static partial class RegistrationNumberValidation
{
    [GeneratedRegex(@"^[A-Z0-9]+$")]
    private static partial Regex LegalDataRegex();
    public static bool IsValidNumber(string value)
    {
        Regex regex = LegalDataRegex();
        return regex.IsMatch(value);
    }
}
