using Core.Results;
using Core.Messages;
using MediatR;

namespace Account.API.Applications.Queries.GetPhoneNumberByUserId;

public record GetPhoneNumberByUserIdQuery(Guid UserId) : IRequest<Result>;
