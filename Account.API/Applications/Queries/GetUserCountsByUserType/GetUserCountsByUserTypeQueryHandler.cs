using Account.API.Applications.Dtos.ResponseDtos;
using Account.API.Applications.Services;
using Account.Domain.SeedWork;
using Core.CustomMessages;
using Core.Results;
using MediatR;

namespace Account.API.Applications.Queries.GetUserCountsByUserType;

public class GetUserCountsByUserTypeQueryHandler : IRequestHandler<GetUserCountsByUserTypeQuery, Result>
{ 
    private readonly IUnitOfWork _unitOfWork;
    private readonly ApplicationService _appService;
    public GetUserCountsByUserTypeQueryHandler(IUnitOfWork unitOfWork, ApplicationService appService)
    {
        _unitOfWork = unitOfWork;
        _appService = appService;
    }  

    public async Task<Result> Handle(GetUserCountsByUserTypeQuery request, CancellationToken cancellationToken)
    {
        try
        { 
            var users = await _unitOfWork.BaseUsers.CountTotalUsersByUserType();

            if (users == null)
            {
                return Result.Failure("Users is empty", ResponseStatus.NotFound);
            }
 
            List<UserCountResponseDto> userCountResponseDtos = users
                .Select(u => new UserCountResponseDto(u.UserType, u.Count))
                .ToList(); 

            return Result.Success(userCountResponseDtos, ResponseStatus.Ok);
        }
        catch (Exception ex)
        {
            _appService.Logger.LogError(ex.Message, ex.InnerException?.Message);
            return Result.Failure(Messages.InternalServerError, ResponseStatus.InternalServerError);
        }
    }
}