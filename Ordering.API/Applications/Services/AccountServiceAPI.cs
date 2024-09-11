using Ordering.API.Applications.Dtos.Responses;
using Ordering.API.Applications.Services.Interfaces; 
using System.Text.Json;

namespace Ordering.API.Applications.Services;

public class AccountServiceAPI : IAccountServiceAPI
{
    private readonly HttpClient _httpClient;

    public AccountServiceAPI(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<UserInfoResponseDto?> GetUserDetail(Guid userId)
    {
        try
        {
            var endpoint = $"/api/v1/users/{userId}"; 

            var response = await _httpClient.GetAsync(endpoint);

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<UserInfoResponseDto>(content);
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
