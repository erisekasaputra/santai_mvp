using Core.Exceptions;
using Ordering.API.Applications.Dtos.Responses;
using Ordering.API.Applications.Services.Interfaces;
using System.Net;
using System.Text.Json;

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

            string content;

            if (response.StatusCode is HttpStatusCode.NotFound or HttpStatusCode.BadRequest)
            {
                content = await response.Content.ReadAsStringAsync(); 
                var result = JsonSerializer.Deserialize<ResultResponseDto<TDataType>>(content); 
                return (null, false);
            }

            response.EnsureSuccessStatusCode(); 
            content = await response.Content.ReadAsStringAsync(); 
            return (JsonSerializer.Deserialize<ResultResponseDto<TDataType>>(content), true);
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
