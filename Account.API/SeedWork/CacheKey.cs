namespace Account.API.SeedWork;

public static class CacheKey
{
    public const string MechanicUserPrefix = "AccountService:MechanicUser";
    public const string RegularUserPrefix = "AccountService:RegularUser";
    public const string BusinessUserPrefix = "AccountService:BusinessUser";
    public const string FleetPrefix = "AccountService:Fleet";
    public const string StaffPrefix = "AccountService:Staff";
    public const string UserEmailPrefix = "AccountService:UserEmail";
    public const string UserPhoneNumberPrefix = "AccountService:UserPhoneNumber";
    public const string UserTimeZonePrefix = "AccountService:UserTimeZone";
    public const string AccountPrefix = "AccountService:Account";


    public static string MechanicOrderBlacklistPrefix(string value) => $"mechanics:order:blacklist:{value}"; 
    public static string RedlockPrefix(string value) => $"redlock:{value}";
    public static string LockMechanicPrefix(string value) => $"lock:mechanic:{value}";
    public static string LockMechanicPenaltyPrefix(string value) => $"lock:mechanic:penalty:{value}";
    public static string LockOrderPrefix(string value) => $"lock:order:{value}";
    public static string MechanicGeo() => $"mechanics:geo";
    public static string MechanicExistencePrefix(string value) => $"mechanics:{value}";
    public static string OrderWaitingMechanicAssignDataPrefix(string value) => $"orders:waiting_mechanic_assign:data:{value}";
    public static string OrderWaitingMechanicAssignQueue() => $"orders:waiting_mechanic_assign:queue";
    public static string OrderWaitingMechanicConfirmDataPrefix(string value) => $"orders:waiting_mechanic_confirm:data:{value}";
    public static string OrderWaitingMechanicConfirmQueue() => $"orders:waiting_mechanic_confirm:queue"; 
}
