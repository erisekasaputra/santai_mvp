using Core.Enumerations;
using System.ComponentModel.DataAnnotations;

namespace Catalog.API.Applications.Dtos.ItemPrice;

public record ItemPriceDto(
    Guid ItemId,
    [Required] decimal Amount,
    Currency? Currency,
    [Required] string Message);
