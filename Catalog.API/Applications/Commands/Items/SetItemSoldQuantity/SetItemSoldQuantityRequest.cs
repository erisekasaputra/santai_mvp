namespace Catalog.API.Applications.Commands.Items.SetItemSoldQuantity;

public record SetItemSoldQuantityRequest(string ItemId, int Quantity);
