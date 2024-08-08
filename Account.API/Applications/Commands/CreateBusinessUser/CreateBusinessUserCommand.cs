using Account.API.Applications.Dtos.RequestDtos; 
using Account.API.SeedWork;
using MediatR;

namespace Account.API.Applications.Commands.CreateBusinessUser;

public record CreateBusinessUserCommand(BusinessUserRequestDto Request) : IRequest<Result>;
