
using Core.Enumerations;
using Order.API.Applications.Services.Interfaces; 
using Order.Infrastructure.SeedWorks;
using System.Text.Json;

namespace Order.API.Applications.Services;

public class AccountService : IAccountService
{
    private readonly HttpClient _httpClient;

    public AccountService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<string?> GetTimeZoneByUserIdAsync(Guid userId, string userType)
    {
        try
        {
            var endpoint = "/api/v1/users/time-zone";

            if (userType == UserType.StaffUser.ToString())
            {
                endpoint = "/api/v1/users/business/staffs/time-zone";
            }

            var response = await _httpClient.GetAsync(endpoint);

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<AccountServiceResponseDto<UserTimeZoneResponse>>(content)?.Data?.TimeZoneId;
        }
        catch (HttpRequestException ex)
        {
            throw new Exception("An error occurred while fetching time zone data from the account service.", ex);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while fetching time zone data from the account service.", ex);
        }
    }
}
