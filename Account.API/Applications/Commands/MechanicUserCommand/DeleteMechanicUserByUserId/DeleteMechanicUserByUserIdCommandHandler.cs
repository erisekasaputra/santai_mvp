using Core.Results; 
using Account.Domain.SeedWork;
using MediatR; 
using Core.Messages; 
using Core.Exceptions;
using Account.API.Applications.Services.Interfaces;
using Core.Utilities;
using System.Data;

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
            var mechanicUser = await _unitOfWork.BaseUsers.GetMechanicUserByIdAsync(request.UserId);

            if (mechanicUser is null)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                return Result.Failure($"Mechanic user not found", ResponseStatus.NotFound)
                     .WithError(new("MechanicUser.Id", "Mechanic user not found"));
            }

            mechanicUser.Delete(); 
            await _cache.Deactivate(request.UserId.ToString());  
            _unitOfWork.BaseUsers.Delete(mechanicUser);   
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
