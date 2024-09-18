using Ordering.API.Applications.Dtos.Responses;

namespace Ordering.API.Applications.Services.Interfaces;

public interface IMasterDataServiceAPI
{
    Task<IEnumerable<CancellationFeeResponseDto>?> GetCancellationFeeParametersMaster(); 
    Task<IEnumerable<PreServiceInspectionResponseDto>?> GetPreServiceInspectionMaster();
    Task<IEnumerable<FeeResponseDto>?> GetFeeParametersMaster();
    Task<IEnumerable<BasicInspectionResponseDto>?> GetBasicInspectionMaster();
    Task<MasterDataInitializationMasterResponseDto?> GetMasterDataInitializationMasterResponseDto();
}
