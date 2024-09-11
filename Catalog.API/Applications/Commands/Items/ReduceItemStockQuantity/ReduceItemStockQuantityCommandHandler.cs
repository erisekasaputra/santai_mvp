using Catalog.Domain.SeedWork;
using MediatR;
using System.Data;
using Polly;
using Polly.Retry;
using Microsoft.EntityFrameworkCore;
using Catalog.API.Applications.Services;
using Core.Results;
using Catalog.API.Applications.Dtos.ItemStock;
using Catalog.API.Extensions;

namespace Catalog.API.Applications.Commands.Items.ReduceItemStockQuantity;

public class ReduceItemStockQuantityCommandHandler : IRequestHandler<ReduceItemStockQuantityCommand, Result>
{
    private readonly AsyncRetryPolicy _asyncRetryPolicy;
    private readonly IUnitOfWork _unitOfWork;

    public ReduceItemStockQuantityCommandHandler(IUnitOfWork unitOfWork, ApplicationService service)
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

    public async Task<Result> Handle(ReduceItemStockQuantityCommand request, CancellationToken cancellationToken)
    {
        var result = await _asyncRetryPolicy.ExecuteAsync(async () =>
        {
            try
            {
                int insufficientItem = 0;
                var itemErrors = new List<ItemStockDto>();

                await _unitOfWork.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);

                // Extract the item IDs from the request
                var itemIds = request.Items.Select(x => x.ItemId).ToList();

                // Retrieve the items with row locks
                var items = await _unitOfWork.Items.GetItemsWithLockAsync(itemIds);

                // Find the missing item IDs
                var retrievedItemIds = items.Select(item => item.Id).ToHashSet();
                var missingItemRequests = request.Items.Where(x => !retrievedItemIds.Contains(x.ItemId)).ToList();

                if (missingItemRequests.Count > 0)
                {
                    var message = missingItemRequests.Count == 1
                        ? $"There is one missing item"
                        : $"There are {missingItemRequests.Count} missing items";

                    await _unitOfWork.RollbackTransactionAsync(cancellationToken);

                    return Result.Failure(message, ResponseStatus.UnprocessableEntity)
                    .WithData(missingItemRequests.Select(x =>
                    {
                        return new ItemStockDto(x.ItemId, 0, "Data not found");
                    }).ToList());
                }

                foreach (var item in items)
                {
                    var quantity = request.Items.First(x => x.ItemId == item.Id).Quantity;

                    try
                    {
                        item.ReduceStockQuantity(quantity);
                        _unitOfWork.Items.UpdateItem(item);
                    }
                    catch (Exception ex)
                    {
                        itemErrors.Add(new ItemStockDto(item.Id, quantity, ex.Message));
                        insufficientItem++;
                    }
                }

                if (insufficientItem == 0)
                {
                    await _unitOfWork.CommitTransactionAsync(cancellationToken);
                    return Result.Success(items.ToItemsDto(), ResponseStatus.Ok);
                }

                var messageInsufficient = insufficientItem == 1
                       ? $"There is one error item"
                       : $"There are {insufficientItem} error items";

                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                return Result.Failure(messageInsufficient, ResponseStatus.UnprocessableEntity)
                    .WithData(itemErrors);
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
