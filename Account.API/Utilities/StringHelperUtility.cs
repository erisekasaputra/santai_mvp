namespace Account.API.Utilities;
 
public class StringHelperService : IStringHelperUtility
{
    public string NormalizeString(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return string.Empty;
        }

        return input.Trim().ToLowerInvariant();
    }
}