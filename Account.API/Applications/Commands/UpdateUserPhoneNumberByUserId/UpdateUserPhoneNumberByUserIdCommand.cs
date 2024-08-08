using Account.API.Applications.Dtos.RequestDtos;
using Account.API.SeedWork;
using MediatR;

namespace Account.API.Applications.Commands.UpdateUserPhoneNumberByUserId;

public record UpdateUserPhoneNumberByUserIdCommand(Guid Id, PhoneNumberRequestDto Request) : IRequest<Result>;
