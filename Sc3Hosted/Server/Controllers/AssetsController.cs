using Microsoft.AspNetCore.Mvc;
using Sc3Hosted.Server.Services;
using Sc3Hosted.Shared.Dtos;
namespace Sc3Hosted.Server.Controllers;
[Route("api/[controller]")]
[ApiController]
public class AssetsController : ControllerBase
{
    private readonly IAssetService _assetService;
    private readonly IUserContextService _userContextService;

    public AssetsController(IUserContextService userContextService, IAssetService assetService)
    {
        _userContextService = userContextService;
        _assetService = assetService;
    }
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] AssetCreateDto assetCreateDto)
    {
        throw new NotImplementedException();
    }
    [HttpPut("{assetId:int}")]
    public async Task<IActionResult> Update(int assetId, AssetUpdateDto assetUpdateDto)
    {
        var userId = _userContextService.UserId;
        var result = await _assetService.UpdateAsset(assetId, assetUpdateDto, userId);

       
            return Ok(result);
      
    }
    [HttpPut("{assetId:int}/categories/{categoryId:int}")]
    public async Task<IActionResult> UpdateCategory(AssetCategoryDto assetCategoryDto, int assetId, int categoryId)
    {
        var userId = _userContextService.UserId;
        var result = await _assetService.AddOrUpdateAssetCategory(assetCategoryDto, userId);
     
            return Ok(result);
    }
}
