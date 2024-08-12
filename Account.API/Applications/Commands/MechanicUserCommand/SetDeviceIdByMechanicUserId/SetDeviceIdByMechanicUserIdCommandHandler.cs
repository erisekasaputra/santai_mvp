using Account.API.Options;
using Account.API.SeedWork;
using Account.API.Services;
using Account.Domain.SeedWork;
using MediatR;
using Microsoft.Extensions.Options;

namespace Account.API.Applications.Commands.MechanicUserCommand.SetDeviceIdByMechanicUserId;

public class SetDeviceIdByMechanicUserIdCommandHandler : IRequestHandler<SetDeviceIdByMechanicUserIdCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IOptionsMonitor<ReferralProgramOption> _referralOptions;
    private readonly ApplicationService _service;
    private readonly IKeyManagementService _kmsClient;
    private readonly IHashService _hashService;

    public SetDeviceIdByMechanicUserIdCommandHandler(
        IUnitOfWork unitOfWork,
        IOptionsMonitor<ReferralProgramOption> referralOptions,
        ApplicationService service,
        IKeyManagementService kmsClient,
        IHashService hashService)
    {
        _unitOfWork = unitOfWork;
        _referralOptions = referralOptions;
        _service = service;
        _kmsClient = kmsClient;
        _hashService = hashService;
    }

    public async Task<Result> Handle(SetDeviceIdByMechanicUserIdCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
