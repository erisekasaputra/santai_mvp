using Core.Results;
using MediatR;

namespace Account.API.Applications.Commands.RegularUserCommand.UpdateProfilPictureRegularUser;

public record UpdateProfilePictureRegularUserCommand(
        Guid UserId,
        string Path
    ) : IRequest<Result>;
