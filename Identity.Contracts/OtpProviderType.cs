namespace Identity.Contracts;

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
}
