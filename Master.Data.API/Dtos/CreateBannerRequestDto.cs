using System.ComponentModel.DataAnnotations;

namespace Master.Data.API.Dtos;

public class CreateBannerRequestDto
{
    [Required(ErrorMessage = "Name is required.")]
    [StringLength(100, ErrorMessage = "Name cannot be longer than 100 characters.")]
    public string Name { get; set; }
    [Required(ErrorMessage = "Description is required.")]
    [StringLength(500, ErrorMessage = "Description cannot be longer than 500 characters.")]
    public string Description { get; set; }
    [Required(ErrorMessage = "ImagePath is required.")]
    public string ImagePath { get; set; }
    public bool IsActive { get; set; }

    public CreateBannerRequestDto(
       string name,
       string description,
       string imagePath,
       bool isActive)
    { 
        Name = name;
        Description = description;
        ImagePath = imagePath;
        IsActive = isActive;
    }
}

 