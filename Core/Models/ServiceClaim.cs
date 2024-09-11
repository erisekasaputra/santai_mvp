using Core.Enumerations;

namespace Core.Models;

public class ServiceClaim
{ 
    public UserType CurrentUserType { get; set; }  

    public ServiceClaim( 
        UserType userType)
    { 
        CurrentUserType = userType;  
    }
}
