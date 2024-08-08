using Account.API.Applications.Dtos.RequestDtos;
using Account.API.SeedWork;
using MediatR;

namespace Account.API.Applications.Commands.CreateStaffBusinessUserByUserId;

public record CreateStaffBusinessUserByUserIdCommand(Guid Id, StaffRequestDto Staff) : IRequest<Result>;