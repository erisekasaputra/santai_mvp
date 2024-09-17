using Core.Exceptions;
using Newtonsoft.Json;
using Ordering.API.Applications.Dtos.Responses;
using Ordering.API.Applications.Services.Interfaces;
namespace Ordering.API.Applications.Services;

public class AccountServiceAPI : IAccountServiceAPI
{
    private readonly HttpClient _httpClient;

    public AccountServiceAPI(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<(ResultResponseDto<TDataType>?, bool isSuccess)> GetUserDetail<TDataType>(Guid userId, IEnumerable<Guid> fleetIds)
    {
        try
        {
            var endpoint = $"/api/v1/users/{userId}"; 

            var response = await _httpClient.GetAsync(endpoint);
              
            response.EnsureSuccessStatusCode(); 
            string content = await response.Content.ReadAsStringAsync(); 
            return (JsonConvert.DeserializeObject<ResultResponseDto<TDataType>>(content), true);
        } 
        catch (Exception ex)
        {
            throw new CatalogServiceHttpRequestException(
                message: "Custom message: Failed to communicate with the Account Service.",
                inner: ex
            );
        }
    } 
}
