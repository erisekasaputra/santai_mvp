using Account.API.Applications.Dtos.RequestDtos;
using Account.API.SeedWork;
using MediatR;

namespace Account.API.Applications.Commands.StaffCommand.UpdateStaffByStaffId;

public record UpdateStaffByStaffIdCommand(Guid UserId, Guid StaffId, string Name, AddressRequestDto Address, string TimeZoneId) : IRequest<Result>;
