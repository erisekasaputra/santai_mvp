namespace Account.API.Extensions;

public static class DateTimeExtension
{
    public static DateTime ToUtc(this DateTime dateTime)
    {
        return TimeZoneInfo.ConvertTimeToUtc(dateTime);
    }

    public static DateTime FromUtc(this DateTime dateTime, TimeZoneInfo timeZone) 
    {
        return TimeZoneInfo.ConvertTimeFromUtc(dateTime, timeZone);
    }
}
