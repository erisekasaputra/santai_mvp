using Account.API.Applications.Services;
using Account.API.Extensions;
using Account.API.Mapper;
using Account.API.SeedWork;
using Account.Domain.Enumerations;
using Account.Domain.Exceptions;
using Account.Domain.SeedWork;
using MediatR; 

namespace Account.API.Applications.Commands.CreateStaffBusinessUserByUserId;

public class CreateStaffBusinessUserByUserIdCommandHandler(IUnitOfWork unitOfWork, AppService service) : IRequestHandler<CreateStaffBusinessUserByUserIdCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly AppService _appService = service;

    public async Task<Result> Handle(CreateStaffBusinessUserByUserIdCommand request, CancellationToken cancellationToken)
    {
        try
        { 
            var staff = request.Staff;
            var user = await _unitOfWork.Users.GetBusinessUserByIdAsync(request.Id);

            if (user is null)
            {
                return Result.Failure($"Business user with id {request.Id} is not found", ResponseStatus.NotFound);
            }

            var staffConflicts = await _unitOfWork.Staffs.GetByIdsAsNoTrackAsync(
                    (IdentityParameter.Username, [staff.Username]),
                    (IdentityParameter.Email, [staff.Email]),
                    (IdentityParameter.PhoneNumber, [staff.PhoneNumber])
                );

            
            if (staffConflicts is not null && staffConflicts.Any())
            {
                var staffConflict = staffConflicts.FirstOrDefault();  
                var errorDetails = new List<ErrorDetail>();

                if (staffConflict!.Username.CleanAndLowering() == staff.Username.CleanAndLowering())
                {
                    errorDetails.Add(new ErrorDetail("Username", $"Username {staff.Username} already registered"));
                }

                if (staffConflict!.Username.CleanAndLowering() == staff.Username.CleanAndLowering())
                {
                    errorDetails.Add(new ErrorDetail("Username", $"Username {staff.Username} already registered"));
                }

                if (staffConflict!.Username.CleanAndLowering() == staff.Username.CleanAndLowering())
                {
                    errorDetails.Add(new ErrorDetail("Username", $"Username {staff.Username} already registered"));
                }

                var message = errorDetails.Count switch
                {
                    <= 1 => "There is a data conflict",
                    _ => $"There are {errorDetails.Count} data conflicts"
                };

                return Result.Failure(message, ResponseStatus.BadRequest).WithErrors(errorDetails);
            }

            (string? errorParameter, string? errorMessage) = user.AddStaff(
                                                                staff.Username,
                                                                staff.Email,
                                                                staff.PhoneNumber,
                                                                staff.Name,
                                                                staff.Address.ToAddress(),
                                                                staff.TimeZoneId);

            if (errorParameter is not null)
            {
                return Result.Failure("There is a conflict while saving the staff data into a business user", ResponseStatus.BadRequest)
                    .WithError(new ErrorDetail(errorParameter, errorMessage ?? string.Empty));
            }

            _unitOfWork.Users.UpdateUser(user);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success(null, ResponseStatus.NoContent);
        }
        catch (DomainException ex)
        {
            _appService.Logger.LogError(ex.Message);
            return Result.Failure(ex.Message, ResponseStatus.BadRequest);

        }
        catch (Exception ex)
        {
            _appService.Logger.LogError(ex.Message);
            return Result.Failure("An error has occurred while creating staff for a business user", ResponseStatus.InternalServerError);
        }
    }
}
