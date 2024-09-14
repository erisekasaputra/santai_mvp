using Core.Results; 
using Account.Domain.SeedWork;
using MediatR; 
using Core.Messages; 
using Core.Exceptions;
using Account.API.Applications.Services.Interfaces;
using Core.Utilities;

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
        await _unitOfWork.BeginTransactionAsync(System.Data.IsolationLevel.ReadCommitted, cancellationToken);
        try
        {
            await _cache.Ping();

            var mechanicUser = await _unitOfWork.BaseUsers.GetMechanicUserByIdAsync(request.UserId);

            if (mechanicUser is null)
            {
                return Result.Failure($"Mechanic user not found", ResponseStatus.NotFound)
                     .WithError(new("MechanicUser.Id", "Mechanic user not found"));
            }

            mechanicUser.Delete();

            await _cache.RemoveGeoAsync(request.UserId);
            await _cache.RemoveHsetAsync(request.UserId); 
            _unitOfWork.BaseUsers.Delete(mechanicUser); 
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            return Result.Success(null, ResponseStatus.NoContent);
        }
        catch (DomainException ex)
        { 
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
