namespace Catalog.API.Applications.Commands.Items.SetItemPrice;

public record SetItemPriceRequest(string ItemId, decimal Amount);
