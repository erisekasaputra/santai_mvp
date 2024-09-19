namespace Core.Exceptions;

public class InvalidDateOperationException : Exception
{
    public DateTime InvalidDateUtc { get; set; }
    public InvalidDateOperationException(
        string message, 
        DateTime invalidDate) : base(message)  
    { 
        InvalidDateUtc = invalidDate;
    }
}
