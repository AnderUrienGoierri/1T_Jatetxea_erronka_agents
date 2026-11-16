namespace Jatetxea.Api.Helpers;

public static class OrduHelper
{
    private static readonly TimeZoneInfo EuskalHerriaTz = GetEuskalHerriaTimeZone();

    private static TimeZoneInfo GetEuskalHerriaTimeZone()
    {
        try
        {
            return TimeZoneInfo.FindSystemTimeZoneById("Europe/Madrid");
        }
        catch
        {
            try
            {
                return TimeZoneInfo.FindSystemTimeZoneById("Romance Standard Time");
            }
            catch
            {
                return TimeZoneInfo.Local;
            }
        }
    }

    public static DateTime EuskalHerriaNow => TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, EuskalHerriaTz);

    public static string NowFormatted => EuskalHerriaNow.ToString("HH:mm:ss");

    public static string ToEuskalHerriaOrdua(DateTime dt)
    {
        if (dt == DateTime.MinValue) return "Ez da konektatu";
        DateTime utc = dt.Kind == DateTimeKind.Utc ? dt : dt.ToUniversalTime();
        return TimeZoneInfo.ConvertTimeFromUtc(utc, EuskalHerriaTz).ToString("HH:mm:ss");
    }
}
