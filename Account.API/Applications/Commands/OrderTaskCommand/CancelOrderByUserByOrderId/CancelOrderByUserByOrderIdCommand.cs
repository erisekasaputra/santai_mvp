using Core.Results;
using MediatR;

namespace Account.API.Applications.Commands.OrderTaskCommand.CancelOrderByUserByOrderId;

public record CancelOrderByUserByOrderIdCommand(Guid OrderId) : IRequest<Result>;
