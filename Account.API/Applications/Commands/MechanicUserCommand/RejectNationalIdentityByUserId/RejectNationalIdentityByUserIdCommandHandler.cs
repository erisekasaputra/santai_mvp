using Account.API.Applications.Services;
using Account.API.Applications.Services.Interfaces;
using Core.Results;
using Core.Messages;
using Account.Domain.Exceptions;
using Account.Domain.SeedWork;
using MediatR;
using Microsoft.Extensions.Options;

namespace Account.API.Applications.Commands.MechanicUserCommand.RejectNationalIdentityByUserId;

public class RejectNationalIdentityByUserIdCommandHandler : IRequestHandler<RejectNationalIdentityByUserIdCommand, Result>
{ 
    private readonly IUnitOfWork _unitOfWork; 
    private readonly ApplicationService _service;
    private readonly IKeyManagementService _kmsClient;
    private readonly IHashService _hashService;

    public RejectNationalIdentityByUserIdCommandHandler(
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

    public async Task<Result> Handle(RejectNationalIdentityByUserIdCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var nationalIdentity = await _unitOfWork.NationalIdentities.GetByUserIdAndIdAsync(request.UserId, request.NationalIdentiyId);

            if (nationalIdentity is null)
            {
                return Result.Failure($"National identity not found", ResponseStatus.NotFound)
                     .WithError(new("NationalIdentity.Id", "National identity not found"));
            }

            nationalIdentity.RejectDocument();

            _unitOfWork.NationalIdentities.Update(nationalIdentity);

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
