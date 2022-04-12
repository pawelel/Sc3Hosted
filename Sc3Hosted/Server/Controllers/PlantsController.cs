using Microsoft.AspNetCore.Mvc;
using Sc3Hosted.Server.Services;
namespace Sc3Hosted.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PlantsController : ControllerBase
{
    private readonly ILocationService _locationService;
    // GET: api/<PlantsController>
    public PlantsController(ILocationService locationService)
    {
        _locationService = locationService;
    }
    [HttpGet]
    public async Task<ActionResult> GetPlants()
    {
        return Ok(await _locationService.GetPlants());
    }

    // GET api/<PlantsController>/5
    [HttpGet("{id}")]
    public string Get(int id)
    {
        return "value";
    }

    // POST api/<PlantsController>
    [HttpPost]
    public void Post([FromBody] string value)
    {
    }

    // PUT api/<PlantsController>/5
    [HttpPut("{id}")]
    public void Put(int id, [FromBody] string value)
    {
    }

    // DELETE api/<PlantsController>/5
    [HttpDelete("{id}")]
    public void Delete(int id)
    {
    }
}
