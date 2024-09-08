using Core.Results;
using Core.Messages;
using MediatR;

namespace Account.API.Applications.Queries.GetNationalIdentityByMechanicUserId;

public record GetNationalIdentityByMechanicUserIdQuery(Guid UserId) : IRequest<Result>;
