using Core.Results;
using MediatR;

namespace Account.API.Applications.Commands.MechanicUserCommand.ConfirmNationalIdentityByUserId;

public record ConfirmNationalIdentityByUserIdCommand(Guid UserId, Guid NationalIdentityId) : IRequest<Result>;