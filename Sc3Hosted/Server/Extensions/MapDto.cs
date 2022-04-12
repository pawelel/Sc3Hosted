using System.Linq.Expressions;
using Sc3Hosted.Server.Entities;
using Sc3Hosted.Shared.Dtos;
namespace Sc3Hosted.Server.Extensions;
public static class MapDto
{
    public static AssetCategoryDisplayDto AssetCategoryToCategoryDisplayDto(this AssetCategory ac)
    {
        return new AssetCategoryDisplayDto
        {
            Name = ac.Category.Name,

            AssetId = ac.AssetId,
            CategoryId = ac.CategoryId,
            Description = ac.Category.Description,
            IsDeleted = ac.IsDeleted
        };
    }
    public static ModelParameterDisplayDto ModelParameterToModelParameterDisplayDto(this ModelParameter mp)
    {
        return new ModelParameterDisplayDto
        {
            Name = mp.Parameter.Name,
            Value = mp.Value,
            ParameterId = mp.ParameterId,
            ModelId = mp.ModelId,

            Description = mp.Parameter.Description,
            IsDeleted = mp.IsDeleted
        };
    }
    public static AssetDetailDisplayDto AssetDetailToAssetDisplayDetailDto(this AssetDetail ad)
    {
        return new AssetDetailDisplayDto
        {
            Name = ad.Detail.Name,
            Value = ad.Value,
            DetailId = ad.DetailId,
            AssetId = ad.AssetId,

            Description = ad.Detail.Description,
            IsDeleted = ad.IsDeleted
        };
    }
    public static Expression<Func<Category, CategoryDto>> CategoryToCategoryDto()
    {

        return c => new CategoryDto
        {
            Name = c.Name,
            Description = c.Description,
            CategoryId = c.CategoryId
        };
    }
}
