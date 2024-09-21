using Core.Results; 

using MediatR;

namespace Account.API.Applications.Commands.MechanicUserCommand.RejectDrivingLicenseByUserId;

public record RejectDrivingLicenseByUserIdCommand(Guid UserId, Guid DrivingLicenseId) : IRequest<Result>;
