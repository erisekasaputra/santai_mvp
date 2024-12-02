namespace Account.API.Applications.Dtos.RequestDtos;

public class UpdateUserProfilePictureRequestDto
{
    public string ImageUrl { get; set; }
    public UpdateUserProfilePictureRequestDto(string imageUrl)
    {
        ImageUrl = imageUrl;    
    }
}
