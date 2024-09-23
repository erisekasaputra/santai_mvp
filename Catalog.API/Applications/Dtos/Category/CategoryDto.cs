using System.ComponentModel.DataAnnotations;

namespace Catalog.API.Applications.Dtos.Category;

public record CategoryDto(Guid Id, [Required] string Name, [Required] string ImageUrl);