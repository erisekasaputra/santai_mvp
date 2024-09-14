using Account.API.Applications.Services.Interfaces;
using Core.Results;
using MediatR;
using Polly.Retry;
using System.Data.Common;
using System.Data;
using Account.Domain.SeedWork;
using Microsoft.EntityFrameworkCore;
using Polly;
using Core.Utilities;

namespace Account.API.Applications.Commands.OrderTaskCommand.CancelOrderByMechanicByIdByOrderId;

public class CancelOrderByMechanicByIdByOrderIdCommandHandler : IRequestHandler<CancelOrderByMechanicByIdByOrderIdCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly AsyncRetryPolicy _asyncRetryPolicy;
    private readonly ILogger<CancelOrderByMechanicByIdByOrderIdCommandHandler> _logger;
    private readonly IMechanicCache _mechanicCache;

    public CancelOrderByMechanicByIdByOrderIdCommandHandler(
        IUnitOfWork unitOfWork,
        ILogger<CancelOrderByMechanicByIdByOrderIdCommandHandler> logger,
        IMechanicCache mechanicCache)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mechanicCache = mechanicCache;
        _asyncRetryPolicy = Policy
            .Handle<DBConcurrencyException>()
            .Or<DbUpdateException>()
            .Or<DbException>()
            .WaitAndRetryAsync(3, retryAttempt =>
                TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                onRetry: (exception, timeSpan, retryCount, context) =>
                {
                    LoggerHelper.LogError(logger, exception);
                });
    }

    public async Task<Result> Handle(CancelOrderByMechanicByIdByOrderIdCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
