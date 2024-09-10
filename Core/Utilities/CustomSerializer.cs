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

    public static string Serialize(object data)
    {
        if (data is null) return string.Empty;

        return JsonSerializer.Serialize(data);
    }
    public static T Deserialize<T>(string json) where T : notnull, new()
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            throw new ArgumentException("Input JSON string cannot be null or empty", nameof(json));
        }

        var result = JsonSerializer.Deserialize<T>(json);

        if (result == null)
        {
            throw new InvalidOperationException($"Deserialization of type {typeof(T).Name} returned null.");
        }

        return result;
    }
}
