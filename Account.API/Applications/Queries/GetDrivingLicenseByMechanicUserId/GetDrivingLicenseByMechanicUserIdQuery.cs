using Core.Results;
using Core.Messages;
using MediatR;
namespace Account.API.Applications.Queries.GetDrivingLicenseByMechanicUserId;

public record GetDrivingLicenseByMechanicUserIdQuery(Guid UserId) : IRequest<Result>;