using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using MediatR;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;


using Sc3Hosted.Server.Features.Plants.Commands;
using Sc3Hosted.Server.Features.Plants.Queries;

namespace Sc3Hosted.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PlantsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger _logger;
    public PlantsController(IMediator mediator, ILogger logger)
    {
        _mediator = mediator;
        _logger = logger;
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
        try
        {
            var result = await _mediator.Send(new GetAllPlantsQuery());
            if (result==null|| !result.Any())
            {
                return NotFound();
            }
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error receiving data from the database.");
            return StatusCode(StatusCodes.Status500InternalServerError, "Error receiving data from the database");
        }
        
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var plant = await _mediator.Send(new GetPlantByIdQuery { Id = id });
        return Ok(plant);
    }
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdatePlantCommand command)
    {
        command.Id = id;
        return Ok(await _mediator.Send(command));
    }
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        return Ok(await _mediator.Send(new DeletePlantByIdCommand { Id = id }));
    }
}