using Account.API.Applications.Dtos.RequestDtos;
using Account.API.SeedWork;
using MediatR;

namespace Account.API.Applications.Commands.StaffCommand.CreateStaffBusinessUserByUserId;

public record CreateStaffBusinessUserByUserIdCommand(
    Guid Id,
    string Username,
    string PhoneNumber,
    string Email,
    string Name,
    AddressRequestDto Address,
    string TimeZoneId) : IRequest<Result>;