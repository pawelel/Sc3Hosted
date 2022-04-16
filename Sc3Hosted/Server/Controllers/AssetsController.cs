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
    public async Task<IActionResult> CreateAsset([FromBody] AssetCreateDto assetCreateDto)
    {
        var newAssetId = await _assetService.CreateAsset(assetCreateDto);
        return Created($"/api/assets/{newAssetId}", null);
    }

    [HttpDelete("{assetId:int}")]
    public async Task<IActionResult> DeleteAsset(int assetId)
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
    [HttpPost]
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
    public async Task<IActionResult> UpdateAsset(int assetId, AssetUpdateDto assetUpdateDto)
    {
        await _assetService.UpdateAsset(assetId, assetUpdateDto);
        return NoContent();
    }
    

}
