using Microsoft.EntityFrameworkCore;

using Sc3Hosted.Server.Data;
using Sc3Hosted.Server.Entities;
using Sc3Hosted.Server.Exceptions;
using Sc3Hosted.Shared.Dtos;

namespace Sc3Hosted.Server.Services;

public interface ICommunicateService
{
    Task<int> CreateCommunicate(CommunicateCreateDto communicateCreateDto);

    Task<(int, int)> CreateCommunicateArea(int communicateId, int areaId);

    Task<(int, int)> CreateCommunicateAsset(int communicateId, int assetId);

    Task<(int, int)> CreateCommunicateCategory(int communicateId, int categoryId);

    Task<(int, int)> CreateCommunicateCoordinate(int communicateId, int coordinateId);

    Task<(int, int)> CreateCommunicateDevice(int communicateId, int deviceId);

    Task<(int, int)> CreateCommunicateModel(int communicateId, int modelId);

    Task<(int, int)> CreateCommunicateSpace(int communicateId, int spaceId);

    Task DeleteCommunicate(int communicateId);

    Task DeleteCommunicateArea(int communicateId, int areaId);

    Task DeleteCommunicateAsset(int communicateId, int assetId);

    Task DeleteCommunicateCategory(int communicateId, int categoryId);

    Task DeleteCommunicateCoordinate(int communicateId, int coordinateId);

    Task DeleteCommunicateDevice(int communicateId, int deviceId);

    Task DeleteCommunicateModel(int communicateId, int modelId);

    Task DeleteCommunicateSpace(int communicateId, int spaceId);

    Task<CommunicateDto> GetCommunicateById(int communicateId);

    Task<IEnumerable<CommunicateDto>> GetCommunicates();

    Task<IEnumerable<CommunicateWithAssetsDto>> GetCommunicatesWithAssets();

    Task MarkDeleteCommunicate(int communicateId);

    Task MarkDeleteCommunicateArea(int communicateId, int areaId);

    Task MarkDeleteCommunicateAsset(int communicateId, int assetId);

    Task MarkDeleteCommunicateCategory(int communicateId, int categoryId);

    Task MarkDeleteCommunicateCoordinate(int communicateId, int coordinateId);

    Task MarkDeleteCommunicateDevice(int communicateId, int deviceId);

    Task MarkDeleteCommunicateModel(int communicateId, int modelId);

    Task MarkDeleteCommunicateSpace(int communicateId, int spaceId);

    Task UpdateCommunicate(int communicateId, CommunicateUpdateDto communicateUpdateDto);

    Task UpdateCommunicateArea(int communicateId, int areaId);
    Task UpdateCommunicateAsset(int communicateId, int assetId);
    Task UpdateCommunicateCategory(int communicateId, int categoryId);
    Task UpdateCommunicateCoordinate(int communicateId, int coordinateId);
    Task UpdateCommunicateDevice(int communicateId, int deviceId);
    Task UpdateCommunicateModel(int communicateId, int modelId);
    Task UpdateCommunicateSpace(int communicateId, int spaceId);
}

