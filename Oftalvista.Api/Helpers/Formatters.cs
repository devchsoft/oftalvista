using System.Globalization;

namespace Oftalvista.Api.Helpers;

public static class Formatters
{
    public static string ToDateText(DateOnly value) =>
        value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);

    public static string? ToDateText(DateOnly? value) =>
        value?.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);

    public static string ToDateText(DateTime value) =>
        value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);

    public static string? ToDateText(DateTime? value) =>
        value?.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);

    public static string ToTimeText(TimeOnly value) =>
        value.ToString("HH:mm", CultureInfo.InvariantCulture);

    public static DateOnly ParseDateOnly(string value) =>
        DateOnly.Parse(value, CultureInfo.InvariantCulture);

    public static DateOnly? ParseNullableDateOnly(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        return ParseDateOnly(value);
    }

    public static DateTime ParseDateTime(string value) =>
        DateTime.Parse(value, CultureInfo.InvariantCulture);

    public static DateTime? ParseNullableDateTime(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        return DateTime.Parse(value, CultureInfo.InvariantCulture);
    }

    public static TimeOnly ParseTimeOnly(string value) =>
        TimeOnly.Parse(value, CultureInfo.InvariantCulture);

    public static DateTime? ParseDateRangeStart(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        return DateTime.Parse(value, CultureInfo.InvariantCulture).Date;
    }

    public static DateTime? ParseDateRangeEnd(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        return DateTime.Parse(value, CultureInfo.InvariantCulture).Date.AddDays(1);
    }
}
