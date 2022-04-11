using Microsoft.EntityFrameworkCore;
using Sc3Hosted.Server.Data;
using Sc3Hosted.Server.Entities;
using Sc3Hosted.Shared.Dtos;
using Sc3Hosted.Shared.Helpers;

namespace Sc3Hosted.Server.Services;
public interface IAssetService
{
    Task<ServiceResponse> ChangeAssetsModel(int assetId, int modelId, string userId);

    Task<ServiceResponse> CreateAsset(AssetCreateDto assetCreateDto, string userId);

    Task<ServiceResponse> CreateCategory(CategoryCreateDto categoryCreateDto, string userId);

    Task<ServiceResponse> CreateDetail(DetailCreateDto detailCreateDto, string userId);

    Task<ServiceResponse> CreateDevice(DeviceCreateDto deviceCreateDto, string userId);

    Task<ServiceResponse> CreateModel(int deviceId, ModelCreateDto modelCreateDto, string userId);

    Task<ServiceResponse> CreateParameter(ParameterCreateDto parameterCreateDto, string userId);

    Task<ServiceResponse> DeleteAsset(int assetId);

    Task<ServiceResponse> DeleteCategory(int categoryId);

    Task<ServiceResponse> DeleteDetail(int detailId);

    Task<ServiceResponse> DeleteDevice(int deviceId);

    Task<ServiceResponse> DeleteModel(int modelId);

    Task<ServiceResponse> DeleteParameter(int parameterId);

    Task<ServiceResponse<AssetDto>> GetAssetById(int assetId);

    Task<ServiceResponse<IEnumerable<AssetDto>>> GetAssets();

    Task<ServiceResponse<IEnumerable<AssetDisplayDto>>> GetAssetDisplay();

    Task<ServiceResponse<IEnumerable<CategoryDto>>> GetCategories();

    Task<ServiceResponse<IEnumerable<CategoryWithAssetsDto>>> GetCategoriesWithAssets();

    Task<ServiceResponse<CategoryDto>> GetCategoryById(int categoryId);

    Task<ServiceResponse<CategoryWithAssetsDto>> GetCategoryByIdWithAssets(int categoryId);

    Task<ServiceResponse<DetailDto>> GetDetailById(int detailId);

    Task<ServiceResponse<IEnumerable<DetailDto>>> GetDetails();

    Task<ServiceResponse<IEnumerable<DetailWithAssetsDto>>> GetDetailsWithAssets();

    Task<ServiceResponse<DeviceDto>> GetDeviceById(int deviceId);

    Task<ServiceResponse<IEnumerable<DeviceDto>>> GetDevices();

    Task<ServiceResponse<IEnumerable<DeviceDto>>> GetDevicesWithModels();

    Task<ServiceResponse<ModelDto>> GetModelById(int modelId);

    Task<ServiceResponse<IEnumerable<ModelDto>>> GetModels();

    Task<ServiceResponse<IEnumerable<ModelDto>>> GetModelsWithAssets();

    Task<ServiceResponse<ParameterDto>> GetParameterById(int parameterId);

    Task<ServiceResponse<IEnumerable<ParameterDto>>> GetParameters();

    Task<ServiceResponse<IEnumerable<ParameterWithModelsDto>>> GetParametersWithModels();

    Task<ServiceResponse> MarkDeleteAsset(int assetId, string userId);

    Task<ServiceResponse> MarkDeleteCategory(int categoryId, string userId);

    Task<ServiceResponse> MarkDeleteDetail(int detailId, string userId);

    Task<ServiceResponse> MarkDeleteDevice(int deviceId, string userId);

    Task<ServiceResponse> MarkDeleteModel(int modelId, string userId);

    Task<ServiceResponse> MarkDeleteParameter(int parameterId, string userId);

    Task<ServiceResponse> UpdateAsset(int assetId, AssetUpdateDto assetUpdateDto, string userId);

    Task<ServiceResponse> UpdateCategory(int categoryId, CategoryUpdateDto categoryUpdateDto, string userId);

    Task<ServiceResponse> UpdateDetail(int detailId, DetailUpdateDto detailUpdateDto, string userId);

    Task<ServiceResponse> UpdateDevice(int deviceId, DeviceUpdateDto deviceUpdateDto, string userId);

    Task<ServiceResponse> UpdateModel(int modelId, ModelUpdateDto modelUpdateDto, string userId);

    Task<ServiceResponse> UpdateParameter(int parameterId, ParameterUpdateDto parameterUpdateDto, string userId);

    Task<ServiceResponse> AddOrUpdateAssetCategory(AssetCategoryDto assetCategoryDto, string userId);

    Task<ServiceResponse> AddOrUpdateAssetDetail(AssetDetailDto assetDetailDto, string userId);

    Task<ServiceResponse> AddOrUpdateModelParameter(ModelParameterDto modelParameterDto, string userId);

    Task<ServiceResponse> DeleteAssetCategory(AssetCategoryDto assetCategoryDto, string userId);

    Task<ServiceResponse> DeleteAssetDetail(AssetDetailDto assetDetailDto, string userId);

    Task<ServiceResponse> DeleteModelParameter(ModelParameterDto modelParameterDto, string userId);
    Task<ServiceResponse> MarkDeleteAssetCategory(AssetCategoryDto assetCategoryDto, string userId);
    Task<ServiceResponse> MarkDeleteAssetDetail(AssetDetailDto assetDetailDto, string userId);
    Task<ServiceResponse> MarkDeleteModelParameter(ModelParameterDto modelParameterDto, string userId);
}

public class AssetService : IAssetService
{
    private readonly IDbContextFactory<Sc3HostedDbContext> _contextFactory;
    private readonly ILogger<AssetService> _logger;

    public AssetService(IDbContextFactory<Sc3HostedDbContext> contextFactory, ILogger<AssetService> logger)
    {
        _contextFactory = contextFactory;
        _logger = logger;
    }

    /// <summary>
    /// Allows to assign different model to an asset. Deletes all data related to asset.
    /// Requires existing model not marked as deleted
    /// Removes any existing asset details from asset
    /// Removes any existing asset categories from asset
    /// Removes any existing communicate assets from asset
    /// </summary>
    /// <param name="assetId"></param>
    /// <param name="userId"></param>
    /// <param name="modelId"></param>
    /// <returns>ServiceResponse(string, true) on success</returns>
    public async Task<ServiceResponse> ChangeAssetsModel(int assetId, int modelId, string userId)
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();
        // await using transaction
        await using var transaction = await context.Database.BeginTransactionAsync();

        try
        {
            var asset = await context.Assets.FirstOrDefaultAsync(a => a.AssetId == assetId);
            if (asset == null)
            {
                _logger.LogWarning("Asset not found");
                return new ServiceResponse("Asset not found");
            }
            if (asset.ModelId == modelId)
            {
                _logger.LogWarning("Asset already assigned to model");
                return new ServiceResponse("Asset already has this model");
            }
            if (asset.IsDeleted)
            {
                _logger.LogWarning("Asset marked as deleted");
                return new ServiceResponse("Asset marked as deleted");
            }

            var model = await context.Models.FirstOrDefaultAsync(m => m.ModelId == modelId);
            if (model == null)
            {
                _logger.LogWarning("Model not found");
                return new ServiceResponse("Model not found");
            }

            if (model.IsDeleted)
            {
                _logger.LogWarning("Model marked as deleted");
                return new ServiceResponse("Model marked as deleted");
            }
            // delete all asset details
            context.AssetDetails.RemoveRange(asset.AssetDetails);
            // delete all asset categories
            context.AssetCategories.RemoveRange(asset.AssetCategories);
            // delete all communicate assets
            context.CommunicateAssets.RemoveRange(asset.CommunicateAssets);

            // update asset model
            asset.ModelId = modelId;
            asset.UserId = userId;
            context.Assets.Update(asset);

            // save changes
            await context.SaveChangesAsync();
            // commit transaction
            await transaction.CommitAsync();
            _logger.LogInformation("Asset with id {AssetId} updated", assetId);
            return new ServiceResponse($"Asset {assetId} updated", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating asset with id {AssetId}", assetId);
            // rollback transaction
            await transaction.RollbackAsync();
            return new ServiceResponse($"Error updating asset with id {assetId}");
        }
    }
    /// <summary>
    /// Creates new asset
    /// </summary>
    /// <param name="assetCreateDto"></param>
    /// <param name="userId"></param>
    /// <returns>ServiceResponse</returns>
    public async Task<ServiceResponse> CreateAsset(AssetCreateDto assetCreateDto, string userId)
    {

        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();
        // await using transaction
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            // get model
            var model = await context.Models.FirstOrDefaultAsync(m => m.ModelId == assetCreateDto.ModelId);
            if (model == null)
            {
                _logger.LogWarning("Model not found");
                return new ServiceResponse("Model not found");
            }
            if (model.IsDeleted)
            {
                _logger.LogWarning("Model marked as deleted");
                return new ServiceResponse("Model marked as deleted");
            }
            // get coordinate
            var coordinate = await context.Coordinates.FirstOrDefaultAsync(c => c.CoordinateId == assetCreateDto.CoordinateId);
            if (coordinate == null)
            {
                _logger.LogWarning("Coordinate not found");
                return new ServiceResponse("Coordinate not found");
            }
            if (coordinate.IsDeleted)
            {
                _logger.LogWarning("Coordinate marked as deleted");
                return new ServiceResponse("Coordinate marked as deleted");
            }

            // validate asset name
            var duplicate = await context.Assets.AnyAsync(a => a.Name.ToLower().Trim() == assetCreateDto.Name.ToLower().Trim());
            if (duplicate)
            {
                _logger.LogWarning("Asset name already exists");
                return new ServiceResponse("Asset name already exists");
            }

            var asset = new Asset
            {
                UserId = userId,
                ModelId = assetCreateDto.ModelId,
                CoordinateId = assetCreateDto.CoordinateId,
                Name = assetCreateDto.Name,
                Description = assetCreateDto.Description,
                Process = assetCreateDto.Process,
                Status = assetCreateDto.Status,
                IsDeleted = false
            };
            // create asset
            context.Assets.Add(asset);
            // save changes
            await context.SaveChangesAsync();
            // commit transaction
            await transaction.CommitAsync();
            _logger.LogInformation("Asset with id {AssetId} created", asset.AssetId);
            return new ServiceResponse($"Asset {asset.AssetId} created", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating asset");
            // rollback transaction
            await transaction.RollbackAsync();
            return new ServiceResponse($"Error creating asset");
        }
    }
    /// <summary>
    /// Creates new category
    /// </summary>
    /// <param name="categoryCreateDto"></param>
    /// <param name="userId"></param>
    /// <returns>ServiceResponse(string, true) on success</returns>
    public async Task<ServiceResponse> CreateCategory(CategoryCreateDto categoryCreateDto, string userId)
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();
        // await using transaction
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            // validate category name
            var duplicate = await context.Categories.AnyAsync(c => c.Name.ToLower().Trim() == categoryCreateDto.Name.ToLower().Trim());
            if (duplicate)
            {
                _logger.LogWarning("Category name already exists");
                return new ServiceResponse("Category name already exists");
            }

