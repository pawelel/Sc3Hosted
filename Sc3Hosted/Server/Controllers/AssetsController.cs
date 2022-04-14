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
}
