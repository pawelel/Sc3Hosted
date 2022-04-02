using MudBlazor;

using Sc3Hosted.Shared.Enumerations;

namespace Sc3Hosted.Client.Helpers;

public static class Utils
{
    public static readonly int[] PageSizeOptions = new[] { 5, 10, 25, 50, 100 };


    public static Color SetColor(Status status)
    {
        Color color = Color.Default;
        switch (status)
        {
            case Status.Retired:
                color = Color.Error;
                break;
            case Status.InUse:
                color = Color.Default;
                break;
            case Status.InStock:
                color = Color.Success;
                break;
            case Status.Unknown:
                color = Color.Warning;
                break;
            case Status.InRepair:
                color = Color.Secondary;
                break;
        }
        return color;
    }

    public static string WarnMissingValue(string? style)
    {
        return string.IsNullOrEmpty(style) || style.Contains("Unknown") ? "background-color:#ff9800ff; color" : "";
    }

    public static string MissingTextCheck(string? value)
    {
        return string.IsNullOrEmpty(value) ? "Brak danych" : value;
    }
}