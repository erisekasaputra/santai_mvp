using Account.API.Applications.Dtos.RequestDtos;
using Core.Results; 
using MediatR;

namespace Account.API.Applications.Commands.StaffCommand.CreateStaffBusinessUserByUserId;

public record CreateStaffBusinessUserByUserIdCommand(
    Guid Id, 
    string PhoneNumber,
    string? Email,
    string Name,
    AddressRequestDto Address,
    string TimeZoneId,
    string Password) : IRequest<Result>;