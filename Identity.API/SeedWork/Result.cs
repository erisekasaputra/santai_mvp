namespace Identity.API.SeedWork;

public class Result
{
    public bool IsSuccess { get; set; }
    public object? Data { get; set; }
    public string? Message { get; set; }
    public int ResponseCode { get; set; }
    public List<ErrorDetail> Errors { get; set; } = [];

    public List<string>? Links { get; set; } = [];

    public static Result Success(object? data, int responseCode = 200, string message = "")
    {
        return new Result
        {
            Data = data,
            IsSuccess = true,
            Message = message,
            ResponseCode = responseCode
        };
    }

    public static Result Failure(string message, int responseStatus)
    {
        return new Result
        {
            IsSuccess = false,
            Message = message,
            ResponseCode = responseStatus,
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

