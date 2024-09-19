namespace Core.Exceptions;

public class DomainException : Exception 
{
    public string Parameter { get; set; }
    public DomainException(string message, string parameter = "") : base(message)
    {
        Parameter = parameter;
    }
}