            var category = new Category
            {
                UserId = userId,
                Name = categoryCreateDto.Name,
                Description = categoryCreateDto.Description,
                IsDeleted = false
            };
            // create category
            context.Categories.Add(category);
            // save changes
            await context.SaveChangesAsync();
            // commit transaction
            await transaction.CommitAsync();
            _logger.LogInformation("Category with id {CategoryId} created", category.CategoryId);
            return new ServiceResponse($"Category {category.CategoryId} created", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating category");
            // rollback transaction
            await transaction.RollbackAsync();
            return new ServiceResponse($"Error creating category");
        }

    }
    /// <summary>
    /// Creates new detail
    /// </summary>
    /// <param name="detailCreateDto"></param>
    /// <param name="userId"></param>
    /// <returns>ServiceResponse(string, true) on success</returns>
    public async Task<ServiceResponse> CreateDetail(DetailCreateDto detailCreateDto, string userId)
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();
        // await using transaction
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            // validate detail name
            var duplicate = await context.Details.AnyAsync(d => d.Name.ToLower().Trim() == detailCreateDto.Name.ToLower().Trim());
            if (duplicate)
            {
                _logger.LogWarning("Detail name already exists");
                return new ServiceResponse("Detail name already exists");
            }

            var detail = new Detail
            {
                UserId = userId,
                Name = detailCreateDto.Name,
                Description = detailCreateDto.Description,
                IsDeleted = false
            };
            // create detail
            context.Details.Add(detail);
            // save changes
            await context.SaveChangesAsync();
            // commit transaction
            await transaction.CommitAsync();
            _logger.LogInformation("Detail with id {DetailId} created", detail.DetailId);
            return new ServiceResponse($"Detail {detail.DetailId} created", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating detail");
            // rollback transaction
            await transaction.RollbackAsync();
            return new ServiceResponse($"Error creating detail");
        }
    }
    /// <summary>
    /// Creates new device
    /// </summary>
    /// <param name="deviceCreateDto"></param>
    /// <param name="userId"></param>
    /// <returns>ServiceResponse(string, true) on success</returns>
    public async Task<ServiceResponse> CreateDevice(DeviceCreateDto deviceCreateDto, string userId)
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();
        // await using transaction
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            // validate device name
            var duplicate = await context.Devices.AnyAsync(d => d.Name.ToLower().Trim() == deviceCreateDto.Name.ToLower().Trim());
            if (duplicate)
            {
                _logger.LogWarning("Device name already exists");
                return new ServiceResponse("Device name already exists");
            }

            var device = new Device
            {
                UserId = userId,
                Name = deviceCreateDto.Name,
                Description = deviceCreateDto.Description,
                IsDeleted = false
            };
            // create device
            context.Devices.Add(device);
            // save changes
            await context.SaveChangesAsync();
            // commit transaction
            await transaction.CommitAsync();
            _logger.LogInformation("Device with id {DeviceId} created", device.DeviceId);
            return new ServiceResponse($"Device {device.DeviceId} created", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating device");
            // rollback transaction
            await transaction.RollbackAsync();
            return new ServiceResponse($"Error creating device");
        }
    }
    /// <summary>
    /// Creates new model
    /// </summary>
    /// <param name="deviceId"></param>
    /// <param name="modelCreateDto"></param>
    /// <param name="userId"></param>
    /// <returns>ServiceResponse(string, true) on success</returns>
    public async Task<ServiceResponse> CreateModel(int deviceId, ModelCreateDto modelCreateDto, string userId)
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();
        // await using transaction
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            // validate model name
            var duplicate = await context.Models.AnyAsync(m => m.Name.ToLower().Trim() == modelCreateDto.Name.ToLower().Trim());
            if (duplicate)
            {
                _logger.LogWarning("Model name already exists");
                return new ServiceResponse("Model name already exists");
            }
            // get device
            var device = await context.Devices.FirstOrDefaultAsync(d => d.DeviceId == deviceId);
            if (device == null)
            {
                _logger.LogWarning("Device not found");
                return new ServiceResponse("Device not found");
            }
            // check if device is marked as deleted
            if (device.IsDeleted)
            {
                _logger.LogWarning("Device is marked as deleted");
                return new ServiceResponse("Device is marked as deleted");
            }

            var model = new Model
            {
                UserId = userId,
                DeviceId = deviceId,
                Name = modelCreateDto.Name,
                Description = modelCreateDto.Description,
                IsDeleted = false
            };
            // create model
            context.Models.Add(model);
            // save changes
            await context.SaveChangesAsync();
            // commit transaction
            await transaction.CommitAsync();
            _logger.LogInformation("Model with id {ModelId} created", model.ModelId);
            return new ServiceResponse($"Model {model.ModelId} created", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating model");
            // rollback transaction
            await transaction.RollbackAsync();
            return new ServiceResponse($"Error creating model");
        }
    }
    /// <summary>
    /// Creates new parameter
    /// </summary>
    /// <param name="parameterCreateDto"></param>
    /// <param name="userId"></param>
    /// <returns>ServiceResponse(string, true) on success</returns>
    public async Task<ServiceResponse> CreateParameter(ParameterCreateDto parameterCreateDto, string userId)
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();
        // await using transaction
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            // validate parameter name
            var duplicate = await context.Parameters.AnyAsync(p => p.Name.ToLower().Trim() == parameterCreateDto.Name.ToLower().Trim());
            if (duplicate)
            {
                _logger.LogWarning("Parameter name already exists");
                return new ServiceResponse("Parameter name already exists");
            }
            var parameter = new Parameter
            {
                UserId = userId,
                Name = parameterCreateDto.Name,
                Description = parameterCreateDto.Description,
                IsDeleted = false
            };
            // create parameter
            context.Parameters.Add(parameter);
            // save changes
            await context.SaveChangesAsync();
            // commit transaction
            await transaction.CommitAsync();
            _logger.LogInformation("Parameter with id {ParameterId} created", parameter.ParameterId);
            return new ServiceResponse($"Parameter {parameter.ParameterId} created", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating parameter");
            // rollback transaction
            await transaction.RollbackAsync();
            return new ServiceResponse($"Error creating parameter");
        }
    }
    /// <summary>
    /// Deletes asset
    /// </summary>
    /// <param name="assetId"></param>
    /// <returns>ServiceResponse(string, true) on success</returns>
    public async Task<ServiceResponse> DeleteAsset(int assetId)
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();
        // await using transaction
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            // get asset
            var asset = await context.Assets.FindAsync(assetId);
            if (asset == null)
            {
                _logger.LogWarning("Asset not found");
                return new ServiceResponse("Asset not found");
            }
            // check if asset is marked as deleted
            if (asset.IsDeleted == false)
            {
                _logger.LogWarning("Asset is not marked as deleted");
                return new ServiceResponse("Asset is not marked as deleted");
            }
            // delete asset
            context.Assets.Remove(asset);
            // save changes
            await context.SaveChangesAsync();
            // commit transaction
            await transaction.CommitAsync();
            _logger.LogInformation("Asset with id {AssetId} deleted", asset.AssetId);
            return new ServiceResponse($"Asset {asset.AssetId} deleted", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting asset");
            // rollback transaction
            await transaction.RollbackAsync();
            return new ServiceResponse($"Error deleting asset");
        }

    }
    /// <summary>
    /// Deletes category
    /// </summary>
    /// <param name="categoryId"></param>
    /// <returns>ServiceResponse(string, true) on success</returns>
    public async Task<ServiceResponse> DeleteCategory(int categoryId)
    {
       
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();
        // await using transaction
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            // get category
            var category = await context.Categories.FindAsync(categoryId);
            if (category == null)
            {
                _logger.LogWarning("Category not found");
                return new ServiceResponse("Category not found");
            }
            // check if category is marked as deleted
            if (category.IsDeleted == false)
            {
                _logger.LogWarning("Category is not marked as deleted");
                return new ServiceResponse("Category is not marked as deleted");
            }
            // delete category
            context.Categories.Remove(category);
            // save changes
            await context.SaveChangesAsync();
            // commit transaction
            await transaction.CommitAsync();
            _logger.LogInformation("Category with id {CategoryId} deleted", category.CategoryId);
            return new ServiceResponse($"Category {category.CategoryId} deleted", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting category");
            // rollback transaction
            await transaction.RollbackAsync();
            return new ServiceResponse($"Error deleting category");
        }
    }
    /// <summary>
    /// Deletes detail
    /// </summary>
    /// <param name="detailId"></param>
    /// <returns>ServiceResponse(string, true) on success</returns>
    public async Task<ServiceResponse> DeleteDetail(int detailId)
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();
        // await using transaction
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            // get detail
            var detail = await context.Details.FindAsync(detailId);
            if (detail == null)
            {
                _logger.LogWarning("Detail not found");
                return new ServiceResponse("Detail not found");
            }
            // check if detail is marked as deleted
            if (detail.IsDeleted == false)
            {
                _logger.LogWarning("Detail is not marked as deleted");
                return new ServiceResponse("Detail is not marked as deleted");
            }
            // delete detail
            context.Details.Remove(detail);
            // save changes
            await context.SaveChangesAsync();
            // commit transaction
            await transaction.CommitAsync();
            _logger.LogInformation("Detail with id {DetailId} deleted", detail.DetailId);
            return new ServiceResponse($"Detail {detail.DetailId} deleted", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting detail");
            // rollback transaction
            await transaction.RollbackAsync();
            return new ServiceResponse($"Error deleting detail");
        }
    }

    /// <summary>
    /// Deletes device
    /// </summary>
    /// <param name="deviceId"></param>
    /// <returns>ServiceResponse(string, true) on success</returns>
    public async Task<ServiceResponse> DeleteDevice(int deviceId)
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();
        // await using transaction
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            // get device
            var device = await context.Devices.FindAsync(deviceId);
            if (device == null)
            {
                _logger.LogWarning("Device not found");
                return new ServiceResponse("Device not found");
            }
            // check if device is marked as deleted
            if (device.IsDeleted == false)
            {
                _logger.LogWarning("Device is not marked as deleted");
                return new ServiceResponse("Device is not marked as deleted");
            }
            // delete device
            context.Devices.Remove(device);
            // save changes
            await context.SaveChangesAsync();
            // commit transaction
            await transaction.CommitAsync();
            _logger.LogInformation("Device with id {DeviceId} deleted", device.DeviceId);
            return new ServiceResponse($"Device {device.DeviceId} deleted", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting device");
            // rollback transaction
            await transaction.RollbackAsync();
            return new ServiceResponse($"Error deleting device");
        }
    }
    /// <summary>
    /// Deletes model
    /// </summary>
    /// <param name="modelId"></param>
    /// <returns>ServiceResponse(string, true) on success</returns>
    public async Task<ServiceResponse> DeleteModel(int modelId)
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();
        // await using transaction
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            // get model
            var model = await context.Models.FindAsync(modelId);
            if (model == null)
            {
                _logger.LogWarning("Model not found");
                return new ServiceResponse("Model not found");
            }
            // check if model is marked as deleted
            if (model.IsDeleted == false)
            {
                _logger.LogWarning("Model is not marked as deleted");
                return new ServiceResponse("Model is not marked as deleted");
            }
            // delete model
            context.Models.Remove(model);
            // save changes
            await context.SaveChangesAsync();
            // commit transaction
            await transaction.CommitAsync();
            _logger.LogInformation("Model with id {ModelId} deleted", model.ModelId);
            return new ServiceResponse($"Model {model.ModelId} deleted", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting model");
            // rollback transaction
            await transaction.RollbackAsync();
            return new ServiceResponse($"Error deleting model");
        }
    }

    /// <summary>
    /// Deletes parameter
    /// </summary>
    /// <param name="parameterId"></param>
    /// <returns>ServiceResponse(string, true) on success</returns>
    public async Task<ServiceResponse> DeleteParameter(int parameterId)
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();
        // await using transaction
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            // get parameter
            var parameter = await context.Parameters.FindAsync(parameterId);
            if (parameter == null)
            {
                _logger.LogWarning("Parameter not found");
                return new ServiceResponse("Parameter not found");
            }
            // check if parameter is marked as deleted
            if (parameter.IsDeleted == false)
            {
                _logger.LogWarning("Parameter is not marked as deleted");
                return new ServiceResponse("Parameter is not marked as deleted");
            }
            // delete parameter
            context.Parameters.Remove(parameter);
            // save changes
            await context.SaveChangesAsync();
            // commit transaction
            await transaction.CommitAsync();
            _logger.LogInformation("Parameter with id {ParameterId} deleted", parameter.ParameterId);
            return new ServiceResponse($"Parameter {parameter.ParameterId} deleted", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting parameter");
            // rollback transaction
            await transaction.RollbackAsync();
            return new ServiceResponse($"Error deleting parameter");
        }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="assetId"></param>
    /// <returns>ServiceResponse(dto, string, true) on success</returns>
    public async Task<ServiceResponse<AssetDto>> GetAssetById(int assetId)
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();
        try
        {
            // get asset
            var asset = await context.Assets.AsNoTracking().Select(a => new AssetDto
            {
                AssetId = a.AssetId,
                Name = a.Name,
                Description = a.Description,
                IsDeleted = a.IsDeleted,
                UserId = a.UserId,
                Status = a.Status,
                CoordinateId = a.CoordinateId,
                ModelId = a.ModelId,
                Process = a.Process
            }).FirstOrDefaultAsync();
            if (asset == null)
            {
                _logger.LogWarning("Asset not found");
                return new ServiceResponse<AssetDto>("Asset not found");
            }
            // return asset
            _logger.LogInformation("Asset with id {AssetId} found", asset.AssetId);
            return new ServiceResponse<AssetDto>(asset, "Asset found");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error getting asset");
            return new ServiceResponse<AssetDto>("Error getting asset");
        }

    }
    /// <summary>
    /// 
    /// </summary>
    /// <returns>ServiceResponse(data, string, true) on success</returns>
    public async Task<ServiceResponse<IEnumerable<AssetDto>>> GetAssets()
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();
        try
        {
            // get assets
            var assets = await context.Assets
                .AsNoTracking()
                .Select(a => new AssetDto
                {
                    AssetId = a.AssetId,
                    Name = a.Name,
                    Description = a.Description,
                    IsDeleted = a.IsDeleted,
                    UserId = a.UserId,
                    Status = a.Status,
                    CoordinateId = a.CoordinateId,
                    ModelId = a.ModelId,
                    Process = a.Process
                }).ToListAsync();
            if (assets.Count == 0)
            {
                _logger.LogWarning("No assets found");
                return new ServiceResponse<IEnumerable<AssetDto>>("No assets found", true);
            }
            // return assets
            _logger.LogInformation("Assets found");
            return new ServiceResponse<IEnumerable<AssetDto>>(assets, "Assets found");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error getting assets");
            return new ServiceResponse<IEnumerable<AssetDto>>("Error getting assets");
        }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <returns>ServiceResponse(data, string, true) on success</returns>
    public async Task<ServiceResponse<IEnumerable<AssetDisplayDto>>> GetAssetDisplay()
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();
        try
        {
// get asset
            var query = await context.Assets
                .AsNoTracking()
                .Select(a => new AssetDisplayDto
                {
                    Name = a.Name,
                    Description = a.Description,
                    AssetId = a.AssetId,
                    AreaId = a.Coordinate.Space.AreaId,
                    SpaceId = a.Coordinate.SpaceId,
                    CoordinateId = a.CoordinateId,
                    PlantId = a.Coordinate.Space.Area.PlantId,
                    AreaName = a.Coordinate.Space.Area.Name,
                    SpaceName = a.Coordinate.Space.Name,
                    CoordinateName = a.Coordinate.Name,
                    Status = a.Status,
                    IsDeleted = a.IsDeleted,
                    UserId = a.UserId,
                    Categories = a.AssetCategories.Select(ac => new AssetCategoryDisplayDto()
                    {
                        Name = ac.Category.Name,
                        AssetId = ac.AssetId,
                        UserId = ac.UserId,
                        CategoryId = ac.CategoryId,
                        Description = ac.Category.Description,
                        IsDeleted = ac.IsDeleted
                    }).ToList(),
                    Details = a.AssetDetails.Select(ad => new AssetDetailDisplayDto()
                    {
                        Name = ad.Detail.Name,
                        Value = ad.Value,
                        DetailId = ad.DetailId,
                        UserId = ad.UserId,
                        AssetId = ad.AssetId,
                        Description = ad.Detail.Description,
                        IsDeleted = ad.IsDeleted
                    }).ToList(),
                    Parameters = a.Model.ModelParameters.Select(mp => new ModelParameterDisplayDto()
                    {
                        Name = mp.Parameter.Name,
                        Value = mp.Value,
                        ParameterId = mp.ParameterId,
                        UserId = mp.UserId,
                        ModelId = mp.ModelId,
                        Description = mp.Parameter.Description,
                        IsDeleted = mp.IsDeleted
                    }).ToList(),
                }).ToListAsync();
            if (query.Count == 0)
            {
                _logger.LogWarning("No assets found");
                return new ServiceResponse<IEnumerable<AssetDisplayDto>>("No assets found", true);
            }
            // return assets
            _logger.LogInformation("Assets found");
            return new ServiceResponse<IEnumerable<AssetDisplayDto>>(query, "Assets found");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error getting assets");
            return new ServiceResponse<IEnumerable<AssetDisplayDto>>("Error getting assets");
        }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <returns>ServiceResponse(data, string, true) on success</returns>
    public async Task<ServiceResponse<IEnumerable<CategoryDto>>> GetCategories()
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();
        try
        {
            // get categories
            var query = await context.Categories
                .AsNoTracking()
                .Select(c => new CategoryDto
                {
                    Name = c.Name,
                    Description = c.Description,
                    CategoryId = c.CategoryId,
                    IsDeleted = c.IsDeleted,
                    UserId = c.UserId
                }).ToListAsync();
            if (query.Count == 0)
            {
                _logger.LogWarning("No categories found");
                return new ServiceResponse<IEnumerable<CategoryDto>>("No categories found", true);
            }
            // return categories
            _logger.LogInformation("Categories found");
            return new ServiceResponse<IEnumerable<CategoryDto>>(query, "Categories found");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error getting categories");
            return new ServiceResponse<IEnumerable<CategoryDto>>("Error getting categories");
        }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <returns>ServiceResponse(data, string, true) on success</returns>
    public async Task<ServiceResponse<IEnumerable<CategoryWithAssetsDto>>> GetCategoriesWithAssets()
    {
        // await context
        await using var context = await _contextFactory.CreateDbContextAsync();
        try
        {
            // get categories
            var query = await context.Categories
                .AsNoTracking()
                .Select(c => new CategoryWithAssetsDto
                {
                    CategoryId = c.CategoryId,
                    Name = c.Name,
                    Description = c.Description,
                    UserId = c.UserId,
                    IsDeleted = c.IsDeleted,
                    Assets = c.AssetCategories.Select(ac => new AssetDto()
                    {
                        Name = ac.Asset.Name,
                        Description = ac.Asset.Description,
                        AssetId = ac.AssetId,
                        Status = ac.Asset.Status,
                        UserId = ac.UserId,
                        Process = ac.Asset.Process,
                        IsDeleted = ac.Asset.IsDeleted
                    }).ToList()
                }).ToListAsync();
            if (query.Count == 0)
            {
                _logger.LogWarning("No categories found");
                return new ServiceResponse<IEnumerable<CategoryWithAssetsDto>>("No categories found");
            }
            // return categories
            _logger.LogInformation("Categories found");
            return new ServiceResponse<IEnumerable<CategoryWithAssetsDto>>(query, "Categories found");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error getting categories");
            return new ServiceResponse<IEnumerable<CategoryWithAssetsDto>>("Error getting categories");
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="categoryId"></param>
    /// <returns></returns>
    public async Task<ServiceResponse<CategoryDto>> GetCategoryById(int categoryId)
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();
        try
        {
            // get category
            var query = await context.Categories
                .AsNoTracking()
                .Select(c => new CategoryDto
                {
                    Name = c.Name,
                    UserId = c.UserId,
                    IsDeleted = c.IsDeleted,
                    Description = c.Description,
                    CategoryId = c.CategoryId
                }).FirstOrDefaultAsync(c => c.CategoryId == categoryId);
            if (query == null)
            {
                _logger.LogWarning("No category found");
                return new ServiceResponse<CategoryDto>("No category found", true);
            }
            // return category
            _logger.LogInformation("Category found");
            return new ServiceResponse<CategoryDto>(query, "Category found");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error getting category");
            return new ServiceResponse<CategoryDto>("Error getting category");
        }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="categoryId"></param>
    /// <returns></returns>
    public async Task<ServiceResponse<CategoryWithAssetsDto>> GetCategoryByIdWithAssets(int categoryId)
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();
        try
        {
            // get category
            var query = await context.Categories
                .AsNoTracking()
                .Select(c => new CategoryWithAssetsDto
                {
                    CategoryId = c.CategoryId,
                    Name = c.Name,
                    Description = c.Description,
                    UserId = c.UserId,
                    IsDeleted = c.IsDeleted,
                    Assets = c.AssetCategories.Select(ac => new AssetDto()
                    {
                        Name = ac.Asset.Name,
                        Description = ac.Asset.Description,
                        AssetId = ac.AssetId,
                        UserId = ac.UserId,
                        IsDeleted = ac.IsDeleted,
                        Status = ac.Asset.Status,
                        Process = ac.Asset.Process
                    }).ToList()
                }).FirstOrDefaultAsync(c => c.CategoryId == categoryId);
            if (query == null)
            {
                _logger.LogWarning("No category found");
                return new ServiceResponse<CategoryWithAssetsDto>("No category found", true);
            }
            // return category
            _logger.LogInformation("Category found");
            return new ServiceResponse<CategoryWithAssetsDto>(query, "Category found");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error getting category");
            return new ServiceResponse<CategoryWithAssetsDto>("Error getting category");
        }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="detailId"></param>
    /// <returns></returns>
    public async Task<ServiceResponse<DetailDto>> GetDetailById(int detailId)
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();
        try
        {
            // get detail
            var query = await context.Details
                .AsNoTracking()
                .Select(d => new DetailDto
                {
                    DetailId = d.DetailId,
                    Name = d.Name,
                    UserId = d.UserId,
                    Description = d.Description,
                    IsDeleted = d.IsDeleted
                }).FirstOrDefaultAsync(d => d.DetailId == detailId);
            if (query == null)
            {
                _logger.LogWarning("No detail found");
                return new ServiceResponse<DetailDto>("No detail found", true);
            }
            // return detail
            _logger.LogInformation("Detail found");
            return new ServiceResponse<DetailDto>(query, "Detail found");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error getting detail");
            return new ServiceResponse<DetailDto>("Error getting detail");
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns>ServiceResponse(data, string, true) on success</returns>
    public async Task<ServiceResponse<IEnumerable<DetailDto>>> GetDetails()
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();
        try
        {
            // get details
            var query = await context.Details
                .AsNoTracking()
                .Select(d => new DetailDto
                {
                    DetailId = d.DetailId,
                    Name = d.Name,
                    UserId = d.UserId,
                    Description = d.Description,
                    IsDeleted = d.IsDeleted
                }).ToListAsync();
            if (query.Count == 0)
            {
                _logger.LogWarning("No details found");
                return new ServiceResponse<IEnumerable<DetailDto>>("No details found");
            }

            // return details
            _logger.LogInformation("Details found");
            return new ServiceResponse<IEnumerable<DetailDto>>(query, "Details found");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error getting details");
            return new ServiceResponse<IEnumerable<DetailDto>>("Error getting details");
        }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <returns>ServiceResponse(data, string, true) on success</returns>
    public async Task<ServiceResponse<IEnumerable<DetailWithAssetsDto>>> GetDetailsWithAssets()
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();
        try
        {
            // get details
            var query = await context.Details
                .AsNoTracking()
                .Select(d => new DetailWithAssetsDto
                {
                    DetailId = d.DetailId,
                    Name = d.Name,
                    UserId = d.UserId,
                    Description = d.Description,
                    IsDeleted = d.IsDeleted,
                    Assets = d.AssetDetails.Select(ad => new AssetDto
                    {
                        Name = ad.Asset.Name,
                        Description = ad.Asset.Description,
                        AssetId = ad.AssetId,
                        UserId = ad.UserId,
                        Status = ad.Asset.Status,
                        Process = ad.Asset.Process,
                        IsDeleted = ad.Asset.IsDeleted
                    }).ToList()
                }).ToListAsync();
            if (query.Count == 0)
            {
                _logger.LogWarning("No details found");
                return new ServiceResponse<IEnumerable<DetailWithAssetsDto>>("No details found");
            }
            // return details
            _logger.LogInformation("Details found");
            return new ServiceResponse<IEnumerable<DetailWithAssetsDto>>(query, "Details found");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error getting details");
            return new ServiceResponse<IEnumerable<DetailWithAssetsDto>>("Error getting details");
        }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="deviceId"></param>
    /// <returns>ServiceResponse(data, string, true) on success</returns>
    public async Task<ServiceResponse<DeviceDto>> GetDeviceById(int deviceId)
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();
        try
        {
            // get device
            var query = await context.Devices
                .AsNoTracking()
                .Select(d => new DeviceDto
                {
                    DeviceId = d.DeviceId,
                    UserId = d.UserId,
                    Name = d.Name,
                    Description = d.Description,
                    IsDeleted = d.IsDeleted
                }).FirstOrDefaultAsync(d => d.DeviceId == deviceId);
            if (query == null)
            {
                _logger.LogWarning("No device found");
                return new ServiceResponse<DeviceDto>("No device found");
            }
            // return device
            _logger.LogInformation("Device found");
            return new ServiceResponse<DeviceDto>(query, "Device found");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error getting device");
            return new ServiceResponse<DeviceDto>("Error getting device");
        }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    /// <returns>ServiceResponse(data, string, true) on success</returns>
    public async Task<ServiceResponse<IEnumerable<DeviceDto>>> GetDevices()
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();
        try
        {
            // get devices
            var query = await context.Devices
                .AsNoTracking()
                .Select(d => new DeviceDto
                {
                    DeviceId = d.DeviceId,
                    Name = d.Name,
                    Description = d.Description,
                    IsDeleted = d.IsDeleted,
                    UserId = d.UserId
                }).ToListAsync();
            if (query.Count == 0)
            {
                _logger.LogWarning("No devices found");
                return new ServiceResponse<IEnumerable<DeviceDto>>("No devices found");
            }
            // return devices
            _logger.LogInformation("Devices found");
            return new ServiceResponse<IEnumerable<DeviceDto>>(query, "Devices found");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error getting devices");
            return new ServiceResponse<IEnumerable<DeviceDto>>("Error getting devices");
        }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    /// <returns>ServiceResponse(data, string, true) on success</returns>
    public async Task<ServiceResponse<IEnumerable<DeviceDto>>> GetDevicesWithModels()
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();
        try
        {
            // get devices
            var query = await context.Devices
                .AsNoTracking()
                .Select(d => new DeviceDto
                {
                    DeviceId = d.DeviceId,
                    Name = d.Name,
                    Description = d.Description,
                    IsDeleted = d.IsDeleted,
                    UserId = d.UserId,
                    Models = d.Models.Select(m => new ModelDto
                    {
                        ModelId = m.ModelId,
                        Name = m.Name,
                        Description = m.Description,
                        IsDeleted = m.IsDeleted,
                        UserId = m.UserId
                    }).ToList()
                }).ToListAsync();
            if (query.Count == 0)
            {
                _logger.LogWarning("No devices found");
                return new ServiceResponse<IEnumerable<DeviceDto>>("No devices found");
            }
            // return devices
            _logger.LogInformation("Devices found");
            return new ServiceResponse<IEnumerable<DeviceDto>>(query, "Devices found");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error getting devices");
            return new ServiceResponse<IEnumerable<DeviceDto>>("Error getting devices");
        }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="modelId"></param>
    /// <returns>ServiceResponse(data, string, true) on success</returns>
    public async Task<ServiceResponse<ModelDto>> GetModelById(int modelId)
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();
        try
        {
            // get model
            var query = await context.Models
                .AsNoTracking()
                .Select(m => new ModelDto
                {
                    ModelId = m.ModelId,
                    Name = m.Name,
                    Description = m.Description,
                    IsDeleted = m.IsDeleted,
                    UserId = m.UserId
                }).FirstOrDefaultAsync(m => m.ModelId == modelId);
            if (query == null)
            {
                _logger.LogWarning("Model not found");
                return new ServiceResponse<ModelDto>("Model not found");
            }
            // return model
            _logger.LogInformation("Model found");
            return new ServiceResponse<ModelDto>(query, "Model found");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error getting model");
            return new ServiceResponse<ModelDto>("Error getting model");
        }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <returns>ServiceResponse(data, string, true) on success</returns>
    public async Task<ServiceResponse<IEnumerable<ModelDto>>> GetModels()
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();
        try
        {
            // get models
            var query = await context.Models
                .AsNoTracking()
                .Select(m => new ModelDto
                {
                    ModelId = m.ModelId,
                    Name = m.Name,
                    Description = m.Description,
                    IsDeleted = m.IsDeleted,
                    UserId = m.UserId
                }).ToListAsync();
            if (query.Count == 0)
            {
                _logger.LogWarning("No models found");
                return new ServiceResponse<IEnumerable<ModelDto>>("No models found");
            }
            // return models
            _logger.LogInformation("Models found");
            return new ServiceResponse<IEnumerable<ModelDto>>(query, "Models found");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error getting models");
            return new ServiceResponse<IEnumerable<ModelDto>>("Error getting models");
        }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <returns>ServiceResponse(data, string, true) on success</returns>
    public async Task<ServiceResponse<IEnumerable<ModelDto>>> GetModelsWithAssets()
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();
        try
        {
            // get models
            var query = await context.Models
                .AsNoTracking()
                .Select(m => new ModelDto
                {
                    ModelId = m.ModelId,
                    Name = m.Name,
                    Description = m.Description,
                    IsDeleted = m.IsDeleted,
                    UserId = m.UserId,
                    Assets = m.Assets.Select(a => new AssetDto
                    {
                        AssetId = a.AssetId,
                        Name = a.Name,
                        Description = a.Description,
                        IsDeleted = a.IsDeleted,
                        UserId = a.UserId
                    }).ToList()
                }).ToListAsync();
            if (query.Count == 0)
            {
                _logger.LogWarning("No models found");
                return new ServiceResponse<IEnumerable<ModelDto>>("No models found");
            }
            // return models
            _logger.LogInformation("Models found");
            return new ServiceResponse<IEnumerable<ModelDto>>(query, "Models found");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error getting models");
            return new ServiceResponse<IEnumerable<ModelDto>>("Error getting models");
        }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="parameterId"></param>
    /// <returns>ServiceResponse(data, string, true) on success</returns>
    public async Task<ServiceResponse<ParameterDto>> GetParameterById(int parameterId)
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();
        try
        {
            // get parameter
            var query = await context.Parameters
                .AsNoTracking()
                .Select(m => new ParameterDto
                {
                    ParameterId = m.ParameterId,
                    Name = m.Name,
                    Description = m.Description,
                    IsDeleted = m.IsDeleted,
                    UserId = m.UserId
                }).FirstOrDefaultAsync(m => m.ParameterId == parameterId);
            if (query == null)
            {
                _logger.LogWarning("Parameter not found");
                return new ServiceResponse<ParameterDto>("Parameter not found");
            }
            // return parameter
            _logger.LogInformation("Parameter found");
            return new ServiceResponse<ParameterDto>(query, "Parameter found");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error getting parameter");
            return new ServiceResponse<ParameterDto>("Error getting parameter");
        }
    }
    /// <returns>ServiceResponse(data, string, true) on success</returns>
    public async Task<ServiceResponse<IEnumerable<ParameterDto>>> GetParameters()
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();
        try
        {
            // get parameters
            var query = await context.Parameters
                .AsNoTracking()
                .Select(m => new ParameterDto
                {
                    ParameterId = m.ParameterId,
                    Name = m.Name,
                    Description = m.Description,
                    IsDeleted = m.IsDeleted,
                    UserId = m.UserId
                }).ToListAsync();
            if (query.Count == 0)
            {
                _logger.LogWarning("No parameters found");
                return new ServiceResponse<IEnumerable<ParameterDto>>("No parameters found");
            }
            // return parameters
            _logger.LogInformation("Parameters found");
            return new ServiceResponse<IEnumerable<ParameterDto>>(query, "Parameters found");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error getting parameters");
            return new ServiceResponse<IEnumerable<ParameterDto>>("Error getting parameters");
        }
    }

    /// <returns>ServiceResponse(data, string, true) on success</returns>
    public async Task<ServiceResponse<IEnumerable<ParameterWithModelsDto>>> GetParametersWithModels()
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();
        try
        {
            // get parameters
            var query = await context.Parameters
                .AsNoTracking()
                .Select(p => new ParameterWithModelsDto
                {
                    ParameterId = p.ParameterId,
                    Name = p.Name,
                    Description = p.Description,
                    IsDeleted = p.IsDeleted,
                    UserId = p.UserId,
                    Models = p.ModelParameters.Select(mp => new ModelDto
                    {
                        ModelId = mp.ModelId,
                        Name = mp.Model.Name,
                        Description = mp.Model.Description,
                        IsDeleted = mp.Model.IsDeleted,
                        UserId = mp.Model.UserId
                    }).ToList()
                }).ToListAsync();
            if (query.Count == 0)
            {
                _logger.LogWarning("No parameters found");
                return new ServiceResponse<IEnumerable<ParameterWithModelsDto>>("No parameters found");
            }
            // return parameters
            _logger.LogInformation("Parameters found");
            return new ServiceResponse<IEnumerable<ParameterWithModelsDto>>(query, "Parameters found");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error getting parameters");
            return new ServiceResponse<IEnumerable<ParameterWithModelsDto>>("Error getting parameters");
        }
    }
    /// <returns>ServiceResponse(string, true) on success</returns>
    public async Task<ServiceResponse> MarkDeleteAsset(int assetId, string userId)
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();
        // await using transaction
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            // get asset
            var asset = await context.Assets
                .FirstOrDefaultAsync(m => m.AssetId == assetId);
            if (asset == null)
            {
                _logger.LogWarning("Asset not found");
                return new ServiceResponse("Asset not found");
            }
            // check if asset is already deleted
            if (asset.IsDeleted)
            {
                _logger.LogWarning("Asset is already deleted");
                return new ServiceResponse("Asset is already deleted");
            }
            // mark asset as deleted
            asset.IsDeleted = true;
            asset.UserId = userId;
            // update asset
            context.Update(asset);
            // save changes
            await context.SaveChangesAsync();
            // await commit transaction
            await transaction.CommitAsync();
            // return success
            _logger.LogInformation("Asset marked as deleted");
            return new ServiceResponse("Asset marked as deleted", true);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error marking asset as deleted");
            // await rollback transaction
            await transaction.RollbackAsync();
            return new ServiceResponse("Error marking asset as deleted");
        }
    }
    /// <returns>ServiceResponse(data, string, true) on success</returns>
    public async Task<ServiceResponse> MarkDeleteCategory(int categoryId, string userId)
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();
        // await using transaction
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            // get category
            var category = await context.Categories
                .FirstOrDefaultAsync(m => m.CategoryId == categoryId);
            if (category == null)
            {
                _logger.LogWarning("Category not found");
                return new ServiceResponse("Category not found");
            }
            // check if category is already deleted
            if (category.IsDeleted)
            {
                _logger.LogWarning("Category is already deleted");
                return new ServiceResponse("Category is already deleted");
            }
            // mark category as deleted
            category.IsDeleted = true;
            category.UserId = userId;
            // update category
            context.Update(category);
            // save changes
            await context.SaveChangesAsync();
            // await commit transaction
            await transaction.CommitAsync();
            // return success
            _logger.LogInformation("Category marked as deleted");
            return new ServiceResponse("Category marked as deleted", true);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error marking category as deleted");
            // await rollback transaction
            await transaction.RollbackAsync();
            return new ServiceResponse("Error marking category as deleted");
        }

    }
    /// <returns>ServiceResponse(data, string, true) on success</returns>
    public async Task<ServiceResponse> MarkDeleteDetail(int detailId, string userId)
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();
        // await using transaction
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            // get detail
            var detail = await context.Details
                .FirstOrDefaultAsync(m => m.DetailId == detailId);
            if (detail == null)
            {
                _logger.LogWarning("Detail not found");
                return new ServiceResponse("Detail not found");
            }
            // check if detail is already deleted
            if (detail.IsDeleted)
            {
                _logger.LogWarning("Detail is already deleted");
                return new ServiceResponse("Detail is already deleted");
            }
            // mark detail as deleted
            detail.IsDeleted = true;
            detail.UserId = userId;
            // update detail
            context.Update(detail);
            // save changes
            await context.SaveChangesAsync();
            // await commit transaction
            await transaction.CommitAsync();
            // return success
            _logger.LogInformation("Detail marked as deleted");
            return new ServiceResponse("Detail marked as deleted", true);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error marking detail as deleted");
            // await rollback transaction
            await transaction.RollbackAsync();
            return new ServiceResponse("Error marking detail as deleted");
        }
    }
    /// <returns>ServiceResponse(data, string, true) on success</returns>
    public async Task<ServiceResponse> MarkDeleteDevice(int deviceId, string userId)
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();
        // await using transaction
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            // get device
            var device = await context.Devices
                .FirstOrDefaultAsync(m => m.DeviceId == deviceId);
            if (device == null)
            {
                _logger.LogWarning("Device not found");
                return new ServiceResponse("Device not found");
            }
            // check if device is already deleted
            if (device.IsDeleted)
            {
                _logger.LogWarning("Device is already deleted");
                return new ServiceResponse("Device is already deleted");
            }
            // mark device as deleted
            device.IsDeleted = true;
            device.UserId = userId;
            // update device
            context.Update(device);
            // save changes
            await context.SaveChangesAsync();
            // await commit transaction
            await transaction.CommitAsync();
            // return success
            _logger.LogInformation("Device marked as deleted");
            return new ServiceResponse("Device marked as deleted", true);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error marking device as deleted");
            // await rollback transaction
            await transaction.RollbackAsync();
            return new ServiceResponse("Error marking device as deleted");
        }
    }
    /// <returns>ServiceResponse(data, string, true) on success</returns>
    public async Task<ServiceResponse> MarkDeleteModel(int modelId, string userId)
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();
        // await using transaction
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            // get model
            var model = await context.Models
                .FirstOrDefaultAsync(m => m.ModelId == modelId);
            if (model == null)
            {
                _logger.LogWarning("Model not found");
                return new ServiceResponse("Model not found");
            }
            // check if model is already deleted
            if (model.IsDeleted)
            {
                _logger.LogWarning("Model is already deleted");
                return new ServiceResponse("Model is already deleted");
            }
            // mark model as deleted
            model.IsDeleted = true;
            model.UserId = userId;
            // update model
            context.Update(model);
            // save changes
            await context.SaveChangesAsync();
            // await commit transaction
            await transaction.CommitAsync();
            // return success
            _logger.LogInformation("Model marked as deleted");
            return new ServiceResponse("Model marked as deleted", true);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error marking model as deleted");
            // await rollback transaction
            await transaction.RollbackAsync();
            return new ServiceResponse("Error marking model as deleted");
        }
    }
    /// <returns>ServiceResponse(data, string, true) on success</returns>
    public async Task<ServiceResponse> MarkDeleteParameter(int parameterId, string userId)
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();
        // await using transaction
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            // get parameter
            var parameter = await context.Parameters
                .FirstOrDefaultAsync(m => m.ParameterId == parameterId);
            if (parameter == null)
            {
                _logger.LogWarning("Parameter not found");
                return new ServiceResponse("Parameter not found");
            }
            // check if parameter is already deleted
            if (parameter.IsDeleted)
            {
                _logger.LogWarning("Parameter is already deleted");
                return new ServiceResponse("Parameter is already deleted");
            }
            // mark parameter as deleted
            parameter.IsDeleted = true;
            parameter.UserId = userId;
            // update parameter
            context.Update(parameter);
            // save changes
            await context.SaveChangesAsync();
            // await commit transaction
            await transaction.CommitAsync();
            // return success
            _logger.LogInformation("Parameter marked as deleted");
            return new ServiceResponse("Parameter marked as deleted", true);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error marking parameter as deleted");
            // await rollback transaction
            await transaction.RollbackAsync();
            return new ServiceResponse("Error marking parameter as deleted");
        }
    }

    /// <returns>ServiceResponse(data, string, true) on success</returns>
    public async Task<ServiceResponse> UpdateAsset(int assetId, AssetUpdateDto assetUpdateDto, string userId)
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();
        // await using transaction
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            // get asset with all related data
            var asset = await context.Assets.Include(a => a.AssetCategories).Include(a => a.AssetDetails).Include(a => a.AssetSituations).Include(a => a.CommunicateAssets)
                .FirstOrDefaultAsync(m => m.AssetId == assetId);
            if (asset == null)
            {
                _logger.LogWarning("Asset not found");
                return new ServiceResponse("Asset not found");
            }
            var modelFromDto = asset.ModelId == assetUpdateDto.ModelId?null:await context.Models.FirstOrDefaultAsync(m => m.ModelId == assetUpdateDto.ModelId);

            // if new asset model is different from old model
            if (modelFromDto is { IsDeleted: true } && asset.ModelId != assetUpdateDto.ModelId)
            {
                context.RemoveRange(asset.AssetCategories);
                context.RemoveRange(asset.AssetDetails);
                context.RemoveRange(asset.AssetSituations);
                context.RemoveRange(asset.CommunicateAssets);
                asset.ModelId = assetUpdateDto.ModelId;
            }

            // get coordinate
            var coordinateFromDto = asset.CoordinateId == assetUpdateDto.CoordinateId
                ?null
                :await context.Coordinates
                    .FirstOrDefaultAsync(m => m.CoordinateId == assetUpdateDto.CoordinateId);

            // if new coordinate is different from old coordinate
            if (coordinateFromDto is { IsDeleted: false } && asset.CoordinateId != assetUpdateDto.CoordinateId)
            {
                asset.CoordinateId = assetUpdateDto.CoordinateId;
            }
            // if new asset name is different from old asset name

            // check if asset name from dto is already taken
            var duplicate = await context.Assets.AnyAsync(a => a.Name.ToLower().Trim() == assetUpdateDto.Name.ToLower().Trim());
            if (duplicate || asset.Name.ToLower().Trim() == assetUpdateDto.Name.ToLower().Trim())
            {
                _logger.LogWarning("Asset name is already taken");
                return new ServiceResponse("Asset name is already taken");
            }
            else
            {
                asset.Name = assetUpdateDto.Name;
            }

            // assign userId to update
            asset.Description = assetUpdateDto.Description;
            asset.UserId = userId;
            asset.IsDeleted = false;
            // update asset
            context.Update(asset);
            // save changes
            await context.SaveChangesAsync();
            // await commit transaction
            await transaction.CommitAsync();
            // return success
            _logger.LogInformation("Asset updated");
            return new ServiceResponse("Asset updated", true);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error updating asset");
            // await rollback transaction
            await transaction.RollbackAsync();
            return new ServiceResponse("Error updating asset");
        }
    }

    public async Task<ServiceResponse> UpdateCategory(int categoryId, CategoryUpdateDto categoryUpdateDto, string userId)
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();
        // await using transaction
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            // get category with all related data
            var category = await context.Categories.FirstOrDefaultAsync(m => m.CategoryId == categoryId);
            if (category == null)
            {
                _logger.LogWarning("Category not found");
                return new ServiceResponse("Category not found");
            }
            // check if category name from dto is already taken
            var duplicate = await context.Categories.AnyAsync(a => a.Name.ToLower().Trim() == categoryUpdateDto.Name.ToLower().Trim());
            if (duplicate || category.Name.ToLower().Trim() == categoryUpdateDto.Name.ToLower().Trim())
            {
                _logger.LogWarning("Category name is already taken");
                return new ServiceResponse("Category name is already taken");
            }
            else
            {
                category.Name = categoryUpdateDto.Name;
            }
            // assign userId to update
            category.UserId = userId;
            category.Description = categoryUpdateDto.Description;
            category.IsDeleted = false;
            // update category
            context.Update(category);
            // save changes
            await context.SaveChangesAsync();
            // await commit transaction
            await transaction.CommitAsync();
            // return success
            _logger.LogInformation("Category updated");
            return new ServiceResponse("Category updated", true);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error updating category");
            // await rollback transaction
            await transaction.RollbackAsync();
            return new ServiceResponse("Error updating category");
        }
    }

    public async Task<ServiceResponse> UpdateDetail(int detailId, DetailUpdateDto detailUpdateDto, string userId)
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();
        // await using transaction
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            // get detail
            var detail = await context.Details.FirstOrDefaultAsync(m => m.DetailId == detailId);
            if (detail == null)
            {
                _logger.LogWarning("Detail not found");
                return new ServiceResponse("Detail not found");
            }
            // check if detail name from dto is already taken
            var duplicate = await context.Details.AnyAsync(a => a.Name.ToLower().Trim() == detailUpdateDto.Name.ToLower().Trim());
            if (duplicate || detail.Name.ToLower().Trim() == detailUpdateDto.Name.ToLower().Trim())
            {
                _logger.LogWarning("Detail name is already taken");
                return new ServiceResponse("Detail name is already taken");
            }
            else
            {
                detail.Name = detailUpdateDto.Name;
            }
            // assign userId to update
            detail.UserId = userId;
            detail.Description = detailUpdateDto.Description;
            detail.IsDeleted = false;
            // update detail
            context.Update(detail);
            // save changes
            await context.SaveChangesAsync();
            // await commit transaction
            await transaction.CommitAsync();
            // return success
            _logger.LogInformation("Detail updated");
            return new ServiceResponse("Detail updated", true);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error updating detail");
            // await rollback transaction
            await transaction.RollbackAsync();
            return new ServiceResponse("Error updating detail");
        }
    }

    public async Task<ServiceResponse> UpdateDevice(int deviceId, DeviceUpdateDto deviceUpdateDto, string userId)
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();
        // await using transaction
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            // get device
            var device = await context.Devices.FirstOrDefaultAsync(m => m.DeviceId == deviceId);
            if (device == null)
            {
                _logger.LogWarning("Device not found");
                return new ServiceResponse("Device not found");
            }
            // check if device name from dto is already taken
            var duplicate = await context.Devices.AnyAsync(a => a.Name.ToLower().Trim() == deviceUpdateDto.Name.ToLower().Trim());
            if (duplicate || device.Name.ToLower().Trim() == deviceUpdateDto.Name.ToLower().Trim())
            {
                _logger.LogWarning("Device name is already taken");
                return new ServiceResponse("Device name is already taken");
            }
            else
            {
                device.Name = deviceUpdateDto.Name;
            }
            // assign userId to update
            device.UserId = userId;
            device.Description = deviceUpdateDto.Description;
            device.IsDeleted = false;
            // update device
            context.Update(device);
            // save changes
            await context.SaveChangesAsync();
            // await commit transaction
            await transaction.CommitAsync();
            // return success
            _logger.LogInformation("Device updated");
            return new ServiceResponse("Device updated", true);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error updating device");
            // await rollback transaction
            await transaction.RollbackAsync();
            return new ServiceResponse("Error updating device");
        }
    }

    public async Task<ServiceResponse> UpdateModel(int modelId, ModelUpdateDto modelUpdateDto, string userId)
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();
        // await using transaction
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            // get model
            var model = await context.Models.FirstOrDefaultAsync(m => m.ModelId == modelId);
            if (model == null)
            {
                _logger.LogWarning("Model not found");
                return new ServiceResponse("Model not found");
            }
            // check if model name from dto is already taken
            var duplicate = await context.Models.AnyAsync(a => a.Name.ToLower().Trim() == modelUpdateDto.Name.ToLower().Trim());
            if (duplicate || model.Name.ToLower().Trim() == modelUpdateDto.Name.ToLower().Trim())
            {
                _logger.LogWarning("Model name is already taken");
                return new ServiceResponse("Model name is already taken");
            }
            else
            {
                model.Name = modelUpdateDto.Name;
            }
            // assign userId to update
            model.UserId = userId;
            model.Description = modelUpdateDto.Description;
            model.IsDeleted = false;
            // update model
            context.Update(model);
            // save changes
            await context.SaveChangesAsync();
            // await commit transaction
            await transaction.CommitAsync();
            // return success
            _logger.LogInformation("Model updated");
            return new ServiceResponse("Model updated", true);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error updating model");
            // await rollback transaction
            await transaction.RollbackAsync();
            return new ServiceResponse("Error updating model");
        }
    }

    public async Task<ServiceResponse> UpdateParameter(int parameterId, ParameterUpdateDto parameterUpdateDto, string userId)
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();
        // await using transaction
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            // get parameter
            var parameter = await context.Parameters.FirstOrDefaultAsync(p => p.ParameterId == parameterId);
            if (parameter == null)
            {
                _logger.LogWarning("Parameter not found");
                return new ServiceResponse("Parameter not found");
            }
            // check if parameter name from dto is already taken
            var duplicate = await context.Parameters.AnyAsync(a => a.Name.ToLower().Trim() == parameterUpdateDto.Name.ToLower().Trim());
            if (duplicate || parameter.Name.ToLower().Trim() == parameterUpdateDto.Name.ToLower().Trim())
            {
                _logger.LogWarning("Parameter name is already taken");
                return new ServiceResponse("Parameter name is already taken");
            }
            else
            {
                parameter.Name = parameterUpdateDto.Name;
            }
            // assign userId to update
            parameter.UserId = userId;
            parameter.Description = parameterUpdateDto.Description;
            parameter.IsDeleted = false;
            // update parameter
            context.Update(parameter);
            // save changes
            await context.SaveChangesAsync();
            // await commit transaction
            await transaction.CommitAsync();
            // return success
            _logger.LogInformation("Parameter updated");
            return new ServiceResponse("Parameter updated", true);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error updating parameter");
            // await rollback transaction
            await transaction.RollbackAsync();
            return new ServiceResponse("Error updating parameter");
        }
    }

    /// <summary>
    /// Updates or creates AssetCategory, returns ServiceResponse(string, true) if successful
    /// </summary>
    /// <param name="assetCategoryDto"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    public async Task<ServiceResponse> AddOrUpdateAssetCategory(AssetCategoryDto assetCategoryDto, string userId)
    {

        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();
        // await using transaction
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            // get assetCategory
            var assetCategory = await context.AssetCategories.FindAsync(assetCategoryDto.AssetId, assetCategoryDto.CategoryId);
            if (assetCategory == null)
            {
                var category = assetCategoryDto.CategoryId < 1?null:await context.AssetCategories.FirstOrDefaultAsync(a => a.CategoryId == assetCategoryDto.CategoryId);
                if (category == null || category.IsDeleted)
                {
                    _logger.LogWarning("Category not found");
                    return new ServiceResponse("Category not found");
                }
                var asset = assetCategoryDto.AssetId < 1?null:await context.Assets.FirstOrDefaultAsync(a => a.AssetId == assetCategoryDto.AssetId);
                if (asset == null || asset.IsDeleted)
                {
                    _logger.LogWarning("Asset not found");
                    return new ServiceResponse("Asset not found");
                }
                assetCategory = new AssetCategory
                {
                    AssetId = assetCategoryDto.AssetId,
                    CategoryId = assetCategoryDto.CategoryId,
                    UserId = userId,
                    IsDeleted = false
                };
                context.Add(assetCategory);
            }
            else
            {
                assetCategory.UserId = userId;
                assetCategory.IsDeleted = false;
                context.Update(assetCategory);
            }

            // save changes
            await context.SaveChangesAsync();
            await transaction.CommitAsync();
            _logger.LogInformation("AssetCategory updated");
            return new ServiceResponse("AssetCategory updated", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating assetCategory");
            await transaction.RollbackAsync();
            return new ServiceResponse("Error updating assetCategory");
        }
    }
    public async Task<ServiceResponse> MarkDeleteAssetCategory(AssetCategoryDto assetCategoryDto, string userId)    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();
        // await using transaction
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            // get assetCategory
            var assetCategory = await context.AssetCategories.FindAsync(assetCategoryDto.AssetId, assetCategoryDto.CategoryId);
            if (assetCategory == null)
            {
                _logger.LogWarning("AssetCategory not found");
                return new ServiceResponse("AssetCategory not found");
            }
            // check if assetCategory is already deleted
            if (assetCategory.IsDeleted)
            {
                _logger.LogWarning("AssetCategory already deleted");
                return new ServiceResponse("AssetCategory already deleted");
            }
            assetCategory.UserId = userId;
            assetCategory.IsDeleted = true;
            context.Update(assetCategory);
            // save changes
            await context.SaveChangesAsync();
            await transaction.CommitAsync();
            _logger.LogInformation("AssetCategory marked as deleted");
            return new ServiceResponse("AssetCategory marked as deleted", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking assetCategory as deleted");
            await transaction.RollbackAsync();
            return new ServiceResponse("Error marking assetCategory as deleted");
        }
    }
    public async Task<ServiceResponse> AddOrUpdateAssetDetail(AssetDetailDto assetDetailDto, string userId)
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();
        // await using transaction
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            // get assetDetail
            var assetDetail = await context.AssetDetails.FindAsync(assetDetailDto.AssetId, assetDetailDto.DetailId);
            if (assetDetail == null)
            {
                var detail = assetDetailDto.DetailId < 1?null:await context.AssetDetails.FirstOrDefaultAsync(a => a.DetailId == assetDetailDto.DetailId);
                if (detail == null || detail.IsDeleted)
                {
                    _logger.LogWarning("Detail not found");
                    return new ServiceResponse("Detail not found");
                }
                var asset = assetDetailDto.AssetId < 1?null:await context.Assets.FirstOrDefaultAsync(a => a.AssetId == assetDetailDto.AssetId);
                if (asset == null || asset.IsDeleted)
                {
                    _logger.LogWarning("Asset not found");
                    return new ServiceResponse("Asset not found");
                }
                assetDetail = new AssetDetail
                {
                    AssetId = assetDetailDto.AssetId,
                    DetailId = assetDetailDto.DetailId,
                    Value = assetDetailDto.Value,
                    UserId = userId,
                    IsDeleted = false
                };
                context.Add(assetDetail);
            }
            else
            {
                assetDetail.Value = assetDetailDto.Value;
                assetDetail.UserId = userId;
                assetDetail.IsDeleted = false;
                context.Update(assetDetail);
            }

            // save changes
            await context.SaveChangesAsync();
            await transaction.CommitAsync();
            _logger.LogInformation("AssetDetail updated");
            return new ServiceResponse("AssetDetail updated", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating assetDetail");
            await transaction.RollbackAsync();
            return new ServiceResponse("Error updating assetDetail");
        }
    }
    public async Task<ServiceResponse> MarkDeleteAssetDetail(AssetDetailDto assetDetailDto, string userId)
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();
        // await using transaction
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            // get assetDetail
            var assetDetail = await context.AssetDetails.FindAsync(assetDetailDto.AssetId, assetDetailDto.DetailId);
            if (assetDetail == null)
            {
                _logger.LogWarning("AssetDetail not found");
                return new ServiceResponse("AssetDetail not found");
            }
            // check if assetDetail is already deleted
            if (assetDetail.IsDeleted)
            {
                _logger.LogWarning("AssetDetail already deleted");
                return new ServiceResponse("AssetDetail already deleted");
            }
            assetDetail.UserId = userId;
            assetDetail.IsDeleted = true;
            context.Update(assetDetail);
            // save changes
            await context.SaveChangesAsync();
            await transaction.CommitAsync();
            _logger.LogInformation("AssetDetail marked as deleted");
            return new ServiceResponse("AssetDetail marked as deleted", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking assetDetail as deleted");
            await transaction.RollbackAsync();
            return new ServiceResponse("Error marking assetDetail as deleted");
        }
    }
    public async Task<ServiceResponse> AddOrUpdateModelParameter(ModelParameterDto modelParameterDto, string userId)
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();
        // await using transaction
        await using var transaction = await context.Database.BeginTransactionAsync();
        var modelParameter = await context.ModelParameters.FindAsync(modelParameterDto.ModelId, modelParameterDto.ParameterId);
        try
        {
            // get modelParameter
            if (modelParameter == null)
            {
                var parameter = modelParameterDto.ParameterId < 1?null:await context.ModelParameters.FirstOrDefaultAsync(a => a.ParameterId == modelParameterDto.ParameterId);
                if (parameter == null || parameter.IsDeleted)
                {
                    _logger.LogWarning("Parameter not found");
                    return new ServiceResponse("Parameter not found");
                }
                var model = modelParameterDto.ModelId < 1?null:await context.Models.FirstOrDefaultAsync(a => a.ModelId == modelParameterDto.ModelId);
                if (model == null || model.IsDeleted)
                {
                    _logger.LogWarning("Model not found");
                    return new ServiceResponse("Model not found");
                }
                modelParameter = new ModelParameter
                {
                    ModelId = modelParameterDto.ModelId,
                    ParameterId = modelParameterDto.ParameterId,
                    Value = modelParameterDto.Value,
                    UserId = userId,
                    IsDeleted = false
                };
                context.Add(modelParameter);
            }
            else
            {
                modelParameter.Value = modelParameterDto.Value;
                modelParameter.UserId = userId;
                modelParameter.IsDeleted = false;
                context.Update(modelParameter);
            }

            // save changes
            await context.SaveChangesAsync();
            await transaction.CommitAsync();
            _logger.LogInformation("ModelParameter updated");
            return new ServiceResponse("ModelParameter updated", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating modelParameter");
            await transaction.RollbackAsync();
            return new ServiceResponse("Error updating modelParameter");
        }
    }
    public async Task<ServiceResponse> MarkDeleteModelParameter(ModelParameterDto modelParameterDto, string userId)
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();
        // await using transaction
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            var modelParameter = await context.ModelParameters.FindAsync(modelParameterDto.ModelId, modelParameterDto.ParameterId);
            if (modelParameter == null)
            {
                _logger.LogWarning("ModelParameter not found");
                return new ServiceResponse("ModelParameter not found");
            }
            if (modelParameter.IsDeleted)
            {
                _logger.LogWarning("ModelParameter already deleted");
                return new ServiceResponse("ModelParameter already deleted");
            }
            modelParameter.UserId = userId;
            modelParameter.IsDeleted = true;
            context.Update(modelParameter);
            // save changes
            await context.SaveChangesAsync();
            await transaction.CommitAsync();
            _logger.LogInformation("ModelParameter marked as deleted");
            return new ServiceResponse("ModelParameter marked as deleted", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking modelParameter as deleted");
            await transaction.RollbackAsync();
            return new ServiceResponse("Error marking modelParameter as deleted");
        }
    }
    public async Task<ServiceResponse> DeleteAssetCategory(AssetCategoryDto assetCategoryDto, string userId)
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();
        // await using transaction
        await using var transaction = await context.Database.BeginTransactionAsync();
        var assetCategory = await context.AssetCategories.FindAsync(assetCategoryDto.AssetId, assetCategoryDto.CategoryId);
        try
        {
            // get assetCategory
            if (assetCategory == null)
            {
                _logger.LogWarning("AssetCategory not found");
                return new ServiceResponse("AssetCategory not found");
            }
            if (assetCategory.IsDeleted)
            {
                context.AssetCategories.Remove(assetCategory);
                // save changes
                await context.SaveChangesAsync();
                await transaction.CommitAsync();
                _logger.LogInformation("AssetCategory deleted");
                return new ServiceResponse("AssetCategory deleted", true);
            }
            _logger.LogWarning("AssetCategory is not marked as deleted");
            return new ServiceResponse("AssetCategory is not marked as deleted");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting assetCategory");
            await transaction.RollbackAsync();
            return new ServiceResponse("Error deleting assetCategory");
        }
    }

    public async Task<ServiceResponse> DeleteAssetDetail(AssetDetailDto assetDetailDto, string userId)
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();
        // await using transaction
        await using var transaction = await context.Database.BeginTransactionAsync();
        var assetDetail = await context.AssetDetails.FindAsync(assetDetailDto.AssetId, assetDetailDto.DetailId);
        try
        {
            // get assetDetail
            if (assetDetail == null)
            {
                _logger.LogWarning("AssetDetail not found");
                return new ServiceResponse("AssetDetail not found");
            }
            if (assetDetail.IsDeleted)
            {
                context.AssetDetails.Remove(assetDetail);
                // save changes
                await context.SaveChangesAsync();
                await transaction.CommitAsync();
                _logger.LogInformation("AssetDetail deleted");
                return new ServiceResponse("AssetDetail deleted", true);
            }
            _logger.LogWarning("AssetDetail is not marked as deleted");
            return new ServiceResponse("AssetDetail is not marked as deleted");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting assetDetail");
            await transaction.RollbackAsync();
            return new ServiceResponse("Error deleting assetDetail");
        }
    }

    public async Task<ServiceResponse> DeleteModelParameter(ModelParameterDto modelParameterDto, string userId)
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();
        // await using transaction
        await using var transaction = await context.Database.BeginTransactionAsync();
        var modelParameter = await context.ModelParameters.FindAsync(modelParameterDto.ModelId, modelParameterDto.ParameterId);
        try
        {
            // get modelParameter
            if (modelParameter == null)
            {
                _logger.LogWarning("ModelParameter not found");
                return new ServiceResponse("ModelParameter not found");
            }
            if (modelParameter.IsDeleted)
            {
                context.ModelParameters.Remove(modelParameter);
                // save changes
                await context.SaveChangesAsync();
                await transaction.CommitAsync();
                _logger.LogInformation("ModelParameter deleted");
                return new ServiceResponse("ModelParameter deleted", true);
            }
            _logger.LogWarning("ModelParameter is not marked as deleted");
            return new ServiceResponse("ModelParameter is not marked as deleted");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting modelParameter");
            await transaction.RollbackAsync();
            return new ServiceResponse("Error deleting modelParameter");
        }
    }
}
