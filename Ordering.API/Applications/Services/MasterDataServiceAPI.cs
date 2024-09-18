using Core.Exceptions;
using Newtonsoft.Json;
using Ordering.API.Applications.Dtos.Responses;
using Ordering.API.Applications.Services.Interfaces;
using System.Net;
using System.Text;

namespace Ordering.API.Applications.Services;

public class MasterDataServiceAPI : IMasterDataServiceAPI
{
    private readonly HttpClient _httpClient;
    public MasterDataServiceAPI(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    public async Task<IEnumerable<BasicInspectionResponseDto>?> GetBasicInspectionMaster()
    {
        try
        {
            var endpoint = $"/api/v1/master/order/basic-inspection";

            var response = await _httpClient.GetAsync(endpoint);
            response.EnsureSuccessStatusCode();
            string content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<IEnumerable<BasicInspectionResponseDto>>(content);
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<IEnumerable<CancellationFeeResponseDto>?> GetCancellationFeeParametersMaster()
    {
        try
        {
            var endpoint = $"/api/v1/master/order/cancellation-fee";

            var response = await _httpClient.GetAsync(endpoint);
            response.EnsureSuccessStatusCode();
            string content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<IEnumerable<CancellationFeeResponseDto>>(content);
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<IEnumerable<FeeResponseDto>?> GetFeeParametersMaster()
    {
        try
        {
            var endpoint = $"/api/v1/master/order/service-fee";

            var response = await _httpClient.GetAsync(endpoint);
            response.EnsureSuccessStatusCode();
            string content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<IEnumerable<FeeResponseDto>>(content);
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
            response.EnsureSuccessStatusCode();
            string content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<MasterDataInitializationMasterResponseDto>(content);
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<IEnumerable<PreServiceInspectionResponseDto>?> GetPreServiceInspectionMaster()
    {
        try
        {
            var endpoint = $"/api/v1/master/order/pre-service-inspection";
             
            var response = await _httpClient.GetAsync(endpoint);
            response.EnsureSuccessStatusCode();
            string content = await response.Content.ReadAsStringAsync(); 
            return JsonConvert.DeserializeObject<IEnumerable<PreServiceInspectionResponseDto>>(content);
        } 
        catch (Exception)
        {
            throw;
        }
    }
}
