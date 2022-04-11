using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Sc3Hosted.Server.Entities;
using Sc3Hosted.Server.Services;
using Sc3Hosted.Shared.Dtos;
using Sc3Hosted.Shared.Helpers;
namespace Sc3Hosted.Server.Controllers;
[Route("api/[controller]")]
[ApiController]
public class AssetsController : ControllerBase
{
    private readonly IUserContextService _userContextService;
    private readonly IAssetService _assetService;

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
        
        if (result.Success)
        {
            return Ok(result);
        }
        return BadRequest(result.Message);
    }
    [HttpPut("{assetId:int}/categories/{categoryId:int}")]
    public async Task<IActionResult> UpdateCategory(AssetCategoryDto assetCategoryDto, int assetId, int categoryId)
    {
        var userId = _userContextService.UserId;
        var result = await _assetService.AddOrUpdateAssetCategory(assetCategoryDto, userId);
        if (result.Success)
        {
            return Ok(result);
        }
        return BadRequest(result.Message);
    }
    
}
