namespace Account.API.Applications.Dtos.ResponseDtos;

public class UserCountResponseDto
{
    public string UserType { get; set; }
    public int UserTotal { get; set; }

    public UserCountResponseDto(string userType, int userTotal)
    {
        UserType = userType;
        UserTotal = userTotal;
    }
}