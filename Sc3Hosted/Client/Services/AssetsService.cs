using System.Net.Http.Json;
using System.Text.Json.Serialization;
using System.Text.Json;
using Sc3Hosted.Shared.Dtos;

namespace Sc3Hosted.Client.Services;

public interface IAssetsService
{
    Task<List<AssetDisplayDto>> GetAssetsAsync();
}

public class AssetsService : IAssetsService
{
    private readonly HttpClient _http;
    public IEnumerable<AssetDto> Assets { get; set; }= new List<AssetDto>();
    public AssetsService(HttpClient http)
    {
        _http = http;
    }
    public async Task<List<AssetDisplayDto>> GetAssetsAsync()
    {
        var options = new JsonSerializerOptions()
        {

            WriteIndented = true,
            PropertyNameCaseInsensitive = true
        };

        try
        {
            var response = await _http.GetAsync("/api/assets");
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<List<AssetDisplayDto>>(options);
            return result??new();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return new();
        }
    }
}
