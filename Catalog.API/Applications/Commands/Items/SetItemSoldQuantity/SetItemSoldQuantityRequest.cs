namespace Catalog.API.Applications.Commands.Items.SetItemSoldQuantity;

public record SetItemSoldQuantityRequest(Guid ItemId, int Quantity);
