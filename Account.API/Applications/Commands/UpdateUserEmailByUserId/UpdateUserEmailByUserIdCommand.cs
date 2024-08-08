using Account.API.Applications.Dtos.RequestDtos;
using Account.API.SeedWork;
using MediatR;

namespace Account.API.Applications.Commands.UpdateUserEmailByUserId;

public record UpdateUserEmailByUserIdCommand(Guid Id, EmailRequestDto Request) : IRequest<Result>;
