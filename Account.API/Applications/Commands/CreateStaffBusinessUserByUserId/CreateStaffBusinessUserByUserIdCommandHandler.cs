using Account.API.Applications.Services;
using Account.API.Extensions;
using Account.API.Mapper;
using Account.API.SeedWork;
using Account.Domain.Aggregates.UserAggregate;
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
            var entity = await _unitOfWork.Users.GetBusinessUserByIdAsync(request.Id);

            if (entity is null)
            {
                return Result.Failure($"Business user '{request.Id}' not found", ResponseStatus.NotFound);
            }

            var staffConflicts = await _unitOfWork.Staffs.GetByIdentitiesAsNoTrackAsync(
                    (IdentityParameter.Username, [staff.Username]),
                    (IdentityParameter.Email, [staff.Email]),
                    (IdentityParameter.PhoneNumber, [staff.PhoneNumber])
                );
             
            if (staffConflicts is not null && staffConflicts.Any())
            {
                var errorDetails = new List<ErrorDetail>();

                var staffConflict = staffConflicts.FirstOrDefault(); 
                   
                if (staffConflict!.Username == staff.Username)
                {
                    errorDetails.Add(new ErrorDetail("Username", $"Username {staff.Username} already registered"));
                }

                if (staffConflict!.Email == staff.Email || staffConflict.NewEmail == staff.Email)
                {
                    errorDetails.Add(new ErrorDetail("Email", $"Email {staff.Email} already registered"));
                }

                if (staffConflict!.PhoneNumber == staff.PhoneNumber || staffConflict.NewPhoneNumber == staff.PhoneNumber) 
                {
                    errorDetails.Add(new ErrorDetail("PhoneNumber", $"Phone number {staff.PhoneNumber} already registered"));
                }

                var message = errorDetails.Count switch
                {
                    <= 1 => "There is a conflict",
                    _ => $"There are {errorDetails.Count} conflicts"
                };

                return Result.Failure(message, ResponseStatus.BadRequest).WithErrors(errorDetails);
            }


            var newStaff = new Staff(entity.Id, entity.Code, staff.Username, staff.Email, staff.PhoneNumber, staff.Name, staff.Address.ToAddress(), staff.TimeZoneId, null);

            var savedStaff = await _unitOfWork.Staffs.CreateAsync(newStaff);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success(savedStaff, ResponseStatus.Created);
        }
        catch (DomainException ex)
        { 
            return Result.Failure(ex.Message, ResponseStatus.BadRequest);

        }
        catch (Exception ex)
        {
            _appService.Logger.LogError(ex.Message, ex.InnerException?.Message);
            return Result.Failure(Messages.InternalServerError, ResponseStatus.InternalServerError);
        }
    }
}
