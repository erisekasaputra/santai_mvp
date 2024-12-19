using Core.Results;
using Account.Domain.SeedWork;
using MediatR;
using Core.Exceptions;
using Account.API.Applications.Services.Interfaces;
using Core.Utilities;
using System.Data;
using Core.CustomMessages;

namespace Account.API.Applications.Commands.MechanicUserCommand.DeleteMechanicUserByUserId;

public class DeleteMechanicUserByUserIdCommandHandler : IRequestHandler<DeleteMechanicUserByUserIdCommand, Result>
{

    private readonly IUnitOfWork _unitOfWork;  
    private readonly IMechanicCache _cache;
    private readonly ILogger<DeleteMechanicUserByUserIdCommandHandler> _logger;
    public DeleteMechanicUserByUserIdCommandHandler(
        IUnitOfWork unitOfWork,  
        IMechanicCache mechanicCache,
        ILogger<DeleteMechanicUserByUserIdCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;  
        _cache = mechanicCache;
        _logger = logger;
    }


    public async Task<Result> Handle(DeleteMechanicUserByUserIdCommand request, CancellationToken cancellationToken)
    {
        await _unitOfWork.BeginTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken);
        try
        {
            await _cache.PingAsync(); 
            var mechanic = await _unitOfWork.BaseUsers.GetMechanicUserByIdAsync(request.UserId);

            if (mechanic is null)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                return Result.Failure($"Mechanic user not found", ResponseStatus.NotFound);
            }

            mechanic.Delete(); 
            await _cache.Deactivate(request.UserId.ToString(), mechanic.Name, mechanic.PersonalInfo.ProfilePictureUrl ?? "");  
            _unitOfWork.BaseUsers.Delete(mechanic);   
            await _unitOfWork.CommitTransactionAsync(cancellationToken); 
            return Result.Success(null, ResponseStatus.NoContent);
        }
        catch (DomainException ex)
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            return Result.Failure(ex.Message, ResponseStatus.BadRequest);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            LoggerHelper.LogError(_logger, ex);
            return Result.Failure(Messages.InternalServerError, ResponseStatus.InternalServerError);
        }
    } 
}
