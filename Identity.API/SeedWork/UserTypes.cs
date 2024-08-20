using System.Collections.ObjectModel;

namespace Identity.API.SeedWork;

public class UserTypes
{
    public const string AdministratorRole = "Administrator";
    public const string RegularUserRole = "RegularUser";
    public const string BusinessUserRole = "BusinessUser";
    public const string MechanicUserRole = "MechanicUser";
    public const string StaffUserRole = "StaffUser"; 

    private static readonly List<string> _allowed = [RegularUserRole, MechanicUserRole];

    private static readonly List<string> allTypes = [RegularUserRole, MechanicUserRole, AdministratorRole, StaffUserRole];
    public static ReadOnlyCollection<string> AllTypes() => allTypes.AsReadOnly();
    public static ReadOnlyCollection<string> AllowedTypes() => _allowed.AsReadOnly();
}
