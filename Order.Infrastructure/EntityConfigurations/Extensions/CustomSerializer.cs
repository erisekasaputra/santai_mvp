using System.Text.Json;

namespace Order.Infrastructure.EntityConfigurations.Extensions;

internal class CustomSerializer
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
}
