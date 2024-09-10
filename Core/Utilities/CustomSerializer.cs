using System.Text.Json;

namespace Core.Utilities;

public class CustomSerializer
{
    public static string SerializeList(ICollection<string>? list)
    {
        if (list == null) return string.Empty;

        return JsonSerializer.Serialize(list.ToList());
    }
    public static ICollection<string> DeserializeList(string json)
    {
        if (string.IsNullOrWhiteSpace(json)) return [];

        return JsonSerializer.Deserialize<List<string>>(json) ?? [];
    }

    public static string Serialize(object? data)
    {
        if (data is null) return string.Empty;

        return JsonSerializer.Serialize(data);
    }
    public static T? Deserialize<T>(string? json)
    {
        if (string.IsNullOrWhiteSpace(json)) return default;

        return JsonSerializer.Deserialize<T>(json) ?? default;
    }
}
