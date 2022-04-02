using Sc3Hosted.Shared.Dtos;

namespace Sc3Hosted.Client.Components.LocationComponents;

public partial class LocationGrid
{
    private List<LocationDto> _locations=new();

    string _searchString = string.Empty;

    private Func<LocationDto, bool> AreaFilter => x =>
    {
        if (string.IsNullOrWhiteSpace(_searchString))
            return true;
        if (x.Area?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) ?? false)
            return true;
        return false;
    };

    private Func<LocationDto, bool> SpaceFilter => x =>
    {
        if (string.IsNullOrWhiteSpace(_searchString))
            return true;

        if (x.Area?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) ?? false)
            return true;
        if (x.Space?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) ?? false)
            return true;
        return false;

    };
    private Func<LocationDto, bool> CoordinateFilter => x =>
    {
        return _searchString != null && typeof(LocationDto).GetProperties().Any(p => p.GetValue(x)?.ToString()?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true);
    };

    protected override void OnInitialized()
    {
        _locations = new()
        {
            new()
            {

                AreaId = 1,
                Area = "Area 1",
                PlantId = 1,
                Plant = "Plant 1",
                SpaceId = 1,
                Space = "Space 1",
                CoordinateId = 1,
                Coordinate = "Coordinate 1"
            }
        };
    }
}
