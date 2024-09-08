namespace Core.Results;

public class Result
{
    public bool IsSuccess { get; set; }
    public object? Data { get; set; }
    public string? Message { get; set; }
    public ResponseStatus ResponseStatus { get; set; }
    public List<ErrorDetail> Errors { get; set; } = [];

    public List<string>? Links { get; set; } = [];

    public Result WithData(object data)
    {
        Data = data;
        return this;
    }

    public static Result Success(object? data, ResponseStatus responseStatus = ResponseStatus.Ok, string message = "")
    {
        return new Result
        {
            Data = data,
            IsSuccess = true,
            Message = message,
            ResponseStatus = responseStatus
        };
    }

    public static Result Failure(string message, ResponseStatus responseStatus)
    {
        return new Result
        {
            IsSuccess = false,
            Message = message,
            ResponseStatus = responseStatus,
            Errors = []
        };
    }

    public Result WithError(ErrorDetail errors)
    {
        if (IsSuccess)
        {
            return this;
        }

        Errors.Add(errors);
        return this;
    }

    public Result WithErrors(List<ErrorDetail> errors)
    {
        if (IsSuccess)
        {
            return this;
        }

        Errors.AddRange(errors);
        return this;
    }

    public Result WithLinks(params string?[] links)
    {
        var filteredLinks = links.Where(x => !string.IsNullOrWhiteSpace(x));
        if (filteredLinks is null || !filteredLinks.Any())
        {
            return this;
        }

        Links ??= [];
        Links.AddRange(filteredLinks!);
        return this;
    }

    public bool TryGetData<T>(out T? result) where T : class
    {
        result = Data as T;
        return result != null;
    }
}

