namespace Core.Enumerations;

public enum UserType
{
    Administrator = 1,
    RegularUser = 2,
    BusinessUser = 3,
    MechanicUser = 4,
    StaffUser = 5,
    ServiceToService = 6
}

public static class GetUserTypeConfiguration
{
    public static IList<UserType> AllowedUserRegisterBySelf
    {
        get
        {
            return [UserType.RegularUser, UserType.MechanicUser];
        }
    }

    public static IList<UserType> AllowedUserRegisterByAdministrator
    {
        get
        {
            return [UserType.Administrator, UserType.BusinessUser, UserType.StaffUser];
        }
    }

    public static IList<UserType> GetAll
    {
        get
        {
            return [UserType.Administrator, UserType.BusinessUser, UserType.StaffUser, UserType.RegularUser, UserType.MechanicUser];
        }
    }
}
