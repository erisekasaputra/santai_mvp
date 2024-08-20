using System.Collections.ObjectModel;

namespace Identity.API.SeedWork;

public class UserTypes
{
    public const string AdministratorRole = "administrator";
    public const string RegularUserRole = "regular-user";
    public const string BusinessUserRole = "business-user";
    public const string MechanicUserRole = "mechanic-user";
    public const string StaffUserRole = "staff-user";

    private static Dictionary<string, string> _allTypes = new()
    {
        { AdministratorRole, "Administrator" },
        { RegularUserRole, "RegularUser" },
        { BusinessUserRole, "BusinessUser" },
        { StaffUserRole, "StaffUser" },
        { MechanicUserRole, "MechanicUser" }
    };

    private static readonly List<string> _allowed = [RegularUserRole, MechanicUserRole];
    public static ReadOnlyDictionary<string, string> AllTypes() => _allTypes.AsReadOnly(); 
    public static ReadOnlyCollection<string> AllowedTypes() => _allowed.AsReadOnly();
}
