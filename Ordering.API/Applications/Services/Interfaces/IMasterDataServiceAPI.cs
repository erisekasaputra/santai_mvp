using Ordering.API.Applications.Dtos.Responses;

namespace Ordering.API.Applications.Services.Interfaces;

public interface IMasterDataServiceAPI
{
    Task<CancellationFeeResponseDto?> GetCancellationFeeParametersMaster(); 
    Task<List<PreServiceInspectionResponseDto>?> GetPreServiceInspectionMaster();
    Task<List<FeeResponseDto>?> GetFeeParametersMaster();
    Task<List<BasicInspectionResponseDto>?> GetBasicInspectionMaster();
    Task<MasterDataInitializationMasterResponseDto?> GetMasterDataInitializationMasterResponseDto();
}
