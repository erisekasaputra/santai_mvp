using Account.API.Applications.Dtos.ResponseDtos;
using Account.Domain.Aggregates.UserAggregate;

namespace Account.API.Mapper;

public static class StaffMapperExtension
{
    public static IEnumerable<StaffResponseDto>? ToStaffResponseDtos(this ICollection<Staff>? staffs)
    {
        if (staffs is null)
        {
            yield break;   
        }
         
        foreach(var staff in staffs)
        {
            if (staff is not null)
            { 
                yield return staff.ToStaffResponseDto();
            }
        }
    }

    public static StaffResponseDto ToStaffResponseDto(this Staff staff)
    {
        return new StaffResponseDto(
            staff.Id,
            staff.Username,
            staff.PhoneNumber,
            staff.Email,
            staff.Name,
            staff.Address.ToAddressResponseDto(),
            staff.TimeZoneId);
    }
}
