using Account.API.Applications.Services.Interfaces;
using Core.Results;
using MediatR;
using Polly.Retry;
using System.Data.Common;
using System.Data;
using Microsoft.EntityFrameworkCore;
using Polly;
using Account.Domain.SeedWork;
using Core.Utilities;

namespace Account.API.Applications.Commands.OrderTaskCommand.CancelOrderByUserByOrderId;

public class CancelOrderByUserByOrderIdCommandHandler : IRequestHandler<CancelOrderByUserByOrderIdCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly AsyncRetryPolicy _asyncRetryPolicy;
    private readonly ILogger<CancelOrderByUserByOrderIdCommandHandler> _logger;
    private readonly IMechanicCache _mechanicCache;

    public CancelOrderByUserByOrderIdCommandHandler(
        IUnitOfWork unitOfWork,
        ILogger<CancelOrderByUserByOrderIdCommandHandler> logger,
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

    public async Task<Result> Handle(CancelOrderByUserByOrderIdCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
