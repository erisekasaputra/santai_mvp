 
using Catalog.API.Applications.Dtos.Item;
using Catalog.Domain.SeedWork;
using Core.Events.Ordering;
using Core.Exceptions;
using Core.Results;
using MassTransit; 
using System.Data;

namespace Catalog.API.Applications.Consumers;

public class OrderFailedRecoveryStockIntegrationEventConsumer(
    IUnitOfWork unitOfWork,
    ILogger<OrderFailedRecoveryStockIntegrationEventConsumer> logger): IConsumer<OrderFailedRecoveryStockIntegrationEvent>
{

    private readonly IUnitOfWork _unitOfWork = unitOfWork; 
    private readonly ILogger<OrderFailedRecoveryStockIntegrationEventConsumer> _logger = logger;
    public async Task Consume(ConsumeContext<OrderFailedRecoveryStockIntegrationEvent> context)
    { 
        await _unitOfWork.BeginTransactionAsync(IsolationLevel.ReadCommitted);
         
        var requestItemIds = context.Message.Items.Select(x => x.Id).ToList();

        var items = await _unitOfWork.Items.GetItemsWithLockAsync(requestItemIds);  

        var itemErrors = new List<ItemDto>();

        foreach (var item in items)
        {
            var quantity = context.Message.Items.First(x => x.Id == item.Id).Quantity;
            if (quantity <= 0)
            {
                continue;
            }

            try
            {
                item.AddStockQuantity(quantity);
                _unitOfWork.Items.UpdateItem(item);
            }
            catch (DomainException)
            {
                continue;
            }
            catch (Exception)
            {
                continue;
            }
        }

        await _unitOfWork.CommitTransactionAsync();  
    }
}
