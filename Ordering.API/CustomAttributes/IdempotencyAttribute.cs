namespace Ordering.API.CustomAttributes;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class IdempotencyAttribute(string name) : Attribute
{
    public string Name { get; set; } = name;
}
