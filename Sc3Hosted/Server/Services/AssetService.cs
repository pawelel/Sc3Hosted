using Microsoft.EntityFrameworkCore;
using Sc3Hosted.Server.Data;
using Sc3Hosted.Server.Entities;
using Sc3Hosted.Server.Exceptions;
using Sc3Hosted.Shared.Dtos;

namespace Sc3Hosted.Server.Services;
public interface IAssetService
{
    Task<bool> AddOrUpdateAssetCategory(AssetCategoryDto assetCategoryDto);

    Task<bool> AddOrUpdateAssetDetail(AssetDetailDto assetDetailDto);

    Task<bool> AddOrUpdateModelParameter(ModelParameterDto modelParameterDto);

    Task<bool> ChangeAssetsModel(int assetId, int modelId);

    Task<int> CreateAsset(AssetCreateDto assetCreateDto);

    Task<int> CreateCategory(CategoryCreateDto categoryCreateDto);

    Task<int> CreateDetail(DetailCreateDto detailCreateDto);

    Task<int> CreateDevice(DeviceCreateDto deviceCreateDto);

    Task<int> CreateModel(int deviceId, ModelCreateDto modelCreateDto);

    Task<int> CreateParameter(ParameterCreateDto parameterCreateDto);

    Task<bool> DeleteAsset(int assetId);

    Task<bool> DeleteAssetCategory(AssetCategoryDto assetCategoryDto);

    Task<bool> DeleteAssetDetail(AssetDetailDto assetDetailDto);

    Task<bool> DeleteCategory(int categoryId);

    Task<bool> DeleteDetail(int detailId);

    Task<bool> DeleteDevice(int deviceId);

    Task<bool> DeleteModel(int modelId);

    Task<bool> DeleteModelParameter(ModelParameterDto modelParameterDto);

    Task<bool> DeleteParameter(int parameterId);

    Task<AssetDto> GetAssetById(int assetId);

    Task<IEnumerable<AssetDisplayDto>> GetAssetDisplay();

    Task<IEnumerable<AssetDto>> GetAssets();
    Task<IEnumerable<CategoryDto>> GetCategories();

    Task<IEnumerable<CategoryWithAssetsDto>> GetCategoriesWithAssets();

    Task<CategoryDto> GetCategoryById(int categoryId);

    Task<CategoryWithAssetsDto> GetCategoryByIdWithAssets(int categoryId);

    Task<DetailDto> GetDetailById(int detailId);

    Task<IEnumerable<DetailDto>> GetDetails();

    Task<IEnumerable<DetailWithAssetsDto>> GetDetailsWithAssets();

    Task<DeviceDto> GetDeviceById(int deviceId);

    Task<IEnumerable<DeviceDto>> GetDevices();

    Task<IEnumerable<DeviceDto>> GetDevicesWithModels();

    Task<ModelDto> GetModelById(int modelId);

    Task<IEnumerable<ModelDto>> GetModels();

    Task<IEnumerable<ModelDto>> GetModelsWithAssets();

    Task<ParameterDto> GetParameterById(int parameterId);

    Task<IEnumerable<ParameterDto>> GetParameters();

    Task<IEnumerable<ParameterWithModelsDto>> GetParametersWithModels();

    Task<bool> MarkDeleteAsset(int assetId);

    Task<bool> MarkDeleteAssetCategory(AssetCategoryDto assetCategoryDto);

    Task<bool> MarkDeleteAssetDetail(AssetDetailDto assetDetailDto);

    Task<bool> MarkDeleteCategory(int categoryId);

    Task<bool> MarkDeleteDetail(int detailId);

    Task<bool> MarkDeleteDevice(int deviceId);

    Task<bool> MarkDeleteModel(int modelId);

    Task<bool> MarkDeleteModelParameter(ModelParameterDto modelParameterDto);

    Task<bool> MarkDeleteParameter(int parameterId);

    Task<bool> UpdateAsset(int assetId, AssetUpdateDto assetUpdateDto);

    Task<bool> UpdateCategory(int categoryId, CategoryUpdateDto categoryUpdateDto);

    Task<bool> UpdateDetail(int detailId, DetailUpdateDto detailUpdateDto);

    Task<bool> UpdateDevice(int deviceId, DeviceUpdateDto deviceUpdateDto);

    Task<bool> UpdateModel(int modelId, ModelUpdateDto modelUpdateDto);

    Task<bool> UpdateParameter(int parameterId, ParameterUpdateDto parameterUpdateDto);
}

public class AssetService : IAssetService
{private readonly IUserContextService _userContextService;
    private readonly IDbContextFactory<Sc3HostedDbContext> _contextFactory;
    private readonly ILogger<AssetService> _logger;

    public AssetService(IDbContextFactory<Sc3HostedDbContext> contextFactory, ILogger<AssetService> logger, IUserContextService userContextService)
    {
        _contextFactory = contextFactory;
        _logger = logger;
        _userContextService = userContextService;
    }

