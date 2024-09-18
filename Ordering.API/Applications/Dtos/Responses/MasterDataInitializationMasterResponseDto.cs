namespace Ordering.API.Applications.Dtos.Responses;

public class MasterDataInitializationMasterResponseDto
{
    public IEnumerable<FeeResponseDto> Fees { get; set; }
    public IEnumerable<BasicInspectionResponseDto> BasicInspections { get; set; }
    public IEnumerable<PreServiceInspectionResponseDto> PreServiceInspections { get; set; }
    public MasterDataInitializationMasterResponseDto(
        IEnumerable<FeeResponseDto> fees,
        IEnumerable<BasicInspectionResponseDto> basicInspections,
        IEnumerable<PreServiceInspectionResponseDto> preServiceInspections)
    {
        Fees = fees;
        BasicInspections = basicInspections;
        PreServiceInspections = preServiceInspections;
    }
}
