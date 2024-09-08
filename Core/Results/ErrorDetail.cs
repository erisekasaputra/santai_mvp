namespace Core.Results;

public class ErrorDetail
{
    public string Field { get; set; }
    public string Message { get; set; }

    public ErrorDetail(string field, string message)
    {
        Field = field;
        Message = message;
    }
}