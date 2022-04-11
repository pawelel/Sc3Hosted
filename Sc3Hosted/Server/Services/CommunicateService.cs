using Microsoft.EntityFrameworkCore;
using Sc3Hosted.Server.Data;
using Sc3Hosted.Server.Entities;
using Sc3Hosted.Shared.Dtos;
using Sc3Hosted.Shared.Helpers;

namespace Sc3Hosted.Server.Services;
public interface ICommunicateService
{
    Task<ServiceResponse> AddOrUpdateCommunicateArea(CommunicateAreaDto communicateAreaDto, string userId);

    Task<ServiceResponse> AddOrUpdateCommunicateAsset(CommunicateAssetDto communicateAssetDto, string userId);

    Task<ServiceResponse> AddOrUpdateCommunicateCategory(CommunicateCategoryDto communicateCategoryDto, string userId);

    Task<ServiceResponse> AddOrUpdateCommunicateCoordinate(CommunicateCoordinateDto communicateCoordinateDto, string userId);

    Task<ServiceResponse> AddOrUpdateCommunicateDevice(CommunicateDeviceDto communicateDeviceDto, string userId);

    Task<ServiceResponse> AddOrUpdateCommunicateModel(CommunicateModelDto communicateModelDto, string userId);

    Task<ServiceResponse> AddOrUpdateCommunicateSpace(CommunicateSpaceDto communicateSpaceDto, string userId);

    Task<ServiceResponse> CreateCommunicate(CommunicateCreateDto communicateCreateDto, string userId);

    Task<ServiceResponse> DeleteCommunicate(int communicateId);

    Task<ServiceResponse> DeleteCommunicateArea(CommunicateAreaDto communicateAreaDto, string userId);

    Task<ServiceResponse> DeleteCommunicateAsset(CommunicateAssetDto communicateAssetDto, string userId);

    Task<ServiceResponse> DeleteCommunicateCategory(CommunicateCategoryDto communicateCategoryDto, string userId);

    Task<ServiceResponse> DeleteCommunicateCoordinate(CommunicateCoordinateDto communicateCoordinateDto, string userId);

    Task<ServiceResponse> DeleteCommunicateDevice(CommunicateDeviceDto communicateDeviceDto, string userId);

    Task<ServiceResponse> DeleteCommunicateModel(CommunicateModelDto communicateModelDto, string userId);

    Task<ServiceResponse> DeleteCommunicateSpace(CommunicateSpaceDto communicateSpaceDto, string userId);

    Task<ServiceResponse<CommunicateDto>> GetCommunicateById(int communicateId);

    Task<ServiceResponse<IEnumerable<CommunicateDto>>> GetCommunicates();

    Task<ServiceResponse<IEnumerable<CommunicateWithAssetsDto>>> GetCommunicatesWithAssets();

    Task<ServiceResponse> MarkDeleteCommunicate(int communicateId, string userId);


    Task<ServiceResponse> MarkDeleteCommunicateArea(CommunicateAreaDto communicateAreaDto, string userId);

    Task<ServiceResponse> MarkDeleteCommunicateAsset(CommunicateAssetDto communicateAssetDto, string userId);

    Task<ServiceResponse> MarkDeleteCommunicateCategory(CommunicateCategoryDto communicateCategoryDto, string userId);

    Task<ServiceResponse> MarkDeleteCommunicateCoordinate(CommunicateCoordinateDto communicateCoordinateDto, string userId);

    Task<ServiceResponse> MarkDeleteCommunicateDevice(CommunicateDeviceDto communicateDeviceDto, string userId);

    Task<ServiceResponse> MarkDeleteCommunicateModel(CommunicateModelDto communicateModelDto, string userId);

    Task<ServiceResponse> MarkDeleteCommunicateSpace(CommunicateSpaceDto communicateSpaceDto, string userId);

    Task<ServiceResponse> UpdateCommunicate(int communicateId, string userId, CommunicateUpdateDto communicateUpdateDto);
}
public class CommunicateService : ICommunicateService
{
    private readonly IDbContextFactory<Sc3HostedDbContext> _contextFactory;
    private readonly ILogger<CommunicateService> _logger;

    public CommunicateService(IDbContextFactory<Sc3HostedDbContext> contextFactory, ILogger<CommunicateService> logger)
    {
        _contextFactory = contextFactory;
        _logger = logger;
    }


    public async Task<ServiceResponse> AddOrUpdateCommunicateArea(CommunicateAreaDto communicateAreaDto, string userId)
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();
        // await using transaction
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            // get communicateArea
            var communicateArea = await context.CommunicateAreas.FindAsync(communicateAreaDto.CommunicateId, communicateAreaDto.AreaId);
            if (communicateArea == null)
            {
                var area = communicateAreaDto.AreaId < 1?null:await context.CommunicateAreas.FirstOrDefaultAsync(a => a.AreaId == communicateAreaDto.AreaId);
                if (area == null || area.IsDeleted)
                {
                    _logger.LogWarning("Area not found");
                    return new ServiceResponse("Area not found");
                }
                var communicate = communicateAreaDto.CommunicateId < 1?null:await context.Communicates.FirstOrDefaultAsync(a => a.CommunicateId == communicateAreaDto.CommunicateId);
                if (communicate == null || communicate.IsDeleted)
                {
                    _logger.LogWarning("Communicate not found");
                    return new ServiceResponse("Communicate not found");
                }
                communicateArea = new CommunicateArea
                {
                    CommunicateId = communicateAreaDto.CommunicateId,
                    AreaId = communicateAreaDto.AreaId,
                    UserId = userId,
                    IsDeleted = false
                };
                context.Add(communicateArea);
            }
            else
            {
                communicateArea.UserId = userId;
                communicateArea.IsDeleted = false;
                context.Update(communicateArea);
            }

