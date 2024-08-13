using Account.API.SeedWork;
using Account.Domain.SeedWork;
using MediatR;

namespace Account.API.Applications.Queries.GetPaginatedRegularUser;

public class GetPaginatedRegularUserQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetPaginatedRegularUserQuery, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    public async Task<Result> Handle(GetPaginatedRegularUserQuery request, CancellationToken cancellationToken)
    {
        var (TotalCount, TotalPages, Brands) = await _unitOfWork.Users.GetPaginatedRegularUser(request.PageNumber, request.PageSize);

        if (Brands is null)
        {
            return Result.Failure("Regular user data is empty", ResponseStatus.NotFound);
        }

        return Result.Failure(null, ResponseStatus.BadRequest);
    }
}
