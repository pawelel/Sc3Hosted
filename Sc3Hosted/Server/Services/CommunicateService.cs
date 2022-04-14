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
    private readonly IDbContextFactory<Sc3HostedDbContext> _contextFactory;
    private readonly ILogger<CommunicateService> _logger;
    private readonly IUserContextService _userContextService;
    public CommunicateService(IDbContextFactory<Sc3HostedDbContext> contextFactory, ILogger<CommunicateService> logger, IUserContextService userContextService)
    {
        _contextFactory = contextFactory;
        _logger = logger;
        _userContextService = userContextService;
    }

    public async Task<int> CreateCommunicate(CommunicateCreateDto communicateCreateDto)
    {
        var userId = _userContextService.UserId;
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();

        // validate category name
        var duplicate = await context.Communicates.AnyAsync(c => c.Name.ToLower().Trim() == communicateCreateDto.Name.ToLower().Trim());
        if (duplicate)
        {
            _logger.LogWarning("Communicate name already exists");
            throw new BadRequestException("Communicate name already exists");
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
        // await using transaction
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            // save changes
            await context.SaveChangesAsync();
            // commit transaction
            await transaction.CommitAsync();
            _logger.LogInformation("Communicate with id {CommunicateId} created", communicate.CommunicateId);
            return communicate.CommunicateId;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating communicate");
            // rollback transaction
            await transaction.RollbackAsync();
            throw new BadRequestException("Error creating communicate");
        }
    }

    public async Task<(int, int)> CreateCommunicateArea(int communicateId, int areaId)
    {
        var userId = _userContextService.UserId;
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();
        // get communicateArea
        var communicateArea = await context.CommunicateAreas.FindAsync(communicateId, areaId);
        if (communicateArea != null)
            throw new BadRequestException("CommunicateArea already exists");
        var area = await context.Areas.FindAsync(areaId);
        if (area == null || area.IsDeleted)
        {
            _logger.LogWarning("Area not found");
            throw new NotFoundException("Area not found");
        }
        var communicate = await context.Communicates.FindAsync(communicateId);
        if (communicate == null || communicate.IsDeleted)
        {
            _logger.LogWarning("Communicate not found");
            throw new NotFoundException("Communicate not found");
        }
        communicateArea = new CommunicateArea
        {
            CommunicateId = communicateId,
            AreaId = areaId,
            UserId = userId,
            IsDeleted = false
        };
        context.Add(communicateArea);
        // save changes
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            await context.SaveChangesAsync();
            await transaction.CommitAsync();
            return (communicateId, areaId);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Error creating communicateArea");
            throw new BadRequestException("Error while saving changes");
        }
    }

    public async Task<(int, int)> CreateCommunicateAsset(int communicateId, int assetId)
    {
        var userId = _userContextService.UserId;
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();
        // get communicateAsset
        var communicateAsset = await context.CommunicateAssets.FindAsync(communicateId, assetId);
        if (communicateAsset != null)
            throw new BadRequestException("CommunicateAsset already exists");
        var asset = await context.Assets.FindAsync(assetId);
        if (asset == null || asset.IsDeleted)
        {
            _logger.LogWarning("Asset not found");
            throw new NotFoundException("Asset not found");
        }
        var communicate = await context.Communicates.FindAsync(communicateId);
        if (communicate == null || communicate.IsDeleted)
        {
            _logger.LogWarning("Communicate not found");
            throw new NotFoundException("Communicate not found");
        }
        communicateAsset = new CommunicateAsset
        {
            CommunicateId = communicateId,
            AssetId = assetId,
            UserId = userId,
            IsDeleted = false
        };
        context.Add(communicateAsset);
        // save changes
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            await context.SaveChangesAsync();
            await transaction.CommitAsync();
            return (communicateId, assetId);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Error creating communicateAsset");
            throw new BadRequestException("Error while saving changes");
        }
    }

    public async Task<(int, int)> CreateCommunicateCategory(int communicateId, int categoryId)
    {
        var userId = _userContextService.UserId;
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();
        // get communicateCategory
        var communicateCategory = await context.CommunicateCategories.FindAsync(communicateId, categoryId);
        if (communicateCategory != null)
            throw new BadRequestException("CommunicateCategory already exists");
        var communicate = await context.Communicates.FindAsync(communicateId);
        if (communicate == null || communicate.IsDeleted)
        {
            _logger.LogWarning("Communicate not found");
            throw new NotFoundException("Communicate not found");
        }
        var category = await context.Categories.FindAsync(categoryId);
        if (category == null || category.IsDeleted)
        {
            _logger.LogWarning("Category not found");
            throw new NotFoundException("Category not found");
        }
        communicateCategory = new CommunicateCategory
        {
            CommunicateId = communicateId,
            CategoryId = categoryId,
            UserId = userId,
            IsDeleted = false
        };
        context.Add(communicateCategory);
        // save changes
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            await context.SaveChangesAsync();
            await transaction.CommitAsync();
            return (communicateId, categoryId);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Error creating communicateCategory");
            throw new BadRequestException("Error while saving changes");
        }
    }

    public async Task<(int, int)> CreateCommunicateCoordinate(int communicateId, int coordinateId)
    {
        var userId = _userContextService.UserId;
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();
        // get communicateCoordinate
        var communicateCoordinate = await context.CommunicateCoordinates.FindAsync(communicateId, coordinateId);
        if (communicateCoordinate != null)
            throw new BadRequestException("CommunicateCoordinate already exists");
        var communicate = await context.Communicates.FindAsync(communicateId);
        if (communicate == null || communicate.IsDeleted)
        {
            _logger.LogWarning("Communicate not found");
            throw new NotFoundException("Communicate not found");
        }
        var coordinate = await context.Coordinates.FindAsync(coordinateId);
        if (coordinate == null || coordinate.IsDeleted)
        {
            _logger.LogWarning("Coordinate not found");
            throw new NotFoundException("Coordinate not found");
        }
        communicateCoordinate = new CommunicateCoordinate
        {
            CommunicateId = communicateId,
            CoordinateId = coordinateId,
            UserId = userId,
            IsDeleted = false
        };
        context.Add(communicateCoordinate);
        // save changes
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            await context.SaveChangesAsync();
            await transaction.CommitAsync();
            return (communicateId, coordinateId);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Error creating communicateCoordinate");
            throw new BadRequestException("Error while saving changes");
        }
    }

    public async Task<(int, int)> CreateCommunicateDevice(int communicateId, int deviceId)
    {
        var userId = _userContextService.UserId;
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();
        // get communicateDevice
        var communicateDevice = await context.CommunicateDevices.FindAsync(communicateId, deviceId);
        if (communicateDevice != null)
            throw new BadRequestException("CommunicateDevice already exists");
        var communicate = await context.Communicates.FindAsync(communicateId);
        if (communicate == null || communicate.IsDeleted)
        {
            _logger.LogWarning("Communicate not found");
            throw new NotFoundException("Communicate not found");
        }
        var device = await context.Devices.FindAsync(deviceId);
        if (device == null || device.IsDeleted)
        {
            _logger.LogWarning("Device not found");
            throw new NotFoundException("Device not found");
        }
        communicateDevice = new CommunicateDevice
        {
            CommunicateId = communicateId,
            DeviceId = deviceId,
            UserId = userId,
            IsDeleted = false
        };
        context.Add(communicateDevice);
        // save changes
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            await context.SaveChangesAsync();
            await transaction.CommitAsync();
            return (communicateId, deviceId);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Error creating communicateDevice");
            throw new BadRequestException("Error while saving changes");
        }
    }

    public async Task<(int, int)> CreateCommunicateModel(int communicateId, int modelId)
    {
        var userId = _userContextService.UserId;
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();
        // get communicateModel
        var communicateModel = await context.CommunicateModels.FindAsync(communicateId, modelId);
        if (communicateModel != null)
            throw new BadRequestException("CommunicateModel already exists");
        var communicate = await context.Communicates.FindAsync(communicateId);
        if (communicate == null || communicate.IsDeleted)
        {
            _logger.LogWarning("Communicate not found");
            throw new NotFoundException("Communicate not found");
        }
        var model = await context.Models.FindAsync(modelId);
        if (model == null || model.IsDeleted)
        {
            _logger.LogWarning("Model not found");
            throw new NotFoundException("Model not found");
        }
        communicateModel = new CommunicateModel
        {
            CommunicateId = communicateId,
            ModelId = modelId,
            UserId = userId,
            IsDeleted = false
        };
        context.Add(communicateModel);
        // save changes
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            await context.SaveChangesAsync();
            await transaction.CommitAsync();
            return (communicateId, modelId);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Error creating communicateModel");
            throw new BadRequestException("Error while saving changes");
        }
    }

    public async Task<(int, int)> CreateCommunicateSpace(int communicateId, int spaceId)
    {
        var userId = _userContextService.UserId;
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();
        // get communicateSpace
        var communicateSpace = await context.CommunicateSpaces.FindAsync(communicateId, spaceId);
        if (communicateSpace != null)
            throw new BadRequestException("CommunicateSpace already exists");
        var communicate = await context.Communicates.FindAsync(communicateId);
        if (communicate == null || communicate.IsDeleted)
        {
            _logger.LogWarning("Communicate not found");
            throw new NotFoundException("Communicate not found");
        }
        var space = await context.Spaces.FindAsync(spaceId);
        if (space == null || space.IsDeleted)
        {
            _logger.LogWarning("Space not found");
            throw new NotFoundException("Space not found");
        }
        communicateSpace = new CommunicateSpace
        {
            CommunicateId = communicateId,
            SpaceId = spaceId,
            UserId = userId,
            IsDeleted = false
        };
        context.Add(communicateSpace);
        // save changes
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            await context.SaveChangesAsync();
            await transaction.CommitAsync();
            return (communicateId, spaceId);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Error creating communicateSpace");
            throw new BadRequestException("Error while saving changes");
        }
    }

    public async Task DeleteCommunicate(int communicateId)
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();

        // get communicate
        var communicate = await context.Communicates.FindAsync(communicateId);
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
        context.Communicates.Remove(communicate);
        // await using transaction
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            // save changes
            await context.SaveChangesAsync();
            // commit transaction
            await transaction.CommitAsync();
            _logger.LogInformation("Communicate with id {CommunicateId} deleted", communicate.CommunicateId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting communicate");
            // rollback transaction
            await transaction.RollbackAsync();
            throw new BadRequestException("Error deleting communicate");
        }
    }

    public async Task DeleteCommunicateArea(int communicateId, int areaId)
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();

        // get communicate area
        var communicateArea = await context.CommunicateAreas.FirstOrDefaultAsync(c => c.CommunicateId == communicateId && c.AreaId == areaId);
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
        context.CommunicateAreas.Remove(communicateArea);
        // await using transaction
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            // save changes
            await context.SaveChangesAsync();
            // commit transaction
            await transaction.CommitAsync();
            _logger.LogInformation("Communicate area with id {CommunicateId}, {AreaId}  deleted", communicateId, areaId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting communicate area");
            // rollback transaction
            await transaction.RollbackAsync();
            throw new BadRequestException("Error deleting communicate area");
        }
    }

    public async Task DeleteCommunicateAsset(int communicateId, int assetId)
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();

        // get communicate asset
        var communicateAsset = await context.CommunicateAssets.FirstOrDefaultAsync(c => c.CommunicateId == communicateId && c.AssetId == assetId);
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
        context.CommunicateAssets.Remove(communicateAsset);
        // await using transaction
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            // save changes
            await context.SaveChangesAsync();
            // commit transaction
            await transaction.CommitAsync();
            _logger.LogInformation("Communicate asset with id {CommunicateId}, {AssetId}  deleted", communicateId, assetId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting communicate asset");
            // rollback transaction
            await transaction.RollbackAsync();
            throw new BadRequestException("Error deleting communicate asset");
        }
    }

    public async Task DeleteCommunicateCategory(int communicateId, int categoryId)
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();

        // get communicate category
        var communicateCategory = await context.CommunicateCategories.FirstOrDefaultAsync(c => c.CommunicateId == communicateId && c.CategoryId == categoryId);
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
        context.CommunicateCategories.Remove(communicateCategory);
        // await using transaction
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            // save changes
            await context.SaveChangesAsync();
            // commit transaction
            await transaction.CommitAsync();
            _logger.LogInformation("Communicate category with id {CommunicateId}, {CategoryId}  deleted", communicateId, categoryId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting communicate category");
            // rollback transaction
            await transaction.RollbackAsync();
            throw new BadRequestException("Error deleting communicate category");
        }
    }

    public async Task DeleteCommunicateCoordinate(int communicateId, int coordinateId)
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();
        // await using transaction

        // get communicate coordinate
        var communicateCoordinate = await context.CommunicateCoordinates.FirstOrDefaultAsync(c => c.CommunicateId == communicateId && c.CoordinateId == coordinateId);
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
        context.CommunicateCoordinates.Remove(communicateCoordinate);
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            // save changes
            await context.SaveChangesAsync();
            // commit transaction
            await transaction.CommitAsync();
            _logger.LogInformation("Communicate coordinate with id {CommunicateId}, {CoordinateId}  deleted", communicateId, coordinateId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting communicate coordinate");
            // rollback transaction
            await transaction.RollbackAsync();
            throw new BadRequestException("Error deleting communicate coordinate");
        }
    }

    public async Task DeleteCommunicateDevice(int communicateId, int deviceId)
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();
        // await using transaction

        // get communicate device
        var communicateDevice = await context.CommunicateDevices.FirstOrDefaultAsync(c => c.CommunicateId == communicateId && c.DeviceId == deviceId);
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
        context.CommunicateDevices.Remove(communicateDevice);
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            // save changes
            await context.SaveChangesAsync();
            // commit transaction
            await transaction.CommitAsync();
            _logger.LogInformation("Communicate device with id {CommunicateId}, {DeviceId}  deleted", communicateId, deviceId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting communicate device");
            // rollback transaction
            await transaction.RollbackAsync();
            throw new BadRequestException("Error deleting communicate device");
        }
    }

    public async Task DeleteCommunicateModel(int communicateId, int modelId)
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();
        // await using transaction

        // get communicate model
        var communicateModel = await context.CommunicateModels.FirstOrDefaultAsync(c => c.CommunicateId == communicateId && c.ModelId == modelId);
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
        context.CommunicateModels.Remove(communicateModel);
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            // save changes
            await context.SaveChangesAsync();
            // commit transaction
            await transaction.CommitAsync();
            _logger.LogInformation("Communicate model with id {CommunicateId}, {ModelId}  deleted", communicateId, modelId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting communicate model");
            // rollback transaction
            await transaction.RollbackAsync();
            throw new BadRequestException("Error deleting communicate model");
        }
    }

    public async Task DeleteCommunicateSpace(int communicateId, int spaceId)
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();
        // await using transaction

        // get communicate space
        var communicateSpace = await context.CommunicateSpaces.FirstOrDefaultAsync(c => c.CommunicateId == communicateId && c.SpaceId == spaceId);
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
        context.CommunicateSpaces.Remove(communicateSpace);
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            // save changes
            await context.SaveChangesAsync();
            // commit transaction
            await transaction.CommitAsync();
            _logger.LogInformation("Communicate space with id {CommunicateId}, {SpaceId}  deleted", communicateId, spaceId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting communicate space");
            // rollback transaction
            await transaction.RollbackAsync();
            throw new BadRequestException("Error deleting communicate space");
        }
    }

    public async Task<CommunicateDto> GetCommunicateById(int communicateId)
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();

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
            throw new NotFoundException("Communicate not found");
        }
        // return communicate
        _logger.LogInformation("Communicate with id {CommunicateId} returned", communicateId);
        return communicate;
    }

    public async Task<IEnumerable<CommunicateDto>> GetCommunicates()
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();

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
        await using var context = await _contextFactory.CreateDbContextAsync();

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
        await using var context = await _contextFactory.CreateDbContextAsync();

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
            throw new NotFoundException("Situation not found");
        }
        // return situation
        _logger.LogInformation("Situation with id {SituationId} returned", situationId);
        return situation;
    }

    public async Task MarkDeleteCommunicate(int communicateId)
    {
        var userId = _userContextService.UserId;
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
            communicate.UserId = userId;
            // save changes
            await context.SaveChangesAsync();
            // commit transaction
            await transaction.CommitAsync();
            // return success
            _logger.LogInformation("Communicate with id {CommunicateId} marked as deleted", communicateId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking communicate with id {CommunicateId} as deleted", communicateId);
            // rollback transaction
            await transaction.RollbackAsync();
            throw new BadRequestException($"Error marking communicate with id {communicateId} as deleted");
        }
    }

    public async Task MarkDeleteCommunicateArea(int communicateId, int areaId)
    {
        var userId = _userContextService.UserId;
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();
        // await using transaction
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            // get communicateArea
            var communicateArea = await context.CommunicateAreas.FindAsync(communicateId, areaId);
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

            communicateArea.UserId = userId;
            communicateArea.IsDeleted = true;
            context.Update(communicateArea);
            // save changes
            await context.SaveChangesAsync();
            await transaction.CommitAsync();
            _logger.LogInformation("CommunicateArea with id {CommunicateId}, {AreaId} marked as deleted", communicateId, areaId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking communicateArea with id {CommunicateId}, {AreaId} as deleted", communicateId, areaId);
            await transaction.RollbackAsync();
            throw new BadRequestException($"Error marking communicateArea with id {communicateId}, {areaId} as deleted");
        }
    }

    public async Task MarkDeleteCommunicateAsset(int communicateId, int assetId)
    {
        var userId = _userContextService.UserId;
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();
        // await using transaction
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            // get communicateAsset
            var communicateAsset = await context.CommunicateAssets.FindAsync(communicateId, assetId);
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

            communicateAsset.UserId = userId;
            communicateAsset.IsDeleted = true;
            context.Update(communicateAsset);
            // save changes
            await context.SaveChangesAsync();
            await transaction.CommitAsync();
            _logger.LogInformation("CommunicateAsset with id {CommunicateId}, {AssetId} marked as deleted", communicateId, assetId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking communicateAsset with id {CommunicateId}, {AssetId} as deleted", communicateId, assetId);
            await transaction.RollbackAsync();
            throw new BadRequestException($"Error marking communicateAsset with id {communicateId}, {assetId} as deleted");
        }
    }

    public async Task MarkDeleteCommunicateCategory(int communicateId, int categoryId)
    {
        var userId = _userContextService.UserId;
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();
        // await using transaction
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            // get communicateCategory
            var communicateCategory = await context.CommunicateCategories.FindAsync(communicateId, categoryId);
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

            communicateCategory.UserId = userId;
            communicateCategory.IsDeleted = true;
            context.Update(communicateCategory);
            // save changes
            await context.SaveChangesAsync();
            await transaction.CommitAsync();
            _logger.LogInformation("CommunicateCategory with id {CommunicateId}, {CategoryId} marked as deleted", communicateId, categoryId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking communicateCategory with id {CommunicateId}, {CategoryId} as deleted", communicateId, categoryId);
            await transaction.RollbackAsync();
            throw new BadRequestException($"Error marking communicateCategory with id {communicateId}, {categoryId} as deleted");
        }
    }

    public async Task MarkDeleteCommunicateCoordinate(int communicateId, int coordinateId)
    {
        var userId = _userContextService.UserId;
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();
        // await using transaction
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            // get communicateCoordinate
            var communicateCoordinate = await context.CommunicateCoordinates.FindAsync(communicateId, coordinateId);
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

            communicateCoordinate.UserId = userId;
            communicateCoordinate.IsDeleted = true;
            context.Update(communicateCoordinate);
            // save changes
            await context.SaveChangesAsync();
            await transaction.CommitAsync();
            _logger.LogInformation("CommunicateCoordinate with id {CommunicateId}, {CoordinateId} marked as deleted", communicateId, coordinateId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking communicateCoordinate with id {CommunicateId}, {CoordinateId} as deleted", communicateId, coordinateId);
            await transaction.RollbackAsync();
            throw new BadRequestException($"Error marking communicateCoordinate with id {communicateId}, {coordinateId} as deleted");
        }
    }

    public async Task MarkDeleteCommunicateDevice(int communicateId, int deviceId)
    {
        var userId = _userContextService.UserId;
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();
        // await using transaction
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            // get communicateDevice
            var communicateDevice = await context.CommunicateDevices.FindAsync(communicateId, deviceId);
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

            communicateDevice.UserId = userId;
            communicateDevice.IsDeleted = true;
            context.Update(communicateDevice);
            // save changes
            await context.SaveChangesAsync();
            await transaction.CommitAsync();
            _logger.LogInformation("CommunicateDevice with id {CommunicateId}, {DeviceId} marked as deleted", communicateId, deviceId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking communicateDevice with id {CommunicateId}, {DeviceId} as deleted", communicateId, deviceId);
            await transaction.RollbackAsync();
            throw new BadRequestException($"Error marking communicateDevice with id {communicateId}, {deviceId} as deleted");
        }
    }

    public async Task MarkDeleteCommunicateModel(int communicateId, int modelId)
    {
        var userId = _userContextService.UserId;
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();
        // await using transaction
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            // get communicateModel
            var communicateModel = await context.CommunicateModels.FindAsync(communicateId, modelId);
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

            communicateModel.UserId = userId;
            communicateModel.IsDeleted = true;
            context.Update(communicateModel);
            // save changes
            await context.SaveChangesAsync();
            await transaction.CommitAsync();
            _logger.LogInformation("CommunicateModel with id {CommunicateId}, {ModelId} marked as deleted", communicateId, modelId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking communicateModel with id {CommunicateId}, {ModelId} as deleted", communicateId, modelId);
            await transaction.RollbackAsync();
            throw new BadRequestException($"Error marking communicateModel with id {communicateId}, {modelId} as deleted");
        }
    }

    public async Task MarkDeleteCommunicateSpace(int communicateId, int spaceId)
    {
        var userId = _userContextService.UserId;
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();
        // await using transaction
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            // get communicateSpace
            var communicateSpace = await context.CommunicateSpaces.FindAsync(communicateId, spaceId);
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

            communicateSpace.UserId = userId;
            communicateSpace.IsDeleted = true;
            context.Update(communicateSpace);
            // save changes
            await context.SaveChangesAsync();
            await transaction.CommitAsync();
            _logger.LogInformation("CommunicateSpace with id {CommunicateId}, {SpaceId} marked as deleted", communicateId, spaceId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking communicateSpace with id {CommunicateId}, {SpaceId} as deleted", communicateId, spaceId);
            await transaction.RollbackAsync();
            throw new BadRequestException($"Error marking communicateSpace with id {communicateId}, {spaceId} as deleted");
        }
    }

    public async Task UpdateCommunicate(int communicateId, CommunicateUpdateDto communicateUpdateDto)
    {
        var userId = _userContextService.UserId;
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();
        // await using transaction
        await using var transaction = await context.Database.BeginTransactionAsync();
        // try-catch
        try
        {
            var communicate = await context.Communicates.FirstOrDefaultAsync(c => c.CommunicateId == communicateId);
            if (communicate == null)
                throw new NotFoundException("Communicate not found");
            // check if communicate is not marked as deleted
            if (communicate.IsDeleted)
            {
                _logger.LogWarning("Communicate is marked as deleted");
                throw new BadRequestException("Communicate is marked as deleted");
            }
            // check if duplicate exists and is not marked as deleted
            var exists = await context.Communicates.AnyAsync(c =>
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
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating communicate");
            await transaction.RollbackAsync();
            throw new BadRequestException("Error updating communicate");
        }
    }

    public async Task UpdateCommunicateArea(int communicateId, int areaId)
    {
        var userId = _userContextService.UserId;
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();
        // get communicateArea
        var communicateArea = await context.CommunicateAreas.FindAsync(communicateId, areaId);
        if (communicateArea == null)
        {
            _logger.LogWarning("CommunicateArea not found");
            throw new NotFoundException("CommunicateArea not found");
        }
        if (!communicateArea.IsDeleted)
            throw new BadRequestException("CommunicateArea not marked as deleted");
        var communicate = await context.Communicates.FindAsync(communicateId);
        if (communicate == null || communicate.IsDeleted)
        {
            _logger.LogWarning("Communicate not found");
            throw new NotFoundException("Communicate not found");
        }
        var area = await context.Areas.FindAsync(areaId);
        if (area == null || area.IsDeleted)
        {
            _logger.LogWarning("Area not found");
            throw new NotFoundException("Area not found");
        }
        communicateArea.UserId = userId;
        communicateArea.IsDeleted = false;
        // save changes
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            await context.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Error updating communicateArea");
            throw new BadRequestException("Error while saving changes");
        }
    }

    public async Task UpdateCommunicateAsset(int communicateId, int assetId)
    {
        var userId = _userContextService.UserId;
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();
        // get communicateAsset
        var communicateAsset = await context.CommunicateAssets.FindAsync(communicateId, assetId);
        if (communicateAsset == null)
        {
            _logger.LogWarning("CommunicateAsset not found");
            throw new NotFoundException("CommunicateAsset not found");
        }
        if (!communicateAsset.IsDeleted)
            throw new BadRequestException("CommunicateAsset not marked as deleted");
        var communicate = await context.Communicates.FindAsync(communicateId);
        if (communicate == null || communicate.IsDeleted)
        {
            _logger.LogWarning("Communicate not found");
            throw new NotFoundException("Communicate not found");
        }
        var asset = await context.Assets.FindAsync(assetId);
        if (asset == null || asset.IsDeleted)
        {
            _logger.LogWarning("Asset not found");
            throw new NotFoundException("Asset not found");
        }
        communicateAsset.UserId = userId;
        communicateAsset.IsDeleted = false;
        // save changes
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            await context.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Error updating communicateAsset");
            throw new BadRequestException("Error while saving changes");
        }
    }

    public async Task UpdateCommunicateCategory(int communicateId, int categoryId)
    {
        var userId = _userContextService.UserId;
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();
        // get communicateCategory
        var communicateCategory = await context.CommunicateCategories.FindAsync(communicateId, categoryId);
        if (communicateCategory == null)
        {
            _logger.LogWarning("CommunicateCategory not found");
            throw new NotFoundException("CommunicateCategory not found");
        }
        if (!communicateCategory.IsDeleted)
            throw new BadRequestException("CommunicateCategory not marked as deleted");
        var communicate = await context.Communicates.FindAsync(communicateId);
        if (communicate == null || communicate.IsDeleted)
        {
            _logger.LogWarning("Communicate not found");
            throw new NotFoundException("Communicate not found");
        }
        var category = await context.Categories.FindAsync(categoryId);
        if (category == null || category.IsDeleted)
        {
            _logger.LogWarning("Category not found");
            throw new NotFoundException("Category not found");
        }
        communicateCategory.UserId = userId;
        communicateCategory.IsDeleted = false;
        // save changes
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            await context.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Error updating communicateCategory");
            throw new BadRequestException("Error while saving changes");
        }
    }

    public async Task UpdateCommunicateCoordinate(int communicateId, int coordinateId)
    {
        var userId = _userContextService.UserId;
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();
        // get communicateCoordinate
        var communicateCoordinate = await context.CommunicateCoordinates.FindAsync(communicateId, coordinateId);
        if (communicateCoordinate == null)
        {
            _logger.LogWarning("CommunicateCoordinate not found");
            throw new NotFoundException("CommunicateCoordinate not found");
        }
        if (!communicateCoordinate.IsDeleted)
            throw new BadRequestException("CommunicateCoordinate not marked as deleted");
        var communicate = await context.Communicates.FindAsync(communicateId);
        if (communicate == null || communicate.IsDeleted)
        {
            _logger.LogWarning("Communicate not found");
            throw new NotFoundException("Communicate not found");
        }
        var coordinate = await context.Coordinates.FindAsync(coordinateId);
        if (coordinate == null || coordinate.IsDeleted)
        {
            _logger.LogWarning("Coordinate not found");
            throw new NotFoundException("Coordinate not found");
        }
        communicateCoordinate.UserId = userId;
        communicateCoordinate.IsDeleted = false;
        // save changes
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            await context.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Error updating communicateCoordinate");
            throw new BadRequestException("Error while saving changes");
        }
    }

    public async Task UpdateCommunicateDevice(int communicateId, int deviceId)
    {
        var userId = _userContextService.UserId;
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();
        // get communicateDevice
        var communicateDevice = await context.CommunicateDevices.FindAsync(communicateId, deviceId);
        if (communicateDevice == null)
        {
            _logger.LogWarning("CommunicateDevice not found");
            throw new NotFoundException("CommunicateDevice not found");
        }
        if (!communicateDevice.IsDeleted)
            throw new BadRequestException("CommunicateDevice not marked as deleted");
        var communicate = await context.Communicates.FindAsync(communicateId);
        if (communicate == null || communicate.IsDeleted)
        {
            _logger.LogWarning("Communicate not found");
            throw new NotFoundException("Communicate not found");
        }
        var device = await context.Devices.FindAsync(deviceId);
        if (device == null || device.IsDeleted)
        {
            _logger.LogWarning("Device not found");
            throw new NotFoundException("Device not found");
        }
        communicateDevice.UserId = userId;
        communicateDevice.IsDeleted = false;
        // save changes
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            await context.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Error updating communicateDevice");
            throw new BadRequestException("Error while saving changes");
        }
    }

    public async Task UpdateCommunicateModel(int communicateId, int modelId)
    {
        var userId = _userContextService.UserId;
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();
        // get communicateModel
        var communicateModel = await context.CommunicateModels.FindAsync(communicateId, modelId);
        if (communicateModel == null)
        {
            _logger.LogWarning("CommunicateModel not found");
            throw new NotFoundException("CommunicateModel not found");
        }
        if (!communicateModel.IsDeleted)
            throw new BadRequestException("CommunicateModel not marked as deleted");
        var communicate = await context.Communicates.FindAsync(communicateId);
        if (communicate == null || communicate.IsDeleted)
        {
            _logger.LogWarning("Communicate not found");
            throw new NotFoundException("Communicate not found");
        }
        var model = await context.Models.FindAsync(modelId);
        if (model == null || model.IsDeleted)
        {
            _logger.LogWarning("Model not found");
            throw new NotFoundException("Model not found");
        }
        communicateModel.UserId = userId;
        communicateModel.IsDeleted = false;
        // save changes
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            await context.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Error updating communicateModel");
            throw new BadRequestException("Error while saving changes");
        }
    }

    public async Task UpdateCommunicateSpace(int communicateId, int spaceId)
    {
        var userId = _userContextService.UserId;
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();
        // get communicateSpace
        var communicateSpace = await context.CommunicateSpaces.FindAsync(communicateId, spaceId);
        if (communicateSpace == null)
        {
            _logger.LogWarning("CommunicateSpace not found");
            throw new NotFoundException("CommunicateSpace not found");
        }
        if (!communicateSpace.IsDeleted)
            throw new BadRequestException("CommunicateSpace not marked as deleted");
        var communicate = await context.Communicates.FindAsync(communicateId);
        if (communicate == null || communicate.IsDeleted)
        {
            _logger.LogWarning("Communicate not found");
            throw new NotFoundException("Communicate not found");
        }
        var space = await context.Spaces.FindAsync(spaceId);
        if (space == null || space.IsDeleted)
        {
            _logger.LogWarning("Space not found");
            throw new NotFoundException("Space not found");
        }
        communicateSpace.UserId = userId;
        communicateSpace.IsDeleted = false;
        // save changes
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            await context.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Error updating communicateSpace");
            throw new BadRequestException("Error while saving changes");
        }
    }
}