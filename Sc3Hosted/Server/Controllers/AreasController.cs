using Microsoft.AspNetCore.Mvc;

using Sc3Hosted.Server.Services;
using Sc3Hosted.Shared.Dtos;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Sc3Hosted.Server.Controllers;
[Route("api/[controller]/[action]")]
[ApiController]
public class AreasController : ControllerBase
{
    private readonly ILocationService _locationService;
    public AreasController(ILocationService locationService)
    {
        _locationService = locationService;
    }

    [HttpPut]
    public async Task<IActionResult> MarkDeleteArea(int areaId)
    {
        await _locationService.MarkDeleteArea(areaId);
        return NoContent();
    }

    [HttpPut]
    public async Task<IActionResult> UpdateArea(int areaId, AreaUpdateDto areaUpdateDto)
    {
        await _locationService.UpdateArea(areaId, areaUpdateDto);
        return NoContent();
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteArea(int areaId)
    {
        await _locationService.DeleteArea(areaId);
        return Ok();
    }

    [HttpPost]
    public async Task<IActionResult> CreateArea(int plantId, AreaCreateDto areaCreateDto)
    {
        var areaId = await _locationService.CreateArea(plantId, areaCreateDto);
        return Created($"api/areas/{areaId}", null);
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
}
