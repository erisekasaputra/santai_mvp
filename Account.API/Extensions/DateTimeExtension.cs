using System;
using TimeZoneConverter;

namespace Account.API.Extensions;

public static class DateTimeExtension
{
    /// <summary>
    /// Converts a local DateTime in a specific time zone to UTC.
    /// </summary> 
    /// <param name="timeZoneId">The IANA or Windows time zone ID.</param>
    /// <returns>The corresponding UTC DateTime.</returns>
    public static DateTime FromLocalToUtc(this DateTime dateTime, string timeZoneId)
    { 
        TimeZoneInfo? timeZone;

        try
        { 
            string windowsTimeZoneId = TZConvert.IanaToWindows(timeZoneId);
            timeZone = TimeZoneInfo.FindSystemTimeZoneById(windowsTimeZoneId);
        }
        catch (TimeZoneNotFoundException)
        { 
            try
            {
                timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
            }
            catch (TimeZoneNotFoundException)
            {
                throw new InvalidCastException($"Time zone with id '{timeZoneId}' does not exist");
            }
            catch (InvalidTimeZoneException)
            {
                throw new InvalidCastException($"Time zone with id '{timeZoneId}' is invalid");
            }
        }
        catch (InvalidTimeZoneException)
        {
            throw new InvalidCastException($"Time zone with id '{timeZoneId}' is invalid");
        }
        catch (Exception)
        {
            throw new InvalidCastException($"Time zone with id '{timeZoneId}' is invalid");
        }

        DateTime specifiedDateTime = DateTime.SpecifyKind(dateTime, DateTimeKind.Unspecified);
         
        DateTime utcDateTime = TimeZoneInfo.ConvertTimeToUtc(specifiedDateTime, timeZone);

        utcDateTime = DateTime.SpecifyKind(utcDateTime, DateTimeKind.Utc);

        return utcDateTime;
    }

    /// <summary>
    /// Converts a UTC DateTime to a specific time zone.
    /// </summary> 
    /// <param name="timeZoneId">The IANA or Windows time zone ID.</param>
    /// <returns>The corresponding local DateTime in the specified time zone.</returns>

    public static DateTime FromUtcToLocal(this DateTime dateTime, string timeZoneId)
    {
        TimeZoneInfo? timeZone;

        try
        {
            // Konversi IANA time zone ke Windows time zone ID
            string windowsTimeZoneId = TZConvert.IanaToWindows(timeZoneId);
            timeZone = TimeZoneInfo.FindSystemTimeZoneById(windowsTimeZoneId);
        }
        catch (TimeZoneNotFoundException)
        {
            try
            {
                // Jika konversi IANA ke Windows gagal, coba langsung dengan timeZoneId yang diberikan
                timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
            }
            catch (TimeZoneNotFoundException)
            {
                throw new InvalidCastException($"Time zone dengan id '{timeZoneId}' tidak ditemukan.");
            }
            catch (InvalidTimeZoneException)
            {
                throw new InvalidCastException($"Time zone dengan id '{timeZoneId}' tidak valid.");
            }
        }
        catch (InvalidTimeZoneException)
        {
            throw new InvalidCastException($"Time zone dengan id '{timeZoneId}' tidak valid.");
        }

        // Pastikan DateTime yang diberikan sudah dalam UTC
        if (dateTime.Kind != DateTimeKind.Utc && dateTime.Kind != DateTimeKind.Unspecified)
        {
            throw new ArgumentException("Datetime should be in utc kind.", nameof(dateTime));
        }

        dateTime = DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);

        // Konversi dari UTC ke zona waktu spesifik
        return TimeZoneInfo.ConvertTimeFromUtc(dateTime, timeZone);
    }


    /// <summary>
    /// Converts a local DateTime in a specific time zone to another time zone.
    /// </summary>
    /// <param name="localDateTime">The local DateTime to convert.</param>
    /// <param name="fromTimeZoneId">The IANA or Windows time zone ID of the original time zone.</param>
    /// <param name="toTimeZoneId">The IANA or Windows time zone ID of the target time zone.</param>
    /// <returns>The corresponding local DateTime in the target time zone.</returns>
    public static DateTime ConvertTimeZone(DateTime localDateTime, string fromTimeZoneId, string toTimeZoneId)
    {
        TimeZoneInfo fromTimeZone = GetTimeZoneInfo(fromTimeZoneId);
        TimeZoneInfo toTimeZone = GetTimeZoneInfo(toTimeZoneId);

        DateTime utcDateTime = TimeZoneInfo.ConvertTimeToUtc(localDateTime, fromTimeZone);
        return TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, toTimeZone);
    }

    /// <summary>
    /// Gets a TimeZoneInfo object from an IANA or Windows time zone ID.
    /// </summary>
    /// <param name="timeZoneId">The IANA or Windows time zone ID.</param>
    /// <returns>The TimeZoneInfo object.</returns>
    private static TimeZoneInfo GetTimeZoneInfo(string timeZoneId)
    {
        try
        {
            // Try converting from IANA to Windows time zone ID
            string windowsTimeZoneId = TZConvert.IanaToWindows(timeZoneId);
            return TimeZoneInfo.FindSystemTimeZoneById(windowsTimeZoneId);
        }
        catch (TimeZoneNotFoundException)
        {
            // If IANA to Windows conversion fails, try directly using the provided ID
            return TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
        }
        catch (InvalidTimeZoneException)
        {
            throw new InvalidCastException($"Time zone with id '{timeZoneId}' is invalid");
        }
    }
    
    public static bool IsTimeZoneExists(string timeZoneId)
    { 
        try
        {
            string windowsTimeZoneId = TZConvert.IanaToWindows(timeZoneId);
            TimeZoneInfo.FindSystemTimeZoneById(windowsTimeZoneId);
            return true;
        }
        catch (TimeZoneNotFoundException)
        {
            try
            {
                TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
                return true;
            }
            catch (TimeZoneNotFoundException)
            {
                return false;
            }
            catch (InvalidTimeZoneException)
            {
                return false;
            }
        }
        catch (InvalidTimeZoneException)
        {
            return false;
        }
        catch (Exception)
        {
            return false;
        } 
    }
}
