namespace Account.API.Applications.Dtos.RequestDtos;

public class UpdateRegularUserProfilePictureRequestDto
{
    public string ImageUrl { get; set; }
    public UpdateRegularUserProfilePictureRequestDto(string imageUrl)
    {
        ImageUrl = imageUrl;    
    }
}
