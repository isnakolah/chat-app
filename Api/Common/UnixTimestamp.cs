namespace Api.Common;

internal static class Timestamp
{
    public static long ToMillisecondsTimestamp(this DateTime dateTime)
    {
        var milliseconds = new DateTimeOffset(dateTime).ToUnixTimeMilliseconds();

        return milliseconds;
    }

    public static DateTime ToDateTime(this long milliseconds)
    {
        var nineteenSeventy = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        var convertedTime = nineteenSeventy.AddMilliseconds(milliseconds).ToLocalTime();

        return convertedTime;
    }
}