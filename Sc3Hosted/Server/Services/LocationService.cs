using Microsoft.EntityFrameworkCore;
using Sc3Hosted.Server.Data;
using Sc3Hosted.Server.Entities;
using Sc3Hosted.Server.Exceptions;
using Sc3Hosted.Shared.Dtos;

namespace Sc3Hosted.Server.Services;
public interface ILocationService
{
    Task<int> CreateArea(int plantId, AreaCreateDto areaCreateDto);

    Task<int> CreateCoordinate(int spaceId, CoordinateCreateDto coordinateCreateDto);

    Task<int> CreatePlant(PlantCreateDto plantCreateDto);

    Task<int> CreateSpace(int areaId, SpaceCreateDto spaceCreateDto);

    Task<bool> DeleteArea(int areaId);

    Task<bool> DeleteCoordinate(int coordinateId);

    Task<bool> DeletePlant(int plantId);

    Task<bool> DeleteSpace(int spaceId);

    Task<AreaDto> GetAreaById(int areaId);

    Task<IEnumerable<AreaDto>> GetAreas();

    Task<IEnumerable<AreaDto>> GetAreasWithSpaces();

    Task<CoordinateDto> GetCoordinateByIdWithAssets(int coordinateId);

    Task<IEnumerable<CoordinateDto>> GetCoordinates();

    Task<IEnumerable<CoordinateDto>> GetCoordinatesWithAssets();

    Task<PlantDto> GetPlantById(int plantId);

    Task<IEnumerable<PlantDto>> GetPlants();

    Task<IEnumerable<PlantDto>> GetPlantsWithAreas();

    Task<SpaceDto> GetSpaceById(int spaceId);

    Task<IEnumerable<SpaceDto>> GetSpaces();

    Task<IEnumerable<SpaceDto>> GetSpacesWithCoordinates();

    Task<bool> MarkDeleteArea(int areaId);

    Task<bool> MarkDeleteCoordinate(int coordinateId);

    Task<bool> MarkDeletePlant(int plantId);

    Task<bool> MarkDeleteSpace(int spaceId);

    Task<bool> UpdateArea(int areaId, AreaUpdateDto areaUpdateDto);

    Task<bool> UpdateCoordinate(int coordinateId, CoordinateUpdateDto coordinateUpdateDto);

    Task<bool> UpdatePlant(int plantId, PlantUpdateDto plantUpdateDto);

    Task<bool> UpdateSpace(int spaceId, SpaceUpdateDto spaceUpdateDto);
}

public class LocationService : ILocationService
{
    private readonly IDbContextFactory<Sc3HostedDbContext> _contextFactory;
    private readonly ILogger<LocationService> _logger;
    private readonly IUserContextService _userContextService;
    public LocationService(IDbContextFactory<Sc3HostedDbContext> contextFactory, ILogger<LocationService> logger, IUserContextService userContextService)
    {
        _contextFactory = contextFactory;
        _logger = logger;
        _userContextService = userContextService;
    }

