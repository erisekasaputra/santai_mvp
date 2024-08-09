using Account.API.Applications.Dtos.RequestDtos;
using Account.API.SeedWork;
using MediatR;

namespace Account.API.Applications.Commands.CreateRegularUser;

public record CreateRegularUserCommand(RegularUserRequestDto Request) : IRequest<Result>;