    public async Task<bool> AddOrUpdateAssetCategory(AssetCategoryDto assetCategoryDto)
    {var userId = _userContextService.UserId;
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();

        // get assetCategory
        var assetCategory = await context.AssetCategories.FindAsync(assetCategoryDto.AssetId, assetCategoryDto.CategoryId);
        if (assetCategory == null)
        {
            var category = assetCategoryDto.CategoryId < 1?null:await context.AssetCategories.FirstOrDefaultAsync(a => a.CategoryId == assetCategoryDto.CategoryId);
            if (category == null || category.IsDeleted)
            {
                _logger.LogWarning("Category not found");
                throw new NotFoundException("Category not found");
            }
            var asset = assetCategoryDto.AssetId < 1?null:await context.Assets.FirstOrDefaultAsync(a => a.AssetId == assetCategoryDto.AssetId);
            if (asset == null || asset.IsDeleted)
            {
                _logger.LogWarning("Asset not found");
                throw new NotFoundException("Asset not found");
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
        // await using transaction
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            // save changes
            await context.SaveChangesAsync();
            await transaction.CommitAsync();
            _logger.LogInformation("AssetCategory updated");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating assetCategory");
            await transaction.RollbackAsync();
            throw new BadRequestException("Error updating assetCategory");
        }
    }

    public async Task<bool> AddOrUpdateAssetDetail(AssetDetailDto assetDetailDto)
    {var userId = _userContextService.UserId;
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();

        // get assetDetail
        var assetDetail = await context.AssetDetails.FindAsync(assetDetailDto.AssetId, assetDetailDto.DetailId);
        if (assetDetail == null)
        {
            var detail = assetDetailDto.DetailId < 1?null:await context.AssetDetails.FirstOrDefaultAsync(a => a.DetailId == assetDetailDto.DetailId);
            if (detail == null || detail.IsDeleted)
            {
                _logger.LogWarning("Detail not found");
                throw new NotFoundException("Detail not found");
            }
            var asset = assetDetailDto.AssetId < 1?null:await context.Assets.FirstOrDefaultAsync(a => a.AssetId == assetDetailDto.AssetId);
            if (asset == null || asset.IsDeleted)
            {
                _logger.LogWarning("Asset not found");
                throw new NotFoundException("Asset not found");
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
        // await using transaction
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            // save changes
            await context.SaveChangesAsync();
            await transaction.CommitAsync();
            _logger.LogInformation("AssetDetail updated");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating assetDetail");
            await transaction.RollbackAsync();
            throw new BadRequestException("Error updating assetDetail");
        }
    }

    public async Task<bool> AddOrUpdateModelParameter(ModelParameterDto modelParameterDto)
    {var userId = _userContextService.UserId;
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();
        var modelParameter = await context.ModelParameters.FindAsync(modelParameterDto.ModelId, modelParameterDto.ParameterId);
        // get modelParameter
        if (modelParameter == null)
        {
            var parameter = modelParameterDto.ParameterId < 1?null:await context.ModelParameters.FirstOrDefaultAsync(a => a.ParameterId == modelParameterDto.ParameterId);
            if (parameter == null || parameter.IsDeleted)
            {
                _logger.LogWarning("Parameter not found");
                throw new NotFoundException("Parameter not found");
            }
            var model = modelParameterDto.ModelId < 1?null:await context.Models.FirstOrDefaultAsync(a => a.ModelId == modelParameterDto.ModelId);
            if (model == null || model.IsDeleted)
            {
                _logger.LogWarning("Model not found");
                throw new NotFoundException("Model not found");
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
        // await using transaction
        await using var transaction = await context.Database.BeginTransactionAsync();

        try
        {
            // save changes
            await context.SaveChangesAsync();
            await transaction.CommitAsync();
            _logger.LogInformation("ModelParameter updated");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating modelParameter");
            await transaction.RollbackAsync();
            throw new BadRequestException("Error updating modelParameter");
        }
    }

    public async Task<bool> ChangeAssetsModel(int assetId, int modelId)
    {var userId = _userContextService.UserId;
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();

        var asset = await context.Assets.FirstOrDefaultAsync(a => a.AssetId == assetId);
        if (asset == null)
        {
            _logger.LogWarning("Asset not found");
            throw new NotFoundException("Asset not found");
        }
        if (asset.ModelId == modelId)
        {
            _logger.LogWarning("Asset already assigned to model");
            throw new BadRequestException("Asset already has this model");
        }
        if (asset.IsDeleted)
        {
            _logger.LogWarning("Asset marked as deleted");
            return true;
        }

        var model = await context.Models.FirstOrDefaultAsync(m => m.ModelId == modelId);
        if (model == null)
        {
            _logger.LogWarning("Model not found");
            throw new NotFoundException("Model not found");
        }

        if (model.IsDeleted)
        {
            _logger.LogWarning("Model marked as deleted");
            throw new BadRequestException("Model marked as deleted");
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
        // await using transaction
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            // save changes
            await context.SaveChangesAsync();
            // commit transaction
            await transaction.CommitAsync();
            _logger.LogInformation("Asset with id {AssetId} updated", assetId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating asset with id {AssetId}", assetId);
            // rollback transaction
            await transaction.RollbackAsync();
            throw new BadRequestException($"Error updating asset with id {assetId}");
        }
    }

    public async Task<int> CreateAsset(AssetCreateDto assetCreateDto)
    {var userId = _userContextService.UserId;
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();

        // get model
        var model = await context.Models.FirstOrDefaultAsync(m => m.ModelId == assetCreateDto.ModelId);
        if (model == null)
        {
            _logger.LogWarning("Model not found");
            throw new NotFoundException("Model not found");
        }
        if (model.IsDeleted)
        {
            _logger.LogWarning("Model marked as deleted");
            throw new BadRequestException("Model marked as deleted");
        }
        // get coordinate
        var coordinate = await context.Coordinates.FirstOrDefaultAsync(c => c.CoordinateId == assetCreateDto.CoordinateId);
        if (coordinate == null)
        {
            _logger.LogWarning("Coordinate not found");
            throw new NotFoundException("Coordinate not found");
        }
        if (coordinate.IsDeleted)
        {
            _logger.LogWarning("Coordinate marked as deleted");
            throw new BadRequestException("Coordinate marked as deleted");
        }

        // validate asset name
        var duplicate = await context.Assets.AnyAsync(a => a.Name.ToLower().Trim() == assetCreateDto.Name.ToLower().Trim());
        if (duplicate)
        {
            _logger.LogWarning("Asset name already exists");
            throw new BadRequestException("Asset name already exists");
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
        // await using transaction
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            // save changes
            await context.SaveChangesAsync();
            // commit transaction
            await transaction.CommitAsync();
            _logger.LogInformation("Asset with id {AssetId} created", asset.AssetId);
            return asset.AssetId;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating asset");
            // rollback transaction
            await transaction.RollbackAsync();
            throw new BadRequestException("Error creating asset");
        }
    }

    public async Task<int> CreateCategory(CategoryCreateDto categoryCreateDto)
    {var userId = _userContextService.UserId;
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();

        // validate category name
        var duplicate = await context.Categories.AnyAsync(c => c.Name.ToLower().Trim() == categoryCreateDto.Name.ToLower().Trim());
        if (duplicate)
        {
            _logger.LogWarning("Category name already exists");
            throw new BadRequestException("Category name already exists");
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
        // await using transaction
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            // save changes
            await context.SaveChangesAsync();
            // commit transaction
            await transaction.CommitAsync();
            _logger.LogInformation("Category with id {CategoryId} created", category.CategoryId);
            return category.CategoryId;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating category");
            // rollback transaction
            await transaction.RollbackAsync();
            throw new BadRequestException("Error creating category");
        }
    }

    public async Task<int> CreateDetail(DetailCreateDto detailCreateDto)
    {var userId = _userContextService.UserId;
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();

        // validate detail name
        var duplicate = await context.Details.AnyAsync(d => d.Name.ToLower().Trim() == detailCreateDto.Name.ToLower().Trim());
        if (duplicate)
        {
            _logger.LogWarning("Detail name already exists");
            throw new BadRequestException("Detail name already exists");
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
        // await using transaction
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            // save changes
            await context.SaveChangesAsync();
            // commit transaction
            await transaction.CommitAsync();
            _logger.LogInformation("Detail with id {DetailId} created", detail.DetailId);
            return detail.DetailId;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating detail");
            // rollback transaction
            await transaction.RollbackAsync();
            throw new BadRequestException("Error creating detail");
        }
    }

    public async Task<int> CreateDevice(DeviceCreateDto deviceCreateDto)
    {var userId = _userContextService.UserId;
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();

        // validate device name
        var duplicate = await context.Devices.AnyAsync(d => d.Name.ToLower().Trim() == deviceCreateDto.Name.ToLower().Trim());
        if (duplicate)
        {
            _logger.LogWarning("Device name already exists");
            throw new BadRequestException("Device name already exists");
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
        // await using transaction
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            // save changes
            await context.SaveChangesAsync();
            // commit transaction
            await transaction.CommitAsync();
            _logger.LogInformation("Device with id {DeviceId} created", device.DeviceId);
            return device.DeviceId;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating device");
            // rollback transaction
            await transaction.RollbackAsync();
            throw new BadRequestException("Error creating device");
        }
    }

    public async Task<int> CreateModel(int deviceId, ModelCreateDto modelCreateDto)
    {var userId = _userContextService.UserId;
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();

        // validate model name
        var duplicate = await context.Models.AnyAsync(m => m.Name.ToLower().Trim() == modelCreateDto.Name.ToLower().Trim());
        if (duplicate)
        {
            _logger.LogWarning("Model name already exists");
            throw new BadRequestException("Model name already exists");
        }
        // get device
        var device = await context.Devices.FirstOrDefaultAsync(d => d.DeviceId == deviceId);
        if (device == null)
        {
            _logger.LogWarning("Device not found");
            throw new NotFoundException("Device not found");
        }
        // check if device is marked as deleted
        if (device.IsDeleted)
        {
            _logger.LogWarning("Device is marked as deleted");
            throw new BadRequestException("Device is marked as deleted");
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
        // await using transaction
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            // save changes
            await context.SaveChangesAsync();
            // commit transaction
            await transaction.CommitAsync();
            _logger.LogInformation("Model with id {ModelId} created", model.ModelId);
            return model.ModelId;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating model");
            // rollback transaction
            await transaction.RollbackAsync();
            throw new BadRequestException("Error creating model");
        }
    }

    public async Task<int> CreateParameter(ParameterCreateDto parameterCreateDto)
    {var userId = _userContextService.UserId;
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();

        // validate parameter name
        var duplicate = await context.Parameters.AnyAsync(p => p.Name.ToLower().Trim() == parameterCreateDto.Name.ToLower().Trim());
        if (duplicate)
        {
            _logger.LogWarning("Parameter name already exists");
            throw new BadRequestException("Parameter name already exists");
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
        // await using transaction
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            // save changes
            await context.SaveChangesAsync();
            // commit transaction
            await transaction.CommitAsync();
            _logger.LogInformation("Parameter with id {ParameterId} created", parameter.ParameterId);
            return parameter.ParameterId;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating parameter");
            // rollback transaction
            await transaction.RollbackAsync();
            throw new BadRequestException("Error creating parameter");
        }
    }

    public async Task<bool> DeleteAsset(int assetId)
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();

        // get asset
        var asset = await context.Assets.FindAsync(assetId);
        if (asset == null)
        {
            _logger.LogWarning("Asset not found");
            throw new NotFoundException("Asset not found");
        }
        // check if asset is marked as deleted
        if (asset.IsDeleted == false)
        {
            _logger.LogWarning("Asset is not marked as deleted");
            throw new BadRequestException("Asset is not marked as deleted");
        }
        // delete asset
        context.Assets.Remove(asset);
        // await using transaction
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            // save changes
            await context.SaveChangesAsync();
            // commit transaction
            await transaction.CommitAsync();
            _logger.LogInformation("Asset with id {AssetId} deleted", asset.AssetId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting asset");
            // rollback transaction
            await transaction.RollbackAsync();
            throw new BadRequestException("Error deleting asset");
        }
    }

    public async Task<bool> DeleteAssetCategory(AssetCategoryDto assetCategoryDto)
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();

        var assetCategory = await context.AssetCategories.FindAsync(assetCategoryDto.AssetId, assetCategoryDto.CategoryId);
        // get assetCategory
        if (assetCategory == null)
        {
            _logger.LogWarning("AssetCategory not found");
            throw new NotFoundException("AssetCategory not found");
        }
        if (assetCategory.IsDeleted)
        {
            context.AssetCategories.Remove(assetCategory);
            // await using transaction
            await using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                // save changes
                await context.SaveChangesAsync();
                await transaction.CommitAsync();
                _logger.LogInformation("AssetCategory deleted");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting assetCategory");
                await transaction.RollbackAsync();
                throw new BadRequestException("Error deleting assetCategory");
            }
        }
        _logger.LogWarning("AssetCategory is not marked as deleted");
        throw new BadRequestException("AssetCategory is not marked as deleted");
    }

    public async Task<bool> DeleteAssetDetail(AssetDetailDto assetDetailDto)
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();
        var assetDetail = await context.AssetDetails.FindAsync(assetDetailDto.AssetId, assetDetailDto.DetailId);

        // get assetDetail
        if (assetDetail == null)
        {
            _logger.LogWarning("AssetDetail not found");
            throw new NotFoundException("AssetDetail not found");
        }
        if (assetDetail.IsDeleted)
        {
            // await using transaction
            await using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                context.AssetDetails.Remove(assetDetail);
                // save changes
                await context.SaveChangesAsync();
                await transaction.CommitAsync();
                _logger.LogInformation("AssetDetail deleted");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting assetDetail");
                await transaction.RollbackAsync();
                throw new BadRequestException("Error deleting assetDetail");
            }
        }
        _logger.LogWarning("AssetDetail is not marked as deleted");
        throw new BadRequestException("AssetDetail is not marked as deleted");
    }

