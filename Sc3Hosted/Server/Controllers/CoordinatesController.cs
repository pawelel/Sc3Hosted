using Microsoft.AspNetCore.Mvc;

using Sc3Hosted.Server.Services;
using Sc3Hosted.Shared.Dtos;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Sc3Hosted.Server.Controllers;
[Route("api/[controller]/[action]")]
[ApiController]
public class CoordinatesController : ControllerBase
{
    private readonly ILocationService _locationService;
    public CoordinatesController(ILocationService locationService)
    {
        _locationService = locationService;
    }
    
    [HttpPut]
    public async Task<IActionResult> MarkDeleteCoordinate(int coordinateId)
    {
        await _locationService.MarkDeleteCoordinate(coordinateId);
        return NoContent();
    }

    [HttpPut]
    public async Task<IActionResult> UpdateCoordinate(int coordinateId, CoordinateUpdateDto coordinateUpdateDto)
    {
        await _locationService.UpdateCoordinate(coordinateId, coordinateUpdateDto);
        return NoContent();
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteCoordinate(int coordinateId)
    {
        await _locationService.DeleteCoordinate(coordinateId);
        return NoContent();
    }

    [HttpPost]
    public async Task<IActionResult> CreateCoordinate(int spaceId, CoordinateCreateDto coordinateCreateDto)
    {
        var coordinateId = await _locationService.CreateCoordinate(spaceId, coordinateCreateDto);
        return Created($"api/coordinates/{coordinateId}", null);
    }

    [HttpGet]
    public async Task<IActionResult> GetCoordinatesWithAssets()
    {
        var coordinates = await _locationService.GetCoordinatesWithAssets();
        return Ok(coordinates);
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
}
