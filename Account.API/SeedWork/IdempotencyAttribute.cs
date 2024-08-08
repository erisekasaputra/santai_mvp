namespace Account.API.SeedWork;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class IdempotencyAttribute : Attribute
{
}
