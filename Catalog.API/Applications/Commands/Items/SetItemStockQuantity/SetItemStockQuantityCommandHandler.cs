using Catalog.API.DTOs.ItemStock;
using Catalog.API.SeedWork;
using Catalog.API.Services;
using Catalog.Domain.SeedWork; 
using MediatR;
using Microsoft.EntityFrameworkCore;
using Polly;
using Polly.Retry;
using System.Data;

namespace Catalog.API.Applications.Commands.Items.SetItemStockQuantity;

public class SetItemStockQuantityCommandHandler : IRequestHandler<SetItemStockQuantityCommand, Result<IEnumerable<ItemStockDto>>>
{
    private readonly AsyncRetryPolicy _asyncRetryPolicy;
    private readonly IUnitOfWork _unitOfWork;

    public SetItemStockQuantityCommandHandler(IUnitOfWork unitOfWork, ApplicationService service)
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

    public async Task<Result<IEnumerable<ItemStockDto>>> Handle(SetItemStockQuantityCommand request, CancellationToken cancellationToken)
    {
        var result = await _asyncRetryPolicy.ExecuteAsync(async () =>
        {
            try
            {
                int numberOfErrors = 0;
                var itemErrors = new List<ItemStockDto>();

                await _unitOfWork.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);

                // Extract the item IDs from the request
                var itemIds = request.ItemIds.Select(x => x.ItemId).ToList();

                // Retrieve the items with row locks
                var items = await _unitOfWork.Items.GetItemsWithLockAsync(itemIds);

                // Find the missing item IDs
                var retrievedItemIds = items.Select(item => item.Id).ToHashSet();
                var missingItemRequests = request.ItemIds.Where(x => !retrievedItemIds.Contains(x.ItemId)).ToList();

                if (missingItemRequests.Count > 0)
                {
                    var message = missingItemRequests.Count == 1
                        ? $"There is one missing item"
                        : $"There are {missingItemRequests.Count} missing items";

                    await _unitOfWork.RollbackTransactionAsync(cancellationToken);

                    return Result<IEnumerable<ItemStockDto>>.Failure(message, 404).WithData(missingItemRequests.Select(x =>
                    {
                        return new ItemStockDto(x.ItemId, 0, "Data not found");
                    }).ToList());
                }

                foreach (var item in items)
                {
                    var amount = request.ItemIds.First(x => x.ItemId == item.Id).Quantity;

                    try
                    {
                        item.SetStockQuantity(amount);

                        _unitOfWork.Items.UpdateItem(item);
                    }
                    catch (Exception ex)
                    {
                        itemErrors.Add(new ItemStockDto(item.Id, amount, ex.Message));
                        numberOfErrors++;
                    }
                }

                if (numberOfErrors == 0)
                {
                    await _unitOfWork.CommitTransactionAsync(cancellationToken);
                    return Result<IEnumerable<ItemStockDto>>.SuccessResult([], [], 200);
                }

                var messageError = numberOfErrors == 1
                       ? $"There is one error item"
                       : $"There are {numberOfErrors} error items";

                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                return Result<IEnumerable<ItemStockDto>>.Failure(messageError, 400).WithData(itemErrors);
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
