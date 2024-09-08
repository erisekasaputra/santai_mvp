using Account.API.Applications.Services;
using Account.API.Applications.Services.Interfaces;
using Core.Results;
using Account.Domain.Exceptions;
using Account.Domain.SeedWork;
using MediatR;
using Microsoft.Extensions.Options;
using Core.Messages;

namespace Account.API.Applications.Commands.MechanicUserCommand.DeleteMechanicUserByUserId;

public class DeleteMechanicUserByUserIdCommandHandler : IRequestHandler<DeleteMechanicUserByUserIdCommand, Result>
{

    private readonly IUnitOfWork _unitOfWork; 
    private readonly ApplicationService _service;
    private readonly IKeyManagementService _kmsClient;
    private readonly IHashService _hashService;

    public DeleteMechanicUserByUserIdCommandHandler(
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


    public async Task<Result> Handle(DeleteMechanicUserByUserIdCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var mechanicUser = await _unitOfWork.BaseUsers.GetMechanicUserByIdAsync(request.UserId);

            if (mechanicUser is null)
            {
                return Result.Failure($"Mechanic user not found", ResponseStatus.NotFound)
                     .WithError(new("MechanicUser.Id", "Mechanic user not found"));
            }

            mechanicUser.Delete();

            _unitOfWork.BaseUsers.Delete(mechanicUser);

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
