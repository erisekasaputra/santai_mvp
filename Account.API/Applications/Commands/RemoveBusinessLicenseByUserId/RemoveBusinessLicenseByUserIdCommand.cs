using Account.API.SeedWork;
using MediatR;

namespace Account.API.Applications.Commands.RemoveBusinessLicenseByUserId;

public record RemoveBusinessLicenseByUserIdCommand(Guid BusinessUserId, Guid BusinessLicenseId) : IRequest<Result>;