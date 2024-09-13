using Core.Exceptions;
using Newtonsoft.Json;
using Ordering.API.Applications.Dtos.Responses;
using Ordering.API.Applications.Services.Interfaces;
using System.Net; 

namespace Ordering.API.Applications.Services;

public class CatalogServiceAPI : ICatalogServiceAPI
{
    private readonly HttpClient _httpClient; 
    public CatalogServiceAPI(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<(ResultResponseDto<List<CatalogItemResponseDto>>? ResultItemResponse, bool IsSuccess)> SubstractStockAndGetDetailItems(
        IEnumerable<(Guid ItemId, int Quantity)> substractItems)
    {
        try
        {
            var payload = new
            {
                items = substractItems.Select(item => new
                {
                    itemId = item.ItemId,
                    quantity = item.Quantity
                }).ToArray()
            };

            var endpoint = "/api/v1/catalog/items/stock/reduce";

            var response = await _httpClient.PatchAsJsonAsync(endpoint, payload);

            var content = await response.Content.ReadAsStringAsync();

            if (response.StatusCode is HttpStatusCode.NotFound or HttpStatusCode.UnprocessableEntity)
            {
                if (string.IsNullOrWhiteSpace(content))
                {
                    return (null, false);
                }

                return (JsonConvert.DeserializeObject<ResultResponseDto<List<CatalogItemResponseDto>>?>(content), false);
            }

            response.EnsureSuccessStatusCode();  

            return (JsonConvert.DeserializeObject<ResultResponseDto<List<CatalogItemResponseDto>>?>(content), true);
        }
        catch (HttpRequestException ex)
        {
            throw new CatalogServiceHttpRequestException(
                message: "Custom message: Failed to communicate with the Catalog Service.",
                inner: ex
            );
        }
        catch (Exception)
        {
            throw;
        }
    }
}
