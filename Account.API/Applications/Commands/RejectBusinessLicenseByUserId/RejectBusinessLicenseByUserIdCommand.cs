using Account.API.SeedWork;
using MediatR;

namespace Account.API.Applications.Commands.RejectBusinessLicenseByUserId;

public record RejectBusinessLicenseByUserIdCommand(Guid BusinessUserId, Guid BusinessLicenseId) : IRequest<Result>;
