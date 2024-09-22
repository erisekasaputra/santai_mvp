namespace Core.Enumerations;

public enum OtpProviderType
{
    Sms, 
    Email
}

public static class AllowedOtpProviderType
{
    public static List<OtpProviderType> GetAll
    {
        get
        {
            return [OtpProviderType.Sms, OtpProviderType.Email];
        }
    }

    public static Dictionary<IdentityType, List<OtpProviderType>> GetAllByName = new()
    {
        { IdentityType.PhoneNumber, [OtpProviderType.Sms]},
        { IdentityType.Email, [OtpProviderType.Email]}
    };
}
