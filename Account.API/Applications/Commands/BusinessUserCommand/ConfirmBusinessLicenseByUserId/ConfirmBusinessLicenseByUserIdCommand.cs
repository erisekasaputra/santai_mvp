using Account.API.SeedWork;
using MediatR;

namespace Account.API.Applications.Commands.BusinessUserCommand.ConfirmBusinessLicenseByUserId;

public record ConfirmBusinessLicenseByUserIdCommand(Guid BusinessUserId, Guid BusinessLicenseId) : IRequest<Result>;