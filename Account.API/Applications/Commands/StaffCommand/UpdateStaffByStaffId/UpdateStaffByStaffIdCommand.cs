using Account.API.Applications.Dtos.RequestDtos;
using Core.Results;
using Core.Messages;
using MediatR;

namespace Account.API.Applications.Commands.StaffCommand.UpdateStaffByStaffId;

public record UpdateStaffByStaffIdCommand(Guid UserId, Guid StaffId, string Name, AddressRequestDto Address, string TimeZoneId) : IRequest<Result>;
