using Core.Enumerations;

namespace Catalog.API.Applications.Commands.Items.SetItemPrice;

public record SetItemPriceRequest(Guid ItemId, decimal Amount, Currency Currency);
