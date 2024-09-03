namespace Catalog.API.Applications.Commands.Items.AddItemSoldQuantity;

public record AddItemSoldQuantityRequest(Guid ItemId, int Quantity);