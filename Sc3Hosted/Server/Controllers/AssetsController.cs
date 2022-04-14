using Microsoft.AspNetCore.Mvc;
using Sc3Hosted.Server.Services;
using Sc3Hosted.Shared.Dtos;
namespace Sc3Hosted.Server.Controllers;
[Route("api/[controller]/[action]")]
[ApiController]
public class AssetsController : ControllerBase
{
    private readonly IAssetService _assetService;
    public AssetsController(IAssetService assetService)
    {
        _assetService = assetService;
    }
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] AssetCreateDto assetCreateDto)
    {
        var newAssetId = await _assetService.CreateAsset(assetCreateDto);
        return Created($"/api/assets/{newAssetId}", null);
    }
    [HttpPut("{assetId:int}")]
    public async Task<IActionResult> Update(int assetId, AssetUpdateDto assetUpdateDto)
    {
        await _assetService.UpdateAsset(assetId, assetUpdateDto);
        return NoContent();
    }

    [HttpDelete("{assetId:int}")]
    public async Task<IActionResult> Delete(int assetId)
    {
        await _assetService.DeleteAsset(assetId);
        return NoContent();
    }
    [HttpGet("{assetId:int}")]
    public async Task<IActionResult> GetAssetById(int assetId)
    {
        var asset = await _assetService.GetAssetById(assetId);
        return Ok(asset);
    }
    [HttpGet]
    public async Task<IActionResult> GetAssets()
    {
        var assets = await _assetService.GetAssets();
        return Ok(assets);
    }
    [HttpGet]
    public async Task<IActionResult> GetAssetDisplays()
    {
        var assets = await _assetService.GetAssetDisplays();
        return Ok(assets);
    }
    [HttpPut]
    public async Task<IActionResult> ChangeModelOfAsset(int assetId, int modelId)
    {
        await _assetService.ChangeModelOfAsset(assetId, modelId);
        return NoContent();
    }
    [HttpPost]
    public async Task<IActionResult> CreateAssetCategory(int assetId, int categoryId)
    {
        await _assetService.CreateAssetCategory(assetId, categoryId);
        return Created($"/api/assets/{assetId}/categories/{categoryId}", null);
    }
    [HttpDelete]
    public async Task<IActionResult> DeleteAssetCategory(int assetId, int categoryId)
    {
        await _assetService.DeleteAssetCategory(assetId, categoryId);
        return NoContent();
    }
    [HttpPut]
    public async Task<IActionResult> MarkDeleteAssetCategory(int assetId, int categoryId)
    {
        await _assetService.MarkDeleteAssetCategory(assetId, categoryId);
        return NoContent();
    }
    [HttpPut]
    public async Task<IActionResult> UpdateAssetCategory(int assetId, int categoryId)
    {
        await _assetService.UpdateAssetCategory(assetId, categoryId);
        return NoContent();
    }
    public async Task<IActionResult> CreateAssetDetail(AssetDetailDto assetDetailDto)
    {
        await _assetService.CreateAssetDetail(assetDetailDto);
        return Created($"/api/assets/{assetDetailDto.AssetId}/details/{assetDetailDto.DetailId}", null);
    }
    [HttpDelete]
    public async Task<IActionResult> DeleteAssetDetail(int assetId, int detailId)
    {
        await _assetService.DeleteAssetDetail(assetId, detailId);
        return NoContent();
    }
    [HttpPut]
    public async Task<IActionResult> MarkDeleteAssetDetail(int assetId, int detailId)
    {
        await _assetService.MarkDeleteAssetDetail(assetId, detailId);
        return NoContent();
    }
    [HttpPut]
    public async Task<IActionResult> UpdateAssetDetail(AssetDetailDto assetDetailDto)
    {
        await _assetService.UpdateAssetDetail(assetDetailDto);
        return NoContent();
    }
    [HttpPut]
    public async Task<IActionResult> MarkDeleteAsset(int assetId)
    {
        await _assetService.MarkDeleteAsset(assetId);
        return NoContent();
    }
    [HttpPut]
    public async Task<IActionResult> MarkDeleteCategory(int categoryId)
    {
        await _assetService.MarkDeleteCategory(categoryId);
        return NoContent();
    }
    [HttpPut]
    public async Task<IActionResult> MarkDeleteModel(int modelId)
    {
        await _assetService.MarkDeleteModel(modelId);
        return NoContent();
    }
    [HttpPut]
    public async Task<IActionResult> MarkDeleteDetail(int detailId)
    {
        await _assetService.MarkDeleteDetail(detailId);
        return NoContent();
    }
    [HttpPut]
    public async Task<IActionResult> MarkDeleteDevice(int deviceId)
    {
        await _assetService.MarkDeleteDevice(deviceId);
        return NoContent();
    }
    [HttpPut]
    public async Task<IActionResult> MarkDeleteModelParameter(int modelId, int parameterId)
    {
        await _assetService.MarkDeleteModelParameter(modelId, parameterId);
        return NoContent();
    }
    [HttpPut]
    public async Task<IActionResult> MarkDeleteParameter(int parameterId)
    {
        await _assetService.MarkDeleteParameter(parameterId);
        return NoContent();
    }
    [HttpPut]
    public async Task<IActionResult> UpdateCategory(int categoryId, CategoryUpdateDto categoryUpdateDto)
    {
        await _assetService.UpdateCategory(categoryId, categoryUpdateDto);
        return NoContent();
    }
    [HttpPut]
    public async Task<IActionResult> UpdateModel(int modelId, ModelUpdateDto modelUpdateDto)
    {
        await _assetService.UpdateModel(modelId, modelUpdateDto);
        return NoContent();
    }
    [HttpPut]
    public async Task<IActionResult> UpdateDetail(int detailId, DetailUpdateDto detailUpdateDto)
    {
        await _assetService.UpdateDetail(detailId, detailUpdateDto);
        return NoContent();
    }
    [HttpPut]
    public async Task<IActionResult> UpdateDevice(int deviceId, DeviceUpdateDto deviceUpdateDto)
    {
        await _assetService.UpdateDevice(deviceId, deviceUpdateDto);
        return NoContent();
    }
    [HttpPut]
    public async Task<IActionResult> UpdateModelParameter(ModelParameterDto modelParameterDto)
    {
        await _assetService.UpdateModelParameter(modelParameterDto);
        return NoContent();
    }
    [HttpPut]
    public async Task<IActionResult> UpdateParameter(int parameterId, ParameterUpdateDto parameterUpdateDto)
    {
        await _assetService.UpdateParameter(parameterId, parameterUpdateDto);
        return NoContent();
    }
    [HttpPut]
    public async Task<IActionResult> UpdateAsset(int assetId, AssetUpdateDto assetUpdateDto)
    {
        await _assetService.UpdateAsset(assetId, assetUpdateDto);
        return NoContent();
    }
    [HttpPost]
    public async Task<IActionResult> CreateCategory(CategoryCreateDto categoryCreateDto)
    {
        await _assetService.CreateCategory(categoryCreateDto);
        return NoContent();
    }
    [HttpPost]
    public async Task<IActionResult> CreateModel(int deviceId, ModelCreateDto modelCreateDto)
    {
        await _assetService.CreateModel(deviceId, modelCreateDto);
        return NoContent();
    }
    [HttpPost]
    public async Task<IActionResult> CreateModelParameter(ModelParameterDto modelParameterDto)
    {
        await _assetService.CreateModelParameter(modelParameterDto);
        return NoContent();
    }
    [HttpPost]
    public async Task<IActionResult> CreateParameter(ParameterCreateDto parameterCreateDto)
    {
        await _assetService.CreateParameter(parameterCreateDto);
        return NoContent();
    }
    [HttpPost]
    public async Task<IActionResult> CreateDetail(DetailCreateDto detailCreateDto)
    {
        await _assetService.CreateDetail(detailCreateDto);
        return NoContent();
    }
    [HttpPost]
    public async Task<IActionResult> CreateDevice(DeviceCreateDto deviceCreateDto)
    {
        await _assetService.CreateDevice(deviceCreateDto);
        return NoContent();
    }
    [HttpDelete]
    public async Task<IActionResult> DeleteCategory(int categoryId)
    {
        await _assetService.DeleteCategory(categoryId);
        return NoContent();
    }
    [HttpDelete]
    public async Task<IActionResult> DeleteModel(int modelId)
    {
        await _assetService.DeleteModel(modelId);
        return NoContent();
    }
    [HttpDelete]
    public async Task<IActionResult> DeleteDetail(int detailId)
    {
        await _assetService.DeleteDetail(detailId);
        return NoContent();
    }
    [HttpDelete]
    public async Task<IActionResult> DeleteDevice(int deviceId)
    {
        await _assetService.DeleteDevice(deviceId);
        return NoContent();
    }
    [HttpDelete]
    public async Task<IActionResult> DeleteModelParameter(int modelId, int parameterId)
    {
        await _assetService.DeleteModelParameter(modelId, parameterId);
        return NoContent();
    }
    [HttpDelete]
    public async Task<IActionResult> DeleteParameter(int parameterId)
    {
        await _assetService.DeleteParameter(parameterId);
        return NoContent();
    }
    [HttpGet]
    public async Task<IActionResult> GetCategoriesWithAssets()
    {
        var categories = await _assetService.GetCategoriesWithAssets();
        return Ok(categories);
    }
    [HttpGet]
    public async Task<IActionResult> GetCategoryById(int categoryId)
    {
        var category = await _assetService.GetCategoryById(categoryId);
        return Ok(category);
    }
    [HttpGet]
    public async Task<IActionResult> GetCategoryByIdWithAssets(int categoryId)
    {
        var category = await _assetService.GetCategoryByIdWithAssets(categoryId);
        return Ok(category);
    }
    [HttpGet]
    public async Task<IActionResult> GetCategories()
    {
        var categories = await _assetService.GetCategories();
        return Ok(categories);
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
    [HttpGet]
    public async Task<IActionResult> GetDetailById(int detailId)
    {
        var detail = await _assetService.GetDetailById(detailId);
        return Ok(detail);
    }
    [HttpGet]
    public async Task<IActionResult> GetDetails()
    {
        var details = await _assetService.GetDetails();
        return Ok(details);
    }
    [HttpGet]
    public async Task<IActionResult> GetDetailsWithAssets()
    {
        var details = await _assetService.GetDetailsWithAssets();
        return Ok(details);
    }
    [HttpGet]
    public async Task<IActionResult> GetDeviceById(int deviceId)
    {
        var device = await _assetService.GetDeviceById(deviceId);
        return Ok(device);
    }
    [HttpGet]
    public async Task<IActionResult> GetDevices()
    {
        var devices = await _assetService.GetDevices();
        return Ok(devices);
    }
    [HttpGet]
    public async Task<IActionResult> GetDevicesWithModels()
    {
        var devices = await _assetService.GetDevicesWithModels();
        return Ok(devices);
    }
    [HttpGet]
    public async Task<IActionResult> GetParametersWithModels()
    {
        var parameters = await _assetService.GetParametersWithModels();
        return Ok(parameters);
    }
    [HttpGet]
    public async Task<IActionResult> GetDevicesWithAssets()
    {
        var devices = await _assetService.GetDevicesWithAssets();
        return Ok(devices);
    }
    [HttpGet]
    public async Task<IActionResult> GetDeviceWithAssets(int deviceId)
    {
        var device = await _assetService.GetDeviceWithAssets(deviceId);
        return Ok(device);
    }
}
