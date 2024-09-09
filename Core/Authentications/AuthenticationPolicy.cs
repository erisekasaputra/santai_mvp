using Core.Enumerations;
using Core.SeedWorks;

namespace Core.Authentications;

public class AuthenticationPolicy
{
    public PolicyName PolicyName { get; set; }
    public UserType[] PolicyRole { get; set; } = [];
}