    public async Task<bool> DeleteCategory(int categoryId)
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();

        // get category
        var category = await context.Categories.FindAsync(categoryId);
        if (category == null)
        {
            _logger.LogWarning("Category not found");
            throw new NotFoundException("Category not found");
        }
        // check if category is marked as deleted
        if (category.IsDeleted == false)
        {
            _logger.LogWarning("Category is not marked as deleted");
            throw new BadRequestException("Category is not marked as deleted");
        }
        // delete category
        context.Categories.Remove(category);
        // await using transaction
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            // save changes
            await context.SaveChangesAsync();
            // commit transaction
            await transaction.CommitAsync();
            _logger.LogInformation("Category with id {CategoryId} deleted", category.CategoryId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting category");
            // rollback transaction
            await transaction.RollbackAsync();
            throw new BadRequestException("Error deleting category");
        }
    }

    public async Task<bool> DeleteDetail(int detailId)
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();

        // get detail
        var detail = await context.Details.FindAsync(detailId);
        if (detail == null)
        {
            _logger.LogWarning("Detail not found");
            throw new NotFoundException("Detail not found");
        }
        // check if detail is marked as deleted
        if (detail.IsDeleted == false)
        {
            _logger.LogWarning("Detail is not marked as deleted");
            throw new BadRequestException("Detail is not marked as deleted");
        }
        // delete detail
        context.Details.Remove(detail);
        // await using transaction
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            // save changes
            await context.SaveChangesAsync();
            // commit transaction
            await transaction.CommitAsync();
            _logger.LogInformation("Detail with id {DetailId} deleted", detail.DetailId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting detail");
            // rollback transaction
            await transaction.RollbackAsync();
            throw new BadRequestException("Error deleting detail");
        }
    }

    public async Task<bool> DeleteDevice(int deviceId)
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();

        // get device
        var device = await context.Devices.FindAsync(deviceId);
        if (device == null)
        {
            _logger.LogWarning("Device not found");
            throw new NotFoundException("Device not found");
        }
        // check if device is marked as deleted
        if (device.IsDeleted == false)
        {
            _logger.LogWarning("Device is not marked as deleted");
            throw new BadRequestException("Device is not marked as deleted");
        }
        // delete device
        context.Devices.Remove(device);
        // await using transaction
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            // save changes
            await context.SaveChangesAsync();
            // commit transaction
            await transaction.CommitAsync();
            _logger.LogInformation("Device with id {DeviceId} deleted", device.DeviceId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting device");
            // rollback transaction
            await transaction.RollbackAsync();
            throw new BadRequestException("Error deleting device");
        }
    }

    public async Task<bool> DeleteModel(int modelId)
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();

        // get model
        var model = await context.Models.FindAsync(modelId);
        if (model == null)
        {
            _logger.LogWarning("Model not found");
            throw new NotFoundException("Model not found");
        }
        // check if model is marked as deleted
        if (model.IsDeleted == false)
        {
            _logger.LogWarning("Model is not marked as deleted");
            throw new BadRequestException("Model is not marked as deleted");
        }
        // delete model
        context.Models.Remove(model);
        // await using transaction
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            // save changes
            await context.SaveChangesAsync();
            // commit transaction
            await transaction.CommitAsync();
            _logger.LogInformation("Model with id {ModelId} deleted", model.ModelId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting model");
            // rollback transaction
            await transaction.RollbackAsync();
            throw new BadRequestException("Error deleting model");
        }
    }

    public async Task<bool> DeleteModelParameter(ModelParameterDto modelParameterDto)
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();
        var modelParameter = await context.ModelParameters.FindAsync(modelParameterDto.ModelId, modelParameterDto.ParameterId);

        // get modelParameter
        if (modelParameter == null)
        {
            _logger.LogWarning("ModelParameter not found");
            throw new NotFoundException("ModelParameter not found");
        }
        if (modelParameter.IsDeleted)
        {// await using transaction
            await using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                context.ModelParameters.Remove(modelParameter);
                // save changes
                await context.SaveChangesAsync();
                await transaction.CommitAsync();
                _logger.LogInformation("ModelParameter deleted");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting modelParameter");
                await transaction.RollbackAsync();
                throw new BadRequestException("Error deleting modelParameter");
            }
        }
        _logger.LogWarning("ModelParameter is not marked as deleted");
        throw new BadRequestException("ModelParameter is not marked as deleted");
    }

    public async Task<bool> DeleteParameter(int parameterId)
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();

        // get parameter
        var parameter = await context.Parameters.FindAsync(parameterId);
        if (parameter == null)
        {
            _logger.LogWarning("Parameter not found");
            throw new NotFoundException("Parameter not found");
        }
        // check if parameter is marked as deleted
        if (parameter.IsDeleted == false)
        {
            _logger.LogWarning("Parameter is not marked as deleted");
            throw new BadRequestException("Parameter is not marked as deleted");
        }
        // delete parameter
        context.Parameters.Remove(parameter);
        // await using transaction
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            // save changes
            await context.SaveChangesAsync();
            // commit transaction
            await transaction.CommitAsync();
            _logger.LogInformation("Parameter with id {ParameterId} deleted", parameter.ParameterId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting parameter");
            // rollback transaction
            await transaction.RollbackAsync();
            throw new BadRequestException("Error deleting parameter");
        }
    }

    public async Task<AssetDto> GetAssetById(int assetId)
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();

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
            throw new NotFoundException("Asset not found");
        }
        // return asset
        _logger.LogInformation("Asset with id {AssetId} found", asset.AssetId);
        return asset;
    }

    public async Task<IEnumerable<AssetDisplayDto>> GetAssetDisplay()
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();

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
                Categories = a.AssetCategories.Select(ac => new AssetCategoryDisplayDto
                {
                    Name = ac.Category.Name,
                    AssetId = ac.AssetId,
                    UserId = ac.UserId,
                    CategoryId = ac.CategoryId,
                    Description = ac.Category.Description,
                    IsDeleted = ac.IsDeleted
                }).ToList(),
                Details = a.AssetDetails.Select(ad => new AssetDetailDisplayDto
                {
                    Name = ad.Detail.Name,
                    Value = ad.Value,
                    DetailId = ad.DetailId,
                    UserId = ad.UserId,
                    AssetId = ad.AssetId,
                    Description = ad.Detail.Description,
                    IsDeleted = ad.IsDeleted
                }).ToList(),
                Parameters = a.Model.ModelParameters.Select(mp => new ModelParameterDisplayDto
                {
                    Name = mp.Parameter.Name,
                    Value = mp.Value,
                    ParameterId = mp.ParameterId,
                    UserId = mp.UserId,
                    ModelId = mp.ModelId,
                    Description = mp.Parameter.Description,
                    IsDeleted = mp.IsDeleted
                }).ToList()
            }).ToListAsync();
        if (query.Count == 0)
        {
            _logger.LogWarning("No assets found");
            throw new NotFoundException("No assets found");
        }
        // return assets
        _logger.LogInformation("Assets found");
        return query;
    }

    public async Task<IEnumerable<AssetDto>> GetAssets()
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();

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
            throw new NotFoundException("No assets found");
        }
        // return assets
        _logger.LogInformation("Assets found");
        return assets;
    }
    public async Task<IEnumerable<CategoryDto>> GetCategories()
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();

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
            throw new NotFoundException("No categories found");
        }
        // return categories
        _logger.LogInformation("Categories found");
        return query;
    }

    public async Task<IEnumerable<CategoryWithAssetsDto>> GetCategoriesWithAssets()
    {
        // await context
        await using var context = await _contextFactory.CreateDbContextAsync();

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
                Assets = c.AssetCategories.Select(ac => new AssetDto
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
            throw new NotFoundException("No categories found");
        }
        // return categories
        _logger.LogInformation("Categories found");
        return query;
    }

    public async Task<CategoryDto> GetCategoryById(int categoryId)
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();

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
            throw new NotFoundException("No category found");
        }
        // return category
        _logger.LogInformation("Category found");
        return query;
    }

    public async Task<CategoryWithAssetsDto> GetCategoryByIdWithAssets(int categoryId)
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();

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
                Assets = c.AssetCategories.Select(ac => new AssetDto
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
            throw new NotFoundException("No category found");
        }
        // return category
        _logger.LogInformation("Category found");
        return query;
    }

    public async Task<DetailDto> GetDetailById(int detailId)
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();

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
            throw new NotFoundException("No detail found");
        }
        // return detail
        _logger.LogInformation("Detail found");
        return query;
    }

    public async Task<IEnumerable<DetailDto>> GetDetails()
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();

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
            throw new NotFoundException("No details found");
        }

        // return details
        _logger.LogInformation("Details found");
        return query;
    }

    public async Task<IEnumerable<DetailWithAssetsDto>> GetDetailsWithAssets()
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();

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
            throw new NotFoundException("No details found");
        }
        // return details
        _logger.LogInformation("Details found");
        return query;
    }

    public async Task<DeviceDto> GetDeviceById(int deviceId)
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();

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
            throw new NotFoundException("No device found");
        }
        // return device
        _logger.LogInformation("Device found");
        return query;
    }

    public async Task<IEnumerable<DeviceDto>> GetDevices()
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();

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
            throw new NotFoundException("No devices found");
        }
        // return devices
        _logger.LogInformation("Devices found");
        return query;
    }

    public async Task<IEnumerable<DeviceDto>> GetDevicesWithModels()
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();

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
            throw new NotFoundException("No devices found");
        }
        // return devices
        _logger.LogInformation("Devices found");
        return query;
    }

    public async Task<ModelDto> GetModelById(int modelId)
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();
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
            throw new NotFoundException("Model not found");
        }
        // return model
        _logger.LogInformation("Model found");
        return query;

    }

    public async Task<IEnumerable<ModelDto>> GetModels()
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();

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
            throw new NotFoundException("No models found");
        }
        // return models
        _logger.LogInformation("Models found");
        return query;
    }

    public async Task<IEnumerable<ModelDto>> GetModelsWithAssets()
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();

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
            throw new NotFoundException("No models found");
        }
        // return models
        _logger.LogInformation("Models found");
        return query;
    }

    public async Task<ParameterDto> GetParameterById(int parameterId)
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();

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
            throw new NotFoundException("Parameter not found");
        }
        // return parameter
        _logger.LogInformation("Parameter found");
        return query;
    }

    public async Task<IEnumerable<ParameterDto>> GetParameters()
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();

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
            throw new NotFoundException("No parameters found");
        }
        // return parameters
        _logger.LogInformation("Parameters found");
        return query;
    }

    public async Task<IEnumerable<ParameterWithModelsDto>> GetParametersWithModels()
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();

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
            throw new NotFoundException("No parameters found");
        }
        // return parameters
        _logger.LogInformation("Parameters found");
        return query;
    }

    public async Task<bool> MarkDeleteAsset(int assetId)
    {var userId = _userContextService.UserId;
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();

        // get asset
        var asset = await context.Assets
            .FirstOrDefaultAsync(m => m.AssetId == assetId);
        if (asset == null)
        {
            _logger.LogWarning("Asset not found");
            throw new NotFoundException("Asset not found");
        }
        // check if asset is already deleted
        if (asset.IsDeleted)
        {
            _logger.LogWarning("Asset is already deleted");
            throw new BadRequestException("Asset is already deleted");
        }
        // mark asset as deleted
        asset.IsDeleted = true;
        asset.UserId = userId;
        // update asset
        context.Update(asset);
        // await using transaction
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            // save changes
            await context.SaveChangesAsync();
            // await commit transaction
            await transaction.CommitAsync();
            // return success
            _logger.LogInformation("Asset marked as deleted");
            return true;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error marking asset as deleted");
            // await rollback transaction
            await transaction.RollbackAsync();
            throw new BadRequestException("Error marking asset as deleted");
        }
    }

    public async Task<bool> MarkDeleteAssetCategory(AssetCategoryDto assetCategoryDto)
    {var userId = _userContextService.UserId;
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();

        // get assetCategory
        var assetCategory = await context.AssetCategories.FindAsync(assetCategoryDto.AssetId, assetCategoryDto.CategoryId);
        if (assetCategory == null)
        {
            _logger.LogWarning("AssetCategory not found");
            throw new NotFoundException("AssetCategory not found");
        }
        // check if assetCategory is already deleted
        if (assetCategory.IsDeleted)
        {
            _logger.LogWarning("AssetCategory already deleted");
            throw new BadRequestException("AssetCategory already deleted");
        }
        assetCategory.UserId = userId;
        assetCategory.IsDeleted = true;
        context.Update(assetCategory);
        // await using transaction
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            // save changes
            await context.SaveChangesAsync();
            await transaction.CommitAsync();
            _logger.LogInformation("AssetCategory marked as deleted");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking assetCategory as deleted");
            await transaction.RollbackAsync();
            throw new BadRequestException("Error marking assetCategory as deleted");
        }
    }

    public async Task<bool> MarkDeleteAssetDetail(AssetDetailDto assetDetailDto)
    {var userId = _userContextService.UserId;
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();

        // get assetDetail
        var assetDetail = await context.AssetDetails.FindAsync(assetDetailDto.AssetId, assetDetailDto.DetailId);
        if (assetDetail == null)
        {
            _logger.LogWarning("AssetDetail not found");
            throw new NotFoundException("AssetDetail not found");
        }
        // check if assetDetail is already deleted
        if (assetDetail.IsDeleted)
        {
            _logger.LogWarning("AssetDetail already deleted");
            throw new BadRequestException("AssetDetail already deleted");
        }
        assetDetail.UserId = userId;
        assetDetail.IsDeleted = true;
        context.Update(assetDetail);
        // await using transaction
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            // save changes
            await context.SaveChangesAsync();
            await transaction.CommitAsync();
            _logger.LogInformation("AssetDetail marked as deleted");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking assetDetail as deleted");
            await transaction.RollbackAsync();
            throw new BadRequestException("Error marking assetDetail as deleted");
        }
    }

    public async Task<bool> MarkDeleteCategory(int categoryId)
    {var userId = _userContextService.UserId;
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();

        // get category
        var category = await context.Categories
            .FirstOrDefaultAsync(m => m.CategoryId == categoryId);
        if (category == null)
        {
            _logger.LogWarning("Category not found");
            throw new NotFoundException("Category not found");
        }
        // check if category is already deleted
        if (category.IsDeleted)
        {
            _logger.LogWarning("Category is already deleted");
            throw new BadRequestException("Category is already deleted");
        }
        // mark category as deleted
        category.IsDeleted = true;
        category.UserId = userId;
        // update category
        context.Update(category);
        // await using transaction
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            // save changes
            await context.SaveChangesAsync();
            // await commit transaction
            await transaction.CommitAsync();
            // return success
            _logger.LogInformation("Category marked as deleted");
            return true;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error marking category as deleted");
            // await rollback transaction
            await transaction.RollbackAsync();
            throw new BadRequestException("Error marking category as deleted");
        }
    }

    public async Task<bool> MarkDeleteDetail(int detailId)
    {var userId = _userContextService.UserId;
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();

        // get detail
        var detail = await context.Details
            .FirstOrDefaultAsync(m => m.DetailId == detailId);
        if (detail == null)
        {
            _logger.LogWarning("Detail not found");
            throw new NotFoundException("Detail not found");
        }
        // check if detail is already deleted
        if (detail.IsDeleted)
        {
            _logger.LogWarning("Detail is already deleted");
            throw new BadRequestException("Detail is already deleted");
        }
        // mark detail as deleted
        detail.IsDeleted = true;
        detail.UserId = userId;
        // update detail
        context.Update(detail);
        // await using transaction
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            // save changes
            await context.SaveChangesAsync();
            // await commit transaction
            await transaction.CommitAsync();
            // return success
            _logger.LogInformation("Detail marked as deleted");
            return true;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error marking detail as deleted");
            // await rollback transaction
            await transaction.RollbackAsync();
            throw new BadRequestException("Error marking detail as deleted");
        }
    }

    public async Task<bool> MarkDeleteDevice(int deviceId)
    {var userId = _userContextService.UserId;
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();

        // get device
        var device = await context.Devices
            .FirstOrDefaultAsync(m => m.DeviceId == deviceId);
        if (device == null)
        {
            _logger.LogWarning("Device not found");
            throw new NotFoundException("Device not found");
        }
        // check if device is already deleted
        if (device.IsDeleted)
        {
            _logger.LogWarning("Device is already deleted");
            throw new BadRequestException("Device is already deleted");
        }
        // mark device as deleted
        device.IsDeleted = true;
        device.UserId = userId;
        // update device
        context.Update(device);
        // await using transaction
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            // save changes
            await context.SaveChangesAsync();
            // await commit transaction
            await transaction.CommitAsync();
            // return success
            _logger.LogInformation("Device marked as deleted");
            return true;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error marking device as deleted");
            // await rollback transaction
            await transaction.RollbackAsync();
            throw new BadRequestException("Error marking device as deleted");
        }
    }

    public async Task<bool> MarkDeleteModel(int modelId)
    {var userId = _userContextService.UserId;
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();

        // get model
        var model = await context.Models
            .FirstOrDefaultAsync(m => m.ModelId == modelId);
        if (model == null)
        {
            _logger.LogWarning("Model not found");
            throw new NotFoundException("Model not found");
        }
        // check if model is already deleted
        if (model.IsDeleted)
        {
            _logger.LogWarning("Model is already deleted");
            throw new BadRequestException("Model is already deleted");
        }
        // mark model as deleted
        model.IsDeleted = true;
        model.UserId = userId;
        // update model
        context.Update(model);
        // await using transaction
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            // save changes
            await context.SaveChangesAsync();
            // await commit transaction
            await transaction.CommitAsync();
            // return success
            _logger.LogInformation("Model marked as deleted");
            return true;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error marking model as deleted");
            // await rollback transaction
            await transaction.RollbackAsync();
            throw new BadRequestException("Error marking model as deleted");
        }
    }

    public async Task<bool> MarkDeleteModelParameter(ModelParameterDto modelParameterDto)
    {var userId = _userContextService.UserId;
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();

        var modelParameter = await context.ModelParameters.FindAsync(modelParameterDto.ModelId, modelParameterDto.ParameterId);
        if (modelParameter == null)
        {
            _logger.LogWarning("ModelParameter not found");
            throw new NotFoundException("ModelParameter not found");
        }
        if (modelParameter.IsDeleted)
        {
            _logger.LogWarning("ModelParameter already deleted");
            throw new BadRequestException("ModelParameter already deleted");
        }
        modelParameter.UserId = userId;
        modelParameter.IsDeleted = true;
        context.Update(modelParameter);
        // await using transaction
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            // save changes
            await context.SaveChangesAsync();
            await transaction.CommitAsync();
            _logger.LogInformation("ModelParameter marked as deleted");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking modelParameter as deleted");
            await transaction.RollbackAsync();
            throw new BadRequestException("Error marking modelParameter as deleted");
        }
    }

    public async Task<bool> MarkDeleteParameter(int parameterId)
    {var userId = _userContextService.UserId;
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();

        // get parameter
        var parameter = await context.Parameters
            .FirstOrDefaultAsync(m => m.ParameterId == parameterId);
        if (parameter == null)
        {
            _logger.LogWarning("Parameter not found");
            throw new NotFoundException("Parameter not found");
        }
        // check if parameter is already deleted
        if (parameter.IsDeleted)
        {
            _logger.LogWarning("Parameter is already deleted");
            throw new BadRequestException("Parameter is already deleted");
        }
        // mark parameter as deleted
        parameter.IsDeleted = true;
        parameter.UserId = userId;
        // update parameter
        context.Update(parameter);
        // await using transaction
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            // save changes
            await context.SaveChangesAsync();
            // await commit transaction
            await transaction.CommitAsync();
            // return success
            _logger.LogInformation("Parameter marked as deleted");
            return true;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error marking parameter as deleted");
            // await rollback transaction
            await transaction.RollbackAsync();
            throw new BadRequestException("Error marking parameter as deleted");
        }
    }

    public async Task<bool> UpdateAsset(int assetId, AssetUpdateDto assetUpdateDto)
    {var userId = _userContextService.UserId;
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();

        // get asset with all related data
        var asset = await context.Assets.Include(a => a.AssetCategories).Include(a => a.AssetDetails).Include(a => a.AssetSituations).Include(a => a.CommunicateAssets)
            .FirstOrDefaultAsync(m => m.AssetId == assetId);
        if (asset == null)
        {
            _logger.LogWarning("Asset not found");
            throw new NotFoundException("Asset not found");
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
            asset.CoordinateId = assetUpdateDto.CoordinateId;
        // if new asset name is different from old asset name

        // check if asset name from dto is already taken
        var duplicate = await context.Assets.AnyAsync(a => a.Name.ToLower().Trim() == assetUpdateDto.Name.ToLower().Trim());
        if (duplicate || asset.Name.ToLower().Trim() == assetUpdateDto.Name.ToLower().Trim())
        {
            _logger.LogWarning("Asset name is already taken");
            throw new BadRequestException("Asset name is already taken");
        }
        asset.Name = assetUpdateDto.Name;

        // assign userId to update
        asset.Description = assetUpdateDto.Description;
        asset.UserId = userId;
        asset.IsDeleted = false;
        // update asset
        context.Update(asset);
        // await using transaction
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            // save changes
            await context.SaveChangesAsync();
            // await commit transaction
            await transaction.CommitAsync();
            // return success
            _logger.LogInformation("Asset updated");
            return true;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error updating asset");
            // await rollback transaction
            await transaction.RollbackAsync();
            throw new BadRequestException("Error updating asset");
        }
    }

    public async Task<bool> UpdateCategory(int categoryId, CategoryUpdateDto categoryUpdateDto)
    {var userId = _userContextService.UserId;
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();

        // get category with all related data
        var category = await context.Categories.FirstOrDefaultAsync(m => m.CategoryId == categoryId);
        if (category == null)
        {
            _logger.LogWarning("Category not found");
            throw new NotFoundException("Category not found");
        }
        // check if category name from dto is already taken
        var duplicate = await context.Categories.AnyAsync(a => a.Name.ToLower().Trim() == categoryUpdateDto.Name.ToLower().Trim());
        if (duplicate || category.Name.ToLower().Trim() == categoryUpdateDto.Name.ToLower().Trim())
        {
            _logger.LogWarning("Category name is already taken");
            throw new BadRequestException("Category name is already taken");
        }
        category.Name = categoryUpdateDto.Name;
        // assign userId to update
        category.UserId = userId;
        category.Description = categoryUpdateDto.Description;
        category.IsDeleted = false;
        // update category
        context.Update(category);
        // await using transaction
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            // save changes
            await context.SaveChangesAsync();
            // await commit transaction
            await transaction.CommitAsync();
            // return success
            _logger.LogInformation("Category updated");
            return true;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error updating category");
            // await rollback transaction
            await transaction.RollbackAsync();
            throw new BadRequestException("Error updating category");
        }
    }

    public async Task<bool> UpdateDetail(int detailId, DetailUpdateDto detailUpdateDto)
    {var userId = _userContextService.UserId;
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();

        // get detail
        var detail = await context.Details.FirstOrDefaultAsync(m => m.DetailId == detailId);
        if (detail == null)
        {
            _logger.LogWarning("Detail not found");
            throw new NotFoundException("Detail not found");
        }
        // check if detail name from dto is already taken
        var duplicate = await context.Details.AnyAsync(a => a.Name.ToLower().Trim() == detailUpdateDto.Name.ToLower().Trim());
        if (duplicate || detail.Name.ToLower().Trim() == detailUpdateDto.Name.ToLower().Trim())
        {
            _logger.LogWarning("Detail name is already taken");
            throw new BadRequestException("Detail name is already taken");
        }
        detail.Name = detailUpdateDto.Name;
        // assign userId to update
        detail.UserId = userId;
        detail.Description = detailUpdateDto.Description;
        detail.IsDeleted = false;
        // update detail
        context.Update(detail);
        // await using transaction
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            // save changes
            await context.SaveChangesAsync();
            // await commit transaction
            await transaction.CommitAsync();
            // return success
            _logger.LogInformation("Detail updated");
            return true;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error updating detail");
            // await rollback transaction
            await transaction.RollbackAsync();
            throw new BadRequestException("Error updating detail");
        }
    }

    public async Task<bool> UpdateDevice(int deviceId, DeviceUpdateDto deviceUpdateDto)
    {var userId = _userContextService.UserId;
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();

        // get device
        var device = await context.Devices.FirstOrDefaultAsync(m => m.DeviceId == deviceId);
        if (device == null)
        {
            _logger.LogWarning("Device not found");
            throw new NotFoundException("Device not found");
        }
        // check if device name from dto is already taken
        var duplicate = await context.Devices.AnyAsync(a => a.Name.ToLower().Trim() == deviceUpdateDto.Name.ToLower().Trim());
        if (duplicate || device.Name.ToLower().Trim() == deviceUpdateDto.Name.ToLower().Trim())
        {
            _logger.LogWarning("Device name is already taken");
            throw new BadRequestException("Device name is already taken");
        }
        device.Name = deviceUpdateDto.Name;
        // assign userId to update
        device.UserId = userId;
        device.Description = deviceUpdateDto.Description;
        device.IsDeleted = false;
        // update device
        context.Update(device);
        // await using transaction
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            // save changes
            await context.SaveChangesAsync();
            // await commit transaction
            await transaction.CommitAsync();
            // return success
            _logger.LogInformation("Device updated");
            return true;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error updating device");
            // await rollback transaction
            await transaction.RollbackAsync();
            throw new BadRequestException("Error updating device");
        }
    }

    public async Task<bool> UpdateModel(int modelId, ModelUpdateDto modelUpdateDto)
    {var userId = _userContextService.UserId;
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();

        // get model
        var model = await context.Models.FirstOrDefaultAsync(m => m.ModelId == modelId);
        if (model == null)
        {
            _logger.LogWarning("Model not found");
            throw new NotFoundException("Model not found");
        }
        // check if model name from dto is already taken
        var duplicate = await context.Models.AnyAsync(a => a.Name.ToLower().Trim() == modelUpdateDto.Name.ToLower().Trim());
        if (duplicate || model.Name.ToLower().Trim() == modelUpdateDto.Name.ToLower().Trim())
        {
            _logger.LogWarning("Model name is already taken");
            throw new BadRequestException("Model name is already taken");
        }
        model.Name = modelUpdateDto.Name;
        // assign userId to update
        model.UserId = userId;
        model.Description = modelUpdateDto.Description;
        model.IsDeleted = false;
        // update model
        context.Update(model);
        // await using transaction
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            // save changes
            await context.SaveChangesAsync();
            // await commit transaction
            await transaction.CommitAsync();
            // return success
            _logger.LogInformation("Model updated");
            return true;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error updating model");
            // await rollback transaction
            await transaction.RollbackAsync();
            throw new BadRequestException("Error updating model");
        }
    }

    public async Task<bool> UpdateParameter(int parameterId, ParameterUpdateDto parameterUpdateDto)
    {var userId = _userContextService.UserId;
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();

        // get parameter
        var parameter = await context.Parameters.FirstOrDefaultAsync(p => p.ParameterId == parameterId);
        if (parameter == null)
        {
            _logger.LogWarning("Parameter not found");
            throw new NotFoundException("Parameter not found");
        }
        // check if parameter name from dto is already taken
        var duplicate = await context.Parameters.AnyAsync(a => a.Name.ToLower().Trim() == parameterUpdateDto.Name.ToLower().Trim());
        if (duplicate || parameter.Name.ToLower().Trim() == parameterUpdateDto.Name.ToLower().Trim())
        {
            _logger.LogWarning("Parameter name is already taken");
            throw new BadRequestException("Parameter name is already taken");
        }
        parameter.Name = parameterUpdateDto.Name;
        // assign userId to update
        parameter.UserId = userId;
        parameter.Description = parameterUpdateDto.Description;
        parameter.IsDeleted = false;
        // update parameter
        context.Update(parameter);
        // await using transaction
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            // save changes
            await context.SaveChangesAsync();
            // await commit transaction
            await transaction.CommitAsync();
            // return success
            _logger.LogInformation("Parameter updated");
            return true;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error updating parameter");
            // await rollback transaction
            await transaction.RollbackAsync();
            throw new BadRequestException("Error updating parameter");
        }
    }
}
