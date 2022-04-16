using Microsoft.AspNetCore.Mvc;

using Sc3Hosted.Server.Services;
using Sc3Hosted.Shared.Dtos;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Sc3Hosted.Server.Controllers;
[Route("api/[controller]/[action]")]
[ApiController]
public class CategoriesController : ControllerBase
{
    private readonly IAssetService _assetService;

    public CategoriesController(IAssetService assetService)
    {
        _assetService = assetService;
    }

    [HttpPut]
    public async Task<IActionResult> MarkDeleteCategory(int categoryId)
    {
        await _assetService.MarkDeleteCategory(categoryId);
        return NoContent();
    }

    [HttpPut]
    public async Task<IActionResult> UpdateCategory(int categoryId, CategoryUpdateDto categoryUpdateDto)
    {
        await _assetService.UpdateCategory(categoryId, categoryUpdateDto);
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

    [HttpDelete]
    public async Task<IActionResult> DeleteCategory(int categoryId)
    {
        await _assetService.DeleteCategory(categoryId);
        return NoContent();
    }

    [HttpPost]
    public async Task<IActionResult> CreateCategory(CategoryCreateDto categoryCreateDto)
    {
        await _assetService.CreateCategory(categoryCreateDto);
        return NoContent();
    }
}
