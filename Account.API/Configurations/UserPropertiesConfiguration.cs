namespace Account.API.Configurations;

public class UserPropertiesConfiguration
{

    public int EmailMaxLength { get; set; } 

    public int EmailMinLength { get; set; }

    public int PhoneNumberMaxLength { get; set; }   
    
    public int PhoneNumberMinLength { get; set; }

    public int UsernameMaxLength { get; set; }

    public int UsernameMinLength { get; set; } 

    public int NameMaxLength { get; set; }

    public int NameMinLength { get; set; } 
}

public class PersonalInfoPropertiesConfiguration
{ 
    public int FirstNameMaxLength { get; set; }

    public int FirstNameMinLength { get; set; }

    public int MiddleNameMaxLength { get; set; }

    public int MiddleNameMinLength { get; set; }

    public int LastNameMaxLength { get; set; }

    public int LastNameMinLength { get; set; }

    public int ProfilePictureUrlMaxLength { get; set; }

    public int ProfilePictureUrlMinLength { get; set; }
}

public class BusinessLicensePropertiesConfiguration
{
    public int BusinessLicenseNameMaxLength { get; set; }

    public int BusinessLicenseNameMinLength { get; set; }

    public int BusinessLicenseDescriptionMaxLength { get; set; }

    public int BusinessLicenseDescriptionMinLength { get; set; }

    public int BusinessLicenseNumberMaxLength { get; set; }

    public int BusinessLicenseNumberMinLength { get; set; }
}

public class BusinessUserPropertiesConfiguration
{
    public int BusinessNameMaxLength { get; set; }

    public int BusinessNameMinLength { get; set; }

    public int TaxIdMaxLength { get; set; }

    public int TaxIdMinLength { get; set; }

    public int WebsiteUrlMaxLength { get; set; }

    public int WebsiteUrlMinLength { get; set; }

    public int DescriptionMaxLength { get; set; }

    public int DescriptionMinLength { get; set; }
}

public class AddressPropertiesConfiguration
{
    public int AddressLineMaxLength { get; set; }

    public int AddressLineMinLength { get; set; }

    public int AddressCityMaxLength { get; set; }

    public int AddressCityMinLength { get; set; }

    public int AddressStateMaxLength { get; set; }

    public int AddressStateMinLength { get; set; }

    public int AddressPostalCodeMaxLength { get; set; }

    public int AddressPostalCodeMinLength { get; set; }

    public int AddressCountryMaxLength { get; set; }

    public int AddressCountryMinLength { get; set; }
}

public class CertificationPropertiesConfiguration
{
    
}

public class IdentificationPropertiesConfiguration
{  
    public int IdentificationNumberMaxLength { get; set; }

    public int IdentificationNumberMinLength { get; set; }  

    public int FrontSideImageUrlMaxLength { get; set; }
    
    public int FrontSideImageUrlMinLength { get; set; }

    public int BackSideImageUrlMaxLength { get; set; }
    
    public int BackSideImageUrlMinLength { get; set; }
}