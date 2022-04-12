using Microsoft.AspNetCore.Mvc;



namespace Sc3Hosted.Server.Controllers;
[Route("api/[controller]")]
[ApiController]
public class SpacesController : ControllerBase
{
    // GET: api/<SpacesController>
    [HttpGet]
    public IEnumerable<string> Get()
    {
        return new[] { "value1", "value2" };
    }

    // GET api/<SpacesController>/5
    [HttpGet("{id}")]
    public string Get(int id)
    {
        return "value";
    }

    // POST api/<SpacesController>
    [HttpPost]
    public void Post([FromBody] string value)
    {
    }

    // PUT api/<SpacesController>/5
    [HttpPut("{id}")]
    public void Put(int id, [FromBody] string value)
    {
    }

    // DELETE api/<SpacesController>/5
    [HttpDelete("{id}")]
    public void Delete(int id)
    {
    }
}
