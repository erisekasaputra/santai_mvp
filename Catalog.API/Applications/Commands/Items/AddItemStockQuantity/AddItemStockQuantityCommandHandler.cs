
using Catalog.API.Applications.Dtos.Item; 
using Catalog.API.Applications.Services;
using Catalog.API.Extensions;
using Catalog.Domain.SeedWork;
using Core.CustomMessages;
using Core.Exceptions;
using Core.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Polly;
using Polly.Retry;
using System.Data;

namespace Catalog.API.Applications.Commands.Items.AddItemStockQuantity;

public class AddItemStockQuantityCommandHandler : IRequestHandler<AddItemStockQuantityCommand, Result>
{
    private readonly AsyncRetryPolicy _asyncRetryPolicy;
    private readonly IUnitOfWork _unitOfWork;
    public AddItemStockQuantityCommandHandler(IUnitOfWork unitOfWork, ApplicationService service)
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
                                        service.Logger.LogError("Retry {retryCount} encountered an exception: {Message}. Waiting {timeSpan} before next retry.", retryCount, exception.Message, timeSpan));
    }

    public async Task<Result> Handle(AddItemStockQuantityCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _asyncRetryPolicy.ExecuteAsync(async () =>
            {
                try
                {  
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
                        var quantity = request.Items.First(x => x.ItemId == item.Id).Quantity;
                        if (quantity <= 0)
                        {
                            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                            return Result.Failure("Can not set quantity request with zero or negative", ResponseStatus.BadRequest);
                        }

                        try
                        {
                            item.AddStockQuantity(quantity);
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

                    if (itemErrors.Count == 0)
                    {
                        await _unitOfWork.CommitTransactionAsync(cancellationToken);

                        return Result.Success(
                            items.ToItemsDto(),
                            ResponseStatus.Ok);
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
        catch (DomainException ex)
        {
            return Result.Failure(ex.Message, ResponseStatus.BadRequest);
        }
        catch (Exception)
        {
            return Result.Failure(Messages.InternalServerError, ResponseStatus.BadRequest);
        }
    }
}