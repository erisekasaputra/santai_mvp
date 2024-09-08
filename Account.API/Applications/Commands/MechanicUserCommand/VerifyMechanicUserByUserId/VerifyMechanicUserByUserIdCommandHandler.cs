using Account.API.Applications.Services;
using Account.API.Applications.Services.Interfaces;
using Core.Results;
using Core.Messages;
using Account.Domain.Exceptions;
using Account.Domain.SeedWork;
using MediatR;
using Microsoft.Extensions.Options;

namespace Account.API.Applications.Commands.MechanicUserCommand.VerifyMechanicUserByUserId;

public class VerifyMechanicUserByUserIdCommandHandler : IRequestHandler<VerifyMechanicUserByUserIdCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork; 
    private readonly ApplicationService _service;
    private readonly IKeyManagementService _kmsClient;
    private readonly IHashService _hashService;

    public VerifyMechanicUserByUserIdCommandHandler(
        IUnitOfWork unitOfWork, 
        ApplicationService service,
        IKeyManagementService kmsClient,
        IHashService hashService)
    {
        _unitOfWork = unitOfWork;  
        _service = service;
        _kmsClient = kmsClient;
        _hashService = hashService;
    }

    public async Task<Result> Handle(VerifyMechanicUserByUserIdCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var mechanicUser = await _unitOfWork.BaseUsers.GetMechanicUserByIdAsync(request.UserId);

            if (mechanicUser is null)
            {
                return Result.Failure($"Mechanic user not found", ResponseStatus.NotFound)
                     .WithError(new("MechanicUser.Id", "Mechanic user not found"));
            }

            mechanicUser.VerifyDocument();

            _unitOfWork.BaseUsers.Update(mechanicUser);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success(null, ResponseStatus.NoContent);
        }
        catch (DomainException ex)
        {
            return Result.Failure(ex.Message, ResponseStatus.BadRequest);
        }
        catch (Exception ex)
        {
            _service.Logger.LogError(ex, ex.InnerException?.Message);
            return Result.Failure(Messages.InternalServerError, ResponseStatus.InternalServerError);
        }
    }
}
