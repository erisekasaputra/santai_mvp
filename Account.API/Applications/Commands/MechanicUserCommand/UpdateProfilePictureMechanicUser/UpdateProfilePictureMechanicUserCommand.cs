using Core.Results;
using MediatR;

namespace Account.API.Applications.Commands.MechanicUserCommand.UpdateProfilePictureMechanicUser;

public record UpdateProfilePictureMechanicUserCommand(
        Guid UserId,
        string Path
    ) : IRequest<Result>;
