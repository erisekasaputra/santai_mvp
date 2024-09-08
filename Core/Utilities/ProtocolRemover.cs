namespace Core.Utilities;

public static class ProtocolRemover
{
    public static string? RemovePrefixProcotol(this string? url)
    {
        if (string.IsNullOrEmpty(url))
        {
            return url;
        }

        url = url.Replace("https://", "").Replace("http://", "");
        return url;
    }
}
