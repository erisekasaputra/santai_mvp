using System.Text;

namespace FileHub.API.Utilities;

public class UrlBuilder
{
    public static string Build(params string[] value)
    {
        if (value.Length == 0)
        {
            throw new ArgumentNullException(nameof(value));
        }

        var builder = new StringBuilder();

        foreach (var v in value)
        {
            builder.Append(v);
        }

        return builder.ToString();  
    }
}
