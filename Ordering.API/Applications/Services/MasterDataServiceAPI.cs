
using Newtonsoft.Json;
using Ordering.API.Applications.Dtos.Responses;
using Ordering.API.Applications.Services.Interfaces;
using System.Text.RegularExpressions;

namespace Ordering.API.Applications.Services;

public class MasterDataServiceAPI : IMasterDataServiceAPI
{
    private readonly HttpClient _httpClient;
    public MasterDataServiceAPI(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    public async Task<List<BasicInspectionResponseDto>?> GetBasicInspectionMaster()
    {
        try
        {
            var endpoint = $"/api/v1/master/order/basic-inspection";

            var response = await _httpClient.GetAsync(endpoint);
            string content = Regex.Unescape(await response.Content.ReadAsStringAsync()).Trim('"');
            response.EnsureSuccessStatusCode();
            return JsonConvert.DeserializeObject<List<BasicInspectionResponseDto>>(content);
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<CancellationFeeResponseDto?> GetCancellationFeeParametersMaster()
    {
        try
        {
            var endpoint = $"/api/v1/master/order/cancellation-fee";

            var response = await _httpClient.GetAsync(endpoint);
            string content = Regex.Unescape(await response.Content.ReadAsStringAsync()).Trim('"');
            response.EnsureSuccessStatusCode();
            return JsonConvert.DeserializeObject<CancellationFeeResponseDto>(content);
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<List<FeeResponseDto>?> GetFeeParametersMaster()
    {
        try
        {
            var endpoint = $"/api/v1/master/order/service-fee";

            var response = await _httpClient.GetAsync(endpoint);
            string content = Regex.Unescape(await response.Content.ReadAsStringAsync()).Trim('"');
            response.EnsureSuccessStatusCode();
            return JsonConvert.DeserializeObject<List<FeeResponseDto>>(content);
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<MasterDataInitializationMasterResponseDto?> GetMasterDataInitializationMasterResponseDto()
    {
        try
        {
            var endpoint = $"/api/v1/master/order";

            var response = await _httpClient.GetAsync(endpoint);
            string content = Regex.Unescape(await response.Content.ReadAsStringAsync()).Trim('"');
            response.EnsureSuccessStatusCode();
            return JsonConvert.DeserializeObject<MasterDataInitializationMasterResponseDto>(content);
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<List<PreServiceInspectionResponseDto>?> GetPreServiceInspectionMaster()
    {
        try
        {
            var endpoint = $"/api/v1/master/order/pre-service-inspection";
             
            var response = await _httpClient.GetAsync(endpoint);
            string content = Regex.Unescape(await response.Content.ReadAsStringAsync()).Trim('"'); 
            response.EnsureSuccessStatusCode();
            return JsonConvert.DeserializeObject<List<PreServiceInspectionResponseDto>>(content);
        } 
        catch (Exception)
        {
            throw;
        }
    }
}
