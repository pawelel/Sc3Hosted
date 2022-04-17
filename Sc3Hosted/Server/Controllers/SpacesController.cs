using Microsoft.AspNetCore.Mvc;

using Sc3Hosted.Server.Services;
using Sc3Hosted.Shared.Dtos;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Sc3Hosted.Server.Controllers;
[Route("api/[controller]/[action]")]
[ApiController]
public class SpacesController : ControllerBase
{
    private readonly ILocationService _locationService;
    public SpacesController(ILocationService locationService)
    {
        _locationService = locationService;
    }

    [HttpPut]
    public async Task<IActionResult> MarkDeleteSpace(int spaceId)
    {
        await _locationService.MarkDeleteSpace(spaceId);
        return NoContent();
    }
    
    [HttpPut]
    public async Task<IActionResult> UpdateSpace(int spaceId, SpaceUpdateDto spaceUpdateDto)
    {
        await _locationService.UpdateSpace(spaceId, spaceUpdateDto);
        return NoContent();
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteSpace(int spaceId)
    {
        await _locationService.DeleteSpace(spaceId);
        return NoContent();
    }

    [HttpPost]
    public async Task<IActionResult> CreateSpace(int areaId, SpaceCreateDto spaceCreateDto)
    {
        var spaceId = await _locationService.CreateSpace(areaId, spaceCreateDto);
        return Created($"api/spaces/{spaceId}", null);
    }

    [HttpGet]
    public async Task<IActionResult> GetSpacesWithCoordinates()
    {
        var spaces = await _locationService.GetSpacesWithCoordinates();
        return Ok(spaces);
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
}
