using Core.Results;
using Core.Messages;
using MediatR;

namespace Account.API.Applications.Commands.MechanicUserCommand.SetRatingByUserId;

public record SetRatingByUserIdCommand(Guid UserId, decimal Rating) : IRequest<Result>;
