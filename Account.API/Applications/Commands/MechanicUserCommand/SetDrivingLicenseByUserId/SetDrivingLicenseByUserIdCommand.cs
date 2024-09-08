using Core.Results;
using Core.Messages;
using MediatR;

namespace Account.API.Applications.Commands.MechanicUserCommand.SetDrivingLicenseByUserId;

public record SetDrivingLicenseByUserIdCommand(
    Guid UserId,
    string LicenseNumber,
    string FrontSideImageUrl,
    string BackSideImageUrl) : IRequest<Result>; 
