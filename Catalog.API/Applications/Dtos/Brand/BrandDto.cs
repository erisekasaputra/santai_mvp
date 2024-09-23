using System.ComponentModel.DataAnnotations;

namespace Catalog.API.Applications.Dtos.Brand;

public record BrandDto(Guid Id, [Required] string Name, [Required] string ImageUrl);