namespace Identity.Contracts.Enumerations;

public enum OtpProviderType
{
    Sms,
    Whatsapp,
    Email
}

public static class AllowedOtpProviderType
{
    public static List<OtpProviderType> GetAll
    {
        get
        {
            return [OtpProviderType.Sms, OtpProviderType.Whatsapp];
        }
    }

    public static Dictionary<IdentityType, List<OtpProviderType>> GetAllByName = new Dictionary<IdentityType, List<OtpProviderType>>
    {
        { IdentityType.PhoneNumber, [OtpProviderType.Sms, OtpProviderType.Whatsapp]},
        { IdentityType.Email, [OtpProviderType.Email]}
    };
}
