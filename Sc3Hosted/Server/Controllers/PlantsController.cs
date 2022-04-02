using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sc3Hosted.Server.CQRS.Commands;
using Sc3Hosted.Server.CQRS.Queries;

namespace Sc3Hosted.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PlantsController : ControllerBase
{
    private readonly IMediator _mediator;

    public PlantsController(IMediator mediator)
    {
        _mediator = mediator;
    }
        
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] CreatePlantCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }
        
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var result = await _mediator.Send(new GetAllPlantsQuery());
        return Ok(result);
    }
   
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
      var plant =  await _mediator.Send(new GetPlantByIdQuery { Id = id });
        if (plant is null)
        {
            return NotFound();
        }
        return Ok(plant);
    }
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdatePlantCommand command)
    {
        command.Id = id;
        return Ok(await _mediator.Send(command));
    }
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        return Ok(await _mediator.Send(new DeletePlantByIdCommand { Id = id }));
    }
}