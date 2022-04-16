using Microsoft.AspNetCore.Mvc;

using Sc3Hosted.Server.Services;
using Sc3Hosted.Shared.Dtos;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Sc3Hosted.Server.Controllers;
[Route("api/[controller]/[action]")]
[ApiController]
public class ModelsController : ControllerBase
{
    private readonly IAssetService _assetService;
    public ModelsController(IAssetService assetService)
    {
        _assetService = assetService;
    }

    [HttpPut]
    public async Task<IActionResult> MarkDeleteModel(int modelId)
    {
        await _assetService.MarkDeleteModel(modelId);
        return NoContent();
    }
    
    [HttpPut]
    public async Task<IActionResult> MarkDeleteModelParameter(int modelId, int parameterId)
    {
        await _assetService.MarkDeleteModelParameter(modelId, parameterId);
        return NoContent();
    }

    [HttpPut]
    public async Task<IActionResult> UpdateModel(int modelId, ModelUpdateDto modelUpdateDto)
    {
        await _assetService.UpdateModel(modelId, modelUpdateDto);
        return NoContent();
    }

    [HttpPut]
    public async Task<IActionResult> UpdateModelParameter(ModelParameterDto modelParameterDto)
    {
        await _assetService.UpdateModelParameter(modelParameterDto);
        return NoContent();
    }

    [HttpGet]
    public async Task<IActionResult> GetModelsWithAssets()
    {
        var models = await _assetService.GetModelsWithAssets();
        return Ok(models);
    }
    
    [HttpGet]
    public async Task<IActionResult> GetModelById(int modelId)
    {
        var model = await _assetService.GetModelById(modelId);
        return Ok(model);
    }
    
    [HttpGet]
    public async Task<IActionResult> GetModels()
    {
        var models = await _assetService.GetModels();
        return Ok(models);
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteModelParameter(int modelId, int parameterId)
    {
        await _assetService.DeleteModelParameter(modelId, parameterId);
        return NoContent();
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteModel(int modelId)
    {
        await _assetService.DeleteModel(modelId);
        return NoContent();
    }

    [HttpPost]
    public async Task<IActionResult> CreateModelParameter(ModelParameterDto modelParameterDto)
    {
        await _assetService.CreateModelParameter(modelParameterDto);
        return NoContent();
    }

    [HttpPost]
    public async Task<IActionResult> CreateModel(int deviceId, ModelCreateDto modelCreateDto)
    {
        await _assetService.CreateModel(deviceId, modelCreateDto);
        return NoContent();
    }
}
