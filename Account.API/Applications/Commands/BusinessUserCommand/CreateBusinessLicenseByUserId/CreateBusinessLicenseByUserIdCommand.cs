using Core.Results;
using MediatR;

namespace Account.API.Applications.Commands.BusinessUserCommand.CreateBusinessLicenseByUserId;

public record CreateBusinessLicenseByUserIdCommand(Guid BusinessUserId, string LicenseNumber, string Name, string Description) : IRequest<Result>;