namespace Catalog.API.Applications.Commands.Items.ReduceItemSoldQuantity;

public record ReduceItemSoldQuantityRequest(string ItemId, int Quantity);
