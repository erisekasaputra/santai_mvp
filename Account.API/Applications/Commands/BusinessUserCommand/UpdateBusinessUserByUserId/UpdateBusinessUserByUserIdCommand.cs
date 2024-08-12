using Account.API.Applications.Dtos.RequestDtos;
using Account.API.SeedWork;
using MediatR;

namespace Account.API.Applications.Commands.BusinessUserCommand.UpdateBusinessUserByUserId;

public record UpdateBusinessUserByUserIdCommand(
    Guid Id,
    string BusinessName,
    string ContactPerson,
    string? TaxId,
    string? WebsiteUrl,
    string? Description,
    AddressRequestDto Address,
    string TimeZoneId) : IRequest<Result>;
