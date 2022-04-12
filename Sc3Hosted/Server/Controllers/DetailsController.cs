using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Sc3Hosted.Server.Controllers;
[Route("api/[controller]")]
[ApiController]
public class DetailsController : ControllerBase
{
    // GET: api/<DetailsController>
    [HttpGet]
    public IEnumerable<string> Get()
    {
        return new[] { "value1", "value2" };
    }

    // GET api/<DetailsController>/5
    [HttpGet("{id}")]
    public string Get(int id)
    {
        return "value";
    }

    // POST api/<DetailsController>
    [HttpPost]
    public void Post([FromBody] string value)
    {
    }

    // PUT api/<DetailsController>/5
    [HttpPut("{id}")]
    public void Put(int id, [FromBody] string value)
    {
    }

    // DELETE api/<DetailsController>/5
    [HttpDelete("{id}")]
    public void Delete(int id)
    {
    }
}
