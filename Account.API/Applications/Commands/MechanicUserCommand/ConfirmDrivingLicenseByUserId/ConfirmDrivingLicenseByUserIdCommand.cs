using Account.API.SeedWork;
using MediatR;

namespace Account.API.Applications.Commands.MechanicUserCommand.ConfirmDrivingLicenseByUserId;

public record ConfirmDrivingLicenseByUserIdCommand(Guid UserId, Guid DrivingLicenseId) : IRequest<Result>;