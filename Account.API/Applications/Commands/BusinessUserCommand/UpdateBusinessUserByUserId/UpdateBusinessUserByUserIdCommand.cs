using Account.API.Applications.Dtos.RequestDtos;
using Core.Results;
using MediatR;

namespace Account.API.Applications.Commands.BusinessUserCommand.UpdateBusinessUserByUserId;

public record UpdateBusinessUserByUserIdCommand(
    Guid Id,
    string BusinessName,
    string BusinessImageUrl,
    string ContactPerson,
    string? TaxId,
    string? WebsiteUrl,
    string? Description,
    AddressRequestDto Address,
    string TimeZoneId) : IRequest<Result>;
