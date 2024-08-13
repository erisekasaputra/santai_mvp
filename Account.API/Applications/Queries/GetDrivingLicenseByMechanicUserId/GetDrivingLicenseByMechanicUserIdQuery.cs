using Account.API.SeedWork;
using MediatR;
namespace Account.API.Applications.Queries.GetDrivingLicenseByMechanicUserId;

public record GetDrivingLicenseByMechanicUserIdQuery(Guid UserId) : IRequest<Result>;