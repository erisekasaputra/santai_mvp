using Ordering.API.Applications.Dtos.Responses;
using Ordering.API.Applications.Services.Interfaces; 
using System.Text.Json;

namespace Ordering.API.Applications.Services;

public class CatalogServiceAPI : ICatalogServiceAPI
{
    private readonly HttpClient _httpClient;

    public CatalogServiceAPI(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<(IEnumerable<CatalogItemsResponseDto>?, string? errorMessage)> SubstractStockAndGetDetailItems(
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

            if (response.StatusCode == System.Net.HttpStatusCode.UnprocessableEntity)
            {
                return (null, "");
            } 

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();

            return (JsonSerializer.Deserialize<IEnumerable<CatalogItemsResponseDto>?>(content), null);
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
