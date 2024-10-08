
using Catalog.API.Applications.Dtos.Item;
using Catalog.API.Applications.Dtos.ItemPrice;
using Catalog.API.Applications.Services;
using Catalog.API.Extensions;
using Catalog.Domain.SeedWork;
using Core.Exceptions;
using Core.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Polly;
using Polly.Retry;
using System.Data;

namespace Catalog.API.Applications.Commands.Items.SetItemPrice;

public class SetItemPriceCommandHandler : IRequestHandler<SetItemPriceCommand, Result>
{
    private readonly AsyncRetryPolicy _asyncRetryPolicy;
    private readonly IUnitOfWork _unitOfWork;
    public SetItemPriceCommandHandler(IUnitOfWork unitOfWork, ApplicationService service)
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


    public async Task<Result> Handle(SetItemPriceCommand request, CancellationToken cancellationToken)
    {
        var result = await _asyncRetryPolicy.ExecuteAsync(async () =>
        {
            try
            {
                int numberOfErrors = 0; 
                await _unitOfWork.BeginTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken);
                 

                // Extract the item IDs from the request
                var requestItemIds = request.Items.Select(x => x.ItemId).ToList();

                var items = await _unitOfWork.Items.GetItemsWithLockAsync(requestItemIds);

                var missingItems = requestItemIds.Except(items.Select(x => x.Id).ToList()).ToList();

                if (missingItems.Count > 0)
                {
                    var message = "There are serveral missing items";

                    await _unitOfWork.RollbackTransactionAsync(cancellationToken);

                    return Result.Failure(message, ResponseStatus.NotFound)
                        .WithData(missingItems.ToFailedItemsDto());
                }




                var itemErrors = new List<ItemDto>();
                foreach (var item in items)
                {
                    var selectedItem = request.Items.First(x => x.ItemId == item.Id); 

                    try
                    {
                        item.SetPrice(selectedItem.Amount, selectedItem.Currency);
                        _unitOfWork.Items.UpdateItem(item);
                    }
                    catch (DomainException)
                    {
                        itemErrors.Add(item.ToItemDto());
                    }
                    catch (Exception)
                    {
                        itemErrors.Add(item.ToItemDto());
                    }
                }

                if (numberOfErrors is 0)
                {
                    await _unitOfWork.CommitTransactionAsync(cancellationToken);
                    return Result.Success(items.ToItemsDto(), ResponseStatus.Ok);
                }

                var messageError = "There are several items with error result";

                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                return Result.Failure(messageError, ResponseStatus.UnprocessableEntity)
                    .WithData(itemErrors);
            }
            catch (DBConcurrencyException)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                throw;
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                throw;
            }
        });

        return result;
    }
}
