namespace Core.Results;

public class ErrorDetail
{
    public string PropertyName { get; set; }
    public string ErrorMessage { get; set; }
    public string AttemptedValue { get; set; }
    public string ErrorCode { get; set; }
    public string Severity { get; set; }

    public ErrorDetail(
        string propertyName, 
        string message, 
        string attemptedValue, 
        string errorCode, 
        string severity)
    {
        PropertyName = propertyName;
        ErrorMessage = message;
        AttemptedValue = attemptedValue;
        ErrorCode = errorCode;
        Severity = severity;
    }
}