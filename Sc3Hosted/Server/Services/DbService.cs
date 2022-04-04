using AutoMapper;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

using Sc3Hosted.Server.Data;
using Sc3Hosted.Server.Entities;

using Sc3Hosted.Shared.Dtos;
using Sc3Hosted.Shared.Helpers;

namespace Sc3Hosted.Server.Services;

public class DbService : IDbService
{
    private readonly IMapper _mapper;
    private readonly IDbContextFactory<Sc3HostedDbContext> _contextFactory;
    private readonly ILogger<DbService> _logger;
    public DbService(IMapper mapper, ILogger<DbService> logger, IDbContextFactory<Sc3HostedDbContext> contextFactory)
    {
        _mapper = mapper;
        _logger = logger;
        _contextFactory = contextFactory;
    }

    #region plant service
    public async Task<ServiceResponse> CreatePlant(PlantCreateDto plantCreateDto, string userId)
    {
        using var context = _contextFactory.CreateDbContext();
        using IDbContextTransaction transaction = context.Database.BeginTransaction();
        try
        {
            Plant plant = new();
            var exist = await context.Plants.FirstOrDefaultAsync(p => Equals(p.Name.ToLower().Trim(), plant.Name.ToLower().Trim()));
            if (exist != null && exist.IsDeleted == false)
            {
                return new("Plant already exists", false);
            }

            plant.UserId = userId;
            plant.Name = plantCreateDto.Name;
            plant.Description = plantCreateDto.Description;
            context.Plants.Add(plant);
            await context.SaveChangesAsync();
            transaction.Commit();
            return new("Plant created", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating plant");
            transaction.Rollback();
            return new("Error creating plant", false);
        }
    }
    public async Task<ServiceResponse<PlantDto>> GetPlantById(int id)
    {
        using var context = _contextFactory.CreateDbContext();

        try
        {
            var plant = await context.Plants.FirstOrDefaultAsync(p => p.PlantId == id);
            if (plant == null)
            {
                return new("Plant not found", false);
            }
            return new(_mapper.Map<PlantDto>(plant), "Plant returned", true);

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting plant by id");
            return new("Error getting plant by id", false);
        }
    }
    public async Task<ServiceResponse<IEnumerable<PlantDto>>> GetPlants()
    {
        using var context = _contextFactory.CreateDbContext();
        try
        {
            var plants = await context.Plants.ToListAsync();
            if (plants == null)
            {
                return new("Plants not found", false);
            }
            return new(_mapper.Map<IEnumerable<PlantDto>>(plants), "List of plants returned", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all plants");
            return new("Error getting all plants", false);
        }
    }
    public async Task<ServiceResponse<IEnumerable<PlantDto>>> GetPlantsWithAreas()
    {
        using var context = _contextFactory.CreateDbContext();
        try
        {
            var plants = await context.Plants.Include(a => a.Areas).ToListAsync();
            if (plants == null)
            {
                return new("Plants not found", false);
            }
            return new(_mapper.Map<IEnumerable<PlantDto>>(plants), "List of plants returned", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting plants with areas");
            return new("Error getting plants with areas", false);
        }
    }

    public async Task<ServiceResponse> UpdatePlant(int id, string userId, PlantUpdateDto plantUpdateDto)
    {
        using var context = _contextFactory.CreateDbContext();
        using IDbContextTransaction transaction = context.Database.BeginTransaction();
        try
        {
            var plant = await context.Plants.FirstOrDefaultAsync(p => p.PlantId == id);
            if (plant == null)
            {
                return new("Plant not found", false);
            }
            var exists = await context.Plants.Where(p => Equals(p.Name.ToLower().Trim(), plantUpdateDto.Name.ToLower().Trim()) && p.PlantId != id && p.IsDeleted == false).ToListAsync();
            if (exists.Any())
            {
                return new($"Plant with name {plantUpdateDto.Name} already exists", false);
            }
            plant.Description = plantUpdateDto.Description;
            plant.Name = plantUpdateDto.Name;
            plant.UserId = userId;
            context.Plants.Update(plant);
            await context.SaveChangesAsync();
            transaction.Commit();
            return new("Plant updated", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating plant");
            transaction.Rollback();
            return new("Error updating plant", false);
        }
    }
    public async Task<ServiceResponse> MarkDeletePlant(int id, string userId)
    {
        using var context = _contextFactory.CreateDbContext();
        using IDbContextTransaction transaction = context.Database.BeginTransaction();
        try
        {
            var plant = await context.Plants.Include(a => a.Areas).FirstOrDefaultAsync(p=>p.PlantId==id);
            if (plant == null)
            {
                return new("Plant not found", false);
            }
            if (plant.Areas.Count > 0)
            {
                return new("Plant has areas, can't delete", false);
            }
            plant.IsDeleted = true;
            plant.UserId = userId;
            context.Plants.Update(plant);
       
                await context.SaveChangesAsync();
            transaction.Commit();
                return new("Plant marked as deleted", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting plant");
            transaction.Rollback();
            return new("Error deleting plant", false);
        }
    }
    public async Task<ServiceResponse> MarkUnDeletePlant(int id, string userId)
    {
        using var context = _contextFactory.CreateDbContext();
        using IDbContextTransaction transaction = context.Database.BeginTransaction();
        try
        {
            var plant = await context.Plants.FirstOrDefaultAsync(p=>p.PlantId==id);
            if (plant == null)
            {
                return new("Plant not found", false);
            }
            if (plant.IsDeleted == false)
            {
                return new("Plant is not marked as deleted", false);
            }
            var exists = await context.Plants.Where(p => Equals(p.Name.ToLower().Trim(), plant.Name.ToLower().Trim()) && p.PlantId != id && p.IsDeleted == false).ToListAsync();
            if (exists.Any())
            {
                return new($"Plant with name {plant.Name} already exists", false);
            }
            plant.IsDeleted = false;
            plant.UserId = userId;
            context.Plants.Update(plant);
         
                await context.SaveChangesAsync();
            transaction.Commit();
                return new("Plant marked as undeleted", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error un-deleting plant");
            transaction.Rollback();
            return new("Error un-deleting plant", false);
        }
    }
    public async Task<ServiceResponse> DeletePlant(int id)
    {
        using var context = _contextFactory.CreateDbContext();
        using IDbContextTransaction transaction = context.Database.BeginTransaction();
        try
        {
            var plant = await context.Plants.FirstOrDefaultAsync(p=>p.PlantId==id);
            if (plant == null)
            {
                _logger.LogError("Plant not found");
                return new("Plant not found", false);
            }
            if (plant.IsDeleted == false)
            {
                _logger.LogError("Plant not marked as deleted");
                return new("Plant not marked as deleted", false);
            }
             context.Plants.Remove(plant);
         
                await context.SaveChangesAsync();
            transaction.Commit();
                return new("Plant deleted", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting plant");
            transaction.Rollback();
            return new("Error deleting plant", false);
        }
    }
    #endregion
    #region area service
    public async Task<ServiceResponse> CreateArea(int plantId, AreaCreateDto areaCreateDto, string userId)
    {
        try
        {
            var plant = await context.Plants.FirstOrDefaultAsync(p => p.PlantId == plantId, p => p.Include(a => a.Areas));
            if (plant == null || plant.IsDeleted)
            {
                _logger.LogWarning("Cannot create area for plant with id {plantId}", plantId);
                return new($"Cannot create area for plant with id {plantId}", false);
            }
            if (plant.Areas.Any(a => !a.IsDeleted && Equals(a.Name.ToLower().Trim(), areaCreateDto.Name.ToLower().Trim())))
            {
                _logger.LogWarning("Area with name {areaName} already exists", areaCreateDto.Name);
                return new($"Area with name {areaCreateDto.Name} already exists", false);
            }
            Area area = new();
            area.Name = areaCreateDto.Name;
            area.Description = areaCreateDto.Description;
            area.PlantId = plantId;
            area.UserId = userId;
            var result = await context.Areas.Add(area);
            if (result)
            {
                await context.SaveChangesAsync();
                return new("Area created", true);
            }
            else
            {
                _logger.LogError("Area not created");
                return new("Area not created", false);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating area");
            return new("Error creating area", false);
        }
    }

    public async Task<ServiceResponse<AreaDto>> GetAreaById(int id)
    {
        try
        {
            var area = await context.Areas.FirstOrDefaultAsync(id);
            if (area == null)
            {
                return new("Area not found", false);
            }
            return new(_mapper.Map<AreaDto>(area), "Area returned", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting area by id");
            return new("Error getting area by id", false);
        }
    }

    public async Task<ServiceResponse<IEnumerable<AreaDto>>> GetAreas()
    {
        try
        {
            var areas = await context.Areas.Get();
            if (areas == null)
            {
                return new("Areas not found", false);
            }
            return new(_mapper.Map<IEnumerable<AreaDto>>(areas), "List of areas returned", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all areas");
            return new("Error getting all areas", false);
        }
    }

    public async Task<ServiceResponse<IEnumerable<AreaDto>>> GetAreasWithSpaces()
    {
        try
        {
            var areas = await context.Areas.Get(include: a => a.Include(s => s.Spaces));
            if (areas == null)
            {
                return new("Areas not found", false);
            }
            return new(_mapper.Map<IEnumerable<AreaDto>>(areas), "List of areas with spaces returned", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting areas with spaces");
            return new("Error getting areas with spaces", false);
        }
    }

    public async Task<ServiceResponse> UpdateArea(int id, string userId, AreaUpdateDto areaUpdateDto)
    {
        try
        {
            var area = await context.Areas.FirstOrDefaultAsync(id);
            if (area == null)
            {
                return new("Area not found", false);
            }
            var exists = context.Areas.FirstOrDefaultAsync(a => a.AreaId != id && a.PlantId == area.PlantId && a.IsDeleted == false && Equals(a.Name.ToLower().Trim(), areaUpdateDto.Name.ToLower().Trim()));
            if (exists != null)
            {
                return new($"Area with name {areaUpdateDto.Name} already exists", false);
            }

            area.Description = areaUpdateDto.Description;
            area.Name = areaUpdateDto.Name;
            area.UserId = userId;
            var result = await context.Areas.Update(area);
            if (result)
            {
                await context.SaveChangesAsync();
                return new("Area updated", true);
            }
            else
            {
                return new("Area not updated", false);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating area");
            return new("Error updating area", false);
        }
    }

    public async Task<ServiceResponse> MarkDeleteArea(int id, string userId)
    {
        try
        {
            var area = await context.Areas.FirstOrDefaultAsync(a => a.AreaId == id, a => a.Include(s => s.Spaces));
            if (area == null)
            {
                return new("Area not found", false);
            }
            if (area.Spaces.Count > 0)
            {
                return new("Cannot delete area with spaces", false);
            }
            area.IsDeleted = true;
            area.UserId = userId;

            var result = await context.Areas.Update(area);
            if (result)
            {
                await context.SaveChangesAsync();
                return new("Area marked as deleted", true);
            }
            else
            {
                _logger.LogError("Area not marked as deleted");
                return new("Area not marked as deleted", false);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking area as deleted");
            return new("Error deleting area", false);
        }
    }
    public async Task<ServiceResponse> MarkUnDeleteArea(int id, string userId)
    {
        try
        {
            var area = await context.Areas.FirstOrDefaultAsync(a => a.AreaId == id);
            if (area == null)
            {
                return new("Area not found", false);
            }
            var exists = context.Areas.FirstOrDefaultAsync(a => a.AreaId != id && a.PlantId == area.PlantId && a.IsDeleted == false && Equals(a.Name.ToLower().Trim(), area.Name.ToLower().Trim()));
            if (exists != null)
            {
                return new($"Area with name {area.Name} already exists", false);
            }
            area.IsDeleted = false;
            area.UserId = userId;

            var result = await context.Areas.Update(area);
            if (result)
            {
                await context.SaveChangesAsync();
                return new("Area marked as undeleted", true);
            }
            else
            {
                _logger.LogError("Area not marked as undeleted");
                return new("Area not marked as undeleted", false);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking area as undeleted");
            return new("Error deleting area", false);
        }
    }
    public async Task<ServiceResponse> DeleteArea(int id)
    {
        try
        {
            var area = await context.Areas.FirstOrDefaultAsync(id);
            if (area == null)
            {
                _logger.LogError("Area not found");
                return new("Area not found", false);
            }
            if (area.IsDeleted == false)
            {
                _logger.LogError("Area not marked as deleted");
                return new("Area not marked as deleted", false);
            }
            var result = await context.Areas.Remove(area);
            if (result)
            {
                await context.SaveChangesAsync();
                return new("Area deleted", true);
            }
            else
            {
                _logger.LogError("Area not deleted");
                return new("Area not deleted", false);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting area");
            return new("Error deleting area", false);
        }
    }
    #endregion
    #region space service
    public async Task<ServiceResponse> CreateSpace(int areaId, SpaceCreateDto spaceCreateDto, string userId)
    {
        try
        {
            var area = await context.Areas.FirstOrDefaultAsync(a => a.AreaId == areaId, a => a.Include(s => s.Spaces));
            if (area == null)
            {
                _logger.LogWarning("Cannot create space for area with id {areaId}", areaId);
                return new($"Cannot create space for area with id {areaId}", false);
            }
            if (area.Spaces.Any(s => !s.IsDeleted && Equals(s.Name.ToLower().Trim(), spaceCreateDto.Name.ToLower().Trim())))
            {
                _logger.LogWarning("Space with name {spaceName} already exists", spaceCreateDto.Name);
                return new($"Space with name {spaceCreateDto.Name} already exists", false);
            }
            Space space = new();
            space.UserId = userId;
            space.AreaId = areaId;
            space.Name = spaceCreateDto.Name;
            space.Description = spaceCreateDto.Description;
            var result = await context.Spaces.Add(space);
            if (result)
            {
                await context.SaveChangesAsync();
                return new("Space created", true);
            }
            else
            {
                _logger.LogError("Space not created");
                return new("Space not created", false);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating space");
            return new("Error creating space", false);
        }
    }

    public async Task<ServiceResponse<SpaceDto>> GetSpaceById(int id)
    {
        try
        {
            var space = await context.Spaces.FirstOrDefaultAsync(id);
            if (space == null)
            {
                return new("Space not found", false);
            }
            return new(_mapper.Map<SpaceDto>(space), "Space returned", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting space by id");
            return new("Error getting space by id", false);
        }
    }

    public async Task<ServiceResponse<IEnumerable<SpaceDto>>> GetSpaces()
    {
        try
        {
            var spaces = await context.Spaces.Get();
            if (spaces == null)
            {
                return new("Spaces not found", false);
            }
            return new(_mapper.Map<IEnumerable<SpaceDto>>(spaces), "List of spaces returned", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all spaces");
            return new("Error getting all spaces", false);
        }
    }

    public async Task<ServiceResponse<IEnumerable<SpaceDto>>> GetSpacesWithCoordinates()
    {
        try
        {
            var spaces = await context.Spaces.Get(include: s => s.Include(c => c.Coordinates));
            if (spaces == null)
            {
                return new("Spaces not found", false);
            }
            return new(_mapper.Map<IEnumerable<SpaceDto>>(spaces), "List of spaces with coordinates returned", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting spaces with coordinates");
            return new("Error getting spaces with coordinates", false);
        }
    }

    public async Task<ServiceResponse> UpdateSpace(int id, string userId, SpaceUpdateDto spaceUpdateDto)
    {
        try
        {
            var space = await context.Spaces.FirstOrDefaultAsync(id);
            if (space == null)
            {
                return new("Space not found", false);
            }
            var exists = await context.Spaces.Get(s => s.IsDeleted == false && s.SpaceId != id && Equals(s.Name.ToLower().Trim(), spaceUpdateDto.Name.ToLower().Trim()));
            if (exists.Any())
            {
                return new($"Space with name {spaceUpdateDto.Name} already exists", false);
            }
            space.Name = spaceUpdateDto.Name;
            space.Description = spaceUpdateDto.Description;
            space.SpaceType = spaceUpdateDto.SpaceType;
            space.UserId = userId;
            var result = await context.Spaces.Update(space);
            if (result)
            {
                await context.SaveChangesAsync();
                return new("Space updated", true);
            }
            else
            {
                _logger.LogError("Space with id {id} not updated", id);
                return new("Space not updated", false);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating space");
            return new("Error updating space", false);
        }
    }

    public async Task<ServiceResponse> MarkDeleteSpace(int id, string userId)
    {
        try
        {
            var space = await context.Spaces.FirstOrDefaultAsync(s => s.SpaceId == id, s => s.Include(c => c.Coordinates));
            if (space == null)
            {
                return new("Space not found", false);
            }
            if (space.Coordinates.Count > 0)
            {
                return new("Cannot delete space with coordinates", false);
            }
            space.IsDeleted = true;
            space.UserId = userId;
            var result = await context.Spaces.Update(space);
            if (result)
            {
                await context.SaveChangesAsync();
                return new("Space marked as deleted", true);
            }
            else
            {
                _logger.LogError("Space not deleted");
                return new("Space not deleted", false);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking space as deleted");
            return new("Error marking space as deleted", false);
        }
    }
    public async Task<ServiceResponse> MarkUnDeleteSpace(int id, string userId)
    {
        try
        {
            var space = await context.Spaces.FirstOrDefaultAsync(s => s.SpaceId == id);
            if (space == null)
            {
                return new("Space not found", false);
            }
            if (!space.IsDeleted)
            {
                return new("Space is not marked as deleted", false);
            }
            var exists = await context.Spaces.Get(s => s.IsDeleted == false && s.SpaceId != id && Equals(s.Name.ToLower().Trim(), space.Name.ToLower().Trim()));
            if (exists.Any())
            {
                return new($"Space with name {space.Name} already exists", false);
            }
            space.IsDeleted = false;
            space.UserId = userId;
            var result = await context.Spaces.Update(space);
            if (result)
            {
                await context.SaveChangesAsync();
                return new("Space marked as undeleted", true);
            }
            else
            {
                _logger.LogError("Space not undeleted");
                return new("Space not undeleted", false);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking space as undeleted");
            return new("Error undeleting space", false);
        }
    }
    public async Task<ServiceResponse> DeleteSpace(int id)
    {
        try
        {
            var space = await context.Spaces.FirstOrDefaultAsync(id);
            if (space == null)
            {
                _logger.LogError("Space not found");
                return new("Space not found", false);
            }
            if (space.IsDeleted == false)
            {
                _logger.LogError("Space not marked as deleted");
                return new("Space not marked as deleted", false);
            }
            var result = await context.Spaces.Remove(space);
            if (result)
            {
                await context.SaveChangesAsync();
                return new("Space deleted", true);
            }
            else
            {
                _logger.LogError("Space not deleted");
                return new("Space not deleted", false);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting space");
            return new("Error deleting space", false);
        }
    }
    #endregion
    #region coordinate service
    public async Task<ServiceResponse> CreateCoordinate(int spaceId, CoordinateCreateDto coordinateCreateDto, string userId)
    {
        try
        {
            var space = await context.Spaces.FirstOrDefaultAsync(s => s.SpaceId == spaceId, s => s.Include(c => c.Coordinates));
            if (space == null)
            {
                _logger.LogWarning("Cannot create coordinate for space with id {spaceId}", spaceId);
                return new($"Cannot create coordinate for space with id {spaceId}", false);
            }
            if (space.Coordinates.Any(c => !c.IsDeleted && Equals(c.Name.ToLower().Trim(), coordinateCreateDto.Name.ToLower().Trim())))
            {
                _logger.LogWarning("Coordinate with name {coordinateName} already exists", coordinateCreateDto.Name);
                return new($"Coordinate with name {coordinateCreateDto.Name} already exists", false);
            }


            Coordinate coordinate = new();
            coordinate.Name = coordinateCreateDto.Name;
            coordinate.Description = coordinateCreateDto.Description;
            coordinate.SpaceId = spaceId;
            coordinate.UserId = userId;
            var result = await context.Coordinates.Add(coordinate);
            if (result)
            {
                await context.SaveChangesAsync();
                return new("Coordinate created", true);
            }
            return new("Coordinate not created", false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating coordinate");
            return new("Error creating coordinate", false);
        }
    }

    public async Task<ServiceResponse<CoordinateDto>> GetCoordinateById(int id)
    {
        try
        {
            var coordinate = await context.Coordinates.FirstOrDefaultAsync(id);
            if (coordinate == null)
            {
                return new("Coordinate not found", false);
            }
            return new(_mapper.Map<CoordinateDto>(coordinate), "Coordinate returned", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting coordinate by id");
            return new("Error getting coordinate by id", false);
        }
    }

    public async Task<ServiceResponse<IEnumerable<CoordinateDto>>> GetCoordinates()
    {
        try
        {
            var coordinates = await context.Coordinates.Get();
            if (coordinates == null)
            {
                return new("Coordinates not found", false);
            }
            return new(_mapper.Map<IEnumerable<CoordinateDto>>(coordinates), "List of coordinates returned", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all coordinates");
            return new("Error getting all coordinates", false);
        }
    }

    public async Task<ServiceResponse<IEnumerable<CoordinateDto>>> GetCoordinatesWithAssets()
    {
        try
        {
            var coordinates = await context.Coordinates.Get(include: c => c.Include(a => a.Assets));
            if (coordinates == null)
            {
                return new("Coordinates not found", false);
            }
            return new(_mapper.Map<IEnumerable<CoordinateDto>>(coordinates), "List of coordinates with assets returned", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all coordinates with assets");
            return new("Error getting all coordinates with assets", false);
        }
    }

    public async Task<ServiceResponse> UpdateCoordinate(int id, string userId, CoordinateUpdateDto coordinateUpdateDto)
    {
        try
        {
            var coordinate = await context.Coordinates.FirstOrDefaultAsync(id);
            if (coordinate == null)
            {
                return new("Coordinate not found", false);
            }
            var exists = await context.Coordinates.Get(c => Equals(c.Name.ToLower().Trim(), coordinateUpdateDto.Name.ToLower().Trim()) && c.CoordinateId != id && c.IsDeleted == false && c.Space.Area.PlantId == coordinate.Space.Area.PlantId, include: c => c.Include(a => a.Space).ThenInclude(a => a.Area));
            if (exists.Any())
            {
                return new($"Coordinate with name {coordinateUpdateDto.Name} already exists", false);
            }
            coordinate.Name = coordinateUpdateDto.Name;
            coordinate.Description = coordinateUpdateDto.Description;
            coordinate.UserId = userId;
            var result = await context.Coordinates.Update(coordinate);
            if (result)
            {
                await context.SaveChangesAsync();
                return new("Coordinate updated", true);
            }
            else
            {
                _logger.LogError("Coordinate not updated");
                return new("Coordinate not updated", false);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating coordinate");
            return new("Error updating coordinate", false);
        }
    }

    public async Task<ServiceResponse> MarkDeleteCoordinate(int id, string userId)
    {
        try
        {
            var coordinate = await context.Coordinates.FirstOrDefaultAsync(c => c.CoordinateId == id, c => c.Include(a => a.Assets));
            if (coordinate == null)
            {
                return new("Coordinate not found", false);
            }
            if (coordinate.Assets.Any())
            {
                return new("Cannot delete coordinate with assets", false);
            }
            if (coordinate.IsDeleted)
            {
                return new("Coordinate already marked as deleted", false);
            }
            coordinate.IsDeleted = true;
            coordinate.UserId = userId;
            var result = await context.Coordinates.Update(coordinate);
            if (result)
            {
                await context.SaveChangesAsync();
                return new("Coordinate deleted", true);
            }
            else
            {
                _logger.LogError("Coordinate not deleted");
                return new("Coordinate not deleted", false);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting coordinate");
            return new("Error deleting coordinate", false);
        }
    }
    public async Task<ServiceResponse> MarkUnDeleteCoordinate(int id, string userId)
    {
        try
        {
            var coordinate = await context.Coordinates.FirstOrDefaultAsync(c => c.CoordinateId == id);
            if (coordinate == null)
            {
                return new("Coordinate not found", false);
            }
            if (!coordinate.IsDeleted)
            {
                return new("Coordinate already marked as not deleted", false);
            }
            var exists = await context.Coordinates.Get(c => Equals(c.Name.ToLower().Trim(), coordinate.Name.ToLower().Trim()) && c.CoordinateId != id && c.IsDeleted == false && c.Space.Area.PlantId == coordinate.Space.Area.PlantId, include: c => c.Include(a => a.Space).ThenInclude(a => a.Area));
            if (exists.Any())
            {
                return new($"Coordinate with name {coordinate.Name} already exists", false);
            }
            coordinate.IsDeleted = false;
            coordinate.UserId = userId;
            var result = await context.Coordinates.Update(coordinate);
            if (result)
            {
                await context.SaveChangesAsync();
                return new("Coordinate un-deleted", true);
            }
            else
            {
                _logger.LogError("Coordinate not un-deleted");
                return new("Coordinate not un-deleted", false);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error un-deleting coordinate");
            return new("Error un-deleting coordinate", false);
        }
    }
    public async Task<ServiceResponse> DeleteCoordinate(int id)
    {
        try
        {
            var coordinate = await context.Coordinates.FirstOrDefaultAsync(id);
            if (coordinate == null)
            {
                _logger.LogError("Coordinate not found");
                return new("Coordinate not found", false);
            }
            if (coordinate.IsDeleted == false)
            {
                _logger.LogError("Coordinate not marked as deleted");
                return new("Coordinate not marked as deleted", false);
            }
            var result = await context.Coordinates.Remove(coordinate);
            if (result)
            {
                await context.SaveChangesAsync();
                return new("Coordinate deleted", true);
            }
            else
            {
                _logger.LogError("Coordinate not deleted");
                return new("Coordinate not deleted", false);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting coordinate");
            return new("Error deleting coordinate", false);
        }
    }
    #endregion
    #region asset service
    public async Task<ServiceResponse> CreateAsset(AssetCreateDto assetCreateDto, string userId)
    {
        try
        {
            var coordinate = await context.Coordinates.FirstOrDefaultAsync(c => c.CoordinateId == assetCreateDto.CoordinateId, c => c.Include(a => a.Assets));
            if (coordinate == null)
            {
                _logger.LogWarning("Cannot create asset for coordinate with id {coordinateId}", assetCreateDto.CoordinateId);
                return new($"Cannot create asset for coordinate with id {assetCreateDto.CoordinateId}", false);
            }
            if (coordinate.Assets.Any(a => !a.IsDeleted && Equals(a.Name.ToLower().Trim(), assetCreateDto.Name.ToLower().Trim())))
            {
                _logger.LogWarning("Asset with name {assetName} already exists", assetCreateDto.Name);
                return new($"Asset with name {assetCreateDto.Name} already exists", false);
            }
            var model = await context.Models.FirstOrDefaultAsync(m => m.ModelId == assetCreateDto.ModelId);
            if (model == null)
            {
                _logger.LogWarning("Cannot create asset for model with id {modelId}", assetCreateDto.ModelId);
                return new($"Cannot create asset for model with id {assetCreateDto.ModelId}", false);
            }

            Asset asset = new();
            asset.Name = assetCreateDto.Name;
            asset.ModelId = assetCreateDto.ModelId;
            asset.CoordinateId = assetCreateDto.CoordinateId;
            asset.UserId = userId;
            var result = await context.Assets.Add(asset);
            if (result)
            {
                await context.SaveChangesAsync();
                return new("Asset created", true);
            }
            else
            {
                _logger.LogError("Asset not created");
                return new("Asset not created", false);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating asset");
            return new("Error creating asset", false);
        }
    }

    public async Task<ServiceResponse<AssetDto>> GetAssetById(int id)
    {
        try
        {
            var asset = await context.Assets.FirstOrDefaultAsync(a => a.AssetId == id, a => a.Include(a => a.AssetDetails).Include(a => a.AssetCategories));
            if (asset == null)
            {
                return new("Asset not found", false);
            }
            return new(_mapper.Map<AssetDto>(asset), "Asset returned", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting asset by id");
            return new("Error getting asset by id", false);
        }
    }

    public async Task<ServiceResponse<IEnumerable<AssetDto>>> GetAssets()
    {
        try
        {
            var assets = await context.Assets.Get(include: a => a.Include(a => a.AssetDetails).Include(a => a.AssetCategories));
            if (assets == null)
            {
                return new("Assets not found", false);
            }
            return new(_mapper.Map<IEnumerable<AssetDto>>(assets), "List of assets returned", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all assets");
            return new("Error getting all assets", false);
        }
    }

    public async Task<ServiceResponse<IEnumerable<AssetDto>>> GetAssetsWithAllData()
    {
        try
        {
            var assets = await context.Assets.Get(include: a => a.Include(a => a.AssetDetails).Include(a => a.AssetCategories).Include(a => a.Coordinate).Include(c => c.CommunicateAssets).Include(a => a.SituationAssets).Include(a => a.Model));
            if (assets == null)
            {
                return new("Assets not found", false);
            }
            return new(_mapper.Map<IEnumerable<AssetDto>>(assets), "List of assets returned", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all assets");
            return new("Error getting all assets", false);
        }
    }

    public async Task<ServiceResponse> UpdateAsset(int id, string userId, AssetUpdateDto assetUpdateDto)
    {
        try
        {
            var asset = await context.Assets.FirstOrDefaultAsync(a => a.AssetId == id, a => a.Include(a => a.AssetDetails).Include(a => a.AssetCategories));
            if (asset == null)
            {
                _logger.LogWarning("Asset not found");
                return new("Asset not found", false);
            }
            if (asset.IsDeleted)
            {
                _logger.LogWarning("Asset marked as deleted");
                return new("Asset marked as deleted", false);
            }
            // check if asset name is unique
            var exists = await context.Assets.Get(a => Equals(a.Name.ToLower().Trim(), assetUpdateDto.Name.ToLower().Trim()) && a.AssetId != id && a.IsDeleted == false);
            if (exists.Any())
            {
                _logger.LogWarning("Asset with name {assetName} already exists", assetUpdateDto.Name);
                return new($"Asset with name {assetUpdateDto.Name} already exists", false);
            }
            var coordinate = await context.Coordinates.FirstOrDefaultAsync(assetUpdateDto.CoordinateId);
            if (coordinate == null || coordinate.IsDeleted)
            {
                _logger.LogWarning("Cannot update asset to coordinate with id {coordinateId}", assetUpdateDto.CoordinateId);
                return new($"Cannot update asset to coordinate with id {assetUpdateDto.CoordinateId}", false);
            }

            asset.Status = assetUpdateDto.Status;
            asset.Name = assetUpdateDto.Name;
            asset.Process = assetUpdateDto.Process;
            asset.UserId = userId;
            asset.AssetDetails = asset.AssetDetails.Where(ad => assetUpdateDto.AssetDetails.Any(a => a.AssetDetailId == ad.AssetDetailId)).ToList();
            asset.AssetCategories = asset.AssetCategories.Where(ac => assetUpdateDto.AssetCategories.Any(a => a.AssetCategoryId == ac.AssetCategoryId)).ToList();
            foreach (var assetDetailDto in assetUpdateDto.AssetDetails)
            {
                var assetDetailToUpdate = asset.AssetDetails.FirstOrDefault(ad => ad.AssetDetailId == assetDetailDto.AssetDetailId);
                if (assetDetailToUpdate == null)
                {
                    AssetDetail newAssetDetail = new()
                    {
                        AssetId = asset.AssetId,
                        DetailId = assetDetailDto.DetailId,
                        Value = assetDetailDto.Value,
                        UserId = userId
                    };
                    asset.AssetDetails.Add(newAssetDetail);
                }
                if (assetDetailToUpdate != null && !Equals(assetDetailToUpdate.Value = assetDetailDto.Value))
                {
                    assetDetailToUpdate.Value = assetDetailDto.Value;
                    assetDetailToUpdate.UserId = userId;
                }
            }

            foreach (var assetCategoryDto in assetUpdateDto.AssetCategories)
            {
                var assetCategoryToUpdate = asset.AssetCategories.FirstOrDefault(ac => ac.AssetCategoryId == assetCategoryDto.AssetCategoryId);
                if (assetCategoryToUpdate == null)
                {
                    AssetCategory newAssetCategory = new()
                    {
                        AssetId = asset.AssetId,
                        CategoryId = assetCategoryDto.CategoryId,
                        UserId = userId
                    };
                    asset.AssetCategories.Add(newAssetCategory);
                }

            }
            var result = await context.Assets.Update(asset);
            if (result)
            {
                _logger.LogInformation("Asset with id {assetId} updated", id);
                return new($"Asset {id} updated", true);
            }
            _logger.LogWarning("Asset with id {assetId} not updated", id);
            return new($"Asset {id} not updated", false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating asset with id {assetId}", id);
            return new($"Error updating asset with id {id}", false);
        }
    }
    public async Task<ServiceResponse> AssetChangeModel(int assetId, string userId, int modelId)
    {
        try
        {
            var model = await context.Models.FirstOrDefaultAsync(modelId);
            if (model == null)
            {
                _logger.LogWarning("Model not found");
                return new("Model not found", false);
            }
            var asset = await context.Assets.FirstOrDefaultAsync(assetId);
            if (asset == null)
            {
                _logger.LogWarning("Asset not found");
                return new("Asset not found", false);
            }
            if (asset.IsDeleted)
            {
                _logger.LogWarning("Asset marked as deleted");
                return new("Asset marked as deleted", false);
            }
            asset.AssetDetails = new List<AssetDetail>();
            asset.AssetCategories = new List<AssetCategory>();
            asset.CommunicateAssets = new List<CommunicateAsset>();
            asset.SituationAssets = new List<SituationAsset>();
            asset.ModelId = modelId;
            asset.UserId = userId;
            var result = await context.Assets.Update(asset);
            if (result)
            {
                _logger.LogInformation("Asset with id {assetId} updated", assetId);
                return new($"Asset {assetId} updated", true);
            }
            _logger.LogWarning("Asset with id {assetId} not updated", assetId);
            return new($"Asset {assetId} not updated", false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating asset with id {assetId}", assetId);
            return new($"Error updating asset with id {assetId}", false);
        }
    }

    public async Task<ServiceResponse> MarkDeleteAsset(int id, string userId)
    {
        try
        {
            var asset = await context.Assets.FirstOrDefaultAsync(id);
            if (asset == null)
            {
                _logger.LogWarning("Asset not found");
                return new("Asset not found", false);
            }
            if (asset.IsDeleted)
            {
                _logger.LogWarning("Asset marked as deleted");
                return new("Asset marked as deleted", false);
            }
            asset.IsDeleted = true;
            asset.UserId = userId;
            var result = await context.Assets.Update(asset);
            if (result)
            {
                _logger.LogInformation("Asset with id {assetId} marked as deleted", id);
                return new($"Asset {id} marked as deleted", true);
            }
            _logger.LogWarning("Asset with id {assetId} not marked as deleted", id);
            return new($"Asset {id} not marked as deleted", false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking asset with id {assetId} as deleted", id);
            return new($"Error marking asset with id {id} as deleted", false);
        }
    }
    public async Task<ServiceResponse> MarkUnDeleteAsset(int id, string userId)
    {
        try
        {
            var asset = await context.Assets.FirstOrDefaultAsync(id);
            if (asset == null)
            {
                _logger.LogWarning("Asset not found");
                return new("Asset not found", false);
            }
            if (!asset.IsDeleted)
            {
                _logger.LogWarning("Asset not marked as deleted");
                return new("Asset not marked as deleted", false);
            }
            asset.IsDeleted = false;
            asset.UserId = userId;
            var result = await context.Assets.Update(asset);
            if (result)
            {
                _logger.LogInformation("Asset with id {assetId} marked as not deleted", id);
                return new($"Asset {id} marked as not deleted", true);
            }
            _logger.LogWarning("Asset with id {assetId} not marked as not deleted", id);
            return new($"Asset {id} not marked as not deleted", false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking asset with id {assetId} as not deleted", id);
            return new($"Error marking asset with id {id} as not deleted", false);
        }
    }
    public async Task<ServiceResponse> DeleteAsset(int id)
    {
        try
        {
            var asset = await context.Assets.FirstOrDefaultAsync(id);
            if (asset == null)
            {
                _logger.LogWarning("Asset not found");
                return new("Asset not found", false);
            }
            var result = await context.Assets.Remove(asset);
            if (result)
            {
                _logger.LogInformation("Asset with id {assetId} deleted", id);
                return new($"Asset {id} deleted", true);
            }
            _logger.LogWarning("Asset with id {assetId} not deleted", id);
            return new($"Asset {id} not deleted", false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting asset with id {assetId}", id);
            return new($"Error deleting asset with id {id}", false);
        }
    }
    #endregion
    #region device service
    public async Task<ServiceResponse> CreateDevice(DeviceCreateDto deviceCreateDto, string userId)
    {
        try
        {
            Device device = new();
            var exist = await context.Devices.FirstOrDefaultAsync(p => Equals(p.Name.ToLower().Trim(), device.Name.ToLower().Trim()));
            if (exist != null && exist.IsDeleted == false)
            {
                return new("Device already exists", false);
            }
            device.UserId = userId;
            device.Name = deviceCreateDto.Name;
            device.Description = deviceCreateDto.Description;
            var result = await context.Devices.Add(device);
            if (result)
            {
                await context.SaveChangesAsync();
                return new("Device created", true);
            }
            else
            {
                _logger.LogWarning("Device not created");
                return new("Device not created", false);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating device");
            return new("Error creating device", false);
        }
    }

    public async Task<ServiceResponse<DeviceDto>> GetDeviceById(int id)
    {
        try
        {
            var device = await context.Devices.FirstOrDefaultAsync(id);
            if (device == null)
            {
                return new("Plant not found", false);
            }
            return new(_mapper.Map<DeviceDto>(device), "Device returned", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting device by id");
            return new("Error getting device by id", false);
        }
    }

    public async Task<ServiceResponse<IEnumerable<DeviceDto>>> GetDevices()
    {
        try
        {
            var devices = await context.Devices.Get();
            if (devices == null)
            {
                return new("Devices not found", false);
            }
            return new(_mapper.Map<IEnumerable<DeviceDto>>(devices), "Devices returned", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting devices");
            return new("Error getting devices", false);
        }
    }

    public async Task<ServiceResponse<IEnumerable<DeviceDto>>> GetDevicesWithModels()
    {
        try
        {
            var devices = await context.Devices.Get(include: p => p.Include(d => d.Models));
            if (devices == null)
            {
                return new("Devices not found", false);
            }
            return new(_mapper.Map<IEnumerable<DeviceDto>>(devices), "Devices returned", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting devices");
            return new("Error getting devices", false);
        }
    }

    public async Task<ServiceResponse> UpdateDevice(int id, string userId, DeviceUpdateDto deviceUpdateDto)
    {
        try
        {
            var device = await context.Devices.FirstOrDefaultAsync(id);
            if (device == null)
            {
                _logger.LogWarning("Device not found");
                return new("Device not found", false);
            }
            device.UserId = userId;
            device.Name = deviceUpdateDto.Name;
            device.Description = deviceUpdateDto.Description;




            var result = await context.Devices.Update(device);
            if (result)
            {
                _logger.LogInformation("Device with id {deviceId} updated", id);
                return new($"Device {id} updated", true);
            }
            _logger.LogWarning("Device with id {deviceId} not updated", id);
            return new($"Device {id} not updated", false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating device with id {deviceId}", id);
            return new($"Error updating device with id {id}", false);
        }
    }

    public async Task<ServiceResponse> MarkDeleteDevice(int id, string userId)
    {
        throw new NotImplementedException();
    }
    public async Task<ServiceResponse> MarkUnDeleteDevice(int id, string userId)
    {
        throw new NotImplementedException();
    }
    public async Task<ServiceResponse> DeleteDevice(int id)
    {
        throw new NotImplementedException();
    }
    #endregion
    #region model service
    public async Task<ServiceResponse> CreateModel(int deviceId, ModelCreateDto modelCreateDto, string userId)
    {
        throw new NotImplementedException();
    }

    public async Task<ServiceResponse<ModelDto>> GetModelById(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<ServiceResponse<IEnumerable<ModelDto>>> GetModels()
    {
        throw new NotImplementedException();
    }

    public async Task<ServiceResponse<IEnumerable<ModelDto>>> GetModelsWithAssets()
    {
        throw new NotImplementedException();
    }

    public async Task<ServiceResponse> UpdateModel(int id, string userId, ModelUpdateDto modelUpdateDto)
    {
        throw new NotImplementedException();
    }

    public async Task<ServiceResponse> MarkDeleteModel(int id, string userId)
    {
        throw new NotImplementedException();
    }
    public async Task<ServiceResponse> MarkUnDeleteModel(int id, string userId)
    {
        throw new NotImplementedException();
    }
    public async Task<ServiceResponse> DeleteModel(int id)
    {
        throw new NotImplementedException();
    }
    #endregion
    #region situation service
    public async Task<ServiceResponse> CreateSituation(SituationCreateDto situationCreateDto, string userId)
    {
        throw new NotImplementedException();
    }

    public async Task<ServiceResponse<SituationDto>> GetSituationById(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<ServiceResponse<IEnumerable<SituationDto>>> GetSituations()
    {
        throw new NotImplementedException();
    }

    public async Task<ServiceResponse<IEnumerable<SituationDto>>> GetSituationsWithAssets()
    {
        throw new NotImplementedException();
    }

    public async Task<ServiceResponse<IEnumerable<SituationDto>>> GetSituationsWithCategories()
    {
        throw new NotImplementedException();
    }

    public async Task<ServiceResponse> UpdateSituation(int id, string userId, SituationUpdateDto situationUpdateDto)
    {
        throw new NotImplementedException();
    }

    public async Task<ServiceResponse> MarkDeleteSituation(int id, string userId)
    {
        throw new NotImplementedException();
    }
    public async Task<ServiceResponse> MarkUnDeleteSituation(int id, string userId)
    {
        throw new NotImplementedException();
    }
    public async Task<ServiceResponse> DeleteSituation(int id)
    {
        throw new NotImplementedException();
    }
    #endregion
    #region category service
    public async Task<ServiceResponse> CreateCategory(CategoryCreateDto categoryCreateDto, string userId)
    {
        throw new NotImplementedException();
    }

    public async Task<ServiceResponse<CategoryDto>> GetCategoryById(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<ServiceResponse<IEnumerable<CategoryDto>>> GetCategories()
    {
        throw new NotImplementedException();
    }

    public async Task<ServiceResponse<IEnumerable<CategoryDto>>> GetCategoriesWithAssets()
    {
        throw new NotImplementedException();
    }

    public async Task<ServiceResponse> UpdateCategory(int id, string userId, CategoryUpdateDto categoryUpdateDto)
    {
        throw new NotImplementedException();
    }

    public async Task<ServiceResponse> MarkDeleteCategory(int id, string userId)
    {
        throw new NotImplementedException();
    }
    public async Task<ServiceResponse> MarkUnDeleteCategory(int id, string userId)
    {
        throw new NotImplementedException();
    }
    public async Task<ServiceResponse> DeleteCategory(int id)
    {
        throw new NotImplementedException();
    }
    #endregion
    #region communicate service
    public async Task<ServiceResponse> CreateCommunicate(CommunicateCreateDto communicateCreateDto, string userId)
    {
        throw new NotImplementedException();
    }

    public async Task<ServiceResponse<CommunicateDto>> GetCommunicateById(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<ServiceResponse<IEnumerable<CommunicateDto>>> GetCommunicates()
    {
        throw new NotImplementedException();
    }

    public async Task<ServiceResponse<IEnumerable<CommunicateDto>>> GetCommunicatesWithAssets()
    {
        throw new NotImplementedException();
    }

    public async Task<ServiceResponse> UpdateCommunicate(int id, string userId, CommunicateUpdateDto communicateUpdateDto)
    {
        throw new NotImplementedException();
    }

    public async Task<ServiceResponse> MarkDeleteCommunicate(int id, string userId)
    {
        throw new NotImplementedException();
    }
    public async Task<ServiceResponse> MarkUnDeleteCommunicate(int id, string userId)
    {
        throw new NotImplementedException();
    }
    public async Task<ServiceResponse> DeleteCommunicate(int id)
    {
        throw new NotImplementedException();
    }
    #endregion
    #region detail service
    public async Task<ServiceResponse> CreateDetail(DetailCreateDto detailCreateDto, string userId)
    {
        throw new NotImplementedException();
    }

    public async Task<ServiceResponse<DetailDto>> GetDetailById(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<ServiceResponse<IEnumerable<DetailDto>>> GetDetails()
    {
        throw new NotImplementedException();
    }

    public async Task<ServiceResponse<IEnumerable<DetailDto>>> GetDetailsWithAssets()
    {
        throw new NotImplementedException();
    }

    public async Task<ServiceResponse> UpdateDetail(int id, string userId, DetailUpdateDto detailUpdateDto)
    {
        throw new NotImplementedException();
    }

    public async Task<ServiceResponse> MarkDeleteDetail(int id, string userId)
    {
        throw new NotImplementedException();
    }
    public async Task<ServiceResponse> MarkUnDeleteDetail(int id, string userId)
    {
        throw new NotImplementedException();
    }
    public async Task<ServiceResponse> DeleteDetail(int id)
    {
        throw new NotImplementedException();
    }
    #endregion
    #region parameter service
    public async Task<ServiceResponse> CreateParameter(ParameterCreateDto parameterCreateDto, string userId)
    {
        throw new NotImplementedException();
    }

    public async Task<ServiceResponse<ParameterDto>> GetParameterById(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<ServiceResponse<IEnumerable<ParameterDto>>> GetParameters()
    {
        throw new NotImplementedException();
    }

    public async Task<ServiceResponse<IEnumerable<ParameterDto>>> GetParametersWithModels()
    {
        throw new NotImplementedException();
    }

    public async Task<ServiceResponse> UpdateParameter(int id, string userId, ParameterUpdateDto parameterUpdateDto)
    {
        throw new NotImplementedException();
    }

    public async Task<ServiceResponse> MarkDeleteParameter(int id, string userId)
    {
        throw new NotImplementedException();
    }
    public async Task<ServiceResponse> MarkUnDeleteParameter(int id, string userId)
    {
        throw new NotImplementedException();
    }
    public async Task<ServiceResponse> DeleteParameter(int id)
    {
        throw new NotImplementedException();
    }
    #endregion
    #region question service
    public async Task<ServiceResponse> CreateQuestion(QuestionCreateDto questionCreateDto, string userId)
    {
        throw new NotImplementedException();
    }

    public async Task<ServiceResponse<QuestionDto>> GetQuestionById(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<ServiceResponse<IEnumerable<QuestionDto>>> GetQuestions()
    {
        throw new NotImplementedException();
    }

    public async Task<ServiceResponse<IEnumerable<QuestionDto>>> GetQuestionsWithSituations()
    {
        throw new NotImplementedException();
    }

    public async Task<ServiceResponse> UpdateQuestion(int id, string userId, QuestionUpdateDto questionUpdateDto)
    {
        throw new NotImplementedException();
    }

    public async Task<ServiceResponse> MarkDeleteQuestion(int id, string userId)
    {
        throw new NotImplementedException();
    }
    public async Task<ServiceResponse> MarkUnDeleteQuestion(int id, string userId)
    {
        throw new NotImplementedException();
    }
    public async Task<ServiceResponse> DeleteQuestion(int id)
    {
        throw new NotImplementedException();
    }
    #endregion
}