    public async Task<int> CreateArea(int plantId, AreaCreateDto areaCreateDto)
    {
        var userId = _userContextService.UserId;
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();

        // validate area name
        var duplicate = await context.Areas
            .AnyAsync(a => a.PlantId == plantId && a.Name.ToLower().Trim() == areaCreateDto.Name.ToLower().Trim());
        if (duplicate)
        {
            _logger.LogWarning("Area name already exists");
            throw new BadRequestException("Area name already exists");
        }

        var area = new Area
        {
            UserId = userId,
            Name = areaCreateDto.Name,
            Description = areaCreateDto.Description,
            PlantId = plantId,
            IsDeleted = false
        };
        // create area
        context.Areas.Add(area);
        // await using transaction
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            // save changes
            await context.SaveChangesAsync();
            // commit transaction
            await transaction.CommitAsync();
            _logger.LogInformation("Area with id {AreaId} created", area.AreaId);
            return area.AreaId;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating area");
            // rollback transaction
            await transaction.RollbackAsync();
            throw new BadRequestException("Error creating area");
        }
    }

    public async Task<int> CreateCoordinate(int spaceId, CoordinateCreateDto coordinateCreateDto)
    {
        var userId = _userContextService.UserId;
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();

        // validate coordinate name
        var duplicate = await context.Coordinates
            .AnyAsync(c => c.SpaceId == spaceId && c.Name.ToLower().Trim() == coordinateCreateDto.Name.ToLower().Trim());
        if (duplicate)
        {
            _logger.LogWarning("Coordinate name already exists");
            throw new BadRequestException("Coordinate name already exists");
        }

        var coordinate = new Coordinate
        {
            UserId = userId,
            Name = coordinateCreateDto.Name,
            Description = coordinateCreateDto.Description,
            SpaceId = spaceId,
            IsDeleted = false
        };
        // create coordinate
        context.Coordinates.Add(coordinate);
        // await using transaction
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            // save changes
            await context.SaveChangesAsync();
            // commit transaction
            await transaction.CommitAsync();
            _logger.LogInformation("Coordinate with id {CoordinateId} created", coordinate.CoordinateId);
            return coordinate.CoordinateId;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating coordinate");
            // rollback transaction
            await transaction.RollbackAsync();
            throw new BadRequestException("Error creating coordinate");
        }
    }

    public async Task<int> CreatePlant(PlantCreateDto plantCreateDto)
    {var userId = _userContextService.UserId;
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();

        // validate plant name
        var duplicate = await context.Plants
            .AnyAsync(p => p.Name.ToLower().Trim() == plantCreateDto.Name.ToLower().Trim());
        if (duplicate)
        {
            _logger.LogWarning("Plant name already exists");
            throw new BadRequestException("Plant name already exists");
        }

        var plant = new Plant
        {
            UserId = userId,
            Name = plantCreateDto.Name,
            Description = plantCreateDto.Description,
            IsDeleted = false
        };
        // create plant
        context.Plants.Add(plant);
        // await using transaction
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            // save changes
            await context.SaveChangesAsync();
            // commit transaction
            await transaction.CommitAsync();
            _logger.LogInformation("Plant with id {PlantId} created", plant.PlantId);
            return plant.PlantId;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating plant");
            // rollback transaction
            await transaction.RollbackAsync();
            throw new BadRequestException("Error creating plant");
        }
    }

    public async Task<int> CreateSpace(int areaId, SpaceCreateDto spaceCreateDto)
    {var userId = _userContextService.UserId;
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();

        // validate space name
        var duplicate = await context.Spaces
            .AnyAsync(s => s.AreaId == areaId && s.Name.ToLower().Trim() == spaceCreateDto.Name.ToLower().Trim());
        if (duplicate)
        {
            _logger.LogWarning("Space name already exists");
            throw new BadRequestException("Space name already exists");
        }

        var space = new Space
        {
            UserId = userId,
            Name = spaceCreateDto.Name,
            Description = spaceCreateDto.Description,
            AreaId = areaId,
            IsDeleted = false
        };
        // create space
        context.Spaces.Add(space);
        // await using transaction
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            // save changes
            await context.SaveChangesAsync();
            // commit transaction
            await transaction.CommitAsync();
            _logger.LogInformation("Space with id {SpaceId} created", space.SpaceId);
            return space.SpaceId;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating space");
            // rollback transaction
            await transaction.RollbackAsync();
            throw new BadRequestException("Error creating space");
        }
    }

    public async Task<bool> DeleteArea(int areaId)
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();

        // get area
        var area = await context.Areas.FindAsync(areaId);
        if (area == null)
        {
            _logger.LogWarning("Area not found");
            throw new NotFoundException("Area not found");
        }
        // check if area is marked as deleted
        if (area.IsDeleted == false)
        {
            _logger.LogWarning("Area not marked as deleted");
            throw new BadRequestException("Area not marked as deleted");
        }
        context.Areas.Remove(area);
        // await using transaction
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            // save changes
            await context.SaveChangesAsync();
            // commit transaction
            await transaction.CommitAsync();
            _logger.LogInformation("Area with id {AreaId} deleted", area.AreaId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting area");
            // rollback transaction
            await transaction.RollbackAsync();
            throw new BadRequestException("Error deleting area");
        }
    }

    public async Task<bool> DeleteCoordinate(int coordinateId)
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();

        // get coordinate
        var coordinate = await context.Coordinates.FindAsync(coordinateId);
        if (coordinate == null)
        {
            _logger.LogWarning("Coordinate not found");
            throw new NotFoundException("Coordinate not found");
        }
        // check if coordinate is marked as deleted
        if (coordinate.IsDeleted == false)
        {
            _logger.LogWarning("Coordinate not marked as deleted");
            throw new BadRequestException("Coordinate not marked as deleted");
        }
        context.Coordinates.Remove(coordinate);
        // await using transaction
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            // save changes
            await context.SaveChangesAsync();
            // commit transaction
            await transaction.CommitAsync();
            _logger.LogInformation("Coordinate with id {CoordinateId} deleted", coordinate.CoordinateId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting coordinate");
            // rollback transaction
            await transaction.RollbackAsync();
            throw new BadRequestException("Error deleting coordinate");
        }
    }

    public async Task<bool> DeletePlant(int plantId)
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();

        // get plant
        var plant = await context.Plants.FindAsync(plantId);
        if (plant == null)
        {
            _logger.LogWarning("Plant not found");
            throw new NotFoundException("Plant not found");
        }
        // check if plant is marked as deleted
        if (plant.IsDeleted == false)
        {
            _logger.LogWarning("Plant not marked as deleted");
            throw new BadRequestException("Plant not marked as deleted");
        }
        context.Plants.Remove(plant);
        // await using transaction
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            // save changes
            await context.SaveChangesAsync();
            // commit transaction
            await transaction.CommitAsync();
            _logger.LogInformation("Plant with id {PlantId} deleted", plant.PlantId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting plant");
            // rollback transaction
            await transaction.RollbackAsync();
            throw new BadRequestException("Error deleting plant");
        }
    }

    public async Task<bool> DeleteSpace(int spaceId)
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();

        // get space
        var space = await context.Spaces.FindAsync(spaceId);
        if (space == null)
        {
            _logger.LogWarning("Space not found");
            throw new NotFoundException("Space not found");
        }
        // check if space is marked as deleted
        if (space.IsDeleted == false)
        {
            _logger.LogWarning("Space not marked as deleted");
            throw new BadRequestException("Space not marked as deleted");
        }
        context.Spaces.Remove(space);
        // await using transaction
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            // save changes
            await context.SaveChangesAsync();
            // commit transaction
            await transaction.CommitAsync();
            _logger.LogInformation("Space with id {SpaceId} deleted", space.SpaceId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting space");
            // rollback transaction
            await transaction.RollbackAsync();
            throw new BadRequestException("Error deleting space");
        }
    }

    public async Task<AreaDto> GetAreaById(int areaId)
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();

        // get area
        var area = await context.Areas
            .AsNoTracking()
            .Select(a => new AreaDto
            {
                AreaId = a.AreaId,
                Name = a.Name,
                Description = a.Description,
                IsDeleted = a.IsDeleted,
                UserId = a.UserId
            }).FirstOrDefaultAsync();
        if (area == null)
        {
            _logger.LogWarning("Area not found");
            throw new NotFoundException("Area not found");
        }
        // return area
        _logger.LogInformation("Area with id {AreaId} returned", area.AreaId);
        return area;
    }

    public async Task<IEnumerable<AreaDto>> GetAreas()
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();

        // get areas
        var areas = await context.Areas
            .AsNoTracking()
            .Select(a => new AreaDto
            {
                AreaId = a.AreaId,
                Name = a.Name,
                Description = a.Description,
                IsDeleted = a.IsDeleted,
                UserId = a.UserId
            }).ToListAsync();
        if (areas.Count == 0)
        {
            _logger.LogWarning("No areas found");
            return null!;
        }

        // return areas
        _logger.LogInformation("Areas returned");
        return areas;
    }

    public async Task<IEnumerable<AreaDto>> GetAreasWithSpaces()
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();

        // get areas
        var areas = await context.Areas
            .AsNoTracking()
            .Select(a => new AreaDto
            {
                AreaId = a.AreaId,
                Name = a.Name,
                Description = a.Description,
                IsDeleted = a.IsDeleted,
                UserId = a.UserId,
                Spaces = a.Spaces.Select(s => new SpaceDto
                {
                    SpaceId = s.SpaceId,
                    Name = s.Name,
                    Description = s.Description,
                    IsDeleted = s.IsDeleted,
                    UserId = s.UserId
                }).ToList()
            }).ToListAsync();
        if (areas.Count == 0)
        {
            _logger.LogWarning("No areas found");
            return null!;
        }
        // return areas
        _logger.LogInformation("Areas returned");
        return areas;
    }

    public async Task<CoordinateDto> GetCoordinateByIdWithAssets(int coordinateId)
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();

        // get coordinate
        var coordinate = await context.Coordinates
            .AsNoTracking()
            .Select(c => new CoordinateDto
            {
                CoordinateId = c.CoordinateId,
                Name = c.Name,
                Description = c.Description,
                IsDeleted = c.IsDeleted,
                UserId = c.UserId,
                Assets = c.Assets.Select(a => new AssetDto
                {
                    AssetId = a.AssetId,
                    Name = a.Name,
                    Description = a.Description,
                    IsDeleted = a.IsDeleted,
                    UserId = a.UserId
                }).ToList()
            }).FirstOrDefaultAsync(c => c.CoordinateId == coordinateId);
        if (coordinate == null)
        {
            _logger.LogWarning("Coordinate not found");
            return null!;
        }
        // return coordinate
        _logger.LogInformation("Coordinate with id {CoordinateId} returned", coordinate.CoordinateId);
        return coordinate;
    }

    public async Task<IEnumerable<CoordinateDto>> GetCoordinates()
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();

        // get coordinates
        var coordinates = await context.Coordinates
            .AsNoTracking()
            .Select(c => new CoordinateDto
            {
                CoordinateId = c.CoordinateId,
                Name = c.Name,
                Description = c.Description,
                IsDeleted = c.IsDeleted,
                UserId = c.UserId
            }).ToListAsync();
        if (coordinates.Count == 0)
        {
            _logger.LogWarning("No coordinates found");
            return null!;
        }
        // return coordinates
        _logger.LogInformation("Coordinates returned");
        return coordinates;
    }

    public async Task<IEnumerable<CoordinateDto>> GetCoordinatesWithAssets()
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();

        // get coordinates
        var coordinates = await context.Coordinates
            .AsNoTracking()
            .Select(c => new CoordinateDto
            {
                CoordinateId = c.CoordinateId,
                Name = c.Name,
                Description = c.Description,
                IsDeleted = c.IsDeleted,
                UserId = c.UserId,
                Assets = c.Assets.Select(a => new AssetDto
                {
                    AssetId = a.AssetId,
                    Name = a.Name,
                    Description = a.Description,
                    IsDeleted = a.IsDeleted,
                    UserId = a.UserId
                }).ToList()
            }).ToListAsync();
        if (coordinates.Count == 0)
        {
            _logger.LogWarning("No coordinates found");
            return null!;
        }
        // return coordinates
        _logger.LogInformation("Coordinates returned");
        return coordinates;
    }

    public async Task<PlantDto> GetPlantById(int plantId)
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();

        // get plant
        var plant = await context.Plants
            .AsNoTracking()
            .Select(p => new PlantDto
            {
                PlantId = p.PlantId,
                Name = p.Name,
                Description = p.Description,
                IsDeleted = p.IsDeleted,
                UserId = p.UserId
            }).FirstOrDefaultAsync(p => p.PlantId == plantId);
        if (plant == null)
        {
            _logger.LogWarning("Plant not found");
            return null!;
        }
        // return plant
        _logger.LogInformation("Plant with id {PlantId} returned", plant.PlantId);
        return plant;
    }

    public async Task<IEnumerable<PlantDto>> GetPlants()
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();

        // get plants
        var plants = await context.Plants
            .AsNoTracking()
            .Select(p => new PlantDto
            {
                PlantId = p.PlantId,
                Name = p.Name,
                Description = p.Description,
                IsDeleted = p.IsDeleted,
                UserId = p.UserId
            }).ToListAsync();
        if (plants.Count == 0)
        {
            _logger.LogWarning("No plants found");
            return null!;
        }
        // return plants
        _logger.LogInformation("Plants returned");
        return plants;
    }

    public async Task<IEnumerable<PlantDto>> GetPlantsWithAreas()
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();

        // get plants
        var plants = await context.Plants
            .AsNoTracking()
            .Select(p => new PlantDto
            {
                PlantId = p.PlantId,
                Name = p.Name,
                Description = p.Description,
                IsDeleted = p.IsDeleted,
                UserId = p.UserId,
                Areas = p.Areas.Select(a => new AreaDto
                {
                    AreaId = a.AreaId,
                    Name = a.Name,
                    Description = a.Description,
                    IsDeleted = a.IsDeleted,
                    UserId = a.UserId
                }).ToList()
            }).ToListAsync();
        if (plants.Count == 0)
        {
            _logger.LogWarning("No plants found");
            return null!;
        }
        // return plants
        _logger.LogInformation("Plants returned");
        return plants;
    }

    public async Task<SpaceDto> GetSpaceById(int spaceId)
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();

        // get space
        var space = await context.Spaces
            .AsNoTracking()
            .Select(s => new SpaceDto
            {
                SpaceId = s.SpaceId,
                Name = s.Name,
                Description = s.Description,
                IsDeleted = s.IsDeleted,
                UserId = s.UserId
            }).FirstOrDefaultAsync(s => s.SpaceId == spaceId);
        if (space == null)
        {
            _logger.LogWarning("Space not found");
            return null!;
        }
        // return space
        _logger.LogInformation("Space with id {SpaceId} returned", space.SpaceId);
        return space;
    }

    public async Task<IEnumerable<SpaceDto>> GetSpaces()
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();

        // get spaces
        var spaces = await context.Spaces
            .AsNoTracking()
            .Select(s => new SpaceDto
            {
                SpaceId = s.SpaceId,
                Name = s.Name,
                Description = s.Description,
                IsDeleted = s.IsDeleted,
                UserId = s.UserId
            }).ToListAsync();
        if (spaces.Count == 0)
        {
            _logger.LogWarning("No spaces found");
            return null!;
        }
        // return spaces
        _logger.LogInformation("Spaces returned");
        return spaces;
    }

    public async Task<IEnumerable<SpaceDto>> GetSpacesWithCoordinates()
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();
        // get spaces
        var spaces = await context.Spaces
            .AsNoTracking()
            .Select(s => new SpaceDto
            {
                SpaceId = s.SpaceId,
                Name = s.Name,
                Description = s.Description,
                IsDeleted = s.IsDeleted,
                UserId = s.UserId,
                Coordinates = s.Coordinates.Select(c => new CoordinateDto
                {
                    CoordinateId = c.CoordinateId,
                    Name = c.Name,
                    Description = c.Description,
                    IsDeleted = c.IsDeleted,
                    UserId = c.UserId
                }).ToList()
            }).ToListAsync();
        if (spaces.Count == 0)
        {
            _logger.LogWarning("No spaces found");
            return null!;
        }
        // return spaces
        _logger.LogInformation("Spaces returned");
        return spaces;
    }

    public async Task<bool> MarkDeleteArea(int areaId)
    {var userId = _userContextService.UserId;
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();

        // get area
        var area = await context.Areas
            .Include(a => a.Spaces)
            .FirstOrDefaultAsync(a => a.AreaId == areaId);
        if (area == null)
        {
            _logger.LogWarning("Area not found");

            throw new NotFoundException("Area not found");
        }
        if (area.IsDeleted)
        {
            _logger.LogWarning("Area already deleted");
            throw new BadRequestException("Area already deleted");
        }
        // check if area has active spaces
        if (area.Spaces.Any(s => s.IsDeleted == false))
        {
            _logger.LogWarning("Area has active spaces");
            throw new BadRequestException("Area has active spaces");
        }
        // mark area as deleted
        area.IsDeleted = true;
        area.UserId = userId;
        //update area
        context.Areas.Update(area);
        // await using transaction
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            // save changes
            await context.SaveChangesAsync();
            // commit transaction
            await transaction.CommitAsync();
            // return success
            _logger.LogInformation("Area with id {AreaId} marked as deleted", area.AreaId);
            return true;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error marking area as deleted");
            // rollback transaction
            await transaction.RollbackAsync();
            throw new BadRequestException("Error marking area as deleted");
        }
    }

    public async Task<bool> MarkDeleteCoordinate(int coordinateId)
    {var userId = _userContextService.UserId;
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();

        // get coordinate
        var coordinate = await context.Coordinates.Include(c => c.Assets)
            .FirstOrDefaultAsync(c => c.CoordinateId == coordinateId);
        if (coordinate == null)
        {
            _logger.LogWarning("Coordinate not found");
            throw new NotFoundException("Coordinate not found");
        }
        if (coordinate.IsDeleted)
        {
            _logger.LogWarning("Coordinate already deleted");
            throw new BadRequestException("Coordinate already deleted");
        }
        // check if coordinate has active assets
        if (coordinate.Assets.Any(a => a.IsDeleted == false))
        {
            _logger.LogWarning("Coordinate has active assets");
            throw new BadRequestException("Coordinate has active assets");
        }
        // mark coordinate as deleted
        coordinate.IsDeleted = true;
        coordinate.UserId = userId;
        //update coordinate
        context.Coordinates.Update(coordinate);
        // await using transaction
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            // save changes
            await context.SaveChangesAsync();
            // commit transaction
            await transaction.CommitAsync();
            // return success
            _logger.LogInformation("Coordinate with id {CoordinateId} marked as deleted", coordinate.CoordinateId);
            return true;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error marking coordinate as deleted");
            // rollback transaction
            await transaction.RollbackAsync();
            throw new BadRequestException("Error marking coordinate as deleted");
        }
    }

    public async Task<bool> MarkDeletePlant(int plantId)
    {var userId = _userContextService.UserId;
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();

        // get plant
        var plant = await context.Plants.Include(p => p.Areas)
            .FirstOrDefaultAsync(p => p.PlantId == plantId);
        if (plant == null)
        {
            _logger.LogWarning("Plant not found");
            throw new NotFoundException("Plant not found");
        }
        if (plant.IsDeleted)
        {
            _logger.LogWarning("Plant already deleted");
            throw new BadRequestException("Plant already deleted");
        }
        // check if plant has active areas
        if (plant.Areas.Any(a => a.IsDeleted == false))
        {
            _logger.LogWarning("Plant has active areas");
            throw new BadRequestException("Plant has active areas");
        }
        // mark plant as deleted
        plant.IsDeleted = true;
        plant.UserId = userId;
        //update plant
        context.Plants.Update(plant);
        // await using transaction
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            // save changes
            await context.SaveChangesAsync();
            // commit transaction
            await transaction.CommitAsync();
            // return success
            _logger.LogInformation("Plant with id {PlantId} marked as deleted", plant.PlantId);
            return true;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error marking plant as deleted");
            // rollback transaction
            await transaction.RollbackAsync();
            throw new BadRequestException("Error marking plant as deleted");
        }
    }

    public async Task<bool> MarkDeleteSpace(int spaceId)
    {var userId = _userContextService.UserId;
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();

        // get space
        var space = await context.Spaces.Include(s => s.Coordinates)
            .FirstOrDefaultAsync(s => s.SpaceId == spaceId);
        if (space == null)
        {
            _logger.LogWarning("Space not found");
            throw new NotFoundException("Space not found");
        }
        if (space.IsDeleted)
        {
            _logger.LogWarning("Space already deleted");
            throw new BadRequestException("Space already deleted");
        }
        // check if space has active coordinates
        if (space.Coordinates.Any(c => c.IsDeleted == false))
        {
            _logger.LogWarning("Space has active coordinates");
            throw new BadRequestException("Space has active coordinates");
        }
        // mark space as deleted
        space.IsDeleted = true;
        space.UserId = userId;
        //update space
        context.Spaces.Update(space);
        // await using transaction
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            // save changes
            await context.SaveChangesAsync();
            // commit transaction
            await transaction.CommitAsync();
            // return success
            _logger.LogInformation("Space with id {SpaceId} marked as deleted", space.SpaceId);
            return true;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error marking space as deleted");
            // rollback transaction
            await transaction.RollbackAsync();
            throw new BadRequestException("Error marking space as deleted");
        }
    }

    public async Task<bool> UpdateArea(int areaId, AreaUpdateDto areaUpdateDto)
    {var userId = _userContextService.UserId;
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();

        // get area
        var area = await context.Areas
            .FirstOrDefaultAsync(a => a.AreaId == areaId);
        if (area == null)
        {
            _logger.LogWarning("Area not found");
            throw new NotFoundException("Area not found");
        }
        // check for duplicate name
        if (await context.Areas.AnyAsync(a => a.AreaId != areaId && a.PlantId == area.PlantId && a.Name.ToLower().Trim() == areaUpdateDto.Name.ToLower().Trim()))
        {
            _logger.LogWarning("Area with name {Name} already exists", areaUpdateDto.Name);
            throw new BadRequestException("Area with name already exists");
        }
        // update area
        area.Name = areaUpdateDto.Name;
        area.Description = areaUpdateDto.Description;
        area.IsDeleted = false;
        area.UserId = userId;
        // update area
        context.Areas.Update(area);
        // await using transaction
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            // save changes
            await context.SaveChangesAsync();
            // commit transaction
            await transaction.CommitAsync();
            // return success
            _logger.LogInformation("Area with id {AreaId} updated", area.AreaId);
            return true;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error updating area");
            // rollback transaction
            await transaction.RollbackAsync();
            throw new BadRequestException("Error updating area");
        }
    }

    public async Task<bool> UpdateCoordinate(int coordinateId, CoordinateUpdateDto coordinateUpdateDto)
    {var userId = _userContextService.UserId;
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();

        // get coordinate
        var coordinate = await context.Coordinates
            .FirstOrDefaultAsync(c => c.CoordinateId == coordinateId);
        if (coordinate == null)
        {
            _logger.LogWarning("Coordinate not found");
            throw new NotFoundException("Coordinate not found");
        }
        // check for duplicate name
        if (await context.Coordinates.AnyAsync(c => c.CoordinateId != coordinateId && c.SpaceId == coordinate.SpaceId && c.Name.ToLower().Trim() == coordinateUpdateDto.Name.ToLower().Trim()))
        {
            _logger.LogWarning("Coordinate with name {Name} already exists", coordinateUpdateDto.Name);
            throw new BadRequestException("Coordinate with name already exists");
        }
        // update coordinate
        coordinate.Name = coordinateUpdateDto.Name;
        coordinate.Description = coordinateUpdateDto.Description;
        coordinate.IsDeleted = false;
        coordinate.UserId = userId;
        // update coordinate
        context.Coordinates.Update(coordinate);
        // await using transaction
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            // save changes
            await context.SaveChangesAsync();
            // commit transaction
            await transaction.CommitAsync();
            // return success
            _logger.LogInformation("Coordinate with id {CoordinateId} updated", coordinate.CoordinateId);
            return true;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error updating coordinate");
            // rollback transaction
            await transaction.RollbackAsync();
            throw new BadRequestException("Error updating coordinate");
        }
    }

    public async Task<bool> UpdatePlant(int plantId, PlantUpdateDto plantUpdateDto)
    {var userId = _userContextService.UserId;
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();

        // get plant
        var plant = await context.Plants
            .FirstOrDefaultAsync(p => p.PlantId == plantId);
        if (plant == null)
        {
            _logger.LogWarning("Plant not found");
            throw new NotFoundException("Plant not found");
        }
        // check for duplicate name
        if (await context.Plants.AnyAsync(p => p.PlantId != plantId && p.Name.ToLower().Trim() == plantUpdateDto.Name.ToLower().Trim()))
        {
            _logger.LogWarning("Plant with name {Name} already exists", plantUpdateDto.Name);
            throw new BadRequestException("Plant with name already exists");
        }

        // update plant
        plant.Name = plantUpdateDto.Name;
        plant.Description = plantUpdateDto.Description;
        plant.IsDeleted = false;
        plant.UserId = userId;
        // update plant
        context.Plants.Update(plant);
        // await using transaction
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            // save changes
            await context.SaveChangesAsync();
            // commit transaction
            await transaction.CommitAsync();
            // return success
            _logger.LogInformation("Plant with id {PlantId} updated", plant.PlantId);
            return true;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error updating plant");
            // rollback transaction
            await transaction.RollbackAsync();
            throw new BadRequestException("Error updating plant");
        }
    }

    public async Task<bool> UpdateSpace(int spaceId, SpaceUpdateDto spaceUpdateDto)
    {var userId = _userContextService.UserId;
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();

        // get space
        var space = await context.Spaces
            .FirstOrDefaultAsync(s => s.SpaceId == spaceId);
        if (space == null)
        {
            _logger.LogWarning("Space not found");
            throw new NotFoundException("Space not found");
        }
        // check for duplicate name
        if (await context.Spaces.AnyAsync(s => s.SpaceId != spaceId && s.AreaId == space.AreaId && s.Name.ToLower().Trim() == spaceUpdateDto.Name.ToLower().Trim()))
        {
            _logger.LogWarning("Space with name {Name} already exists", spaceUpdateDto.Name);
            throw new BadRequestException("Space with name already exists");
        }
        // update space
        space.Name = spaceUpdateDto.Name;
        space.Description = spaceUpdateDto.Description;
        space.IsDeleted = false;
        space.UserId = userId;
        // update space
        context.Spaces.Update(space);
        // await using transaction
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            // save changes
            await context.SaveChangesAsync();
            // commit transaction
            await transaction.CommitAsync();
            // return success
            _logger.LogInformation("Space with id {SpaceId} updated", space.SpaceId);
            return true;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error updating space");
            // rollback transaction
            await transaction.RollbackAsync();
            throw new BadRequestException("Error updating space");
        }
    }
}
