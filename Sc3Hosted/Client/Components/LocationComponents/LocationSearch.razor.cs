//namespace Sc3Hosted.Client.Components.LocationComponents;

//public partial class LocationSearch
//{
//    private readonly IEnumerable<LocationDto> _locations = new List<LocationDto>() {
//       new LocationDto() {
//           AreaName = "Area 1",
//           SpaceName =  "Space 1",
//           CoordinateName = "Location 1"
//       }
//   };
//    string _searchString = string.Empty;
//    private Func<LocationDto, bool> LocationFilter => x =>
//    {
//        return _searchString != null && typeof(LocationDto).GetProperties().Any(p => p.GetValue(x)?.ToString()?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true);
//    };
//}
