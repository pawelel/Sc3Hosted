using System.Collections.Generic;

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
            var plants = await _mapper.ProjectTo<PlantDto>(context.Plants).ToListAsync();
            if (plants == null)
            {
                return new("Plants not found", false);
            }
            return new(plants, "List of plants returned", true);
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
            var plant = await context.Plants.Include(a => a.Areas).FirstOrDefaultAsync(p => p.PlantId == id);
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
            var plant = await context.Plants.FirstOrDefaultAsync(p => p.PlantId == id);
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
            var plant = await context.Plants.FirstOrDefaultAsync(p => p.PlantId == id);
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
        using var context = _contextFactory.CreateDbContext();
        using IDbContextTransaction transaction = context.Database.BeginTransaction();
        try
        {
            var plant = await context.Plants.Include(a => a.Areas).FirstOrDefaultAsync(p => p.PlantId == plantId);
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
            context.Areas.Add(area);
            await context.SaveChangesAsync();
            transaction.Commit();
            return new("Area created", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating area");
            transaction.Rollback();
            return new("Error creating area", false);
        }
    }

    public async Task<ServiceResponse<AreaDto>> GetAreaById(int id)
    {
        using var context = _contextFactory.CreateDbContext();

        try
        {
            var area = await context.Areas.FirstOrDefaultAsync(a => a.AreaId == id);
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
        using var context = _contextFactory.CreateDbContext();
        try
        {
            var areas = await context.Areas.ToListAsync();
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
        using var context = _contextFactory.CreateDbContext();
        try
        {
            var areas = await context.Areas.Include(s => s.Spaces).ToListAsync();
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
        using var context = _contextFactory.CreateDbContext();
        using IDbContextTransaction transaction = context.Database.BeginTransaction();
        try
        {
            var area = await context.Areas.FirstOrDefaultAsync(a => a.AreaId == id);
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
            context.Areas.Update(area);

            await context.SaveChangesAsync();
            transaction.Commit();
            return new("Area updated", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating area");
            transaction.Rollback();
            return new("Error updating area", false);
        }
    }

    public async Task<ServiceResponse> MarkDeleteArea(int id, string userId)
    {
        using var context = _contextFactory.CreateDbContext();
        using IDbContextTransaction transaction = context.Database.BeginTransaction();
        try
        {
            var area = await context.Areas.Include(s => s.Spaces).FirstOrDefaultAsync(a => a.AreaId == id);
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

            context.Areas.Update(area);

            await context.SaveChangesAsync();
            transaction.Commit();
            return new("Area marked as deleted", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking area as deleted");
            transaction.Rollback();
            return new("Error deleting area", false);
        }
    }
    public async Task<ServiceResponse> MarkUnDeleteArea(int id, string userId)
    {
        using var context = _contextFactory.CreateDbContext();
        using IDbContextTransaction transaction = context.Database.BeginTransaction();
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

            context.Areas.Update(area);

            await context.SaveChangesAsync();
            transaction.Commit();
            return new("Area marked as undeleted", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking area as undeleted");
            transaction.Rollback();
            return new("Error deleting area", false);
        }
    }
    public async Task<ServiceResponse> DeleteArea(int id)
    {
        using var context = _contextFactory.CreateDbContext();
        using IDbContextTransaction transaction = context.Database.BeginTransaction();
        try
        {
            var area = await context.Areas.FirstOrDefaultAsync(a => a.AreaId == id);
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
            context.Areas.Remove(area);
            await context.SaveChangesAsync();
            transaction.Commit();
            return new("Area deleted", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting area");
            transaction.Rollback();
            return new("Error deleting area", false);
        }
    }
    #endregion
    #region space service
    public async Task<ServiceResponse> CreateSpace(int areaId, SpaceCreateDto spaceCreateDto, string userId)
    {
        using var context = _contextFactory.CreateDbContext();
        using IDbContextTransaction transaction = context.Database.BeginTransaction();
        try
        {
            var area = await context.Areas.Include(s => s.Spaces).FirstOrDefaultAsync(a => a.AreaId == areaId);
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
            context.Spaces.Add(space);
            await context.SaveChangesAsync();
            transaction.Commit();
            return new("Space created", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating space");
            transaction.Rollback();
            return new("Error creating space", false);
        }
    }

    public async Task<ServiceResponse<SpaceDto>> GetSpaceById(int id)
    {
        using var context = _contextFactory.CreateDbContext();
        try
        {
            var space = await context.Spaces.FirstOrDefaultAsync(s => s.SpaceId == id);
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
        using var context = _contextFactory.CreateDbContext();
        try
        {
            var spaces = await context.Spaces.FirstOrDefaultAsync();
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
        using var context = _contextFactory.CreateDbContext();
        try
        {
            var spaces = await context.Spaces.Include(c => c.Coordinates).ToListAsync();
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
        using var context = _contextFactory.CreateDbContext();
        using IDbContextTransaction transaction = context.Database.BeginTransaction();
        try
        {
            var space = await context.Spaces.FirstOrDefaultAsync(s => s.SpaceId == id);
            if (space == null)
            {
                return new("Space not found", false);
            }
            var exists = await context.Spaces
                .Where(s => s.IsDeleted == false)
                .Where(s => s.SpaceId != id)
                .Where(s => Equals(s.Name.ToLower().Trim(), spaceUpdateDto.Name.ToLower().Trim()))
                .ToListAsync();
            if (exists.Any())
            {
                return new($"Space with name {spaceUpdateDto.Name} already exists", false);
            }
            space.Name = spaceUpdateDto.Name;
            space.Description = spaceUpdateDto.Description;
            space.SpaceType = spaceUpdateDto.SpaceType;
            space.UserId = userId;
            context.Spaces.Update(space);
            await context.SaveChangesAsync();
            transaction.Commit();
            return new("Space updated", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating space");
            transaction.Rollback();
            return new("Error updating space", false);
        }
    }

    public async Task<ServiceResponse> MarkDeleteSpace(int id, string userId)
    {
        using var context = _contextFactory.CreateDbContext();
        using IDbContextTransaction transaction = context.Database.BeginTransaction();
        try
        {
            var space = await context.Spaces.Include(c => c.Coordinates).FirstOrDefaultAsync(s => s.SpaceId == id);
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
            context.Spaces.Update(space);
            await context.SaveChangesAsync();
            transaction.Commit();
            return new("Space marked as deleted", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking space as deleted");
            transaction.Rollback();
            return new("Error marking space as deleted", false);
        }
    }
    public async Task<ServiceResponse> MarkUnDeleteSpace(int id, string userId)
    {
        using var context = _contextFactory.CreateDbContext();
        using IDbContextTransaction transaction = context.Database.BeginTransaction();
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
            var exists = await context.Spaces.Where(s => s.IsDeleted == false && s.SpaceId != id && Equals(s.Name.ToLower().Trim(), space.Name.ToLower().Trim())).ToListAsync();
            if (exists.Any())
            {
                return new($"Space with name {space.Name} already exists", false);
            }
            space.IsDeleted = false;
            space.UserId = userId;
            context.Spaces.Update(space);
            await context.SaveChangesAsync();
            transaction.Commit();
            return new("Space marked as undeleted", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking space as undeleted");
            transaction.Rollback();
            return new("Error undeleting space", false);
        }
    }
    public async Task<ServiceResponse> DeleteSpace(int id)
    {
        using var context = _contextFactory.CreateDbContext();
        using IDbContextTransaction transaction = context.Database.BeginTransaction();
        try
        {
            var space = await context.Spaces.FirstOrDefaultAsync(s => s.SpaceId == id);
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
            context.Spaces.Remove(space);
            await context.SaveChangesAsync();
            transaction.Commit();
            return new("Space deleted", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting space");
            transaction.Rollback();
            return new("Error deleting space", false);
        }
    }
    #endregion
    #region coordinate service
    public async Task<ServiceResponse> CreateCoordinate(int spaceId, CoordinateCreateDto coordinateCreateDto, string userId)
    {
        using var context = _contextFactory.CreateDbContext();
        using IDbContextTransaction transaction = context.Database.BeginTransaction();
        try
        {
            var space = await context.Spaces.Include(c => c.Coordinates).FirstOrDefaultAsync(s => s.SpaceId == spaceId);
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
            context.Coordinates.Add(coordinate);


            await context.SaveChangesAsync();
            transaction.Commit();
            return new("Coordinate created", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating coordinate");
            transaction.Rollback();
            return new("Error creating coordinate", false);
        }
    }

    public async Task<ServiceResponse<CoordinateDto>> GetCoordinateById(int id)
    {
        using var context = _contextFactory.CreateDbContext();
        try
        {
            var coordinate = await context.Coordinates.FirstOrDefaultAsync(c => c.CoordinateId == id);
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
        using var context = _contextFactory.CreateDbContext();
        try
        {
            var coordinates = await context.Coordinates.ToListAsync();
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
        using var context = _contextFactory.CreateDbContext();
        try
        {
            var coordinates = await context.Coordinates.Include(a => a.Assets).ToListAsync();
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
        using var context = _contextFactory.CreateDbContext();
        using IDbContextTransaction transaction = context.Database.BeginTransaction();
        try
        {
            var coordinate = await context.Coordinates.FirstOrDefaultAsync(c => c.CoordinateId == id);
            if (coordinate == null)
            {
                return new("Coordinate not found", false);
            }
            var exists = await context.Coordinates.Include(a => a.Space).ThenInclude(a => a.Area).Where(c => Equals(c.Name.ToLower().Trim(), coordinateUpdateDto.Name.ToLower().Trim()) && c.CoordinateId != id && c.IsDeleted == false && c.Space.Area.PlantId == coordinate.Space.Area.PlantId).AnyAsync();
            if (exists)
            {
                return new($"Coordinate with name {coordinateUpdateDto.Name} already exists", false);
            }
            coordinate.Name = coordinateUpdateDto.Name;
            coordinate.Description = coordinateUpdateDto.Description;
            coordinate.UserId = userId;
            context.Coordinates.Update(coordinate);
            await context.SaveChangesAsync();
            transaction.Commit();
            return new("Coordinate updated", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating coordinate");
            transaction.Rollback();
            return new("Error updating coordinate", false);
        }
    }

    public async Task<ServiceResponse> MarkDeleteCoordinate(int id, string userId)
    {
        using var context = _contextFactory.CreateDbContext();
        using IDbContextTransaction transaction = context.Database.BeginTransaction();
        try
        {
            var coordinate = await context.Coordinates.Include(a => a.Assets).FirstOrDefaultAsync(c => c.CoordinateId == id);
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
            context.Coordinates.Update(coordinate);
            await context.SaveChangesAsync();
            transaction.Commit();
            return new("Coordinate deleted", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting coordinate");
            transaction.Rollback();
            return new("Error deleting coordinate", false);
        }
    }
    public async Task<ServiceResponse> MarkUnDeleteCoordinate(int id, string userId)
    {
        using var context = _contextFactory.CreateDbContext();
        using IDbContextTransaction transaction = context.Database.BeginTransaction();
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
            var exists = await context.Coordinates.Include(a => a.Space).ThenInclude(a => a.Area).Where(c => Equals(c.Name.ToLower().Trim(), coordinate.Name.ToLower().Trim()) && c.CoordinateId != id && c.IsDeleted == false && c.Space.Area.PlantId == coordinate.Space.Area.PlantId).AnyAsync();
            if (exists)
            {
                return new($"Coordinate with name {coordinate.Name} already exists", false);
            }
            coordinate.IsDeleted = false;
            coordinate.UserId = userId;
            context.Coordinates.Update(coordinate);
            await context.SaveChangesAsync();
            transaction.Commit();
            return new("Coordinate un-deleted", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error un-deleting coordinate");
            transaction.Rollback();
            return new("Error un-deleting coordinate", false);
        }
    }
    public async Task<ServiceResponse> DeleteCoordinate(int id)
    {
        using var context = _contextFactory.CreateDbContext();
        using IDbContextTransaction transaction = context.Database.BeginTransaction();
        try
        {
            var coordinate = await context.Coordinates.FirstOrDefaultAsync(c => c.CoordinateId == id);
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
            context.Coordinates.Remove(coordinate);
            await context.SaveChangesAsync();
            transaction.Commit();
            return new("Coordinate deleted", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting coordinate");
            transaction.Rollback();
            return new("Error deleting coordinate", false);
        }
    }
    #endregion
    #region category service
    public async Task<ServiceResponse> CreateCategory(CategoryCreateDto categoryCreateDto, string userId)
    {
        using var context = _contextFactory.CreateDbContext();
        using IDbContextTransaction transaction = context.Database.BeginTransaction();
        try
        {
            var exists = await context.Categories.Where(c => c.Name.ToLower().Trim() == categoryCreateDto.Name.ToLower().Trim() && c.IsDeleted == false).AnyAsync();
            if (exists)
            {
                return new($"Category with name {categoryCreateDto.Name} already exists", false);
            }
            Category category = new()
            {
                Name = categoryCreateDto.Name,
                Description = categoryCreateDto.Description,
                UserId = userId
            };
            context.Categories.Add(category);
            await context.SaveChangesAsync();
            transaction.Commit();
            return new("Category created", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating category");
            transaction.Rollback();
            return new("Error creating category", false);
        }
    }

    public async Task<ServiceResponse<CategoryDto>> GetCategoryById(int id)
    {
        using var context = _contextFactory.CreateDbContext();
        try
        {
            var category = await context.Categories.FirstOrDefaultAsync(c => c.CategoryId == id);
            if (category == null)
            {
                return new("Category not found", false);
            }
            return new(_mapper.Map<CategoryDto>(category), "Category found", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting category");
            return new("Error getting category", false);
        }
    }

    public async Task<ServiceResponse<IEnumerable<CategoryDto>>> GetCategories()
    {
        using var context = _contextFactory.CreateDbContext();
        try
        {
            var categories = await context.Categories.ToListAsync();
            return new(_mapper.Map<IEnumerable<CategoryDto>>(categories), "Categories found", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting categories");
            return new("Error getting categories", false);
        }
    }

    public async Task<ServiceResponse<IEnumerable<CategoryDto>>> GetCategoriesWithAssets()
    {
        using var context = _contextFactory.CreateDbContext();
        try
        {
            var categories = await context.Categories.Include(a => a.AssetCategories).ThenInclude(a=>a.Asset).ToListAsync();
            return new(_mapper.Map<IEnumerable<CategoryDto>>(categories), "Categories found", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting categories");
            return new("Error getting categories", false);
        }
    }

    public async Task<ServiceResponse> UpdateCategory(int id, string userId, CategoryUpdateDto categoryUpdateDto)
    {
        using var context = _contextFactory.CreateDbContext();
        using IDbContextTransaction transaction = context.Database.BeginTransaction();
        try
        {
            var category = await context.Categories.FirstOrDefaultAsync(c => c.CategoryId == id);
            if (category == null)
            {
                return new("Category not found", false);
            }
            var exists = await context.Categories.Where(c => c.Name.ToLower().Trim() == categoryUpdateDto.Name.ToLower().Trim() && c.IsDeleted == false && c.CategoryId != id).AnyAsync();
            if (exists)
            {
                return new($"Category with name {categoryUpdateDto.Name} already exists", false);
            }
            if (!Equals(categoryUpdateDto.Name.ToLower().Trim(), category.Name.ToLower().Trim()))
            {
                category.Name = categoryUpdateDto.Name;
                category.UserId = userId;
            }
            if (!Equals(categoryUpdateDto.Description.ToLower().Trim(), category.Description.ToLower().Trim()))
            {
                category.Description = categoryUpdateDto.Description;
                category.UserId = userId;
            }
            context.Categories.Update(category);
            await context.SaveChangesAsync();
            transaction.Commit();
            return new("Category updated", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating category");
            transaction.Rollback();
            return new("Error updating category", false);
        }
    }

    public async Task<ServiceResponse> MarkDeleteCategory(int id, string userId)
    {
        using var context = _contextFactory.CreateDbContext();
        using IDbContextTransaction transaction = context.Database.BeginTransaction();
        try
        {
            var category = await context.Categories.Include(c=>c.AssetCategories).FirstOrDefaultAsync(c => c.CategoryId == id);
            if (category == null)
            {
                return new("Category not found", false);
            }
            if (category.IsDeleted == true)
            {
                return new("Category already marked as deleted", false);
            }
            if (category.AssetCategories.Count > 0)
            {
                return new("Category has assets assigned to it", false);
            }
            category.IsDeleted = true;
            category.UserId = userId;
            context.Categories.Update(category);
            await context.SaveChangesAsync();
            transaction.Commit();
            return new("Category marked as deleted", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking category as deleted");
            transaction.Rollback();
            return new("Error marking category as deleted", false);
        }
    }
    public async Task<ServiceResponse> MarkUnDeleteCategory(int id, string userId)
    {
        using var context = _contextFactory.CreateDbContext();
        using IDbContextTransaction transaction = context.Database.BeginTransaction();
        try
        {
            var category = await context.Categories.FirstOrDefaultAsync(c => c.CategoryId == id);
            if (category == null)
            {
                return new("Category not found", false);
            }
            if (category.IsDeleted == false)
            {
                return new("Category already marked as not deleted", false);
            }
            if (await context.Categories.Where(c=> Equals(category.Name.ToLower().Trim(), c.Name.ToLower().Trim())&&c.CategoryId != category.CategoryId && c.IsDeleted == false).AnyAsync())
            {
                return new($"Category with name {category.Name} already exists", false);
            }
            category.IsDeleted = false;
            category.UserId = userId;
            context.Categories.Update(category);
            await context.SaveChangesAsync();
            transaction.Commit();
            return new("Category marked as not deleted", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking category as not deleted");
            transaction.Rollback();
            return new("Error marking category as not deleted", false);
        }
    }
    public async Task<ServiceResponse> DeleteCategory(int id)
    {
        using var context = _contextFactory.CreateDbContext();
        using IDbContextTransaction transaction = context.Database.BeginTransaction();
        try
        {
            var category = await context.Categories.FirstOrDefaultAsync(c => c.CategoryId == id);
            if (category == null)
            {
                return new("Category not found", false);
            }
            if (category.IsDeleted == false)
            {
                return new("Category not marked as deleted", false);
            }
            
            context.Categories.Remove(category);
            await context.SaveChangesAsync();
            transaction.Commit();
            return new("Category deleted", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting category");
            transaction.Rollback();
            return new("Error deleting category", false);
        }
    }
    #endregion
    #region device service
    public async Task<ServiceResponse> CreateDevice(DeviceCreateDto deviceCreateDto, string userId)
    {
        using var context = _contextFactory.CreateDbContext();
        using IDbContextTransaction transaction = context.Database.BeginTransaction();
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
            context.Devices.Add(device);
            await context.SaveChangesAsync();
            transaction.Commit();
            _logger.LogInformation("Device with id {deviceId} created", device.DeviceId);
            return new("Device created", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating device");
            transaction.Rollback();
            return new("Error creating device", false);
        }
    }

    public async Task<ServiceResponse<DeviceDto>> GetDeviceById(int id)
    {
        using var context = _contextFactory.CreateDbContext();
        try
        {
            var device = await context.Devices.FirstOrDefaultAsync(d => d.DeviceId == id);
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
        using var context = _contextFactory.CreateDbContext();
        try
        {
            var devices = await context.Devices.ToListAsync();
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
        using var context = _contextFactory.CreateDbContext();
        try
        {
            var devices = await context.Devices.Include(d => d.Models).ToListAsync();
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
        using var context = _contextFactory.CreateDbContext();
        using IDbContextTransaction transaction = context.Database.BeginTransaction();
        try
        {
            var device = await context.Devices.FirstOrDefaultAsync(d => d.DeviceId == id);
            if (device == null)
            {
                _logger.LogWarning("Device not found");
                return new("Device not found", false);
            }
            if (device.IsDeleted)
            {
                _logger.LogWarning("Device marked as deleted");
                return new("Device marked as deleted", false);
            }

            var exists = await context.Devices.Where(d => d.DeviceId != id && Equals(d.Name.ToLower().Trim(), deviceUpdateDto.Name.ToLower().Trim()) && !d.IsDeleted).AnyAsync();
            if (exists)
            {
                _logger.LogWarning("Device with name {deviceUpdateDto.Name} already exists", deviceUpdateDto.Name);
                return new($"Device with name {deviceUpdateDto.Name} already exists", false);
            }
            device.UserId = userId;
            device.Name = deviceUpdateDto.Name;
            device.Description = deviceUpdateDto.Description;
            context.Devices.Update(device);
            await context.SaveChangesAsync();
            transaction.Commit();
            _logger.LogInformation("Device with id {deviceId} updated", id);
            return new($"Device {id} updated", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating device with id {deviceId}", id);
            transaction.Rollback();
            return new($"Error updating device with id {id}", false);
        }
    }

    public async Task<ServiceResponse> MarkDeleteDevice(int id, string userId)
    {
            using var context = _contextFactory.CreateDbContext();
            using IDbContextTransaction transaction = context.Database.BeginTransaction();
        try
        {
            var device = await context.Devices.FirstOrDefaultAsync(d => d.DeviceId == id);
            if (device == null)
            {
                _logger.LogWarning("Device not found");
                return new("Device not found", false);
            }
            if (device.IsDeleted)
            {
                _logger.LogWarning("Device marked as deleted");
                return new("Device marked as deleted", false);
            }
            device.UserId = userId;
            device.IsDeleted = true;
            context.Devices.Update(device);
            await context.SaveChangesAsync();
            transaction.Commit();
            _logger.LogInformation("Device with id {deviceId} marked as deleted", id);
            return new($"Device {id} marked as deleted", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking device as deleted with id {deviceId}", id);
            transaction.Rollback();
            return new($"Error marking device as deleted with id {id}", false);
        }
    }
    public async Task<ServiceResponse> MarkUnDeleteDevice(int id, string userId)
    {
        using var context = _contextFactory.CreateDbContext();
        using IDbContextTransaction transaction = context.Database.BeginTransaction();
        try
        {
            var device = await context.Devices.FirstOrDefaultAsync(d => d.DeviceId == id);
            if (device == null)
            {
                _logger.LogWarning("Device not found");
                return new("Device not found", false);
            }
            if (!device.IsDeleted)
            {
                _logger.LogWarning("Device not marked as deleted");
                return new("Device not marked as deleted", false);
            }
            device.UserId = userId;
            device.IsDeleted = false;
            context.Devices.Update(device);
            await context.SaveChangesAsync();
            transaction.Commit();
            _logger.LogInformation("Device with id {deviceId} marked as undeleted", id);
            return new($"Device {id} marked as undeleted", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking device as undeleted with id {deviceId}", id);
            transaction.Rollback();
            return new($"Error marking device as undeleted with id {id}", false);
        }
    }
    public async Task<ServiceResponse> DeleteDevice(int id)
    {
        using var context = _contextFactory.CreateDbContext();
        using IDbContextTransaction transaction = context.Database.BeginTransaction();
        try
        {
            var device = await context.Devices.FirstOrDefaultAsync(d => d.DeviceId == id);
            if (device == null)
            {
                _logger.LogWarning("Device not found");
                return new("Device not found", false);
            }
            if (!device.IsDeleted)
            {
                _logger.LogWarning("Device not marked as deleted");
                return new("Device not marked as deleted", false);
            }
            context.Devices.Remove(device);
            await context.SaveChangesAsync();
            transaction.Commit();
            _logger.LogInformation("Device with id {deviceId} deleted", id);
            return new($"Device {id} deleted", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting device with id {deviceId}", id);
            transaction.Rollback();
            return new($"Error deleting device with id {id}", false);
        }
    }
    #endregion
    #region model service
    public async Task<ServiceResponse> CreateModel(int deviceId, ModelCreateDto modelCreateDto, string userId)
    {
        using var context = _contextFactory.CreateDbContext();
        using IDbContextTransaction transaction = context.Database.BeginTransaction();
        try
        {
            var device = await context.Devices.FirstOrDefaultAsync(d => d.DeviceId == deviceId);
            if (device == null)
            {
                _logger.LogWarning("Device not found");
                return new("Device not found", false);
            }
            if (device.IsDeleted)
            {
                _logger.LogWarning("Device marked as deleted");
                return new("Device marked as deleted", false);
            }
            var exists = await context.Models.Where(m => m.DeviceId == deviceId && Equals(m.Name.ToLower().Trim(), modelCreateDto.Name.ToLower().Trim()) && !m.IsDeleted).AnyAsync();
            if (exists)
            {
                _logger.LogWarning("Model with name {modelCreateDto.Name} already exists", modelCreateDto.Name);
                return new($"Model with name {modelCreateDto.Name} already exists", false);
            }
            var model = new Model
            {
                DeviceId = deviceId,
                Name = modelCreateDto.Name,
                Description = modelCreateDto.Description,
                UserId = userId
            };
            context.Models.Add(model);
            await context.SaveChangesAsync();
            transaction.Commit();
            _logger.LogInformation("Model with id {modelId} created", model.ModelId);
            return new($"Model {model.ModelId} created", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating model");
            transaction.Rollback();
            return new("Error creating model", false);
        }
    }

    public async Task<ServiceResponse<ModelDto>> GetModelById(int id)
    {
        using var context = _contextFactory.CreateDbContext();
        try
        {
            var model = await context.Models.FirstOrDefaultAsync(m => m.ModelId == id);
            if (model == null)
            {
                _logger.LogWarning("Model not found");
                return new( "Model not found", false);
            }
            _logger.LogInformation("Model with id {modelId} retrieved", model.ModelId);
            return new(_mapper.Map<ModelDto>(model), $"Model with id {model.ModelId} retrieved",true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving model with id {modelId}", id);
            return new($"Error retrieving model with id {id}", false);
        }

    }

    public async Task<ServiceResponse<IEnumerable<ModelDto>>> GetModels()
    {
        using var context = _contextFactory.CreateDbContext();
        try
        {
            var models = await context.Models.ToListAsync();
            _logger.LogInformation("Models retrieved");
            return new(_mapper.Map<IEnumerable<ModelDto>>(models), "Models retrieved", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving models");
            return new("Error retrieving models", false);
        }
    }

    public async Task<ServiceResponse<IEnumerable<ModelDto>>> GetModelsWithAssets()
    {
        using var context = _contextFactory.CreateDbContext();
        try
        {
            var models = await context.Models.Include(m => m.Assets).ToListAsync();
            _logger.LogInformation("Models retrieved");
            return new(_mapper.Map<IEnumerable<ModelDto>>(models), "Models retrieved", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving models");
            return new("Error retrieving models", false);
        }
    }

    public async Task<ServiceResponse> UpdateModel(int id, string userId, ModelUpdateDto modelUpdateDto)
    {
        using var context = _contextFactory.CreateDbContext();
        using IDbContextTransaction transaction = context.Database.BeginTransaction();
        try
        {
            var model = await context.Models.FirstOrDefaultAsync(m => m.ModelId == id);
            if (model == null)
            {
                _logger.LogWarning("Model not found");
                return new("Model not found", false);
            }
            if (model.IsDeleted)
            {
                _logger.LogWarning("Model marked as deleted");
                return new("Model marked as deleted", false);
            }
            var exists = await context.Models.Where(m => m.DeviceId == model.DeviceId && Equals(m.Name.ToLower().Trim(), modelUpdateDto.Name.ToLower().Trim()) && !m.IsDeleted && m.ModelId != id).AnyAsync();
            if (exists)
            {
                _logger.LogWarning("Model with name {modelUpdateDto.Name} already exists", modelUpdateDto.Name);
                return new($"Model with name {modelUpdateDto.Name} already exists", false);
            }
            if(!Equals(model.Name.ToLower().Trim(), modelUpdateDto.Name.ToLower().Trim()))
                {
                model.Name = modelUpdateDto.Name;
                model.UserId = userId;
            }
            if (!Equals(model.Description.ToLower().Trim(), modelUpdateDto.Description.ToLower().Trim()))
            {
                model.Description = modelUpdateDto.Description;
                model.UserId = userId;
            }
            context.Models.Update(model);
            await context.SaveChangesAsync();
            transaction.Commit();
            _logger.LogInformation("Model with id {modelId} updated", model.ModelId);
            return new($"Model {model.ModelId} updated", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating model with id {modelId}", id);
            transaction.Rollback();
            return new($"Error updating model with id {id}", false);
        }
    }

    public async Task<ServiceResponse> MarkDeleteModel(int id, string userId)
    {
        using var context = _contextFactory.CreateDbContext();
        using IDbContextTransaction transaction = context.Database.BeginTransaction();
        try
        {
            var model = await context.Models.Include(m=>m.Assets).FirstOrDefaultAsync(m => m.ModelId == id);
            if (model == null)
            {
                _logger.LogWarning("Model not found");
                return new("Model not found", false);
            }
            if (model.IsDeleted)
            {
                _logger.LogWarning("Model marked as deleted");
                return new("Model marked as deleted", false);
            }
         
            if (model.Assets.Count > 0)
            {
                _logger.LogWarning("Model has assets");
                return new("Model has assets", false);
            }
            model.IsDeleted = true;
            model.UserId = userId;
            context.Models.Update(model);
            await context.SaveChangesAsync();
            transaction.Commit();
            _logger.LogInformation("Model with id {modelId} marked as deleted", model.ModelId);
            return new($"Model {model.ModelId} marked as deleted", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking model with id {modelId} as deleted", id);
            transaction.Rollback();
            return new($"Error marking model with id {id} as deleted", false);
        }
    }
    public async Task<ServiceResponse> MarkUnDeleteModel(int id, string userId)
    {
        using var context = _contextFactory.CreateDbContext();
        using IDbContextTransaction transaction = context.Database.BeginTransaction();
        try
        {
            var model = await context.Models.FirstOrDefaultAsync(m => m.ModelId == id);
            if (model == null)
            {
                _logger.LogWarning("Model not found");
                return new("Model not found", false);
            }
            if (!model.IsDeleted)
            {
                _logger.LogWarning("Model not marked as deleted");
                return new("Model not marked as deleted", false);
            }
            model.IsDeleted = false;
            model.UserId = userId;
            context.Models.Update(model);
            await context.SaveChangesAsync();
            transaction.Commit();
            _logger.LogInformation("Model with id {modelId} marked as undeleted", model.ModelId);
            return new($"Model {model.ModelId} marked as undeleted", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking model with id {modelId} as undeleted", id);
            transaction.Rollback();
            return new($"Error marking model with id {id} as undeleted", false);
        }
    }
    public async Task<ServiceResponse> DeleteModel(int id)
    {
        using var context = _contextFactory.CreateDbContext();
        using IDbContextTransaction transaction = context.Database.BeginTransaction();
        try
        {
            var model = await context.Models.FirstOrDefaultAsync(m => m.ModelId == id);
            if (model == null)
            {
                _logger.LogWarning("Model not found");
                return new("Model not found", false);
            }
            if (!model.IsDeleted)
            {
                _logger.LogWarning("Model not marked as deleted");
                return new("Model not marked as deleted", false);
            }
            context.Models.Remove(model);
            await context.SaveChangesAsync();
            transaction.Commit();
            _logger.LogInformation("Model with id {modelId} deleted", model.ModelId);
            return new($"Model {model.ModelId} deleted", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting model with id {modelId}", id);
            transaction.Rollback();
            return new($"Error deleting model with id {id}", false);
        }
    }
    #endregion
    #region parameter service
    public async Task<ServiceResponse> CreateParameter(ParameterCreateDto parameterCreateDto, string userId)
    {
        using var context = _contextFactory.CreateDbContext();
        using IDbContextTransaction transaction = context.Database.BeginTransaction();
        try
        {
            var exists = await context.Parameters.Where(p => Equals(p.Name.ToLower().Trim(), parameterCreateDto.Name.ToLower().Trim()) && !p.IsDeleted).AnyAsync();
            if (exists)
            {
                _logger.LogWarning("Parameter with name {parameterCreateDto.Name} already exists", parameterCreateDto.Name);
                return new($"Parameter with name {parameterCreateDto.Name} already exists", false);
            }
            var parameter = new Parameter
            {
                Name = parameterCreateDto.Name,
                Description = parameterCreateDto.Description,
                UserId = userId
            };
            context.Parameters.Add(parameter);
            await context.SaveChangesAsync();
            transaction.Commit();
            _logger.LogInformation("Parameter with id {parameterId} created", parameter.ParameterId);
            return new($"Parameter {parameter.ParameterId} created", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating parameter");
            transaction.Rollback();
            return new("Error creating parameter", false);
        }
    }

    public async Task<ServiceResponse<ParameterDto>> GetParameterById(int id)
    {
        using var context = _contextFactory.CreateDbContext();
        try
        {
            var parameter = await _mapper.ProjectTo<ParameterDto>(context.Parameters).FirstOrDefaultAsync(p => p.ParameterId == id);
            if (parameter == null)
            {
                _logger.LogWarning("Parameter not found");
                return new("Parameter not found", false);
            }
            var parameterDto = new ParameterDto
            {
                ParameterId = parameter.ParameterId,
                Name = parameter.Name,
                Description = parameter.Description
            };
            _logger.LogInformation("Parameter with id {parameterId} retrieved", parameter.ParameterId);
            return new(parameterDto,$"Parameter {parameter.ParameterId} retrieved", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving parameter with id {parameterId}", id);
            return new($"Error retrieving parameter with id {id}", false);
        }
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
    #region asset service
    public async Task<ServiceResponse> CreateAsset(AssetCreateDto assetCreateDto, string userId)
    {
        using var context = _contextFactory.CreateDbContext();
        using IDbContextTransaction transaction = context.Database.BeginTransaction();
        try
        {
            var coordinate = await context.Coordinates.Include(a => a.Assets).FirstOrDefaultAsync(c => c.CoordinateId == assetCreateDto.CoordinateId);
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
            context.Assets.Add(asset);
            await context.SaveChangesAsync();
            transaction.Commit();
            return new("Asset created", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating asset");
            transaction.Rollback();
            return new("Error creating asset", false);
        }
    }

    public async Task<ServiceResponse<AssetDto>> GetAssetById(int id)
    {
        using var context = _contextFactory.CreateDbContext();
        try
        {
            var asset = await context.Assets.Include(a => a.AssetDetails).Include(a => a.AssetCategories).FirstOrDefaultAsync(a => a.AssetId == id);
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
        using var context = _contextFactory.CreateDbContext();
        try
        {
            var assets = await context.Assets.Include(a => a.AssetDetails).Include(a => a.AssetCategories).ToListAsync();
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
        using var context = _contextFactory.CreateDbContext();
        try
        {
            var assets = await context.Assets.Include(a => a.AssetDetails).Include(a => a.AssetCategories).Include(a => a.Coordinate).Include(c => c.CommunicateAssets).Include(a => a.Model).ToListAsync();
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
        using var context = _contextFactory.CreateDbContext();
        using IDbContextTransaction transaction = context.Database.BeginTransaction();
        try
        {
            var asset = await context.Assets.Include(a => a.AssetDetails).Include(a => a.AssetCategories).FirstOrDefaultAsync(a => a.AssetId == id);
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
            var exists = await context.Assets.Where(a => Equals(a.Name.ToLower().Trim(), assetUpdateDto.Name.ToLower().Trim()) && a.AssetId != id && a.IsDeleted == false).AnyAsync();
            if (exists)
            {
                _logger.LogWarning("Asset with name {assetName} already exists", assetUpdateDto.Name);
                return new($"Asset with name {assetUpdateDto.Name} already exists", false);
            }
            var coordinate = await context.Coordinates.FirstOrDefaultAsync(c => c.CoordinateId == assetUpdateDto.CoordinateId);
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
            context.Assets.Update(asset);
            await context.SaveChangesAsync();
            transaction.Commit();
            _logger.LogInformation("Asset with id {assetId} updated", id);
            return new($"Asset {id} updated", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating asset with id {assetId}", id);
            transaction.Rollback();
            return new($"Error updating asset with id {id}", false);
        }
    }
    public async Task<ServiceResponse> AssetChangeModel(int assetId, string userId, int modelId)
    {
        using var context = _contextFactory.CreateDbContext();
        using IDbContextTransaction transaction = context.Database.BeginTransaction();
        try
        {
            var model = await context.Models.FirstOrDefaultAsync(m => m.ModelId == modelId);
            if (model == null)
            {
                _logger.LogWarning("Model not found");
                return new("Model not found", false);
            }
            var asset = await context.Assets.FirstOrDefaultAsync(a => a.AssetId == assetId);
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
            context.AssetDetails.RemoveRange(asset.AssetDetails);
            context.AssetCategories.RemoveRange(asset.AssetCategories);
            context.CommunicateAssets.RemoveRange(asset.CommunicateAssets);
            asset.ModelId = modelId;
            asset.UserId = userId;
            context.Assets.Update(asset);
            await context.SaveChangesAsync();
            transaction.Commit();
            _logger.LogInformation("Asset with id {assetId} updated", assetId);
            return new($"Asset {assetId} updated", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating asset with id {assetId}", assetId);
            transaction.Rollback();
            return new($"Error updating asset with id {assetId}", false);
        }
    }

    public async Task<ServiceResponse> MarkDeleteAsset(int id, string userId)
    {
        using var context = _contextFactory.CreateDbContext();
        using IDbContextTransaction transaction = context.Database.BeginTransaction();
        try
        {
            var asset = await context.Assets.FirstOrDefaultAsync(a => a.AssetId == id);
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
            context.Assets.Update(asset);
            await context.SaveChangesAsync();
            transaction.Commit();
            _logger.LogInformation("Asset with id {assetId} marked as deleted", id);
            return new($"Asset {id} marked as deleted", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking asset with id {assetId} as deleted", id);
            transaction.Rollback();
            return new($"Error marking asset with id {id} as deleted", false);
        }
    }
    public async Task<ServiceResponse> MarkUnDeleteAsset(int id, string userId)
    {
        using var context = _contextFactory.CreateDbContext();
        using IDbContextTransaction transaction = context.Database.BeginTransaction();
        try
        {
            var asset = await context.Assets.FirstOrDefaultAsync(a => a.AssetId == id);
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
            context.Assets.Update(asset);
            await context.SaveChangesAsync();
            transaction.Commit();
            _logger.LogInformation("Asset with id {assetId} marked as not deleted", id);
            return new($"Asset {id} marked as not deleted", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking asset with id {assetId} as not deleted", id);
            transaction.Rollback();
            return new($"Error marking asset with id {id} as not deleted", false);
        }
    }
    public async Task<ServiceResponse> DeleteAsset(int id)
    {
        using var context = _contextFactory.CreateDbContext();
        using IDbContextTransaction transaction = context.Database.BeginTransaction();
        try
        {
            var asset = await context.Assets.FirstOrDefaultAsync(a => a.AssetId == id);
            if (asset == null)
            {
                _logger.LogWarning("Asset not found");
                return new("Asset not found", false);
            }
            context.Assets.Remove(asset);
            _logger.LogInformation("Asset with id {assetId} deleted", id);
            await context.SaveChangesAsync();
            transaction.Commit();
            return new($"Asset {id} deleted", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting asset with id {assetId}", id);
            transaction.Rollback();
            return new($"Error deleting asset with id {id}", false);
        }
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
    #region situation service
    public async Task<ServiceResponse> CreateSituation(SituationCreateDto situationCreateDto, string userId)
    {
        using var context = _contextFactory.CreateDbContext();
        using IDbContextTransaction transaction = context.Database.BeginTransaction();
        try
        {
            var exists = await context.Situations.Where(s => Equals(s.Name.ToLower().Trim(), situationCreateDto.Name.ToLower().Trim()) && !s.IsDeleted).AnyAsync();
            if (exists)
            {
                _logger.LogWarning("Situation with name {situationCreateDto.Name} already exists", situationCreateDto.Name);
                return new($"Situation with name {situationCreateDto.Name} already exists", false);
            }
            var situation = new Situation
            {
                Name = situationCreateDto.Name,
                Description = situationCreateDto.Description,
                UserId = userId
            };
            context.Situations.Add(situation);
            await context.SaveChangesAsync();
            transaction.Commit();
            _logger.LogInformation("Situation with id {situationId} created", situation.SituationId);
            return new($"Situation {situation.SituationId} created", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating situation");
            transaction.Rollback();
            return new($"Error creating situation", false);
        }
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
}