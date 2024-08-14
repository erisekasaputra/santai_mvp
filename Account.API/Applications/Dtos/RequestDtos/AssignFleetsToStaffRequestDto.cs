namespace Account.API.Applications.Dtos.RequestDtos;

public record AssignFleetsToStaffRequestDto(IEnumerable<Guid> FleetIds);
