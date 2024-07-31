using Catalog.API.DTOs.ItemStock;
using Catalog.API.SeedWork;
using Catalog.Domain.SeedWork;
using MediatR;
using System.Data;
using Polly;
using Polly.Retry;
using Catalog.API.Services;
using Microsoft.EntityFrameworkCore;

namespace Catalog.API.Applications.Commands.Items.ReduceStock;

public class ReduceStockCommandHandler : IRequestHandler<ReduceStockCommand, Result<IEnumerable<ItemStockDto>>>
{
    private readonly AsyncRetryPolicy _asyncRetryPolicy;
    private readonly IUnitOfWork _unitOfWork;

    public ReduceStockCommandHandler(IUnitOfWork unitOfWork, ApplicationService service)
    {
        _unitOfWork = unitOfWork;
        _asyncRetryPolicy = Policy
                                .Handle<DBConcurrencyException>()
                                .Or<DbUpdateConcurrencyException>()
                                .Or<DbUpdateException>()
                                .WaitAndRetryAsync(
                                    retryCount: 3,
                                    sleepDurationProvider: attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)),
                                    onRetry: (exception, timeSpan, retryCount, context) =>
                                        service.Logger.LogInformation("Retry {retryCount} encountered an exception: {Message}. Waiting {timeSpan} before next retry.", retryCount, exception.Message, timeSpan));
    }

    public async Task<Result<IEnumerable<ItemStockDto>>> Handle(ReduceStockCommand request, CancellationToken cancellationToken)
    {
        var result = await _asyncRetryPolicy.ExecuteAsync(async () =>
        {
            try
            {
                int insufficientItem = 0;
                var itemErrors = new List<ItemStockDto>();

                await _unitOfWork.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);

                // Extract the item IDs from the request
                var itemIds = request.ItemStocks.Select(x => x.ItemId).ToList();

                // Retrieve the items with row locks
                var items = await _unitOfWork.Items.GetItemsWithLockAsync(itemIds);

                // Find the missing item IDs
                var retrievedItemIds = items.Select(item => item.Id).ToHashSet();
                var missingItemRequests = request.ItemStocks.Where(x => !retrievedItemIds.Contains(x.ItemId)).ToList();

                if (missingItemRequests.Count > 0)
                {
                    var message = missingItemRequests.Count == 1
                        ? $"There is one missing item"
                        : $"There are {missingItemRequests.Count} missing items";

                    await _unitOfWork.RollbackTransactionAsync(cancellationToken);

                    return Result<IEnumerable<ItemStockDto>>.Failure(message, 404).WithData(missingItemRequests.Select(x =>
                    {
                        return new ItemStockDto(x.ItemId, 0);
                    }).ToList());
                }

                foreach (var item in items)
                {
                    var deductRequest = request.ItemStocks.First(x => x.ItemId == item.Id);
                    var deductQuantity = deductRequest.QuantityDeduct;

                    try
                    {
                        item.ReduceStockQuantity(deductQuantity);
                        _unitOfWork.Items.UpdateItem(item);
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        itemErrors.Add(new ItemStockDto(item.Id, deductQuantity));
                        insufficientItem++;
                    }
                }

                if (insufficientItem == 0)
                {
                    await _unitOfWork.CommitTransactionAsync(cancellationToken);
                    return Result<IEnumerable<ItemStockDto>>.SuccessResult([], [], 200);
                }

                var messageInsufficient = insufficientItem == 1
                       ? $"There is one insufficient stock item"
                       : $"There are {insufficientItem} insufficient stock items";

                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                return Result<IEnumerable<ItemStockDto>>.Failure(messageInsufficient, 400).WithData(itemErrors);
            }
            catch (DBConcurrencyException)
            {
                throw;
            }
            catch
            {
                throw;
            }
        });

        return result;
    }
}
