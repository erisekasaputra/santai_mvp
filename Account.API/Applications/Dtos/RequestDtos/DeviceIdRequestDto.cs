using Account.API.Extensions;

namespace Account.API.Applications.Dtos.RequestDtos;

public class DeviceIdRequestDto(string deviceId)
{
    public string DeviceId { get; set; } = deviceId.Clean();
}
