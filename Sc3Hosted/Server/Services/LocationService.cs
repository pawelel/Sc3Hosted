using Microsoft.EntityFrameworkCore;
using Sc3Hosted.Server.Data;
using Sc3Hosted.Server.Entities;
using Sc3Hosted.Shared.Dtos;
using Sc3Hosted.Shared.Helpers;

namespace Sc3Hosted.Server.Services;
public interface ILocationService
{
    Task<ServiceResponse> CreateArea(int plantId, AreaCreateDto areaCreateDto, string userId);

    Task<ServiceResponse> CreateCoordinate(int spaceId, CoordinateCreateDto coordinateCreateDto, string userId);

    Task<ServiceResponse> CreatePlant(PlantCreateDto plantCreateDto, string userId);

    Task<ServiceResponse> CreateSpace(int areaId, SpaceCreateDto spaceCreateDto, string userId);

    Task<ServiceResponse> DeleteArea(int areaId);

    Task<ServiceResponse> DeleteCoordinate(int coordinateId);

    Task<ServiceResponse> DeletePlant(int plantId);

    Task<ServiceResponse> DeleteSpace(int spaceId);

    Task<ServiceResponse<AreaDto>> GetAreaById(int areaId);

    Task<ServiceResponse<IEnumerable<AreaDto>>> GetAreas();

    Task<ServiceResponse<IEnumerable<AreaDto>>> GetAreasWithSpaces();

    Task<ServiceResponse<CoordinateDto>> GetCoordinateByIdWithAssets(int coordinateId);

    Task<ServiceResponse<IEnumerable<CoordinateDto>>> GetCoordinates();

    Task<ServiceResponse<IEnumerable<CoordinateDto>>> GetCoordinatesWithAssets();

    Task<ServiceResponse<PlantDto>> GetPlantById(int plantId);

    Task<ServiceResponse<IEnumerable<PlantDto>>> GetPlants();

    Task<ServiceResponse<IEnumerable<PlantDto>>> GetPlantsWithAreas();

    Task<ServiceResponse<SpaceDto>> GetSpaceById(int spaceId);

    Task<ServiceResponse<IEnumerable<SpaceDto>>> GetSpaces();

    Task<ServiceResponse<IEnumerable<SpaceDto>>> GetSpacesWithCoordinates();

    Task<ServiceResponse> MarkDeleteArea(int areaId, string userId);

    Task<ServiceResponse> MarkDeleteCoordinate(int coordinateId, string userId);

    Task<ServiceResponse> MarkDeletePlant(int plantId, string userId);

    Task<ServiceResponse> MarkDeleteSpace(int spaceId, string userId);
    
    Task<ServiceResponse> UpdatePlant(int plantId, PlantUpdateDto plantUpdateDto, string userId);

    Task<ServiceResponse> UpdateArea(int areaId, AreaUpdateDto areaUpdateDto, string userId);

    Task<ServiceResponse> UpdateSpace(int spaceId, SpaceUpdateDto spaceUpdateDto, string userId);

    Task<ServiceResponse> UpdateCoordinate(int coordinateId, CoordinateUpdateDto coordinateUpdateDto, string userId);
}

public class LocationService : ILocationService
{
    private readonly IDbContextFactory<Sc3HostedDbContext> _contextFactory;
    private readonly ILogger<LocationService> _logger;

