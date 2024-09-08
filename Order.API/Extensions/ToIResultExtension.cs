using Core.Results;

namespace Order.API.Extensions;

public static class ToIResultExtension
{
    public static IResult ToIResult(this Result result, params string[] links)
    {
        return result switch
        {
            { ResponseStatus: ResponseStatus.Ok } => TypedResults.Ok(result),
            { ResponseStatus: ResponseStatus.NoContent } => TypedResults.NoContent(),
            { ResponseStatus: ResponseStatus.Created } => TypedResults.Created(links.FirstOrDefault(), result.WithLinks(links)),
            { ResponseStatus: ResponseStatus.InternalServerError } => TypedResults.InternalServerError(result),
            { ResponseStatus: ResponseStatus.BadRequest } => TypedResults.BadRequest(result),
            { ResponseStatus: ResponseStatus.Accepted } => TypedResults.Accepted(links.FirstOrDefault()),
            { ResponseStatus: ResponseStatus.Forbidden } => TypedResults.Forbid(),
            { ResponseStatus: ResponseStatus.NotFound } => TypedResults.NotFound(result),
            { ResponseStatus: ResponseStatus.Unauthorized } => TypedResults.Unauthorized(),
            { ResponseStatus: ResponseStatus.Conflict } => TypedResults.Conflict(result),
            _ => TypedResults.InternalServerError(),
        };
    }
}
