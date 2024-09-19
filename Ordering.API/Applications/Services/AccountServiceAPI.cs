using Core.Exceptions;
using Newtonsoft.Json;
using Ordering.API.Applications.Dtos.Responses;
using Ordering.API.Applications.Services.Interfaces;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
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
            var endpoint = $"/api/v1/users/{userId}/info";

            var bodyContext = new
            {
                Fleets = fleetIds
            };

            var jsonContent = JsonConvert.SerializeObject(bodyContext);
            var stringContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(endpoint, stringContent);
            var content = Regex.Unescape(await response.Content.ReadAsStringAsync()).Trim('"');

            if (response.StatusCode is HttpStatusCode.NotFound or HttpStatusCode.UnprocessableEntity)
            {
                if (string.IsNullOrWhiteSpace(content))
                {
                    return (null, false);
                }

                return (JsonConvert.DeserializeObject<ResultResponseDto<TDataType>?>(content), false);
            }

            response.EnsureSuccessStatusCode();
            return (JsonConvert.DeserializeObject<ResultResponseDto<TDataType>>(content), true);
        }
        catch (JsonSerializationException ex)
        {
            throw new AccountServiceHttpRequestException(
                message: "Custom message: Failed to convert object in account service.",
                inner: ex
            );
        }
        catch (HttpRequestException ex)
        {
            throw new AccountServiceHttpRequestException(
                message: "Custom message: Failed to communicate with the account service.",
                inner: ex
            );
        }
        catch (Exception)
        {
            throw;
        } 
    } 
}
