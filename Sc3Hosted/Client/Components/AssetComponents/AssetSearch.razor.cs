using Microsoft.AspNetCore.Components;
using Sc3Hosted.Client.Services;
using Sc3Hosted.Shared.Dtos;
using Sc3Hosted.Shared.Enumerations;
namespace Sc3Hosted.Client.Components.AssetComponents;
public partial class AssetSearch : ComponentBase
{
    private IEnumerable<AssetDisplayDto> _assets = new List<AssetDisplayDto>();
    private IEnumerable<AssetDisplayDto> _filteredAssets = new List<AssetDisplayDto>();

    private string _searchString = string.Empty;
    private string _selectedFilters = string.Empty;
    [Inject]
    private IAssetsService AssetsService { get; set; } = null!;

    private Func<AssetDisplayDto, bool> AssetFilter => x => {
        return _searchString != null && typeof(AssetDisplayDto).GetProperties().Any(p => p.GetValue(x)?.ToString()?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true);
    };
    protected override async Task OnInitializedAsync()
    {
        _assets = await AssetsService.GetAssetsAsync();
        _filteredAssets = _assets;

    }
    private static void ShowBtnPress(AssetDisplayDto aDisplayDto)
    {
        aDisplayDto.ShowDetails = !aDisplayDto.ShowDetails;
    }

    private void ResetAssets()
    {
        _filteredAssets = _assets;
        _selectedFilters = string.Empty;
    }

    private void FilterByCategory(string category)
    {
        var count = 0;
        _filteredAssets = _filteredAssets.Where(a => a.Categories!.Any(c => c.Name == category));
        if (!_selectedFilters.Contains(category))
        {
            count = _filteredAssets.Count();
            _selectedFilters += $" {category} ({count})";
        }
    }
    private void FilterByStatus(Status status)
    {
        var count = 0;
        _filteredAssets = _filteredAssets.Where(a => a.Status == status);
        if (!_selectedFilters.Contains(status.ToString()))
        {
            count = _filteredAssets.Count();
            _selectedFilters += $" {status} ({count})";
        }
    }
    private void FilterByModel(string model)
    {
        var count = 0;
        _filteredAssets = _filteredAssets.Where(a => {
            return a.ModelName == model;
        });
        if (!_selectedFilters.Contains(model))
        {
            count = _filteredAssets.Count();
            _selectedFilters += $" {model} ({count})";
        }
    }
    private void FilterByArea(string area)
    {
        var count = 0;
        _filteredAssets = _filteredAssets.Where(a => a.AreaName == area);
        if (!_selectedFilters.Contains(area))
        {
            count = _filteredAssets.Count();
            _selectedFilters += $" {area} ({count})";
        }
    }
    private void FilterBySpace(string space)
    {
        var count = 0;
        _filteredAssets = _filteredAssets.Where(a => a.SpaceName == space);
        if (!_selectedFilters.Contains(space))
        {
            count = _filteredAssets.Count();
            _selectedFilters += $" {space} ({count})";
        }
    }
    private void FilterByCoordinate(string coordinate)
    {
        var count = 0;
        _filteredAssets = _filteredAssets.Where(a => a.CoordinateName == coordinate);
        if (!_selectedFilters.Contains(coordinate))
        {
            count = _filteredAssets.Count();
            _selectedFilters += $" {coordinate} ({count})";
        }
    }
    private void FilterByDevice(string device)
    {
        var count = 0;
        _filteredAssets = _filteredAssets.Where(a => a.DeviceName == device);
        if (!_selectedFilters.Contains(device))
        {
            count = _filteredAssets.Count();
            _selectedFilters += $" {device} ({count})";
        }

    }
}
