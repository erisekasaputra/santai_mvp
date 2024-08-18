namespace Identity.API.Exceptions;

public class OtpException : Exception
{
    public OtpException() : base()
    {
        
    }

    public OtpException(string message) : base(message)
    {
    }
}
