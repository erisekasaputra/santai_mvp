using Core.Validations;
using PhoneNumbers;

namespace Core.Extensions;
public static class PhoneNumberExtension
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
             
            var number = phoneNumberUtil.Parse(phoneNumber, regionCode);
             
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