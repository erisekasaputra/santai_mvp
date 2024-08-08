using Account.API.Applications.Dtos.RequestDtos;
using Account.API.Applications.Dtos.ResponseDtos;
using Account.Domain.ValueObjects;

namespace Account.API.Mapper;

public static class AddressMapperExtension
{
    public static IEnumerable<Address> ToAddresses(this IEnumerable<AddressRequestDto> addressDtos)
    {
        foreach (var addressDto in addressDtos)
        {
            yield return addressDto.ToAddress();
        }
    }

    public static Address ToAddress(this AddressRequestDto addressDto)
    {
        return new Address(addressDto.AddressLine1,
                           addressDto.AddressLine2,
                           addressDto.AddressLine3,
                           addressDto.City,
                           addressDto.State,
                           addressDto.PostalCode,
                           addressDto.Country);
    }

    public static IEnumerable<AddressResponseDto> ToAddressResponseDtos(this IEnumerable<Address> addresses)
    {
        foreach (var address in addresses)
        {
            yield return address.ToAddressResponseDto();
        }
    }

    public static AddressResponseDto ToAddressResponseDto(this Address addressDto)
    {
        return new AddressResponseDto(
            addressDto.AddressLine1,
            addressDto.AddressLine2,
            addressDto.AddressLine3,
            addressDto.City,
            addressDto.State,
            addressDto.PostalCode,
            addressDto.Country);
    }
}
