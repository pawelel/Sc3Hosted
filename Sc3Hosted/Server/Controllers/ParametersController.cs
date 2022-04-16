using Microsoft.AspNetCore.Mvc;

using Sc3Hosted.Server.Services;
using Sc3Hosted.Shared.Dtos;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Sc3Hosted.Server.Controllers;
[Route("api/[controller]/[action]")]
[ApiController]
public class ParametersController : ControllerBase
{
    private readonly IAssetService _assetService;
    public ParametersController(IAssetService assetService)
    {
        _assetService = assetService;
    }

    [HttpPut]
    public async Task<IActionResult> MarkDeleteParameter(int parameterId)
    {
        await _assetService.MarkDeleteParameter(parameterId);
        return NoContent();
    }
    
    [HttpPut]
    public async Task<IActionResult> UpdateParameter(int parameterId, ParameterUpdateDto parameterUpdateDto)
    {
        await _assetService.UpdateParameter(parameterId, parameterUpdateDto);
        return NoContent();
    }

    [HttpGet]
    public async Task<IActionResult> GetParametersWithModels()
    {
        var parameters = await _assetService.GetParametersWithModels();
        return Ok(parameters);
    }

    [HttpGet]
    public async Task<IActionResult> GetParameterById(int parameterId)
    {
        var parameter = await _assetService.GetParameterById(parameterId);
        return Ok(parameter);
    }
    
    [HttpGet]
    public async Task<IActionResult> GetParameters()
    {
        var parameters = await _assetService.GetParameters();
        return Ok(parameters);
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteParameter(int parameterId)
    {
        await _assetService.DeleteParameter(parameterId);
        return NoContent();
    }

    [HttpPost]
    public async Task<IActionResult> CreateParameter(ParameterCreateDto parameterCreateDto)
    {
        await _assetService.CreateParameter(parameterCreateDto);
        return NoContent();
    }
}
