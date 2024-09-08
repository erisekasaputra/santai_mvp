using Core.Results;
using Core.Messages;
using MediatR;

namespace Account.API.Applications.Queries.GetDeviceIdByRegularUserId;

public record GetDeviceIdByRegularUserIdQuery(Guid UserId) : IRequest<Result>;