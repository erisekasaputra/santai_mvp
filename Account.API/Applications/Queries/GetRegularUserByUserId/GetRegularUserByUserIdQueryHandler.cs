using Account.API.Applications.Services;
using Account.API.Extensions;
using Account.API.Mapper;
using Account.API.SeedWork;
using Account.Domain.SeedWork; 
using MediatR;

namespace Account.API.Applications.Queries.GetRegularUserByUserId;

public class GetRegularUserByUserIdQueryHandler(IUnitOfWork unitOfWork, AppService service) : IRequestHandler<GetRegularUserByUserIdQuery, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly AppService _service = service;

    public async Task<Result> Handle(GetRegularUserByUserIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _unitOfWork.Users.GetRegularUserByIdAsync(request.UserId);

            if (user is null)
            {
                return Result.Failure($"User '{request.UserId}' not found", ResponseStatus.NotFound);
            }

            return Result.Success(user.ToRegularUserResponseDto());
        }
        catch (Exception ex)
        {
            _service.Logger.LogError(ex.Message);
            return Result.Failure(Messages.InternalServerError, ResponseStatus.InternalServerError);
        }
    }
}
