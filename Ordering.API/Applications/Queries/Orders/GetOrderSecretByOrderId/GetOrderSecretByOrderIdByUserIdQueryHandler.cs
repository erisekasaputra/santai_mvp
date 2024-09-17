using Core.Results;
using MediatR;

namespace Ordering.API.Applications.Queries.Orders.GetOrderSecretByOrderId;

public class GetOrderSecretByOrderIdByUserIdQueryHandler : IRequestHandler<GetOrderSecretByOrderIdByUserIdQuery, Result>
{
    public Task<Result> Handle(GetOrderSecretByOrderIdByUserIdQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
