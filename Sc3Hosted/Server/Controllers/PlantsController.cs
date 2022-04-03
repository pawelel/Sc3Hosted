using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using Sc3Hosted.Server.Services;
using Sc3Hosted.Shared.Dtos;
namespace Sc3Hosted.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PlantsController : ControllerBase
{
    private readonly IDbService _dbService;
    private readonly IUserContextService _userContextService;

    public PlantsController(IDbService dbService, IUserContextService userContextService)
    {
        _dbService = dbService;
        _userContextService = userContextService;
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] PlantCreateDto plantCreateDto)
    {
        var userId = _userContextService.UserId;
        var result = await _dbService.CreatePlant(plantCreateDto, userId);
        if (result.Success)
        {
        return Ok(result);
        }
        return BadRequest(result.Message);
    }
    

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var result = await _dbService.GetPlants();
        if (result.Success)
        {
            return Ok(result);
        }
        return BadRequest(result.Message);
        
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _dbService.GetPlantById(id);
        if (result.Success)
        {
            return Ok(result);
        }
        return BadRequest(result.Message);
    }
    
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, PlantUpdateDto plantUpdateDto)
    {
        var userId = _userContextService.UserId;
        var result = await _dbService.UpdatePlant(id, userId, plantUpdateDto);
        if (result.Success)
        {
            return Ok(result);
        }
        return BadRequest(result.Message);
    }
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _dbService.DeletePlant(id);
        if (result.Success)
        {
            return Ok(result);
        }
        return BadRequest(result.Message);
    }
}