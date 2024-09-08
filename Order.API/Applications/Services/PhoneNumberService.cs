using Core.Validations;
using Order.API.SeedWorks;
using PhoneNumbers;

namespace Order.API.Applications.Services; 

public static class PhoneNumberService
{
    public static string? NormalizePhoneNumber(this string phoneNumber, string regionCode)
    {
        try
        {
            if (!RegionCodeValidation.IsValidRegionCode(regionCode))
            {
                return null;
            }

            var phoneNumberUtil = PhoneNumberUtil.GetInstance();

            // Parse the phone number
            var number = phoneNumberUtil.Parse(phoneNumber, regionCode);

            // Format the number in E.164 format
            return phoneNumberUtil.Format(number, PhoneNumberFormat.E164);
        }
        catch (NumberParseException)
        {
            return null;
        }
        catch (Exception)
        {
            return null;
        }
    }
}