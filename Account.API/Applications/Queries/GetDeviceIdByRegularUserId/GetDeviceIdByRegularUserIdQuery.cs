using Core.Results; 
using MediatR;

namespace Account.API.Applications.Queries.GetDeviceIdByRegularUserId;

public record GetDeviceIdByRegularUserIdQuery(Guid UserId) : IRequest<Result>;