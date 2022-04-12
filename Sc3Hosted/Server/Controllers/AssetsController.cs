using Microsoft.AspNetCore.Mvc;
using Sc3Hosted.Server.Services;
using Sc3Hosted.Shared.Dtos;
namespace Sc3Hosted.Server.Controllers;
[Route("api/[controller]")]
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
        var result = await _assetService.UpdateAsset(assetId, assetUpdateDto);
        return Ok(result);

    }
    [HttpPut("{assetId:int}/categories/{categoryId:int}")]
    public async Task<IActionResult> UpdateCategory(AssetCategoryDto assetCategoryDto, int assetId, int categoryId)
    {
        var result = await _assetService.AddOrUpdateAssetCategory(assetCategoryDto);

        return Ok(result);
    }
}
