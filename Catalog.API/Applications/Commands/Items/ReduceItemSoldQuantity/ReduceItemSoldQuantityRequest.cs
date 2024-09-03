namespace Catalog.API.Applications.Commands.Items.ReduceItemSoldQuantity;

public record ReduceItemSoldQuantityRequest(Guid ItemId, int Quantity);
