namespace Api.Common;

internal static class Timestamp
{
    public static long DateTimeToUnixTimestamp(this DateTime dateTime)
    {
        var timestamp = new DateTimeOffset(dateTime).ToUnixTimeSeconds();

        return timestamp;
    }

    public static DateTime UnixTimeStampToDateTime(this long unixTimestamp)
    {
        var nineteenSeventy = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        var convertedTime = nineteenSeventy.AddSeconds(unixTimestamp).ToLocalTime();

        return convertedTime;
    }
}