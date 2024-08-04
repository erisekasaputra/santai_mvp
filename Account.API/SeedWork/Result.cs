namespace Account.API.SeedWork;
 
public class Result<T>
{
    public bool Success { get; }
    public T? Data { get; }
    public IEnumerable<string> Errors { get; }
    public int StatusCode { get; }
    public IEnumerable<string> Links { get; }

    private Result(bool success, T? data, IEnumerable<string> errors, IEnumerable<string> links, int statusCode)
    {
        Success = success;
        Data = data;
        Errors = errors ?? [];
        Links = links;
        StatusCode = statusCode;
    }

    public static Result<T> SuccessResult(T data, IEnumerable<string> links, int statusCode = 200)
    {
        return new Result<T>(true, data, [], links, statusCode);
    }

    public static Result<T> Failure(IEnumerable<string> errors, int statusCode = 400)
    {
        return new Result<T>(false, default, errors, [], statusCode);
    }

    public static Result<T> Failure(string error, int statusCode = 400)
    {
        return Failure([error], statusCode);
    }

    public Result<T> WithData(T data)
    {
        return new Result<T>(Success, data, Errors, Links, StatusCode);
    }

    public Result<T> WithErrors(IEnumerable<string> errors)
    {
        return new Result<T>(Success, Data, errors, Links, StatusCode);
    }

    public Result<T> WithStatusCode(int statusCode)
    {
        return new Result<T>(Success, Data, Errors, Links, statusCode);
    }

    public Result<T> WithLink(IEnumerable<string> links)
    {
        return new Result<T>(Success, Data, Errors, links, StatusCode);
    }

    public Result<T> WithLink(string link)
    {
        return WithLink([link]);
    }
}
