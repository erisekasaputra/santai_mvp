using Account.API.Applications.Dtos.ResponseDtos;
using Account.API.Applications.Services;
using Account.API.Mapper;
using Account.API.SeedWork;
using Account.Domain.SeedWork;
using MediatR;

namespace Account.API.Applications.Queries.GetBusinessUserByUserId;

public class GetBusinessUserByUserIdQueryHandler(IUnitOfWork unitOfWork, AppService service): IRequestHandler<GetBusinessUserByUserIdQuery, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly AppService _appService = service;

    public async Task<Result> Handle(GetBusinessUserByUserIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _unitOfWork.Users.GetBusinessUserByIdAsync(request.Id);
         
            if (user is null)
            {
                return Result.Failure($"Business user with id {request.Id} is not found", ResponseStatus.NotFound);
            }

            var userDto = new BusinessUserResponseDto(
                user.Id,
                user.Username,
                user.Email,
                user.PhoneNumber,
                user.TimeZoneId,
                user.Address.ToAddressResponseDto(),
                user.BusinessName,
                user.ContactPerson,
                user.TaxId,
                user.WebsiteUrl,
                user.Description,
                user.LoyaltyProgram?.ToLoyaltyProgramResponseDto(),
                user.BusinessLicenses.ToBusinessLicenseResponseDtos(),
                user.Staffs.ToStaffResponseDtos()); 

            return Result.Success(userDto); 
        }
        catch (Exception ex) 
        {
            _appService.Logger.LogError(ex.Message);
            return Result.Failure("An error has occurred during get the business user data", ResponseStatus.InternalServerError);
        }
    }
}
