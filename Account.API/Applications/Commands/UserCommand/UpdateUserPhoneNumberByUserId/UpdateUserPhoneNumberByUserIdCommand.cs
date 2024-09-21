using Core.Results; 
using MediatR;

namespace Account.API.Applications.Commands.UserCommand.UpdateUserPhoneNumberByUserId;

public record UpdateUserPhoneNumberByUserIdCommand(Guid Id, string PhoneNumber) : IRequest<Result>;
