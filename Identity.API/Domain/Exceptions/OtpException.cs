namespace Identity.API.Domain.Exceptions;

public class OtpException : Exception
{
    public OtpException() : base()
    {

    }

    public OtpException(string message) : base(message)
    {
    }
}
