using Account.API.Applications.Dtos.RequestDtos;
using Account.API.SeedWork;
using MediatR;

namespace Account.API.Applications.Commands.CreateBusinessLicenseByUserId;

public record CreateBusinessLicenseByUserIdCommand(Guid BusinessUserId, BusinessLicenseRequestDto Request) : IRequest<Result>;