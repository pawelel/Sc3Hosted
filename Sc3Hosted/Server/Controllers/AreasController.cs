using Microsoft.AspNetCore.Mvc;
using Sc3Hosted.Server.Services;
using Sc3Hosted.Shared.Dtos;



namespace Sc3Hosted.Server.Controllers;
[Route("api/[controller]")]
[ApiController]
public class AreasController : ControllerBase
{
    private readonly ILocationService _locationService;
    public AreasController( ILocationService locationService)
    {
        _locationService = locationService;

    }
    // GET: api/<AreasController>
    [HttpGet]
    public async Task<ActionResult> Get()
    {
        var result = await _locationService.GetAreas();

        return Ok(result);
    }

    // GET api/<AreasController>/5
    [HttpGet("{areaId:int}")]
    public async Task<ActionResult>  Get(int areaId)
    {
        return Ok(await _locationService.GetAreaById(areaId));
    }

    // POST api/<AreasController>
    [HttpPost]
    public async Task<ActionResult> Post([FromRoute]int plantId, [FromBody] AreaCreateDto area)
    {
       var newAreaId = await _locationService.CreateArea(plantId, area);
       return Created($"api/plants/{plantId}/areas/{newAreaId}", null);
    }
    

    // PUT api/<AreasController>/5
    [HttpPut("{id}")]
    public void Put(int id, [FromBody] string value)
    {
    }

    // DELETE api/<AreasController>/5
    [HttpDelete("{id}")]
    public void Delete(int id)
    {
    }
}
