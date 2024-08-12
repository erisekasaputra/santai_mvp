using Account.API.SeedWork;
using MediatR;

namespace Account.API.Applications.Commands.MechanicUserCommand.RejectNationalIdentityByUserId;

public record RejectNationalIdentityByUserIdCommand(Guid UserId, Guid NationalIdentiyId) : IRequest<Result>;
