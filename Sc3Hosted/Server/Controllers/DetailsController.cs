using Microsoft.AspNetCore.Mvc;

using Sc3Hosted.Server.Services;
using Sc3Hosted.Shared.Dtos;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Sc3Hosted.Server.Controllers;
[Route("api/[controller]/[action]")]
[ApiController]
public class DetailsController : ControllerBase
{
    private readonly IAssetService _assetService;
    public DetailsController(IAssetService assetService)
    {
        _assetService = assetService;
    }

    [HttpPut]
    public async Task<IActionResult> MarkDeleteDetail(int detailId)
    {
        await _assetService.MarkDeleteDetail(detailId);
        return NoContent();
    }

    [HttpPut]
    public async Task<IActionResult> UpdateDetail(int detailId, DetailUpdateDto detailUpdateDto)
    {
        await _assetService.UpdateDetail(detailId, detailUpdateDto);
        return NoContent();
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

    [HttpDelete]
    public async Task<IActionResult> DeleteDetail(int detailId)
    {
        await _assetService.DeleteDetail(detailId);
        return NoContent();
    }

    [HttpPost]
    public async Task<IActionResult> CreateDetail(DetailCreateDto detailCreateDto)
    {
        await _assetService.CreateDetail(detailCreateDto);
        return NoContent();
    }
}
