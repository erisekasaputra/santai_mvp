using Core.Results;
using MediatR;

namespace Account.API.Applications.Commands.BusinessUserCommand.RejectBusinessLicenseByUserId;

public record RejectBusinessLicenseByUserIdCommand(Guid BusinessUserId, Guid BusinessLicenseId) : IRequest<Result>;
