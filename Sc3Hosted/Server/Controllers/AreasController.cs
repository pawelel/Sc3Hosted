using Microsoft.AspNetCore.Mvc;
using Sc3Hosted.Server.Services;
using Sc3Hosted.Shared.Dtos;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Sc3Hosted.Server.Controllers;
[Route("api/[controller]")]
[ApiController]
public class AreasController : ControllerBase
{
    private readonly ILocationService _locationService;
    private readonly IUserContextService _userContextService;
    public AreasController(IUserContextService userContextService, ILocationService locationService)
    {
        _userContextService = userContextService;
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
    public void Post([FromRoute]int plantId, [FromBody] AreaCreateDto area)
    {
        var userId = _userContextService.UserId;
        _locationService.CreateArea(plantId, area, userId);
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
