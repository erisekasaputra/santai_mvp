
using Account.API.SeedWork;
using MediatR;

namespace Account.API.Applications.Queries.GetDeviceIdByMechanicUserId;

public record GetDeviceIdByMechanicUserIdQuery(Guid UserId) : IRequest<Result>;