public class CommunicateService : ICommunicateService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<CommunicateService> _logger;
    
    public CommunicateService(ApplicationDbContext context, ILogger<CommunicateService> logger)
    {
        _context = context;
        _logger = logger;
    
    }

    public async Task<int> CreateCommunicate(CommunicateCreateDto communicateCreateDto)
    {
        
        // await using context
        

        // validate category name
        var duplicate = await _context.Communicates.AnyAsync(c => c.Name.ToLower().Trim() == communicateCreateDto.Name.ToLower().Trim());
        if (duplicate)
        {
            _logger.LogWarning("Communicate name already exists");
            throw new BadRequestException("Communicate name already exists");
        }

        var communicate = new Communicate
        {
            
            Name = communicateCreateDto.Name,
            Description = communicateCreateDto.Description,
            IsDeleted = false
        };
        // create category
        _context.Communicates.Add(communicate);
        
        
        try
        {
            // save changes
            await _context.SaveChangesAsync();
            
            
            _logger.LogInformation("Communicate with id {CommunicateId} created", communicate.CommunicateId);
            return communicate.CommunicateId;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating communicate");
            
            
            throw new BadRequestException("Error creating communicate");
        }
    }

    public async Task<(int, int)> CreateCommunicateArea(int communicateId, int areaId)
    {
        
        // await using context
        
        // get communicateArea
        var communicateArea = await _context.CommunicateAreas.FindAsync(communicateId, areaId);
        if (communicateArea != null)
            throw new BadRequestException("CommunicateArea already exists");
        var area = await _context.Areas.FindAsync(areaId);
        if (area == null || area.IsDeleted)
        {
            _logger.LogWarning("Area not found");
            throw new NotFoundException("Area not found");
        }
        var communicate = await _context.Communicates.FindAsync(communicateId);
        if (communicate == null || communicate.IsDeleted)
        {
            _logger.LogWarning("Communicate not found");
            throw new NotFoundException("Communicate not found");
        }
        communicateArea = new CommunicateArea
        {
            CommunicateId = communicateId,
            AreaId = areaId,
            
            IsDeleted = false
        };
        _context.Add(communicateArea);
        // save changes
        
        try
        {
            await _context.SaveChangesAsync();
            
            return (communicateId, areaId);
        }
        catch (Exception ex)
        {
            
            _logger.LogError(ex, "Error creating communicateArea");
            throw new BadRequestException("Error while saving changes");
        }
    }

    public async Task<(int, int)> CreateCommunicateAsset(int communicateId, int assetId)
    {
        
        // await using context
        
        // get communicateAsset
        var communicateAsset = await _context.CommunicateAssets.FindAsync(communicateId, assetId);
        if (communicateAsset != null)
            throw new BadRequestException("CommunicateAsset already exists");
        var asset = await _context.Assets.FindAsync(assetId);
        if (asset == null || asset.IsDeleted)
        {
            _logger.LogWarning("Asset not found");
            throw new NotFoundException("Asset not found");
        }
        var communicate = await _context.Communicates.FindAsync(communicateId);
        if (communicate == null || communicate.IsDeleted)
        {
            _logger.LogWarning("Communicate not found");
            throw new NotFoundException("Communicate not found");
        }
        communicateAsset = new CommunicateAsset
        {
            CommunicateId = communicateId,
            AssetId = assetId,
            
            IsDeleted = false
        };
        _context.Add(communicateAsset);
        // save changes
        
        try
        {
            await _context.SaveChangesAsync();
            
            return (communicateId, assetId);
        }
        catch (Exception ex)
        {
            
            _logger.LogError(ex, "Error creating communicateAsset");
            throw new BadRequestException("Error while saving changes");
        }
    }

    public async Task<(int, int)> CreateCommunicateCategory(int communicateId, int categoryId)
    {
        
        // await using context
        
        // get communicateCategory
        var communicateCategory = await _context.CommunicateCategories.FindAsync(communicateId, categoryId);
        if (communicateCategory != null)
            throw new BadRequestException("CommunicateCategory already exists");
        var communicate = await _context.Communicates.FindAsync(communicateId);
        if (communicate == null || communicate.IsDeleted)
        {
            _logger.LogWarning("Communicate not found");
            throw new NotFoundException("Communicate not found");
        }
        var category = await _context.Categories.FindAsync(categoryId);
        if (category == null || category.IsDeleted)
        {
            _logger.LogWarning("Category not found");
            throw new NotFoundException("Category not found");
        }
        communicateCategory = new CommunicateCategory
        {
            CommunicateId = communicateId,
            CategoryId = categoryId,
            
            IsDeleted = false
        };
        _context.Add(communicateCategory);
        // save changes
        
        try
        {
            await _context.SaveChangesAsync();
            
            return (communicateId, categoryId);
        }
        catch (Exception ex)
        {
            
            _logger.LogError(ex, "Error creating communicateCategory");
            throw new BadRequestException("Error while saving changes");
        }
    }

    public async Task<(int, int)> CreateCommunicateCoordinate(int communicateId, int coordinateId)
    {
        
        // await using context
        
        // get communicateCoordinate
        var communicateCoordinate = await _context.CommunicateCoordinates.FindAsync(communicateId, coordinateId);
        if (communicateCoordinate != null)
            throw new BadRequestException("CommunicateCoordinate already exists");
        var communicate = await _context.Communicates.FindAsync(communicateId);
        if (communicate == null || communicate.IsDeleted)
        {
            _logger.LogWarning("Communicate not found");
            throw new NotFoundException("Communicate not found");
        }
        var coordinate = await _context.Coordinates.FindAsync(coordinateId);
        if (coordinate == null || coordinate.IsDeleted)
        {
            _logger.LogWarning("Coordinate not found");
            throw new NotFoundException("Coordinate not found");
        }
        communicateCoordinate = new CommunicateCoordinate
        {
            CommunicateId = communicateId,
            CoordinateId = coordinateId,
            
            IsDeleted = false
        };
        _context.Add(communicateCoordinate);
        // save changes
        
        try
        {
            await _context.SaveChangesAsync();
            
            return (communicateId, coordinateId);
        }
        catch (Exception ex)
        {
            
            _logger.LogError(ex, "Error creating communicateCoordinate");
            throw new BadRequestException("Error while saving changes");
        }
    }

    public async Task<(int, int)> CreateCommunicateDevice(int communicateId, int deviceId)
    {
        
        // await using context
        
        // get communicateDevice
        var communicateDevice = await _context.CommunicateDevices.FindAsync(communicateId, deviceId);
        if (communicateDevice != null)
            throw new BadRequestException("CommunicateDevice already exists");
        var communicate = await _context.Communicates.FindAsync(communicateId);
        if (communicate == null || communicate.IsDeleted)
        {
            _logger.LogWarning("Communicate not found");
            throw new NotFoundException("Communicate not found");
        }
        var device = await _context.Devices.FindAsync(deviceId);
        if (device == null || device.IsDeleted)
        {
            _logger.LogWarning("Device not found");
            throw new NotFoundException("Device not found");
        }
        communicateDevice = new CommunicateDevice
        {
            CommunicateId = communicateId,
            DeviceId = deviceId,
            
            IsDeleted = false
        };
        _context.Add(communicateDevice);
        // save changes
        
        try
        {
            await _context.SaveChangesAsync();
            
            return (communicateId, deviceId);
        }
        catch (Exception ex)
        {
            
            _logger.LogError(ex, "Error creating communicateDevice");
            throw new BadRequestException("Error while saving changes");
        }
    }

    public async Task<(int, int)> CreateCommunicateModel(int communicateId, int modelId)
    {
        
        // await using context
        
        // get communicateModel
        var communicateModel = await _context.CommunicateModels.FindAsync(communicateId, modelId);
        if (communicateModel != null)
            throw new BadRequestException("CommunicateModel already exists");
        var communicate = await _context.Communicates.FindAsync(communicateId);
        if (communicate == null || communicate.IsDeleted)
        {
            _logger.LogWarning("Communicate not found");
            throw new NotFoundException("Communicate not found");
        }
        var model = await _context.Models.FindAsync(modelId);
        if (model == null || model.IsDeleted)
        {
            _logger.LogWarning("Model not found");
            throw new NotFoundException("Model not found");
        }
        communicateModel = new CommunicateModel
        {
            CommunicateId = communicateId,
            ModelId = modelId,
            
            IsDeleted = false
        };
        _context.Add(communicateModel);
        // save changes
        
        try
        {
            await _context.SaveChangesAsync();
            
            return (communicateId, modelId);
        }
        catch (Exception ex)
        {
            
            _logger.LogError(ex, "Error creating communicateModel");
            throw new BadRequestException("Error while saving changes");
        }
    }

    public async Task<(int, int)> CreateCommunicateSpace(int communicateId, int spaceId)
    {
        
        // await using context
        
        // get communicateSpace
        var communicateSpace = await _context.CommunicateSpaces.FindAsync(communicateId, spaceId);
        if (communicateSpace != null)
            throw new BadRequestException("CommunicateSpace already exists");
        var communicate = await _context.Communicates.FindAsync(communicateId);
        if (communicate == null || communicate.IsDeleted)
        {
            _logger.LogWarning("Communicate not found");
            throw new NotFoundException("Communicate not found");
        }
        var space = await _context.Spaces.FindAsync(spaceId);
        if (space == null || space.IsDeleted)
        {
            _logger.LogWarning("Space not found");
            throw new NotFoundException("Space not found");
        }
        communicateSpace = new CommunicateSpace
        {
            CommunicateId = communicateId,
            SpaceId = spaceId,
            
            IsDeleted = false
        };
        _context.Add(communicateSpace);
        // save changes
        
        try
        {
            await _context.SaveChangesAsync();
            
            return (communicateId, spaceId);
        }
        catch (Exception ex)
        {
            
            _logger.LogError(ex, "Error creating communicateSpace");
            throw new BadRequestException("Error while saving changes");
        }
    }

    public async Task DeleteCommunicate(int communicateId)
    {
        // await using context
        

        // get communicate
        var communicate = await _context.Communicates.FindAsync(communicateId);
        if (communicate == null)
        {
            _logger.LogWarning("Communicate not found");
            throw new NotFoundException("Communicate not found");
        }
        // check if communicate is marked as deleted
        if (communicate.IsDeleted == false)
        {
            _logger.LogWarning("Communicate is not marked as deleted");
            throw new BadRequestException("Communicate is not marked as deleted");
        }
        // delete communicate
        _context.Communicates.Remove(communicate);
        
        
        try
        {
            // save changes
            await _context.SaveChangesAsync();
            
            
            _logger.LogInformation("Communicate with id {CommunicateId} deleted", communicate.CommunicateId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting communicate");
            
            
            throw new BadRequestException("Error deleting communicate");
        }
    }

    public async Task DeleteCommunicateArea(int communicateId, int areaId)
    {
        // await using context
        

        // get communicate area
        var communicateArea = await _context.CommunicateAreas.FirstOrDefaultAsync(c => c.CommunicateId == communicateId && c.AreaId == areaId);
        if (communicateArea == null)
        {
            _logger.LogWarning("Communicate area not found");
            throw new NotFoundException("Communicate area not found");
        }
        // check if CommunicateArea is not marked as deleted
        if (communicateArea.IsDeleted == false)
        {
            _logger.LogWarning("Communicate area is not marked as deleted");
            throw new BadRequestException("Communicate area is not marked as deleted");
        }
        // delete communicate area
        _context.CommunicateAreas.Remove(communicateArea);
        
        
        try
        {
            // save changes
            await _context.SaveChangesAsync();
            
            
            _logger.LogInformation("Communicate area with id {CommunicateId}, {AreaId}  deleted", communicateId, areaId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting communicate area");
            
            
            throw new BadRequestException("Error deleting communicate area");
        }
    }

    public async Task DeleteCommunicateAsset(int communicateId, int assetId)
    {
        // await using context
        

        // get communicate asset
        var communicateAsset = await _context.CommunicateAssets.FirstOrDefaultAsync(c => c.CommunicateId == communicateId && c.AssetId == assetId);
        if (communicateAsset == null)
        {
            _logger.LogWarning("Communicate asset not found");
            throw new NotFoundException("Communicate asset not found");
        }
        // check if CommunicateAsset is not marked as deleted
        if (communicateAsset.IsDeleted == false)
        {
            _logger.LogWarning("Communicate asset is not marked as deleted");
            throw new BadRequestException("Communicate asset is not marked as deleted");
        }
        // delete communicate asset
        _context.CommunicateAssets.Remove(communicateAsset);
        
        
        try
        {
            // save changes
            await _context.SaveChangesAsync();
            
            
            _logger.LogInformation("Communicate asset with id {CommunicateId}, {AssetId}  deleted", communicateId, assetId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting communicate asset");
            
            
            throw new BadRequestException("Error deleting communicate asset");
        }
    }

    public async Task DeleteCommunicateCategory(int communicateId, int categoryId)
    {
        // await using context
        

        // get communicate category
        var communicateCategory = await _context.CommunicateCategories.FirstOrDefaultAsync(c => c.CommunicateId == communicateId && c.CategoryId == categoryId);
        if (communicateCategory == null)
        {
            _logger.LogWarning("Communicate category not found");
            throw new NotFoundException("Communicate category not found");
        }
        // check if CommunicateCategory is not marked as deleted
        if (communicateCategory.IsDeleted == false)
        {
            _logger.LogWarning("Communicate category is not marked as deleted");
            throw new BadRequestException("Communicate category is not marked as deleted");
        }
        // delete communicate category
        _context.CommunicateCategories.Remove(communicateCategory);
        
        
        try
        {
            // save changes
            await _context.SaveChangesAsync();
            
            
            _logger.LogInformation("Communicate category with id {CommunicateId}, {CategoryId}  deleted", communicateId, categoryId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting communicate category");
            
            
            throw new BadRequestException("Error deleting communicate category");
        }
    }

    public async Task DeleteCommunicateCoordinate(int communicateId, int coordinateId)
    {
        // await using context
        
        

        // get communicate coordinate
        var communicateCoordinate = await _context.CommunicateCoordinates.FirstOrDefaultAsync(c => c.CommunicateId == communicateId && c.CoordinateId == coordinateId);
        if (communicateCoordinate == null)
        {
            _logger.LogWarning("Communicate coordinate not found");
            throw new NotFoundException("Communicate coordinate not found");
        }
        // check if CommunicateCoordinate is not marked as deleted
        if (communicateCoordinate.IsDeleted == false)
        {
            _logger.LogWarning("Communicate coordinate is not marked as deleted");
            throw new BadRequestException("Communicate coordinate is not marked as deleted");
        }
        // delete communicate coordinate
        _context.CommunicateCoordinates.Remove(communicateCoordinate);
        
        try
        {
            // save changes
            await _context.SaveChangesAsync();
            
            
            _logger.LogInformation("Communicate coordinate with id {CommunicateId}, {CoordinateId}  deleted", communicateId, coordinateId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting communicate coordinate");
            
            
            throw new BadRequestException("Error deleting communicate coordinate");
        }
    }

    public async Task DeleteCommunicateDevice(int communicateId, int deviceId)
    {
        // await using context
        
        

        // get communicate device
        var communicateDevice = await _context.CommunicateDevices.FirstOrDefaultAsync(c => c.CommunicateId == communicateId && c.DeviceId == deviceId);
        if (communicateDevice == null)
        {
            _logger.LogWarning("Communicate device not found");
            throw new NotFoundException("Communicate device not found");
        }
        // check if CommunicateDevice is not marked as deleted
        if (communicateDevice.IsDeleted == false)
        {
            _logger.LogWarning("Communicate device is not marked as deleted");
            throw new BadRequestException("Communicate device is not marked as deleted");
        }
        // delete communicate device
        _context.CommunicateDevices.Remove(communicateDevice);
        
        try
        {
            // save changes
            await _context.SaveChangesAsync();
            
            
            _logger.LogInformation("Communicate device with id {CommunicateId}, {DeviceId}  deleted", communicateId, deviceId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting communicate device");
            
            
            throw new BadRequestException("Error deleting communicate device");
        }
    }

    public async Task DeleteCommunicateModel(int communicateId, int modelId)
    {
        // await using context
        
        

        // get communicate model
        var communicateModel = await _context.CommunicateModels.FirstOrDefaultAsync(c => c.CommunicateId == communicateId && c.ModelId == modelId);
        if (communicateModel == null)
        {
            _logger.LogWarning("Communicate model not found");
            throw new NotFoundException("Communicate model not found");
        }
        // check if CommunicateModel is not marked as deleted
        if (communicateModel.IsDeleted == false)
        {
            _logger.LogWarning("Communicate model is not marked as deleted");
            throw new BadRequestException("Communicate model is not marked as deleted");
        }
        // delete communicate model
        _context.CommunicateModels.Remove(communicateModel);
        
        try
        {
            // save changes
            await _context.SaveChangesAsync();
            
            
            _logger.LogInformation("Communicate model with id {CommunicateId}, {ModelId}  deleted", communicateId, modelId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting communicate model");
            
            
            throw new BadRequestException("Error deleting communicate model");
        }
    }

    public async Task DeleteCommunicateSpace(int communicateId, int spaceId)
    {
        // await using context
        
        

        // get communicate space
        var communicateSpace = await _context.CommunicateSpaces.FirstOrDefaultAsync(c => c.CommunicateId == communicateId && c.SpaceId == spaceId);
        if (communicateSpace == null)
        {
            _logger.LogWarning("Communicate space not found");
            throw new NotFoundException("Communicate space not found");
        }
        // check if CommunicateSpace is not marked as deleted
        if (communicateSpace.IsDeleted == false)
        {
            _logger.LogWarning("Communicate space is not marked as deleted");
            throw new BadRequestException("Communicate space is not marked as deleted");
        }
        // delete communicate space
        _context.CommunicateSpaces.Remove(communicateSpace);
        
        try
        {
            // save changes
            await _context.SaveChangesAsync();
            
            
            _logger.LogInformation("Communicate space with id {CommunicateId}, {SpaceId}  deleted", communicateId, spaceId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting communicate space");
            
            
            throw new BadRequestException("Error deleting communicate space");
        }
    }

    public async Task<CommunicateDto> GetCommunicateById(int communicateId)
    {
        // await using context
        

        // get communicate
        var communicate = await _context.Communicates
            .AsNoTracking()
            .Select(c => new CommunicateDto
            {
                CommunicateId = c.CommunicateId,
                Name = c.Name,
                Description = c.Description,
                IsDeleted = c.IsDeleted,
                UserId = c.UpdatedBy

            })
            .FirstOrDefaultAsync(c => c.CommunicateId == communicateId);
        if (communicate == null)
        {
            _logger.LogWarning("Communicate not found");
            throw new NotFoundException("Communicate not found");
        }
        // return communicate
        _logger.LogInformation("Communicate with id {CommunicateId} returned", communicateId);
        return communicate;
    }

    public async Task<IEnumerable<CommunicateDto>> GetCommunicates()
    {
        // await using context
        

        // get communicates
        var communicates = await _context.Communicates
            .AsNoTracking()
            .Select(c => new CommunicateDto
            {
                CommunicateId = c.CommunicateId,
                Name = c.Name,
                Description = c.Description,
                IsDeleted = c.IsDeleted,
                UserId = c.UpdatedBy
            })
            .ToListAsync();
        if (communicates is null)
        {
            _logger.LogWarning("Communicates not found");
            throw new NotFoundException("Communicates not found");
        }

        // return communicates
        _logger.LogInformation("Communicates returned");
        return communicates;
    }

    public async Task<IEnumerable<CommunicateWithAssetsDto>> GetCommunicatesWithAssets()
    {
        // await using context
        

        // get communicates
        var communicates = await _context.Communicates
            .AsNoTracking()
            .Select(c => new CommunicateWithAssetsDto
            {
                CommunicateId = c.CommunicateId,
                Name = c.Name,
                Description = c.Description,
                IsDeleted = c.IsDeleted,
                UserId = c.UpdatedBy,
                Assets = c.CommunicateAssets.Select(a => new AssetDto
                {
                    AssetId = a.AssetId,
                    Name = a.Asset.Name,
                    Description = a.Asset.Description,
                    IsDeleted = a.Asset.IsDeleted,
                    UserId = a.Asset.UpdatedBy
                }).ToList()
            })
            .ToListAsync();
        if (communicates is null)
        {
            _logger.LogWarning("Communicates not found");
            throw new NotFoundException("Communicates not found");
        }
        // return communicates
        _logger.LogInformation("Communicates returned");
        return communicates;
    }

    public async Task<SituationDto> GetSituationById(int situationId)
    {
        // await using context
        

        // get situation
        var situation = await _context.Situations
            .AsNoTracking()
            .Select(s => new SituationDto
            {
                SituationId = s.SituationId,
                Name = s.Name,
                Description = s.Description,
                IsDeleted = s.IsDeleted,
                UserId = s.UpdatedBy
            })
            .FirstOrDefaultAsync(c => c.SituationId == situationId);
        if (situation == null)
        {
            _logger.LogWarning("Situation not found");
            throw new NotFoundException("Situation not found");
        }
        // return situation
        _logger.LogInformation("Situation with id {SituationId} returned", situationId);
        return situation;
    }

    public async Task MarkDeleteCommunicate(int communicateId)
    {
        
        // await using context
        
        
        
        try
        {
            // get communicate
            var communicate = await _context.Communicates
                .Include(c => c.CommunicateAssets)
                .FirstOrDefaultAsync(c => c.CommunicateId == communicateId);
            // if communicate not found
            if (communicate == null)
            {
                _logger.LogWarning("Communicate with id {CommunicateId} not found", communicateId);
                throw new NotFoundException($"Communicate with id {communicateId} not found");
            }
            // if communicate is already deleted
            if (communicate.IsDeleted)
            {
                _logger.LogWarning("Communicate with id {CommunicateId} is already deleted", communicateId);
                throw new BadRequestException($"Communicate with id {communicateId} is already deleted");
            }
            // check if communicate has CommunicateAssets with IsDeleted = false
            if (communicate.CommunicateAssets.Any(ca => ca.IsDeleted == false))
            {
                _logger.LogWarning("Communicate with id {CommunicateId} has CommunicateAssets with IsDeleted = false", communicateId);
                throw new BadRequestException($"Communicate with id {communicateId} has CommunicateAssets with IsDeleted = false");
            }
            
            // mark communicate as deleted
            communicate.IsDeleted = true;
            
            // save changes
            await _context.SaveChangesAsync();
            
            
            // return success
            _logger.LogInformation("Communicate with id {CommunicateId} marked as deleted", communicateId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking communicate with id {CommunicateId} as deleted", communicateId);
            
            
            throw new BadRequestException($"Error marking communicate with id {communicateId} as deleted");
        }
    }

    public async Task MarkDeleteCommunicateArea(int communicateId, int areaId)
    {
        
        // await using context
        
        
        
        try
        {
            // get communicateArea
            var communicateArea = await _context.CommunicateAreas.FindAsync(communicateId, areaId);
            if (communicateArea == null)
            {
                _logger.LogWarning("CommunicateArea not found");
                throw new NotFoundException("CommunicateArea not found");
            }
            if (communicateArea.IsDeleted)
            {
                _logger.LogWarning("CommunicateArea already marked as deleted");
                throw new BadRequestException("CommunicateArea already marked as deleted");
            }

            communicateArea.IsDeleted = true;
            _context.Update(communicateArea);
            // save changes
            await _context.SaveChangesAsync();
            
            _logger.LogInformation("CommunicateArea with id {CommunicateId}, {AreaId} marked as deleted", communicateId, areaId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking communicateArea with id {CommunicateId}, {AreaId} as deleted", communicateId, areaId);
            
            throw new BadRequestException($"Error marking communicateArea with id {communicateId}, {areaId} as deleted");
        }
    }

    public async Task MarkDeleteCommunicateAsset(int communicateId, int assetId)
    {
        
        // await using context
        
        
        
        try
        {
            // get communicateAsset
            var communicateAsset = await _context.CommunicateAssets.FindAsync(communicateId, assetId);
            if (communicateAsset == null)
            {
                _logger.LogWarning("CommunicateAsset not found");
                throw new NotFoundException("CommunicateAsset not found");
            }
            if (communicateAsset.IsDeleted)
            {
                _logger.LogWarning("CommunicateAsset already marked as deleted");
                throw new BadRequestException("CommunicateAsset already marked as deleted");
            }

            communicateAsset.IsDeleted = true;
            _context.Update(communicateAsset);
            // save changes
            await _context.SaveChangesAsync();
            
            _logger.LogInformation("CommunicateAsset with id {CommunicateId}, {AssetId} marked as deleted", communicateId, assetId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking communicateAsset with id {CommunicateId}, {AssetId} as deleted", communicateId, assetId);
            
            throw new BadRequestException($"Error marking communicateAsset with id {communicateId}, {assetId} as deleted");
        }
    }

    public async Task MarkDeleteCommunicateCategory(int communicateId, int categoryId)
    {
        
        // await using context
        
        
        
        try
        {
            // get communicateCategory
            var communicateCategory = await _context.CommunicateCategories.FindAsync(communicateId, categoryId);
            if (communicateCategory == null)
            {
                _logger.LogWarning("CommunicateCategory not found");
                throw new NotFoundException("CommunicateCategory not found");
            }
            if (communicateCategory.IsDeleted)
            {
                _logger.LogWarning("CommunicateCategory already marked as deleted");
                throw new BadRequestException("CommunicateCategory already marked as deleted");
            }

            communicateCategory.IsDeleted = true;
            _context.Update(communicateCategory);
            // save changes
            await _context.SaveChangesAsync();
            
            _logger.LogInformation("CommunicateCategory with id {CommunicateId}, {CategoryId} marked as deleted", communicateId, categoryId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking communicateCategory with id {CommunicateId}, {CategoryId} as deleted", communicateId, categoryId);
            
            throw new BadRequestException($"Error marking communicateCategory with id {communicateId}, {categoryId} as deleted");
        }
    }

    public async Task MarkDeleteCommunicateCoordinate(int communicateId, int coordinateId)
    {
        
        // await using context
        
        
        
        try
        {
            // get communicateCoordinate
            var communicateCoordinate = await _context.CommunicateCoordinates.FindAsync(communicateId, coordinateId);
            if (communicateCoordinate == null)
            {
                _logger.LogWarning("CommunicateCoordinate not found");
                throw new NotFoundException("CommunicateCoordinate not found");
            }
            if (communicateCoordinate.IsDeleted)
            {
                _logger.LogWarning("CommunicateCoordinate already marked as deleted");
                throw new BadRequestException("CommunicateCoordinate already marked as deleted");
            }

            communicateCoordinate.IsDeleted = true;
            _context.Update(communicateCoordinate);
            // save changes
            await _context.SaveChangesAsync();
            
            _logger.LogInformation("CommunicateCoordinate with id {CommunicateId}, {CoordinateId} marked as deleted", communicateId, coordinateId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking communicateCoordinate with id {CommunicateId}, {CoordinateId} as deleted", communicateId, coordinateId);
            
            throw new BadRequestException($"Error marking communicateCoordinate with id {communicateId}, {coordinateId} as deleted");
        }
    }

    public async Task MarkDeleteCommunicateDevice(int communicateId, int deviceId)
    {
        
        // await using context
        
        
        
        try
        {
            // get communicateDevice
            var communicateDevice = await _context.CommunicateDevices.FindAsync(communicateId, deviceId);
            if (communicateDevice == null)
            {
                _logger.LogWarning("CommunicateDevice not found");
                throw new NotFoundException("CommunicateDevice not found");
            }
            if (communicateDevice.IsDeleted)
            {
                _logger.LogWarning("CommunicateDevice already marked as deleted");
                throw new BadRequestException("CommunicateDevice already marked as deleted");
            }

            communicateDevice.IsDeleted = true;
            _context.Update(communicateDevice);
            // save changes
            await _context.SaveChangesAsync();
            
            _logger.LogInformation("CommunicateDevice with id {CommunicateId}, {DeviceId} marked as deleted", communicateId, deviceId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking communicateDevice with id {CommunicateId}, {DeviceId} as deleted", communicateId, deviceId);
            
            throw new BadRequestException($"Error marking communicateDevice with id {communicateId}, {deviceId} as deleted");
        }
    }

    public async Task MarkDeleteCommunicateModel(int communicateId, int modelId)
    {
        
        // await using context
        
        
        
        try
        {
            // get communicateModel
            var communicateModel = await _context.CommunicateModels.FindAsync(communicateId, modelId);
            if (communicateModel == null)
            {
                _logger.LogWarning("CommunicateModel not found");
                throw new NotFoundException("CommunicateModel not found");
            }
            if (communicateModel.IsDeleted)
            {
                _logger.LogWarning("CommunicateModel already marked as deleted");
                throw new BadRequestException("CommunicateModel already marked as deleted");
            }

            communicateModel.IsDeleted = true;
            _context.Update(communicateModel);
            // save changes
            await _context.SaveChangesAsync();
            
            _logger.LogInformation("CommunicateModel with id {CommunicateId}, {ModelId} marked as deleted", communicateId, modelId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking communicateModel with id {CommunicateId}, {ModelId} as deleted", communicateId, modelId);
            
            throw new BadRequestException($"Error marking communicateModel with id {communicateId}, {modelId} as deleted");
        }
    }

    public async Task MarkDeleteCommunicateSpace(int communicateId, int spaceId)
    {
        
        // await using context
        
        
        
        try
        {
            // get communicateSpace
            var communicateSpace = await _context.CommunicateSpaces.FindAsync(communicateId, spaceId);
            if (communicateSpace == null)
            {
                _logger.LogWarning("CommunicateSpace not found");
                throw new NotFoundException("CommunicateSpace not found");
            }
            if (communicateSpace.IsDeleted)
            {
                _logger.LogWarning("CommunicateSpace already marked as deleted");
                throw new BadRequestException("CommunicateSpace already marked as deleted");
            }

            communicateSpace.IsDeleted = true;
            _context.Update(communicateSpace);
            // save changes
            await _context.SaveChangesAsync();
            
            _logger.LogInformation("CommunicateSpace with id {CommunicateId}, {SpaceId} marked as deleted", communicateId, spaceId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking communicateSpace with id {CommunicateId}, {SpaceId} as deleted", communicateId, spaceId);
            
            throw new BadRequestException($"Error marking communicateSpace with id {communicateId}, {spaceId} as deleted");
        }
    }

    public async Task UpdateCommunicate(int communicateId, CommunicateUpdateDto communicateUpdateDto)
    {
        
        // await using context
        
        
        
        // try-catch
        try
        {
            var communicate = await _context.Communicates.FirstOrDefaultAsync(c => c.CommunicateId == communicateId);
            if (communicate == null)
                throw new NotFoundException("Communicate not found");
            // check if communicate is not marked as deleted
            if (communicate.IsDeleted)
            {
                _logger.LogWarning("Communicate is marked as deleted");
                throw new BadRequestException("Communicate is marked as deleted");
            }
            // check if duplicate exists and is not marked as deleted
            var exists = await _context.Communicates.AnyAsync(c =>
                c.Name.ToLower().Trim() == communicateUpdateDto.Name.ToLower().Trim() && c.IsDeleted == false &&
                c.CommunicateId != communicateId);
            if (exists)
            {
                _logger.LogWarning("Communicate with name {Name} already exists", communicateUpdateDto.Name);
                throw new BadRequestException($"Communicate with name {communicateUpdateDto.Name} already exists");
            }

            // update name
            if (!Equals(communicateUpdateDto.Name.ToLower().Trim(), communicate.Name.ToLower().Trim()))
            {
                communicate.Name = communicateUpdateDto.Name;
            }

            if (!Equals(communicateUpdateDto.Description.ToLower().Trim(), communicate.Description.ToLower().Trim()))
            {
                communicate.Description = communicateUpdateDto.Description;
            }

            _context.Communicates.Update(communicate);
            await _context.SaveChangesAsync();
            
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating communicate");
            
            throw new BadRequestException("Error updating communicate");
        }
    }

    public async Task UpdateCommunicateArea(int communicateId, int areaId)
    {
        
        // await using context
        
        // get communicateArea
        var communicateArea = await _context.CommunicateAreas.FindAsync(communicateId, areaId);
        if (communicateArea == null)
        {
            _logger.LogWarning("CommunicateArea not found");
            throw new NotFoundException("CommunicateArea not found");
        }
        if (!communicateArea.IsDeleted)
            throw new BadRequestException("CommunicateArea not marked as deleted");
        var communicate = await _context.Communicates.FindAsync(communicateId);
        if (communicate == null || communicate.IsDeleted)
        {
            _logger.LogWarning("Communicate not found");
            throw new NotFoundException("Communicate not found");
        }
        var area = await _context.Areas.FindAsync(areaId);
        if (area == null || area.IsDeleted)
        {
            _logger.LogWarning("Area not found");
            throw new NotFoundException("Area not found");
        }
        communicateArea.IsDeleted = false;
        // save changes
        
        try
        {
            await _context.SaveChangesAsync();
            
        }
        catch (Exception ex)
        {
            
            _logger.LogError(ex, "Error updating communicateArea");
            throw new BadRequestException("Error while saving changes");
        }
    }

    public async Task UpdateCommunicateAsset(int communicateId, int assetId)
    {
        
        // await using context
        
        // get communicateAsset
        var communicateAsset = await _context.CommunicateAssets.FindAsync(communicateId, assetId);
        if (communicateAsset == null)
        {
            _logger.LogWarning("CommunicateAsset not found");
            throw new NotFoundException("CommunicateAsset not found");
        }
        if (!communicateAsset.IsDeleted)
            throw new BadRequestException("CommunicateAsset not marked as deleted");
        var communicate = await _context.Communicates.FindAsync(communicateId);
        if (communicate == null || communicate.IsDeleted)
        {
            _logger.LogWarning("Communicate not found");
            throw new NotFoundException("Communicate not found");
        }
        var asset = await _context.Assets.FindAsync(assetId);
        if (asset == null || asset.IsDeleted)
        {
            _logger.LogWarning("Asset not found");
            throw new NotFoundException("Asset not found");
        }
        communicateAsset.IsDeleted = false;
        // save changes
        
        try
        {
            await _context.SaveChangesAsync();
            
        }
        catch (Exception ex)
        {
            
            _logger.LogError(ex, "Error updating communicateAsset");
            throw new BadRequestException("Error while saving changes");
        }
    }

    public async Task UpdateCommunicateCategory(int communicateId, int categoryId)
    {
        
        // await using context
        
        // get communicateCategory
        var communicateCategory = await _context.CommunicateCategories.FindAsync(communicateId, categoryId);
        if (communicateCategory == null)
        {
            _logger.LogWarning("CommunicateCategory not found");
            throw new NotFoundException("CommunicateCategory not found");
        }
        if (!communicateCategory.IsDeleted)
            throw new BadRequestException("CommunicateCategory not marked as deleted");
        var communicate = await _context.Communicates.FindAsync(communicateId);
        if (communicate == null || communicate.IsDeleted)
        {
            _logger.LogWarning("Communicate not found");
            throw new NotFoundException("Communicate not found");
        }
        var category = await _context.Categories.FindAsync(categoryId);
        if (category == null || category.IsDeleted)
        {
            _logger.LogWarning("Category not found");
            throw new NotFoundException("Category not found");
        }
        communicateCategory.IsDeleted = false;
        // save changes
        
        try
        {
            await _context.SaveChangesAsync();
            
        }
        catch (Exception ex)
        {
            
            _logger.LogError(ex, "Error updating communicateCategory");
            throw new BadRequestException("Error while saving changes");
        }
    }

    public async Task UpdateCommunicateCoordinate(int communicateId, int coordinateId)
    {
        
        // await using context
        
        // get communicateCoordinate
        var communicateCoordinate = await _context.CommunicateCoordinates.FindAsync(communicateId, coordinateId);
        if (communicateCoordinate == null)
        {
            _logger.LogWarning("CommunicateCoordinate not found");
            throw new NotFoundException("CommunicateCoordinate not found");
        }
        if (!communicateCoordinate.IsDeleted)
            throw new BadRequestException("CommunicateCoordinate not marked as deleted");
        var communicate = await _context.Communicates.FindAsync(communicateId);
        if (communicate == null || communicate.IsDeleted)
        {
            _logger.LogWarning("Communicate not found");
            throw new NotFoundException("Communicate not found");
        }
        var coordinate = await _context.Coordinates.FindAsync(coordinateId);
        if (coordinate == null || coordinate.IsDeleted)
        {
            _logger.LogWarning("Coordinate not found");
            throw new NotFoundException("Coordinate not found");
        }
        communicateCoordinate.IsDeleted = false;
        // save changes
        
        try
        {
            await _context.SaveChangesAsync();
            
        }
        catch (Exception ex)
        {
            
            _logger.LogError(ex, "Error updating communicateCoordinate");
            throw new BadRequestException("Error while saving changes");
        }
    }

    public async Task UpdateCommunicateDevice(int communicateId, int deviceId)
    {
        
        // await using context
        
        // get communicateDevice
        var communicateDevice = await _context.CommunicateDevices.FindAsync(communicateId, deviceId);
        if (communicateDevice == null)
        {
            _logger.LogWarning("CommunicateDevice not found");
            throw new NotFoundException("CommunicateDevice not found");
        }
        if (!communicateDevice.IsDeleted)
            throw new BadRequestException("CommunicateDevice not marked as deleted");
        var communicate = await _context.Communicates.FindAsync(communicateId);
        if (communicate == null || communicate.IsDeleted)
        {
            _logger.LogWarning("Communicate not found");
            throw new NotFoundException("Communicate not found");
        }
        var device = await _context.Devices.FindAsync(deviceId);
        if (device == null || device.IsDeleted)
        {
            _logger.LogWarning("Device not found");
            throw new NotFoundException("Device not found");
        }
        communicateDevice.IsDeleted = false;
        // save changes
        
        try
        {
            await _context.SaveChangesAsync();
            
        }
        catch (Exception ex)
        {
            
            _logger.LogError(ex, "Error updating communicateDevice");
            throw new BadRequestException("Error while saving changes");
        }
    }

    public async Task UpdateCommunicateModel(int communicateId, int modelId)
    {
        
        // await using context
        
        // get communicateModel
        var communicateModel = await _context.CommunicateModels.FindAsync(communicateId, modelId);
        if (communicateModel == null)
        {
            _logger.LogWarning("CommunicateModel not found");
            throw new NotFoundException("CommunicateModel not found");
        }
        if (!communicateModel.IsDeleted)
            throw new BadRequestException("CommunicateModel not marked as deleted");
        var communicate = await _context.Communicates.FindAsync(communicateId);
        if (communicate == null || communicate.IsDeleted)
        {
            _logger.LogWarning("Communicate not found");
            throw new NotFoundException("Communicate not found");
        }
        var model = await _context.Models.FindAsync(modelId);
        if (model == null || model.IsDeleted)
        {
            _logger.LogWarning("Model not found");
            throw new NotFoundException("Model not found");
        }
        communicateModel.IsDeleted = false;
        // save changes
        
        try
        {
            await _context.SaveChangesAsync();
            
        }
        catch (Exception ex)
        {
            
            _logger.LogError(ex, "Error updating communicateModel");
            throw new BadRequestException("Error while saving changes");
        }
    }

    public async Task UpdateCommunicateSpace(int communicateId, int spaceId)
    {
        
        // await using context
        
        // get communicateSpace
        var communicateSpace = await _context.CommunicateSpaces.FindAsync(communicateId, spaceId);
        if (communicateSpace == null)
        {
            _logger.LogWarning("CommunicateSpace not found");
            throw new NotFoundException("CommunicateSpace not found");
        }
        if (!communicateSpace.IsDeleted)
            throw new BadRequestException("CommunicateSpace not marked as deleted");
        var communicate = await _context.Communicates.FindAsync(communicateId);
        if (communicate == null || communicate.IsDeleted)
        {
            _logger.LogWarning("Communicate not found");
            throw new NotFoundException("Communicate not found");
        }
        var space = await _context.Spaces.FindAsync(spaceId);
        if (space == null || space.IsDeleted)
        {
            _logger.LogWarning("Space not found");
            throw new NotFoundException("Space not found");
        }
        communicateSpace.IsDeleted = false;
        // save changes
        
        try
        {
            await _context.SaveChangesAsync();
            
        }
        catch (Exception ex)
        {
            
            _logger.LogError(ex, "Error updating communicateSpace");
            throw new BadRequestException("Error while saving changes");
        }
    }
}