    public LocationService(IDbContextFactory<Sc3HostedDbContext> contextFactory, ILogger<LocationService> logger)
    {
        _contextFactory = contextFactory;
        _logger = logger;
    }
    public async Task<ServiceResponse> CreateArea(int plantId, AreaCreateDto areaCreateDto, string userId)
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();
        // await using transaction
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            // validate area name
            var duplicate = await context.Areas
                .AnyAsync(a => a.PlantId == plantId && a.Name.ToLower().Trim() == areaCreateDto.Name.ToLower().Trim());
            if (duplicate)
            {
                _logger.LogWarning("Area name already exists");
                return new ServiceResponse("Area name already exists");
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
            // save changes
            await context.SaveChangesAsync();
            // commit transaction
            await transaction.CommitAsync();
            _logger.LogInformation("Area with id {AreaId} created", area.AreaId);
            return new ServiceResponse($"Area {area.AreaId} created", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating area");
            // rollback transaction
            await transaction.RollbackAsync();
            return new ServiceResponse($"Error creating area");
        }
    }
    public async Task<ServiceResponse> CreateCoordinate(int spaceId, CoordinateCreateDto coordinateCreateDto, string userId)
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();
        // await using transaction
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            // validate coordinate name
            var duplicate = await context.Coordinates
                .AnyAsync(c => c.SpaceId == spaceId && c.Name.ToLower().Trim() == coordinateCreateDto.Name.ToLower().Trim());
            if (duplicate)
            {
                _logger.LogWarning("Coordinate name already exists");
                return new ServiceResponse("Coordinate name already exists");
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
            // save changes
            await context.SaveChangesAsync();
            // commit transaction
            await transaction.CommitAsync();
            _logger.LogInformation("Coordinate with id {CoordinateId} created", coordinate.CoordinateId);
            return new ServiceResponse($"Coordinate {coordinate.CoordinateId} created", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating coordinate");
            // rollback transaction
            await transaction.RollbackAsync();
            return new ServiceResponse($"Error creating coordinate");
        }
    }
    public async Task<ServiceResponse> CreatePlant(PlantCreateDto plantCreateDto, string userId)
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();
        // await using transaction
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            // validate plant name
            var duplicate = await context.Plants
                .AnyAsync(p => p.Name.ToLower().Trim() == plantCreateDto.Name.ToLower().Trim());
            if (duplicate)
            {
                _logger.LogWarning("Plant name already exists");
                return new ServiceResponse("Plant name already exists");
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
            // save changes
            await context.SaveChangesAsync();
            // commit transaction
            await transaction.CommitAsync();
            _logger.LogInformation("Plant with id {PlantId} created", plant.PlantId);
            return new ServiceResponse($"Plant {plant.PlantId} created", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating plant");
            // rollback transaction
            await transaction.RollbackAsync();
            return new ServiceResponse($"Error creating plant");
        }
    }
    public async Task<ServiceResponse> CreateSpace(int areaId, SpaceCreateDto spaceCreateDto, string userId)
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();
        // await using transaction
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            // validate space name
            var duplicate = await context.Spaces
                .AnyAsync(s => s.AreaId == areaId && s.Name.ToLower().Trim() == spaceCreateDto.Name.ToLower().Trim());
            if (duplicate)
            {
                _logger.LogWarning("Space name already exists");
                return new ServiceResponse("Space name already exists");
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
            // save changes
            await context.SaveChangesAsync();
            // commit transaction
            await transaction.CommitAsync();
            _logger.LogInformation("Space with id {SpaceId} created", space.SpaceId);
            return new ServiceResponse($"Space {space.SpaceId} created", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating space");
            // rollback transaction
            await transaction.RollbackAsync();
            return new ServiceResponse($"Error creating space");
        }
    }
    public async Task<ServiceResponse> DeleteArea(int areaId)
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();
        // await using transaction
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            // get area
            var area = await context.Areas.FindAsync(areaId);
            if (area == null)
            {
                _logger.LogWarning("Area not found");
                return new ServiceResponse("Area not found");
            }
            // check if area is marked as deleted
            if (area.IsDeleted == false)
            {
                _logger.LogWarning("Area not marked as deleted");
                return new ServiceResponse("Area not marked as deleted");
            }
            context.Areas.Remove(area);
            // save changes
            await context.SaveChangesAsync();
            // commit transaction
            await transaction.CommitAsync();
            _logger.LogInformation("Area with id {AreaId} deleted", area.AreaId);
            return new ServiceResponse($"Area {area.AreaId} deleted", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting area");
            // rollback transaction
            await transaction.RollbackAsync();
            return new ServiceResponse($"Error deleting area");
        }
    }
    public async Task<ServiceResponse> DeleteCoordinate(int coordinateId)
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();
        // await using transaction
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            // get coordinate
            var coordinate = await context.Coordinates.FindAsync(coordinateId);
            if (coordinate == null)
            {
                _logger.LogWarning("Coordinate not found");
                return new ServiceResponse("Coordinate not found");
            }
            // check if coordinate is marked as deleted
            if (coordinate.IsDeleted == false)
            {
                _logger.LogWarning("Coordinate not marked as deleted");
                return new ServiceResponse("Coordinate not marked as deleted");
            }
            context.Coordinates.Remove(coordinate);
            // save changes
            await context.SaveChangesAsync();
            // commit transaction
            await transaction.CommitAsync();
            _logger.LogInformation("Coordinate with id {CoordinateId} deleted", coordinate.CoordinateId);
            return new ServiceResponse($"Coordinate {coordinate.CoordinateId} deleted", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting coordinate");
            // rollback transaction
            await transaction.RollbackAsync();
            return new ServiceResponse($"Error deleting coordinate");
        }
    }
    public async Task<ServiceResponse> DeletePlant(int plantId)
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();
        // await using transaction
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            // get plant
            var plant = await context.Plants.FindAsync(plantId);
            if (plant == null)
            {
                _logger.LogWarning("Plant not found");
                return new ServiceResponse("Plant not found");
            }
            // check if plant is marked as deleted
            if (plant.IsDeleted == false)
            {
                _logger.LogWarning("Plant not marked as deleted");
                return new ServiceResponse("Plant not marked as deleted");
            }
            context.Plants.Remove(plant);
            // save changes
            await context.SaveChangesAsync();
            // commit transaction
            await transaction.CommitAsync();
            _logger.LogInformation("Plant with id {PlantId} deleted", plant.PlantId);
            return new ServiceResponse($"Plant {plant.PlantId} deleted", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting plant");
            // rollback transaction
            await transaction.RollbackAsync();
            return new ServiceResponse($"Error deleting plant");
        }
    }
    public async Task<ServiceResponse> DeleteSpace(int spaceId)
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();
        // await using transaction
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            // get space
            var space = await context.Spaces.FindAsync(spaceId);
            if (space == null)
            {
                _logger.LogWarning("Space not found");
                return new ServiceResponse("Space not found");
            }
            // check if space is marked as deleted
            if (space.IsDeleted == false)
            {
                _logger.LogWarning("Space not marked as deleted");
                return new ServiceResponse("Space not marked as deleted");
            }
            context.Spaces.Remove(space);
            // save changes
            await context.SaveChangesAsync();
            // commit transaction
            await transaction.CommitAsync();
            _logger.LogInformation("Space with id {SpaceId} deleted", space.SpaceId);
            return new ServiceResponse($"Space {space.SpaceId} deleted", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting space");
            // rollback transaction
            await transaction.RollbackAsync();
            return new ServiceResponse($"Error deleting space");
        }
    }
    public async Task<ServiceResponse<AreaDto>> GetAreaById(int areaId)
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();
        try
        {
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
                return new ServiceResponse<AreaDto>("Area not found");
            }
            // return area
            _logger.LogInformation("Area with id {AreaId} returned", area.AreaId);
            return new ServiceResponse<AreaDto>(area, "Area returned");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error getting area");
            return new ServiceResponse<AreaDto>("Error getting area");
        }
    }
    public async Task<ServiceResponse<IEnumerable<AreaDto>>> GetAreas()
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();
        try
        {
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
                return new ServiceResponse<IEnumerable<AreaDto>>("No areas found");
            }

            // return areas
            _logger.LogInformation("Areas returned");
            return new ServiceResponse<IEnumerable<AreaDto>>(areas, "Areas returned");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error getting areas");
            return new ServiceResponse<IEnumerable<AreaDto>>("Error getting areas");
        }
    }
    public async Task<ServiceResponse<IEnumerable<AreaDto>>> GetAreasWithSpaces()
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();
        try
        {
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
                return new ServiceResponse<IEnumerable<AreaDto>>("No areas found");
            }
            // return areas
            _logger.LogInformation("Areas returned");
            return new ServiceResponse<IEnumerable<AreaDto>>(areas, "Areas returned");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error getting areas");
            return new ServiceResponse<IEnumerable<AreaDto>>("Error getting areas");
        }
    }
    public async Task<ServiceResponse<CoordinateDto>> GetCoordinateByIdWithAssets(int coordinateId)
    {
        if (coordinateId < 1)
        {
            _logger.LogWarning("Invalid id");
            return new ServiceResponse<CoordinateDto>("Invalid id");
        }
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();
        try
        {
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
                return new ServiceResponse<CoordinateDto>("Coordinate not found");
            }
            // return coordinate
            _logger.LogInformation("Coordinate with id {CoordinateId} returned", coordinate.CoordinateId);
            return new ServiceResponse<CoordinateDto>(coordinate, "Coordinate returned");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error getting coordinate");
            return new ServiceResponse<CoordinateDto>("Error getting coordinate");
        }
    }
    public async Task<ServiceResponse<IEnumerable<CoordinateDto>>> GetCoordinates()
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();
        try
        {
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
                return new ServiceResponse<IEnumerable<CoordinateDto>>("No coordinates found");
            }
            // return coordinates
            _logger.LogInformation("Coordinates returned");
            return new ServiceResponse<IEnumerable<CoordinateDto>>(coordinates, "Coordinates returned");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error getting coordinates");
            return new ServiceResponse<IEnumerable<CoordinateDto>>("Error getting coordinates");
        }
    }
    public async Task<ServiceResponse<IEnumerable<CoordinateDto>>> GetCoordinatesWithAssets()
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();
        try
        {
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
                return new ServiceResponse<IEnumerable<CoordinateDto>>("No coordinates found");
            }
            // return coordinates
            _logger.LogInformation("Coordinates returned");
            return new ServiceResponse<IEnumerable<CoordinateDto>>(coordinates, "Coordinates returned");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error getting coordinates");
            return new ServiceResponse<IEnumerable<CoordinateDto>>("Error getting coordinates");
        }
    }
    public async Task<ServiceResponse<PlantDto>> GetPlantById(int plantId)
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();
        try
        {
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
                return new ServiceResponse<PlantDto>("Plant not found");
            }
            // return plant
            _logger.LogInformation("Plant with id {PlantId} returned", plant.PlantId);
            return new ServiceResponse<PlantDto>(plant, "Plant returned");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error getting plant");
            return new ServiceResponse<PlantDto>("Error getting plant");
        }
    }
    public async Task<ServiceResponse<IEnumerable<PlantDto>>> GetPlants()
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();
        try
        {
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
                return new ServiceResponse<IEnumerable<PlantDto>>("No plants found");
            }
            // return plants
            _logger.LogInformation("Plants returned");
            return new ServiceResponse<IEnumerable<PlantDto>>(plants, "Plants returned");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error getting plants");
            return new ServiceResponse<IEnumerable<PlantDto>>("Error getting plants");
        }
    }
    public async Task<ServiceResponse<IEnumerable<PlantDto>>> GetPlantsWithAreas()
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();
        try
        {
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
                return new ServiceResponse<IEnumerable<PlantDto>>("No plants found");
            }
            // return plants
            _logger.LogInformation("Plants returned");
            return new ServiceResponse<IEnumerable<PlantDto>>(plants, "Plants returned");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error getting plants");
            return new ServiceResponse<IEnumerable<PlantDto>>("Error getting plants");
        }
    }
    public async Task<ServiceResponse<SpaceDto>> GetSpaceById(int spaceId)
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();
        try
        {
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
                return new ServiceResponse<SpaceDto>("Space not found");
            }
            // return space
            _logger.LogInformation("Space with id {SpaceId} returned", space.SpaceId);
            return new ServiceResponse<SpaceDto>(space, "Space returned");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error getting space");
            return new ServiceResponse<SpaceDto>("Error getting space");
        }
    }
    public async Task<ServiceResponse<IEnumerable<SpaceDto>>> GetSpaces()
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();
        try
        {
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
                return new ServiceResponse<IEnumerable<SpaceDto>>("No spaces found");
            }
            // return spaces
            _logger.LogInformation("Spaces returned");
            return new ServiceResponse<IEnumerable<SpaceDto>>(spaces, "Spaces returned");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error getting spaces");
            return new ServiceResponse<IEnumerable<SpaceDto>>("Error getting spaces");
        }
    }
    public async Task<ServiceResponse<IEnumerable<SpaceDto>>> GetSpacesWithCoordinates()
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();
        try
        {
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
                return new ServiceResponse<IEnumerable<SpaceDto>>("No spaces found");
            }
            // return spaces
            _logger.LogInformation("Spaces returned");
            return new ServiceResponse<IEnumerable<SpaceDto>>(spaces, "Spaces returned");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error getting spaces");
            return new ServiceResponse<IEnumerable<SpaceDto>>("Error getting spaces");
        }
    }
    public async Task<ServiceResponse> MarkDeleteArea(int areaId, string userId)
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();
        // await using transaction
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            // get area
            var area = await context.Areas
                .Include(a => a.Spaces)
                .FirstOrDefaultAsync(a => a.AreaId == areaId);
            if (area == null)
            {
                _logger.LogWarning("Area not found");
                return new ServiceResponse("Area not found");
            }
            if (area.IsDeleted)
            {
                _logger.LogWarning("Area already deleted");
                return new ServiceResponse("Area already deleted");
            }
            // check if area has active spaces
            if (area.Spaces.Any(s => s.IsDeleted == false))
            {
                _logger.LogWarning("Area has active spaces");
                return new ServiceResponse("Area has active spaces");
            }
            // mark area as deleted
            area.IsDeleted = true;
            area.UserId = userId;
            //update area
            context.Areas.Update(area);
            // save changes 
            await context.SaveChangesAsync();
            // commit transaction
            await transaction.CommitAsync();
            // return success
            _logger.LogInformation("Area with id {AreaId} marked as deleted", area.AreaId);
            return new ServiceResponse("Area marked as deleted");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error marking area as deleted");
            // rollback transaction
            await transaction.RollbackAsync();
            return new ServiceResponse("Error marking area as deleted");
        }
    }
    public async Task<ServiceResponse> MarkDeleteCoordinate(int coordinateId, string userId)
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();
        // await using transaction
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            // get coordinate
            var coordinate = await context.Coordinates.Include(c => c.Assets)
                .FirstOrDefaultAsync(c => c.CoordinateId == coordinateId);
            if (coordinate == null)
            {
                _logger.LogWarning("Coordinate not found");
                return new ServiceResponse("Coordinate not found");
            }
            if (coordinate.IsDeleted)
            {
                _logger.LogWarning("Coordinate already deleted");
                return new ServiceResponse("Coordinate already deleted");
            }
            // check if coordinate has active assets
            if (coordinate.Assets.Any(a => a.IsDeleted == false))
            {
                _logger.LogWarning("Coordinate has active assets");
                return new ServiceResponse("Coordinate has active assets");
            }
            // mark coordinate as deleted
            coordinate.IsDeleted = true;
            coordinate.UserId = userId;
            //update coordinate
            context.Coordinates.Update(coordinate);
            // save changes 
            await context.SaveChangesAsync();
            // commit transaction
            await transaction.CommitAsync();
            // return success
            _logger.LogInformation("Coordinate with id {CoordinateId} marked as deleted", coordinate.CoordinateId);
            return new ServiceResponse("Coordinate marked as deleted");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error marking coordinate as deleted");
            // rollback transaction
            await transaction.RollbackAsync();
            return new ServiceResponse("Error marking coordinate as deleted");
        }
    }
    public async Task<ServiceResponse> MarkDeletePlant(int plantId, string userId)
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();
        // await using transaction
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            // get plant
            var plant = await context.Plants.Include(p => p.Areas)
                .FirstOrDefaultAsync(p => p.PlantId == plantId);
            if (plant == null)
            {
                _logger.LogWarning("Plant not found");
                return new ServiceResponse("Plant not found");
            }
            if (plant.IsDeleted)
            {
                _logger.LogWarning("Plant already deleted");
                return new ServiceResponse("Plant already deleted");
            }
            // check if plant has active areas
            if (plant.Areas.Any(a => a.IsDeleted == false))
            {
                _logger.LogWarning("Plant has active areas");
                return new ServiceResponse("Plant has active areas");
            }
            // mark plant as deleted
            plant.IsDeleted = true;
            plant.UserId = userId;
            //update plant
            context.Plants.Update(plant);
            // save changes 
            await context.SaveChangesAsync();
            // commit transaction
            await transaction.CommitAsync();
            // return success
            _logger.LogInformation("Plant with id {PlantId} marked as deleted", plant.PlantId);
            return new ServiceResponse("Plant marked as deleted");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error marking plant as deleted");
            // rollback transaction
            await transaction.RollbackAsync();
            return new ServiceResponse("Error marking plant as deleted");
        }
    }
    public async Task<ServiceResponse> MarkDeleteSpace(int spaceId, string userId)
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();
        // await using transaction
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            // get space
            var space = await context.Spaces.Include(s => s.Coordinates)
                .FirstOrDefaultAsync(s => s.SpaceId == spaceId);
            if (space == null)
            {
                _logger.LogWarning("Space not found");
                return new ServiceResponse("Space not found");
            }
            if (space.IsDeleted)
            {
                _logger.LogWarning("Space already deleted");
                return new ServiceResponse("Space already deleted");
            }
            // check if space has active coordinates
            if (space.Coordinates.Any(c => c.IsDeleted == false))
            {
                _logger.LogWarning("Space has active coordinates");
                return new ServiceResponse("Space has active coordinates");
            }
            // mark space as deleted
            space.IsDeleted = true;
            space.UserId = userId;
            //update space
            context.Spaces.Update(space);
            // save changes 
            await context.SaveChangesAsync();
            // commit transaction
            await transaction.CommitAsync();
            // return success
            _logger.LogInformation("Space with id {SpaceId} marked as deleted", space.SpaceId);
            return new ServiceResponse("Space marked as deleted");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error marking space as deleted");
            // rollback transaction
            await transaction.RollbackAsync();
            return new ServiceResponse("Error marking space as deleted");
        }
    }
    public async Task<ServiceResponse> UpdatePlant(int plantId, PlantUpdateDto plantUpdateDto, string userId)
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();
        // await using transaction
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            // get plant
            var plant = await context.Plants
                .FirstOrDefaultAsync(p => p.PlantId == plantId);
            if (plant == null)
            {
                _logger.LogWarning("Plant not found");
                return new ServiceResponse("Plant not found");
            }
            // check for duplicate name
            if (await context.Plants.AnyAsync(p => p.PlantId != plantId && p.Name.ToLower().Trim() == plantUpdateDto.Name.ToLower().Trim()))
            {
                _logger.LogWarning("Plant with name {Name} already exists", plantUpdateDto.Name);
                return new ServiceResponse("Plant with name already exists");
            }

            // update plant
            plant.Name = plantUpdateDto.Name;
            plant.Description = plantUpdateDto.Description;
            plant.IsDeleted = false;
            plant.UserId = userId;
            // update plant
            context.Plants.Update(plant);
            // save changes 
            await context.SaveChangesAsync();
            // commit transaction
            await transaction.CommitAsync();
            // return success
            _logger.LogInformation("Plant with id {PlantId} updated", plant.PlantId);
            return new ServiceResponse("Plant updated");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error updating plant");
            // rollback transaction
            await transaction.RollbackAsync();
            return new ServiceResponse("Error updating plant");
        }
    }
    public async Task<ServiceResponse> UpdateArea(int areaId, AreaUpdateDto areaUpdateDto, string userId)
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();
        // await using transaction
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            // get area
            var area = await context.Areas
                .FirstOrDefaultAsync(a => a.AreaId == areaId);
            if (area == null)
            {
                _logger.LogWarning("Area not found");
                return new ServiceResponse("Area not found");
            }
            // check for duplicate name
            if (await context.Areas.AnyAsync(a => a.AreaId != areaId &&a.PlantId==area.PlantId &&a.Name.ToLower().Trim() == areaUpdateDto.Name.ToLower().Trim()))
            {
                _logger.LogWarning("Area with name {Name} already exists", areaUpdateDto.Name);
                return new ServiceResponse("Area with name already exists");
            }
            // update area
            area.Name = areaUpdateDto.Name;
            area.Description = areaUpdateDto.Description;
            area.IsDeleted = false;
            area.UserId = userId;
            // update area
            context.Areas.Update(area);
            // save changes 
            await context.SaveChangesAsync();
            // commit transaction
            await transaction.CommitAsync();
            // return success
            _logger.LogInformation("Area with id {AreaId} updated", area.AreaId);
            return new ServiceResponse("Area updated");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error updating area");
            // rollback transaction
            await transaction.RollbackAsync();
            return new ServiceResponse("Error updating area");
        }
    }
    public async Task<ServiceResponse> UpdateSpace(int spaceId, SpaceUpdateDto spaceUpdateDto, string userId)
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();
        // await using transaction
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            // get space
            var space = await context.Spaces
                .FirstOrDefaultAsync(s => s.SpaceId == spaceId);
            if (space == null)
            {
                _logger.LogWarning("Space not found");
                return new ServiceResponse("Space not found");
            }
            // check for duplicate name
            if (await context.Spaces.AnyAsync(s => s.SpaceId != spaceId &&s.AreaId==space.AreaId &&s.Name.ToLower().Trim() == spaceUpdateDto.Name.ToLower().Trim()))
            {
                _logger.LogWarning("Space with name {Name} already exists", spaceUpdateDto.Name);
                return new ServiceResponse("Space with name already exists");
            }
            // update space
            space.Name = spaceUpdateDto.Name;
            space.Description = spaceUpdateDto.Description;
            space.IsDeleted = false;
            space.UserId = userId;
            // update space
            context.Spaces.Update(space);
            // save changes 
            await context.SaveChangesAsync();
            // commit transaction
            await transaction.CommitAsync();
            // return success
            _logger.LogInformation("Space with id {SpaceId} updated", space.SpaceId);
            return new ServiceResponse("Space updated");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error updating space");
            // rollback transaction
            await transaction.RollbackAsync();
            return new ServiceResponse("Error updating space");
        }
    }
    public async Task<ServiceResponse> UpdateCoordinate(int coordinateId, CoordinateUpdateDto coordinateUpdateDto, string userId)
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();
        // await using transaction
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            // get coordinate
            var coordinate = await context.Coordinates
                .FirstOrDefaultAsync(c => c.CoordinateId == coordinateId);
            if (coordinate == null)
            {
                _logger.LogWarning("Coordinate not found");
                return new ServiceResponse("Coordinate not found");
            }
            // check for duplicate name
            if (await context.Coordinates.AnyAsync(c => c.CoordinateId != coordinateId && c.SpaceId == coordinate.SpaceId && c.Name.ToLower().Trim() == coordinateUpdateDto.Name.ToLower().Trim()))
            {
                _logger.LogWarning("Coordinate with name {Name} already exists", coordinateUpdateDto.Name);
                return new ServiceResponse("Coordinate with name already exists");
            }
            // update coordinate
            coordinate.Name = coordinateUpdateDto.Name;
            coordinate.Description = coordinateUpdateDto.Description;
            coordinate.IsDeleted = false;
            coordinate.UserId = userId;
            // update coordinate
            context.Coordinates.Update(coordinate);
            // save changes 
            await context.SaveChangesAsync();
            // commit transaction
            await transaction.CommitAsync();
            // return success
            _logger.LogInformation("Coordinate with id {CoordinateId} updated", coordinate.CoordinateId);
            return new ServiceResponse("Coordinate updated");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error updating coordinate");
            // rollback transaction
            await transaction.RollbackAsync();
            return new ServiceResponse("Error updating coordinate");
        }
    }

}
