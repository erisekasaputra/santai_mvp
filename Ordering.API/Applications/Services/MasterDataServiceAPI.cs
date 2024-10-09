
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
            
            var data = JsonConvert.DeserializeObject<ResultResponseDto<List<BasicInspectionResponseDto>>>(content);

            if (data is null)
            {
                return null;
            }

            return data.Data;
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
            var data = JsonConvert.DeserializeObject<ResultResponseDto<CancellationFeeResponseDto>>(content);

            if (data is null)
            {
                return null;
            }

            return data.Data;
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
            var data = JsonConvert.DeserializeObject<ResultResponseDto<List<FeeResponseDto>>>(content);

            if (data is null)
            {
                return null;
            }

            return data.Data;   
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
            var data = JsonConvert.DeserializeObject<ResultResponseDto<MasterDataInitializationMasterResponseDto>>(content);
        
            if (data is null)
            {
                return null;
            }

            return data.Data;
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
            var data = JsonConvert.DeserializeObject<ResultResponseDto<List<PreServiceInspectionResponseDto>>>(content);
            
            if (data is null)
            {
                return null;
            }

            return data.Data;   
        } 
        catch (Exception)
        {
            throw;
        }
    }
}
