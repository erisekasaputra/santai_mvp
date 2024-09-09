using Core.Authentications;
using Core.Enumerations;
using Core.SeedWorks;

namespace Account.API.SeedWork; 

public static class AuthPolicies
{
    public static IList<AuthenticationClient> GetAuthClients()
    {
        var authenticationClients = new List<AuthenticationClient>()
        {
            new()
            {
                AuthenticationScheme = AuthenticationClientScheme.UserAuthenticationScheme,
                Policies = [
                    new()
                    {
                        PolicyName = PolicyName.BusinessUserPolicy,
                        PolicyRole = [UserType.BusinessUser]
                    },
                    new()
                    { 
                        PolicyName = PolicyName.AdministratorPolicy,
                        PolicyRole = [UserType.Administrator]
                    },
                    new()
                    {
                        PolicyName = PolicyName.StaffUserPolicy,
                        PolicyRole = [UserType.StaffUser]
                    },
                    new()
                    {
                        PolicyName = PolicyName.RegularUserPolicy,
                        PolicyRole = [UserType.RegularUser]
                    },
                    new()
                    {
                        PolicyName = PolicyName.MechanicUserPolicy,
                        PolicyRole = [UserType.MechanicUser]
                    }]
            }
        };

        return authenticationClients;
    }
}
