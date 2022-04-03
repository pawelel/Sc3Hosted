using Sc3Hosted.Shared.Dtos;

using System.Net.Http.Json;

namespace Sc3Hosted.Client.Services;

public interface IPlantService
{
    Task<List<PlantDto>> GetPlants();
    Task<PlantDto> GetPlant(int id);
    Task<PlantCreateDto> CreatePlant(PlantCreateDto plant);
    Task<PlantUpdateDto> UpdatePlant(PlantUpdateDto plant);
    Task<bool> DeletePlant(int id);
    
}

public class PlantService : IPlantService
{
    private readonly HttpClient _http;
    private readonly ILogger<PlantService> _logger;

    public PlantService(HttpClient http, ILogger<PlantService> logger)
    {
        _http = http;
        _logger = logger;
    }

    public async Task<List<PlantDto>> GetPlants()
    {
        throw new NotImplementedException();
    }

    public async Task<PlantDto> GetPlant(int id)
    {
        try
        {
            var result = await _http.GetFromJsonAsync<PlantDto>($"/api/Plant/{id}");
            return result!;
        }
        catch (Exception ex)
        {
            _logger.LogError("Error getting plant: {ex}", ex);
            return null!;
        }
    }

    public async Task<PlantCreateDto> CreatePlant(PlantCreateDto plant)
    {
        throw new NotImplementedException();
    }

    public async Task<PlantUpdateDto> UpdatePlant(PlantUpdateDto plant)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> DeletePlant(int id)
    {
        throw new NotImplementedException();
    }
}