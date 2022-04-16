using Microsoft.AspNetCore.Mvc;

using Sc3Hosted.Server.Services;
using Sc3Hosted.Shared.Dtos;

namespace Sc3Hosted.Server.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class PlantsController : ControllerBase
{
    private readonly ILocationService _locationService;

    public PlantsController(ILocationService locationService)
    {
        _locationService = locationService;
    }

    [HttpPost]
    public async Task<IActionResult> CreatePlant(PlantCreateDto plantCreateDto)
    {
        var plantId = await _locationService.CreatePlant(plantCreateDto);
        return Created($"api/plants/{plantId}", null);
    }

    [HttpDelete]
    public async Task<IActionResult> DeletePlant(int plantId)
    {
        await _locationService.DeletePlant(plantId);
        return Ok();
    }

    [HttpGet]
    public async Task<IActionResult> GetPlantById(int plantId)
    {
        var plant = await _locationService.GetPlantById(plantId);
        return Ok(plant);
    }

    [HttpGet]
    public async Task<IActionResult> GetPlants()
    {
        var plants = await _locationService.GetPlants();
        return Ok(plants);
    }
    [HttpGet]
    public async Task<IActionResult> GetPlantsWithAreas()
    {
        var plants = await _locationService.GetPlantsWithAreas();
        return Ok(plants);
    }
    [HttpPut]
    public async Task<IActionResult> MarkDeletePlant(int plantId)
    {
        await _locationService.MarkDeletePlant(plantId);
        return NoContent();
    }

    [HttpPut]
    public async Task<IActionResult> UpdatePlant(int plantId, PlantUpdateDto plantUpdateDto)
    {
        await _locationService.UpdatePlant(plantId, plantUpdateDto);
        return NoContent();
    }
}