            // save changes
            await context.SaveChangesAsync();
            await transaction.CommitAsync();
            _logger.LogInformation("CommunicateArea updated");
            return new ServiceResponse("CommunicateArea updated", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating communicateArea");
            await transaction.RollbackAsync();
            return new ServiceResponse("Error updating communicateArea");
        }
    }

    public async Task<ServiceResponse> AddOrUpdateCommunicateAsset(CommunicateAssetDto communicateAssetDto, string userId)
    {
       // await using context
       await using var context = await _contextFactory.CreateDbContextAsync();
       // await using transaction
       await using var transaction = await context.Database.BeginTransactionAsync();
       try
       {
           // get communicateAsset
           var communicateAsset = await context.CommunicateAssets.FindAsync(communicateAssetDto.CommunicateId, communicateAssetDto.AssetId);
           if (communicateAsset == null)
           {
               var asset = communicateAssetDto.AssetId < 1?null:await context.CommunicateAssets.FirstOrDefaultAsync(a => a.AssetId == communicateAssetDto.AssetId);
               if (asset == null || asset.IsDeleted)
               {
                   _logger.LogWarning("Asset not found");
                   return new ServiceResponse("Asset not found");
               }
               var communicate = communicateAssetDto.CommunicateId < 1?null:await context.Communicates.FirstOrDefaultAsync(a => a.CommunicateId == communicateAssetDto.CommunicateId);
               if (communicate == null || communicate.IsDeleted)
               {
                   _logger.LogWarning("Communicate not found");
                   return new ServiceResponse("Communicate not found");
               }
               communicateAsset = new CommunicateAsset
               {
                   CommunicateId = communicateAssetDto.CommunicateId,
                   AssetId = communicateAssetDto.AssetId,
                   UserId = userId,
                   IsDeleted = false
               };
               context.Add(communicateAsset);
           }
           else
           {
               communicateAsset.UserId = userId;
               communicateAsset.IsDeleted = false;
               context.Update(communicateAsset);
           }

           // save changes
           await context.SaveChangesAsync();
           await transaction.CommitAsync();
           _logger.LogInformation("CommunicateAsset updated");
           return new ServiceResponse("CommunicateAsset updated", true);
       }
       catch (Exception ex)
       {
           _logger.LogError(ex, "Error updating communicateAsset");
           await transaction.RollbackAsync();
           return new ServiceResponse("Error updating communicateAsset");
       }
    }

    public async Task<ServiceResponse> AddOrUpdateCommunicateCategory(CommunicateCategoryDto communicateCategoryDto, string userId)
    {
       // await using context
       await using var context = await _contextFactory.CreateDbContextAsync();
       // await using transaction
       await using var transaction = await context.Database.BeginTransactionAsync();
       try
       {
           // get communicateCategory
           var communicateCategory = await context.CommunicateCategories.FindAsync(communicateCategoryDto.CommunicateId, communicateCategoryDto.CategoryId);
           if (communicateCategory == null)
           {
               var category = communicateCategoryDto.CategoryId < 1?null:await context.CommunicateCategories.FirstOrDefaultAsync(a => a.CategoryId == communicateCategoryDto.CategoryId);
               if (category == null || category.IsDeleted)
               {
                   _logger.LogWarning("Category not found");
                   return new ServiceResponse("Category not found");
               }
               var communicate = communicateCategoryDto.CommunicateId < 1?null:await context.Communicates.FirstOrDefaultAsync(a => a.CommunicateId == communicateCategoryDto.CommunicateId);
               if (communicate == null || communicate.IsDeleted)
               {
                   _logger.LogWarning("Communicate not found");
                   return new ServiceResponse("Communicate not found");
               }
               communicateCategory = new CommunicateCategory
               {
                   CommunicateId = communicateCategoryDto.CommunicateId,
                   CategoryId = communicateCategoryDto.CategoryId,
                   UserId = userId,
                   IsDeleted = false
               };
               context.Add(communicateCategory);
           }
           else
           {
               communicateCategory.UserId = userId;
               communicateCategory.IsDeleted = false;
               context.Update(communicateCategory);
           }

           // save changes
           await context.SaveChangesAsync();
           await transaction.CommitAsync();
           _logger.LogInformation("CommunicateCategory updated");
           return new ServiceResponse("CommunicateCategory updated", true);
       }
       catch (Exception ex)
       {
           _logger.LogError(ex, "Error updating communicateCategory");
           await transaction.RollbackAsync();
           return new ServiceResponse("Error updating communicateCategory");
       }
    }

    public async Task<ServiceResponse> AddOrUpdateCommunicateCoordinate(CommunicateCoordinateDto communicateCoordinateDto, string userId)
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();
        // await using transaction
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            // get communicateCoordinate
            var communicateCoordinate = await context.CommunicateCoordinates.FindAsync(communicateCoordinateDto.CommunicateId, communicateCoordinateDto.CoordinateId);
            if (communicateCoordinate == null)
            {
                var coordinate = communicateCoordinateDto.CoordinateId < 1?null:await context.CommunicateCoordinates.FirstOrDefaultAsync(a => a.CoordinateId == communicateCoordinateDto.CoordinateId);
                if (coordinate == null || coordinate.IsDeleted)
                {
                    _logger.LogWarning("Coordinate not found");
                    return new ServiceResponse("Coordinate not found");
                }
                var communicate = communicateCoordinateDto.CommunicateId < 1?null:await context.Communicates.FirstOrDefaultAsync(a => a.CommunicateId == communicateCoordinateDto.CommunicateId);
                if (communicate == null || communicate.IsDeleted)
                {
                    _logger.LogWarning("Communicate not found");
                    return new ServiceResponse("Communicate not found");
                }
                communicateCoordinate = new CommunicateCoordinate
                {
                    CommunicateId = communicateCoordinateDto.CommunicateId,
                    CoordinateId = communicateCoordinateDto.CoordinateId,
                    UserId = userId,
                    IsDeleted = false
                };
                context.Add(communicateCoordinate);
            }
            else
            {
                communicateCoordinate.UserId = userId;
                communicateCoordinate.IsDeleted = false;
                context.Update(communicateCoordinate);
            }

            // save changes
            await context.SaveChangesAsync();
            await transaction.CommitAsync();
            _logger.LogInformation("CommunicateCoordinate updated");
            return new ServiceResponse("CommunicateCoordinate updated", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating communicateCoordinate");
            await transaction.RollbackAsync();
            return new ServiceResponse("Error updating communicateCoordinate");
        }
    }

    public async Task<ServiceResponse> AddOrUpdateCommunicateDevice(CommunicateDeviceDto communicateDeviceDto, string userId)
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();
        // await using transaction
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            // get communicateDevice
            var communicateDevice = await context.CommunicateDevices.FindAsync(communicateDeviceDto.CommunicateId, communicateDeviceDto.DeviceId);
            if (communicateDevice == null)
            {
                var device = communicateDeviceDto.DeviceId < 1?null:await context.CommunicateDevices.FirstOrDefaultAsync(a => a.DeviceId == communicateDeviceDto.DeviceId);
                if (device == null || device.IsDeleted)
                {
                    _logger.LogWarning("Device not found");
                    return new ServiceResponse("Device not found");
                }
                var communicate = communicateDeviceDto.CommunicateId < 1?null:await context.Communicates.FirstOrDefaultAsync(a => a.CommunicateId == communicateDeviceDto.CommunicateId);
                if (communicate == null || communicate.IsDeleted)
                {
                    _logger.LogWarning("Communicate not found");
                    return new ServiceResponse("Communicate not found");
                }
                communicateDevice = new CommunicateDevice
                {
                    CommunicateId = communicateDeviceDto.CommunicateId,
                    DeviceId = communicateDeviceDto.DeviceId,
                    UserId = userId,
                    IsDeleted = false
                };
                context.Add(communicateDevice);
            }
            else
            {
                communicateDevice.UserId = userId;
                communicateDevice.IsDeleted = false;
                context.Update(communicateDevice);
            }

            // save changes
            await context.SaveChangesAsync();
            await transaction.CommitAsync();
            _logger.LogInformation("CommunicateDevice updated");
            return new ServiceResponse("CommunicateDevice updated", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating communicateDevice");
            await transaction.RollbackAsync();
            return new ServiceResponse("Error updating communicateDevice");
        }
    }

    public async Task<ServiceResponse> AddOrUpdateCommunicateModel(CommunicateModelDto communicateModelDto, string userId)
    {
       // await using context
       await using var context = await _contextFactory.CreateDbContextAsync();
       // await using transaction
       await using var transaction = await context.Database.BeginTransactionAsync();
       try
       {
           // get communicateModel
           var communicateModel = await context.CommunicateModels.FindAsync(communicateModelDto.CommunicateId, communicateModelDto.ModelId);
           if (communicateModel == null)
           {
               var model = communicateModelDto.ModelId < 1?null:await context.CommunicateModels.FirstOrDefaultAsync(a => a.ModelId == communicateModelDto.ModelId);
               if (model == null || model.IsDeleted)
               {
                   _logger.LogWarning("Model not found");
                   return new ServiceResponse("Model not found");
               }
               var communicate = communicateModelDto.CommunicateId < 1?null:await context.Communicates.FirstOrDefaultAsync(a => a.CommunicateId == communicateModelDto.CommunicateId);
               if (communicate == null || communicate.IsDeleted)
               {
                   _logger.LogWarning("Communicate not found");
                   return new ServiceResponse("Communicate not found");
               }
               communicateModel = new CommunicateModel
               {
                   CommunicateId = communicateModelDto.CommunicateId,
                   ModelId = communicateModelDto.ModelId,
                   UserId = userId,
                   IsDeleted = false
               };
               context.Add(communicateModel);
           }
           else
           {
               communicateModel.UserId = userId;
               communicateModel.IsDeleted = false;
               context.Update(communicateModel);
           }

           // save changes
           await context.SaveChangesAsync();
           await transaction.CommitAsync();
           _logger.LogInformation("CommunicateModel updated");
           return new ServiceResponse("CommunicateModel updated", true);
       }
       catch (Exception ex)
       {
           _logger.LogError(ex, "Error updating communicateModel");
           await transaction.RollbackAsync();
           return new ServiceResponse("Error updating communicateModel");
       }
    }

    public async Task<ServiceResponse> AddOrUpdateCommunicateSpace(CommunicateSpaceDto communicateSpaceDto, string userId)
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();
        // await using transaction
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            // get communicateSpace
            var communicateSpace = await context.CommunicateSpaces.FindAsync(communicateSpaceDto.CommunicateId, communicateSpaceDto.SpaceId);
            if (communicateSpace == null)
            {
                var space = communicateSpaceDto.SpaceId < 1?null:await context.CommunicateSpaces.FirstOrDefaultAsync(a => a.SpaceId == communicateSpaceDto.SpaceId);
                if (space == null || space.IsDeleted)
                {
                    _logger.LogWarning("Space not found");
                    return new ServiceResponse("Space not found");
                }
                var communicate = communicateSpaceDto.CommunicateId < 1?null:await context.Communicates.FirstOrDefaultAsync(a => a.CommunicateId == communicateSpaceDto.CommunicateId);
                if (communicate == null || communicate.IsDeleted)
                {
                    _logger.LogWarning("Communicate not found");
                    return new ServiceResponse("Communicate not found");
                }
                communicateSpace = new CommunicateSpace
                {
                    CommunicateId = communicateSpaceDto.CommunicateId,
                    SpaceId = communicateSpaceDto.SpaceId,
                    UserId = userId,
                    IsDeleted = false
                };
                context.Add(communicateSpace);
            }
            else
            {
                communicateSpace.UserId = userId;
                communicateSpace.IsDeleted = false;
                context.Update(communicateSpace);
            }

            // save changes
            await context.SaveChangesAsync();
            await transaction.CommitAsync();
            _logger.LogInformation("CommunicateSpace updated");
            return new ServiceResponse("CommunicateSpace updated", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating communicateSpace");
            await transaction.RollbackAsync();
            return new ServiceResponse("Error updating communicateSpace");
        }
    }
    public async Task<ServiceResponse> CreateCommunicate(CommunicateCreateDto communicateCreateDto, string userId)
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();
        // await using transaction
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            // validate category name
            var duplicate = await context.Communicates.AnyAsync(c => c.Name.ToLower().Trim() == communicateCreateDto.Name.ToLower().Trim());
            if (duplicate)
            {
                _logger.LogWarning("Communicate name already exists");
                return new ServiceResponse("Communicate name already exists");
            }

            var communicate = new Communicate
            {
                UserId = userId,
                Name = communicateCreateDto.Name,
                Description = communicateCreateDto.Description,
                IsDeleted = false
            };
            // create category
            context.Communicates.Add(communicate);
            // save changes
            await context.SaveChangesAsync();
            // commit transaction
            await transaction.CommitAsync();
            _logger.LogInformation("Communicate with id {CommunicateId} created", communicate.CommunicateId);
            return new ServiceResponse($"Communicate {communicate.CommunicateId} created", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating communicate");
            // rollback transaction
            await transaction.RollbackAsync();
            return new ServiceResponse($"Error creating communicate");
        }
    }
    public async Task<ServiceResponse> DeleteCommunicate(int communicateId)
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();
        // await using transaction
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            // get communicate
            var communicate = await context.Communicates.FindAsync(communicateId);
            if (communicate == null)
            {
                _logger.LogWarning("Communicate not found");
                return new ServiceResponse("Communicate not found");
            }
            // check if communicate is marked as deleted
            if (communicate.IsDeleted == false)
            {
                _logger.LogWarning("Communicate is not marked as deleted");
                return new ServiceResponse("Communicate is not marked as deleted");
            }
            // delete communicate
            context.Communicates.Remove(communicate);
            // save changes
            await context.SaveChangesAsync();
            // commit transaction
            await transaction.CommitAsync();
            _logger.LogInformation("Communicate with id {CommunicateId} deleted", communicate.CommunicateId);
            return new ServiceResponse($"Communicate {communicate.CommunicateId} deleted", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting communicate");
            // rollback transaction
            await transaction.RollbackAsync();
            return new ServiceResponse($"Error deleting communicate");
        }
    }
    public async Task<ServiceResponse> DeleteCommunicateArea(CommunicateAreaDto communicateAreaDto, string userId)
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();
        // await using transaction
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            // get communicate area
            var communicateArea = await context.CommunicateAreas.FirstOrDefaultAsync(c => c.CommunicateId == communicateAreaDto.CommunicateId && c.AreaId == communicateAreaDto.AreaId);
            if (communicateArea == null)
            {
                _logger.LogWarning("Communicate area not found");
                return new ServiceResponse("Communicate area not found");
            }
            // check if CommunicateArea is not marked as deleted
            if (communicateArea.IsDeleted == false)
            {
                _logger.LogWarning("Communicate area is not marked as deleted");
                return new ServiceResponse("Communicate area is not marked as deleted");
            }
            // delete communicate area
            context.CommunicateAreas.Remove(communicateArea);
            // save changes
            await context.SaveChangesAsync();
            // commit transaction
            await transaction.CommitAsync();
            _logger.LogInformation("Communicate area with id {CommunicateId}, {AreaId}  deleted", communicateAreaDto.CommunicateId, communicateAreaDto.AreaId);
            return new ServiceResponse($"Communicate area {communicateAreaDto.CommunicateId}, {communicateAreaDto.AreaId} deleted", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting communicate area");
            // rollback transaction
            await transaction.RollbackAsync();
            return new ServiceResponse($"Error deleting communicate area");
        }
    }
    public async Task<ServiceResponse> DeleteCommunicateAsset(CommunicateAssetDto communicateAssetDto, string userId)
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();
        // await using transaction
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            // get communicate asset
            var communicateAsset = await context.CommunicateAssets.FirstOrDefaultAsync(c => c.CommunicateId == communicateAssetDto.CommunicateId && c.AssetId == communicateAssetDto.AssetId);
            if (communicateAsset == null)
            {
                _logger.LogWarning("Communicate asset not found");
                return new ServiceResponse("Communicate asset not found");
            }
            // check if CommunicateAsset is not marked as deleted
            if (communicateAsset.IsDeleted == false)
            {
                _logger.LogWarning("Communicate asset is not marked as deleted");
                return new ServiceResponse("Communicate asset is not marked as deleted");
            }
            // delete communicate asset
            context.CommunicateAssets.Remove(communicateAsset);
            // save changes
            await context.SaveChangesAsync();
            // commit transaction
            await transaction.CommitAsync();
            _logger.LogInformation("Communicate asset with id {CommunicateId}, {AssetId}  deleted", communicateAssetDto.CommunicateId, communicateAssetDto.AssetId);
            return new ServiceResponse($"Communicate asset {communicateAssetDto.CommunicateId}, {communicateAssetDto.AssetId} deleted", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting communicate asset");
            // rollback transaction
            await transaction.RollbackAsync();
            return new ServiceResponse($"Error deleting communicate asset");
        }
    }
    public async Task<ServiceResponse> DeleteCommunicateCategory(CommunicateCategoryDto communicateCategoryDto, string userId)
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();
        // await using transaction
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            // get communicate category
            var communicateCategory = await context.CommunicateCategories.FirstOrDefaultAsync(c => c.CommunicateId == communicateCategoryDto.CommunicateId && c.CategoryId == communicateCategoryDto.CategoryId);
            if (communicateCategory == null)
            {
                _logger.LogWarning("Communicate category not found");
                return new ServiceResponse("Communicate category not found");
            }
            // check if CommunicateCategory is not marked as deleted
            if (communicateCategory.IsDeleted == false)
            {
                _logger.LogWarning("Communicate category is not marked as deleted");
                return new ServiceResponse("Communicate category is not marked as deleted");
            }
            // delete communicate category
            context.CommunicateCategories.Remove(communicateCategory);
            // save changes
            await context.SaveChangesAsync();
            // commit transaction
            await transaction.CommitAsync();
            _logger.LogInformation("Communicate category with id {CommunicateId}, {CategoryId}  deleted", communicateCategoryDto.CommunicateId, communicateCategoryDto.CategoryId);
            return new ServiceResponse($"Communicate category {communicateCategoryDto.CommunicateId}, {communicateCategoryDto.CategoryId} deleted", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting communicate category");
            // rollback transaction
            await transaction.RollbackAsync();
            return new ServiceResponse($"Error deleting communicate category");
        }
    }
    public async Task<ServiceResponse> DeleteCommunicateCoordinate(CommunicateCoordinateDto communicateCoordinateDto, string userId)
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();
        // await using transaction
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            // get communicate coordinate
            var communicateCoordinate = await context.CommunicateCoordinates.FirstOrDefaultAsync(c => c.CommunicateId == communicateCoordinateDto.CommunicateId && c.CoordinateId == communicateCoordinateDto.CoordinateId);
            if (communicateCoordinate == null)
            {
                _logger.LogWarning("Communicate coordinate not found");
                return new ServiceResponse("Communicate coordinate not found");
            }
            // check if CommunicateCoordinate is not marked as deleted
            if (communicateCoordinate.IsDeleted == false)
            {
                _logger.LogWarning("Communicate coordinate is not marked as deleted");
                return new ServiceResponse("Communicate coordinate is not marked as deleted");
            }
            // delete communicate coordinate
            context.CommunicateCoordinates.Remove(communicateCoordinate);
            // save changes
            await context.SaveChangesAsync();
            // commit transaction
            await transaction.CommitAsync();
            _logger.LogInformation("Communicate coordinate with id {CommunicateId}, {CoordinateId}  deleted", communicateCoordinateDto.CommunicateId, communicateCoordinateDto.CoordinateId);
            return new ServiceResponse($"Communicate coordinate {communicateCoordinateDto.CommunicateId}, {communicateCoordinateDto.CoordinateId} deleted", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting communicate coordinate");
            // rollback transaction
            await transaction.RollbackAsync();
            return new ServiceResponse($"Error deleting communicate coordinate");
        }
    }
    public async Task<ServiceResponse> DeleteCommunicateDevice(CommunicateDeviceDto communicateDeviceDto, string userId)
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();
        // await using transaction
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            // get communicate device
            var communicateDevice = await context.CommunicateDevices.FirstOrDefaultAsync(c => c.CommunicateId == communicateDeviceDto.CommunicateId && c.DeviceId == communicateDeviceDto.DeviceId);
            if (communicateDevice == null)
            {
                _logger.LogWarning("Communicate device not found");
                return new ServiceResponse("Communicate device not found");
            }
            // check if CommunicateDevice is not marked as deleted
            if (communicateDevice.IsDeleted == false)
            {
                _logger.LogWarning("Communicate device is not marked as deleted");
                return new ServiceResponse("Communicate device is not marked as deleted");
            }
            // delete communicate device
            context.CommunicateDevices.Remove(communicateDevice);
            // save changes
            await context.SaveChangesAsync();
            // commit transaction
            await transaction.CommitAsync();
            _logger.LogInformation("Communicate device with id {CommunicateId}, {DeviceId}  deleted", communicateDeviceDto.CommunicateId, communicateDeviceDto.DeviceId);
            return new ServiceResponse($"Communicate device {communicateDeviceDto.CommunicateId}, {communicateDeviceDto.DeviceId} deleted", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting communicate device");
            // rollback transaction
            await transaction.RollbackAsync();
            return new ServiceResponse($"Error deleting communicate device");
        }
    }
    public async Task<ServiceResponse> DeleteCommunicateModel(CommunicateModelDto communicateModelDto, string userId)
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();
        // await using transaction
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            // get communicate model
            var communicateModel = await context.CommunicateModels.FirstOrDefaultAsync(c => c.CommunicateId == communicateModelDto.CommunicateId && c.ModelId == communicateModelDto.ModelId);
            if (communicateModel == null)
            {
                _logger.LogWarning("Communicate model not found");
                return new ServiceResponse("Communicate model not found");
            }
            // check if CommunicateModel is not marked as deleted
            if (communicateModel.IsDeleted == false)
            {
                _logger.LogWarning("Communicate model is not marked as deleted");
                return new ServiceResponse("Communicate model is not marked as deleted");
            }
            // delete communicate model
            context.CommunicateModels.Remove(communicateModel);
            // save changes
            await context.SaveChangesAsync();
            // commit transaction
            await transaction.CommitAsync();
            _logger.LogInformation("Communicate model with id {CommunicateId}, {ModelId}  deleted", communicateModelDto.CommunicateId, communicateModelDto.ModelId);
            return new ServiceResponse($"Communicate model {communicateModelDto.CommunicateId}, {communicateModelDto.ModelId} deleted", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting communicate model");
            // rollback transaction
            await transaction.RollbackAsync();
            return new ServiceResponse($"Error deleting communicate model");
        }
    }
    public async Task<ServiceResponse> DeleteCommunicateSpace(CommunicateSpaceDto communicateSpaceDto, string userId)
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();
        // await using transaction
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            // get communicate space
            var communicateSpace = await context.CommunicateSpaces.FirstOrDefaultAsync(c => c.CommunicateId == communicateSpaceDto.CommunicateId && c.SpaceId == communicateSpaceDto.SpaceId);
            if (communicateSpace == null)
            {
                _logger.LogWarning("Communicate space not found");
                return new ServiceResponse("Communicate space not found");
            }
            // check if CommunicateSpace is not marked as deleted
            if (communicateSpace.IsDeleted == false)
            {
                _logger.LogWarning("Communicate space is not marked as deleted");
                return new ServiceResponse("Communicate space is not marked as deleted");
            }
            // delete communicate space
            context.CommunicateSpaces.Remove(communicateSpace);
            // save changes
            await context.SaveChangesAsync();
            // commit transaction
            await transaction.CommitAsync();
            _logger.LogInformation("Communicate space with id {CommunicateId}, {SpaceId}  deleted", communicateSpaceDto.CommunicateId, communicateSpaceDto.SpaceId);
            return new ServiceResponse($"Communicate space {communicateSpaceDto.CommunicateId}, {communicateSpaceDto.SpaceId} deleted", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting communicate space");
            // rollback transaction
            await transaction.RollbackAsync();
            return new ServiceResponse($"Error deleting communicate space");
        }
    }
    public async Task<ServiceResponse<SituationDto>> GetSituationById(int situationId)
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();
        try
        {
            // get situation
            var situation = await context.Situations
                .AsNoTracking()
                .Select(s => new SituationDto
                {
                    SituationId = s.SituationId,
                    Name = s.Name,
                    Description = s.Description,
                    IsDeleted = s.IsDeleted,
                    UserId = s.UserId
                })
                .FirstOrDefaultAsync(c => c.SituationId == situationId);
            if (situation == null)
            {
                _logger.LogWarning("Situation not found");
                return new ServiceResponse<SituationDto>("Situation not found");
            }
            // return situation
            _logger.LogInformation("Situation with id {SituationId} returned", situationId);
            return new ServiceResponse<SituationDto>(situation, "Situation found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting situation");
            return new ServiceResponse<SituationDto>($"Error getting situation");
        }
    }
    public async Task<ServiceResponse<CommunicateDto>> GetCommunicateById(int communicateId)
    {
       // await using context
       await using var context = await _contextFactory.CreateDbContextAsync();
       try
       {
           // get communicate
           var communicate = await context.Communicates
               .AsNoTracking()
               .Select(c => new CommunicateDto
               {
                   CommunicateId = c.CommunicateId,
                   Name = c.Name,
                   Description = c.Description,
                   IsDeleted = c.IsDeleted,
                   UserId = c.UserId
               })
               .FirstOrDefaultAsync(c => c.CommunicateId == communicateId);
           if (communicate == null)
           {
               _logger.LogWarning("Communicate not found");
               return new ServiceResponse<CommunicateDto>("Communicate not found");
           }
           // return communicate
           _logger.LogInformation("Communicate with id {CommunicateId} returned", communicateId);
           return new ServiceResponse<CommunicateDto>(communicate, "Communicate found");
       }
       catch (Exception ex)
       {
           _logger.LogError(ex, "Error getting communicate");
           return new ServiceResponse<CommunicateDto>($"Error getting communicate");
       }
    }
    public async Task<ServiceResponse<IEnumerable<CommunicateDto>>> GetCommunicates()
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();
        try
        {
            // get communicates
            var communicates = await context.Communicates
                .AsNoTracking()
                .Select(c => new CommunicateDto
                {
                    CommunicateId = c.CommunicateId,
                    Name = c.Name,
                    Description = c.Description,
                    IsDeleted = c.IsDeleted,
                    UserId = c.UserId
                })
                .ToListAsync();
            if (communicates.Count == 0)
            {
                _logger.LogWarning("Communicates not found");
                return new ServiceResponse<IEnumerable<CommunicateDto>>("Communicates not found");
            }
            
            // return communicates
            _logger.LogInformation("Communicates returned");
            return new ServiceResponse<IEnumerable<CommunicateDto>>(communicates, "Communicates found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting communicates");
            return new ServiceResponse<IEnumerable<CommunicateDto>>($"Error getting communicates");
        }
    }
    public async Task<ServiceResponse<IEnumerable<CommunicateWithAssetsDto>>> GetCommunicatesWithAssets()
    {
      // await using context
      await using var context = await _contextFactory.CreateDbContextAsync();
      try
      {
          // get communicates
          var communicates = await context.Communicates
              .AsNoTracking()
              .Select(c => new CommunicateWithAssetsDto
              {
                  CommunicateId = c.CommunicateId,
                  Name = c.Name,
                  Description = c.Description,
                  IsDeleted = c.IsDeleted,
                  UserId = c.UserId,
                  Assets = c.CommunicateAssets.Select(a => new AssetDto
                  {
                      AssetId = a.AssetId,
                      Name = a.Asset.Name,
                      Description = a.Asset.Description,
                      IsDeleted = a.Asset.IsDeleted,
                      UserId = a.Asset.UserId
                  }).ToList()
              })
              .ToListAsync();
          if (communicates.Count == 0)
          {
              _logger.LogWarning("Communicates not found");
              return new ServiceResponse<IEnumerable<CommunicateWithAssetsDto>>("Communicates not found");
          }
          // return communicates
          _logger.LogInformation("Communicates returned");
          return new ServiceResponse<IEnumerable<CommunicateWithAssetsDto>>(communicates, "Communicates found");
      }
      catch (Exception ex)
      {
          _logger.LogError(ex, "Error getting communicates");
          return new ServiceResponse<IEnumerable<CommunicateWithAssetsDto>>($"Error getting communicates");
      }
    }

    public async Task<ServiceResponse> MarkDeleteCommunicate(int communicateId, string userId)
    {
       // await using context
       await using var context = await _contextFactory.CreateDbContextAsync();
       // await using transaction
       await using var transaction = await context.Database.BeginTransactionAsync();
       try
       {
           // get communicate
           var communicate = await context.Communicates
               .Include(c => c.CommunicateAssets)
               .FirstOrDefaultAsync(c => c.CommunicateId == communicateId);
           // if communicate not found
           if (communicate == null)
           {
               _logger.LogWarning("Communicate with id {CommunicateId} not found", communicateId);
               return new ServiceResponse($"Communicate with id {communicateId} not found");
           }
           // if communicate is already deleted
           if (communicate.IsDeleted)
           {
               _logger.LogWarning("Communicate with id {CommunicateId} is already deleted", communicateId);
               return new ServiceResponse($"Communicate with id {communicateId} is already deleted");
           }
           // check if communicate has CommunicateAssets with IsDeleted = false
           if (communicate.CommunicateAssets.Any(ca => ca.IsDeleted == false))
           {
               _logger.LogWarning("Communicate with id {CommunicateId} has CommunicateAssets with IsDeleted = false", communicateId);
               return new ServiceResponse($"Communicate with id {communicateId} has CommunicateAssets with IsDeleted = false");
           }

           // mark communicate as deleted
           communicate.IsDeleted = true;
           communicate.UserId = userId;
           // save changes
           await context.SaveChangesAsync();
           // commit transaction
           await transaction.CommitAsync();
           // return success
           _logger.LogInformation("Communicate with id {CommunicateId} marked as deleted", communicateId);
           return new ServiceResponse($"Communicate with id {communicateId} marked as deleted");
       }
       catch (Exception ex)
       {
           _logger.LogError(ex, "Error marking communicate with id {CommunicateId} as deleted", communicateId);
           // rollback transaction
           await transaction.RollbackAsync();
           return new ServiceResponse($"Error marking communicate with id {communicateId} as deleted");
       }
    }
   
    public async Task<ServiceResponse> MarkDeleteCommunicateArea(CommunicateAreaDto communicateAreaDto, string userId)
    {
      // await using context
      await using var context = await _contextFactory.CreateDbContextAsync();
      // await using transaction
      await using var transaction = await context.Database.BeginTransactionAsync();
      try
      {
        // get communicateArea
        var communicateArea = await context.CommunicateAreas.FindAsync(communicateAreaDto.CommunicateId, communicateAreaDto.AreaId);
        if (communicateArea == null)
        {
          _logger.LogWarning("CommunicateArea not found");
          return new ServiceResponse("CommunicateArea not found");
        }
        if (communicateArea.IsDeleted)
        {
          _logger.LogWarning("CommunicateArea already marked as deleted");
          return new ServiceResponse("CommunicateArea already marked as deleted");
        }

        communicateArea.UserId = userId;
        communicateArea.IsDeleted = true;
        context.Update(communicateArea);
        // save changes
        await context.SaveChangesAsync();
        await transaction.CommitAsync();
        _logger.LogInformation("CommunicateArea with id {CommunicateId}, {AreaId} marked as deleted", communicateAreaDto.CommunicateId, communicateAreaDto.AreaId);
        return new ServiceResponse($"CommunicateArea with id {communicateAreaDto.CommunicateId}, {communicateAreaDto.AreaId} marked as deleted", true);
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error marking communicateArea with id {CommunicateId}, {AreaId} as deleted", communicateAreaDto.CommunicateId, communicateAreaDto.AreaId);
        await transaction.RollbackAsync();
        return new ServiceResponse($"Error marking communicateArea with id {communicateAreaDto.CommunicateId}, {communicateAreaDto.AreaId} as deleted");
      }
    }

    public async Task<ServiceResponse> MarkDeleteCommunicateAsset(CommunicateAssetDto communicateAssetDto, string userId)
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();
        // await using transaction
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            // get communicateAsset
            var communicateAsset = await context.CommunicateAssets.FindAsync(communicateAssetDto.CommunicateId, communicateAssetDto.AssetId);
            if (communicateAsset == null)
            {
                _logger.LogWarning("CommunicateAsset not found");
                return new ServiceResponse("CommunicateAsset not found");
            }
            if (communicateAsset.IsDeleted)
            {
                _logger.LogWarning("CommunicateAsset already marked as deleted");
                return new ServiceResponse("CommunicateAsset already marked as deleted");
            }

            communicateAsset.UserId = userId;
            communicateAsset.IsDeleted = true;
            context.Update(communicateAsset);
            // save changes
            await context.SaveChangesAsync();
            await transaction.CommitAsync();
            _logger.LogInformation("CommunicateAsset with id {CommunicateId}, {AssetId} marked as deleted", communicateAssetDto.CommunicateId, communicateAssetDto.AssetId);
            return new ServiceResponse($"CommunicateAsset with id {communicateAssetDto.CommunicateId}, {communicateAssetDto.AssetId} marked as deleted", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking communicateAsset with id {CommunicateId}, {AssetId} as deleted", communicateAssetDto.CommunicateId, communicateAssetDto.AssetId);
            await transaction.RollbackAsync();
            return new ServiceResponse($"Error marking communicateAsset with id {communicateAssetDto.CommunicateId}, {communicateAssetDto.AssetId} as deleted");
        }
    }

    public async Task<ServiceResponse> MarkDeleteCommunicateCategory(CommunicateCategoryDto communicateCategoryDto, string userId)
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();
        // await using transaction
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            // get communicateCategory
            var communicateCategory = await context.CommunicateCategories.FindAsync(communicateCategoryDto.CommunicateId, communicateCategoryDto.CategoryId);
            if (communicateCategory == null)
            {
                _logger.LogWarning("CommunicateCategory not found");
                return new ServiceResponse("CommunicateCategory not found");
            }
            if (communicateCategory.IsDeleted)
            {
                _logger.LogWarning("CommunicateCategory already marked as deleted");
                return new ServiceResponse("CommunicateCategory already marked as deleted");
            }

            communicateCategory.UserId = userId;
            communicateCategory.IsDeleted = true;
            context.Update(communicateCategory);
            // save changes
            await context.SaveChangesAsync();
            await transaction.CommitAsync();
            _logger.LogInformation("CommunicateCategory with id {CommunicateId}, {CategoryId} marked as deleted", communicateCategoryDto.CommunicateId, communicateCategoryDto.CategoryId);
            return new ServiceResponse($"CommunicateCategory with id {communicateCategoryDto.CommunicateId}, {communicateCategoryDto.CategoryId} marked as deleted", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking communicateCategory with id {CommunicateId}, {CategoryId} as deleted", communicateCategoryDto.CommunicateId, communicateCategoryDto.CategoryId);
            await transaction.RollbackAsync();
            return new ServiceResponse($"Error marking communicateCategory with id {communicateCategoryDto.CommunicateId}, {communicateCategoryDto.CategoryId} as deleted");
        }
    }

    public async Task<ServiceResponse> MarkDeleteCommunicateCoordinate(CommunicateCoordinateDto communicateCoordinateDto, string userId)
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();
        // await using transaction
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            // get communicateCoordinate
            var communicateCoordinate = await context.CommunicateCoordinates.FindAsync(communicateCoordinateDto.CommunicateId, communicateCoordinateDto.CoordinateId);
            if (communicateCoordinate == null)
            {
                _logger.LogWarning("CommunicateCoordinate not found");
                return new ServiceResponse("CommunicateCoordinate not found");
            }
            if (communicateCoordinate.IsDeleted)
            {
                _logger.LogWarning("CommunicateCoordinate already marked as deleted");
                return new ServiceResponse("CommunicateCoordinate already marked as deleted");
            }

            communicateCoordinate.UserId = userId;
            communicateCoordinate.IsDeleted = true;
            context.Update(communicateCoordinate);
            // save changes
            await context.SaveChangesAsync();
            await transaction.CommitAsync();
            _logger.LogInformation("CommunicateCoordinate with id {CommunicateId}, {CoordinateId} marked as deleted", communicateCoordinateDto.CommunicateId, communicateCoordinateDto.CoordinateId);
            return new ServiceResponse($"CommunicateCoordinate with id {communicateCoordinateDto.CommunicateId}, {communicateCoordinateDto.CoordinateId} marked as deleted", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking communicateCoordinate with id {CommunicateId}, {CoordinateId} as deleted", communicateCoordinateDto.CommunicateId, communicateCoordinateDto.CoordinateId);
            await transaction.RollbackAsync();
            return new ServiceResponse($"Error marking communicateCoordinate with id {communicateCoordinateDto.CommunicateId}, {communicateCoordinateDto.CoordinateId} as deleted");
        }
    }

    public async Task<ServiceResponse> MarkDeleteCommunicateDevice(CommunicateDeviceDto communicateDeviceDto, string userId)
    {
       // await using context
       await using var context = await _contextFactory.CreateDbContextAsync();
       // await using transaction
       await using var transaction = await context.Database.BeginTransactionAsync();
       try
       {
           // get communicateDevice
           var communicateDevice = await context.CommunicateDevices.FindAsync(communicateDeviceDto.CommunicateId, communicateDeviceDto.DeviceId);
           if (communicateDevice == null)
           {
               _logger.LogWarning("CommunicateDevice not found");
               return new ServiceResponse("CommunicateDevice not found");
           }
           if (communicateDevice.IsDeleted)
           {
               _logger.LogWarning("CommunicateDevice already marked as deleted");
               return new ServiceResponse("CommunicateDevice already marked as deleted");
           }

           communicateDevice.UserId = userId;
           communicateDevice.IsDeleted = true;
           context.Update(communicateDevice);
           // save changes
           await context.SaveChangesAsync();
           await transaction.CommitAsync();
           _logger.LogInformation("CommunicateDevice with id {CommunicateId}, {DeviceId} marked as deleted", communicateDeviceDto.CommunicateId, communicateDeviceDto.DeviceId);
           return new ServiceResponse($"CommunicateDevice with id {communicateDeviceDto.CommunicateId}, {communicateDeviceDto.DeviceId} marked as deleted", true);
       }
       catch (Exception ex)
       {
           _logger.LogError(ex, "Error marking communicateDevice with id {CommunicateId}, {DeviceId} as deleted", communicateDeviceDto.CommunicateId, communicateDeviceDto.DeviceId);
           await transaction.RollbackAsync();
           return new ServiceResponse($"Error marking communicateDevice with id {communicateDeviceDto.CommunicateId}, {communicateDeviceDto.DeviceId} as deleted");
       }
    }

    public async Task<ServiceResponse> MarkDeleteCommunicateModel(CommunicateModelDto communicateModelDto, string userId)
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();
        // await using transaction
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            // get communicateModel
            var communicateModel = await context.CommunicateModels.FindAsync(communicateModelDto.CommunicateId, communicateModelDto.ModelId);
            if (communicateModel == null)
            {
                _logger.LogWarning("CommunicateModel not found");
                return new ServiceResponse("CommunicateModel not found");
            }
            if (communicateModel.IsDeleted)
            {
                _logger.LogWarning("CommunicateModel already marked as deleted");
                return new ServiceResponse("CommunicateModel already marked as deleted");
            }

            communicateModel.UserId = userId;
            communicateModel.IsDeleted = true;
            context.Update(communicateModel);
            // save changes
            await context.SaveChangesAsync();
            await transaction.CommitAsync();
            _logger.LogInformation("CommunicateModel with id {CommunicateId}, {ModelId} marked as deleted", communicateModelDto.CommunicateId, communicateModelDto.ModelId);
            return new ServiceResponse($"CommunicateModel with id {communicateModelDto.CommunicateId}, {communicateModelDto.ModelId} marked as deleted", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking communicateModel with id {CommunicateId}, {ModelId} as deleted", communicateModelDto.CommunicateId, communicateModelDto.ModelId);
            await transaction.RollbackAsync();
            return new ServiceResponse($"Error marking communicateModel with id {communicateModelDto.CommunicateId}, {communicateModelDto.ModelId} as deleted");
        }
    }

    public async Task<ServiceResponse> MarkDeleteCommunicateSpace(CommunicateSpaceDto communicateSpaceDto, string userId)
    {
      // await using context
      await using var context = await _contextFactory.CreateDbContextAsync();
      // await using transaction
      await using var transaction = await context.Database.BeginTransactionAsync();
      try
      {
        // get communicateSpace
        var communicateSpace = await context.CommunicateSpaces.FindAsync(communicateSpaceDto.CommunicateId, communicateSpaceDto.SpaceId);
        if (communicateSpace == null)
        {
          _logger.LogWarning("CommunicateSpace not found");
          return new ServiceResponse("CommunicateSpace not found");
        }
        if (communicateSpace.IsDeleted)
        {
          _logger.LogWarning("CommunicateSpace already marked as deleted");
          return new ServiceResponse("CommunicateSpace already marked as deleted");
        }

        communicateSpace.UserId = userId;
        communicateSpace.IsDeleted = true;
        context.Update(communicateSpace);
        // save changes
        await context.SaveChangesAsync();
        await transaction.CommitAsync();
        _logger.LogInformation("CommunicateSpace with id {CommunicateId}, {SpaceId} marked as deleted", communicateSpaceDto.CommunicateId, communicateSpaceDto.SpaceId);
        return new ServiceResponse($"CommunicateSpace with id {communicateSpaceDto.CommunicateId}, {communicateSpaceDto.SpaceId} marked as deleted", true);
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error marking communicateSpace with id {CommunicateId}, {SpaceId} as deleted", communicateSpaceDto.CommunicateId, communicateSpaceDto.SpaceId);
        await transaction.RollbackAsync();
        return new ServiceResponse($"Error marking communicateSpace with id {communicateSpaceDto.CommunicateId}, {communicateSpaceDto.SpaceId} as deleted");
      }
    }

    /// <summary>
    /// Updates communicate, returns service response with true if updated
    /// </summary>
    /// <param name="communicateId"></param>
    /// <param name="userId"></param>
    /// <param name="communicateUpdateDto"></param>
    /// <returns></returns>
    public async Task<ServiceResponse> UpdateCommunicate(int communicateId, string userId,
        CommunicateUpdateDto communicateUpdateDto)
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();
        // await using transaction
        await using var transaction = await context.Database.BeginTransactionAsync();
        // try-catch
        try
        {
            var communicate = await context.Communicates.FirstOrDefaultAsync(c => c.CommunicateId == communicateId);
            if (communicate == null)
            {
                return new ServiceResponse("Communicate not found");
            }
            // check if communicate is not marked as deleted
            if (communicate.IsDeleted)
            {
                _logger.LogWarning("Communicate is marked as deleted");
                return new ServiceResponse("Communicate is marked as deleted");
            }
            // check if duplicate exists and is not marked as deleted
            var exists = await context.Communicates.AnyAsync(c =>
                c.Name.ToLower().Trim() == communicateUpdateDto.Name.ToLower().Trim() && c.IsDeleted == false &&
                c.CommunicateId != communicateId);
            if (exists)
            {
                _logger.LogWarning("Communicate with name {Name} already exists", communicateUpdateDto.Name);
                return new ServiceResponse($"Communicate with name {communicateUpdateDto.Name} already exists");
            }

            // update name
            if (!Equals(communicateUpdateDto.Name.ToLower().Trim(), communicate.Name.ToLower().Trim()))
            {
                communicate.Name = communicateUpdateDto.Name;
                communicate.UserId = userId;
            }

            if (!Equals(communicateUpdateDto.Description.ToLower().Trim(), communicate.Description.ToLower().Trim()))
            {
                communicate.Description = communicateUpdateDto.Description;
                communicate.UserId = userId;
            }

            context.Communicates.Update(communicate);
            await context.SaveChangesAsync();
            await transaction.CommitAsync();
            return new ServiceResponse("Communicate updated", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating communicate");
            await transaction.RollbackAsync();
            return new ServiceResponse("Error updating communicate");
        }
    }
}

