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
    [HttpGet]
    public async Task<IActionResult> GetPlants()
    {
        var plants = await _locationService.GetPlants();
        return Ok(plants);
    }

    [HttpGet]
    public async Task<IActionResult> GetPlantById(int plantId)
    {
        var plant = await _locationService.GetPlantById(plantId);
        return Ok(plant);
    }
    [HttpGet]
    public async Task<IActionResult> GetAreasWithSpaces()
    {
        var areas = await _locationService.GetAreasWithSpaces();
        return Ok(areas);
    }
    [HttpGet]
    public async Task<IActionResult> GetAreaById(int areaId)
    {
        var area = await _locationService.GetAreaById(areaId);
        return Ok(area);
    }
    [HttpGet]
    public async Task<IActionResult> GetAreas()
    {
        var areas = await _locationService.GetAreas();
        return Ok(areas);
    }
    [HttpGet]
    public async Task<IActionResult> GetSpaces()
    {
        var spaces = await _locationService.GetSpaces();
        return Ok(spaces);
    }
    [HttpGet]
    public async Task<IActionResult> GetSpaceById(int spaceId)
    {
        var space = await _locationService.GetSpaceById(spaceId);
        return Ok(space);
    }
    [HttpGet]
    public async Task<IActionResult> GetCoordinateByIdWithAssets(int coordinateId)
    {
        var coordinate = await _locationService.GetCoordinateByIdWithAssets(coordinateId);
        return Ok(coordinate);
    }
    [HttpGet]
    public async Task<IActionResult> GetCoordinates()
    {
        var coordinates = await _locationService.GetCoordinates();
        return Ok(coordinates);
    }
    [HttpGet]
    public async Task<IActionResult> GetCoordinatesWithAssets()
    {
        var coordinates = await _locationService.GetCoordinatesWithAssets();
        return Ok(coordinates);
    }
    [HttpGet]
    public async Task<IActionResult> GetPlantsWithAreas()
    {
        var plants = await _locationService.GetPlantsWithAreas();
        return Ok(plants);
    }
    [HttpGet]
    public async Task<IActionResult> GetSpacesWithCoordinates()
    {
        var spaces = await _locationService.GetSpacesWithCoordinates();
        return Ok(spaces);
    }
    [HttpPost]
    public async Task<IActionResult> CreateArea(int plantId, AreaCreateDto areaCreateDto)
    {
        var area = await _locationService.CreateArea(plantId, areaCreateDto);
        return Ok(area);
    }
    [HttpPost]
    public async Task<IActionResult> CreateSpace(int areaId, SpaceCreateDto spaceCreateDto)
    {
        var space = await _locationService.CreateSpace(areaId, spaceCreateDto);
        return Ok(space);
    }
    [HttpPost]
    public async Task<IActionResult> CreateCoordinate(int spaceId, CoordinateCreateDto coordinateCreateDto)
    {
        var coordinate = await _locationService.CreateCoordinate(spaceId, coordinateCreateDto);
        return Ok(coordinate);
    }
    [HttpPost]
    public async Task<IActionResult> CreatePlant(PlantCreateDto plantCreateDto)
    {
        var plant = await _locationService.CreatePlant(plantCreateDto);
        return Ok(plant);
    }
    [HttpDelete]
    public async Task<IActionResult> DeleteArea(int areaId)
    {
        await _locationService.DeleteArea(areaId);
        return Ok();
    }
    [HttpDelete]
    public async Task<IActionResult> DeleteSpace(int spaceId)
    {
        await _locationService.DeleteSpace(spaceId);
        return Ok();
    }
    [HttpDelete]
    public async Task<IActionResult> DeleteCoordinate(int coordinateId)
    {
        await _locationService.DeleteCoordinate(coordinateId);
        return Ok();
    }
    [HttpDelete]
    public async Task<IActionResult> DeletePlant(int plantId)
    {
        await _locationService.DeletePlant(plantId);
        return Ok();
    }
    [HttpPut]
    public async Task<IActionResult> UpdateArea(int areaId, AreaUpdateDto areaUpdateDto)
    {
        var area = await _locationService.UpdateArea(areaId, areaUpdateDto);
        return Ok(area);
    }
    [HttpPut]
    public async Task<IActionResult> UpdateSpace(int spaceId, SpaceUpdateDto spaceUpdateDto)
    {
        var space = await _locationService.UpdateSpace(spaceId, spaceUpdateDto);
        return Ok(space);
    }
    [HttpPut]
    public async Task<IActionResult> UpdateCoordinate(int coordinateId, CoordinateUpdateDto coordinateUpdateDto)
    {
        var coordinate = await _locationService.UpdateCoordinate(coordinateId, coordinateUpdateDto);
        return Ok(coordinate);
    }
    [HttpPut]
    public async Task<IActionResult> UpdatePlant(int plantId, PlantUpdateDto plantUpdateDto)
    {
        var plant = await _locationService.UpdatePlant(plantId, plantUpdateDto);
        return Ok(plant);
    }
    [HttpPut]
    public async Task<IActionResult> MarkDeleteArea(int areaId)
    {
        await _locationService.MarkDeleteArea(areaId);
        return Ok();
    }
    [HttpPut]
    public async Task<IActionResult> MarkDeleteSpace(int spaceId)
    {
        await _locationService.MarkDeleteSpace(spaceId);
        return Ok();
    }
    [HttpPut]
    public async Task<IActionResult> MarkDeleteCoordinate(int coordinateId)
    {
        await _locationService.MarkDeleteCoordinate(coordinateId);
        return Ok();
    }
    [HttpPut]
    public async Task<IActionResult> MarkDeletePlant(int plantId)
    {
        await _locationService.MarkDeletePlant(plantId);
        return Ok();
    }
}
