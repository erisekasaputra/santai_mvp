using Core.Results; 
using MediatR;

namespace Account.API.Applications.Commands.MechanicUserCommand.SetNationalIdentityByUserId;

public record SetNationalIdentityByUserIdCommand(
    Guid UserId,
    string IdentityNumber,
    string FrontSideImageUrl,
    string BackSideImageUrl) : IRequest<Result>;
