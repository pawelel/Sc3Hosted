using AutoMapper;

using Microsoft.EntityFrameworkCore;

using Sc3Hosted.Server.Data;
using Sc3Hosted.Server.Entities;
using Sc3Hosted.Shared.Dtos;
using Sc3Hosted.Shared.Helpers;

namespace Sc3Hosted.Server.Services;

public class DbService : IDbService
{
    private readonly IDbContextFactory<Sc3HostedDbContext> _contextFactory;
    private readonly ILogger<DbService> _logger;
    private readonly IMapper _mapper;

    public DbService(IMapper mapper, ILogger<DbService> logger, IDbContextFactory<Sc3HostedDbContext> contextFactory)
    {
        _mapper = mapper;
        _logger = logger;
        _contextFactory = contextFactory;
    }

    /// <summary>
    /// Allows to assign different model to an asset. Deletes all data related to asset. Irreversible through application
    /// Requires existing model not marked as deleted
    /// Removes any existing asset details from asset
    /// Removes any existing asset categories from asset
    /// Removes any existing communicate assets from asset
    /// </summary>
    /// <param name="assetId"></param>
    /// <param name="userId"></param>
    /// <param name="modelId"></param>
    /// <returns></returns>
    public async Task<ServiceResponse> ChangeAssetsModel(int assetId, string userId, int modelId)
    {
        if (assetId <= 0 || modelId <= 0)
        {
            return new ServiceResponse("Invalid asset or model id");
        }
        await using var context = await _contextFactory.CreateDbContextAsync();
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
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

            var asset = await context.Assets.FirstOrDefaultAsync(a => a.AssetId == assetId);
            if (asset == null)
            {
                _logger.LogWarning("Asset not found");
                return new ServiceResponse("Asset not found");
            }

            if (asset.IsDeleted)
            {
                _logger.LogWarning("Asset marked as deleted");
                return new ServiceResponse("Asset marked as deleted");
            }

            context.AssetDetails.RemoveRange(asset.AssetDetails);
            context.AssetCategories.RemoveRange(asset.AssetCategories);
            context.CommunicateAssets.RemoveRange(asset.CommunicateAssets);
            asset.ModelId = modelId;
            asset.UserId = userId;
            context.Assets.Update(asset);
            await context.SaveChangesAsync();
            await transaction.CommitAsync();
            _logger.LogInformation("Asset with id {AssetId} updated", assetId);
            return new ServiceResponse($"Asset {assetId} updated", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating asset with id {AssetId}", assetId);
            await transaction.RollbackAsync();
            return new ServiceResponse($"Error updating asset with id {assetId}");
        }
    }

    /// <summary>
    /// Creates Area if name is unique.
    /// Requires existing plant not marked as deleted.
    /// </summary>
    /// <param name="plantId"></param>
    /// <param name="areaCreateDto"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    public async Task<ServiceResponse> CreateArea(int plantId, AreaCreateDto areaCreateDto, string userId)
    {
        if (plantId <= 0)
        {
            return new ServiceResponse("Invalid plant id");
        }
        await using var context = await _contextFactory.CreateDbContextAsync();
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            var plant = await context.Plants.Include(a => a.Areas).FirstOrDefaultAsync(p => p.PlantId == plantId);
            if (plant == null)
            {
                _logger.LogWarning("Cannot create area for plant with id {PlantId}", plantId);
                return new ServiceResponse($"Cannot create area for plant with id {plantId}");
            }
            if (plant.IsDeleted)
            {
                _logger.LogWarning("Cannot create area for plant with id {PlantId}", plantId);
                return new ServiceResponse($"Cannot create area for plant with id {plantId}");
            }
            if (plant.Areas.Any(a =>
                    Equals(a.Name.ToLower().Trim(), areaCreateDto.Name.ToLower().Trim())))
            {
                _logger.LogWarning("Area with name {AreaName} already exists", areaCreateDto.Name);
                return new ServiceResponse($"Area with name {areaCreateDto.Name} already exists");
            }

            Area area = new()
            {
                Name = areaCreateDto.Name,
                Description = areaCreateDto.Description,
                PlantId = plantId,
                UserId = userId
            };
            context.Areas.Add(area);
            await context.SaveChangesAsync();
            await transaction.CommitAsync();
            return new ServiceResponse("Area created", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating area");
            await transaction.RollbackAsync();
            return new ServiceResponse("Error creating area");
        }
    }

    /// <summary>
    /// Creates asset if name is unique.
    /// Requires existing model not marked as deleted
    /// Requires existing coordinate not marked as deleted
    /// </summary>
    /// <param name="assetCreateDto"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    public async Task<ServiceResponse> CreateAsset(AssetCreateDto assetCreateDto, string userId)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            var coordinate = await context.Coordinates.Include(a => a.Assets)
                .FirstOrDefaultAsync(c => c.CoordinateId == assetCreateDto.CoordinateId);
            if (coordinate == null)
            {
                _logger.LogWarning("Cannot create asset for coordinate with id {CoordinateId}",
                    assetCreateDto.CoordinateId);
                return new ServiceResponse($"Cannot create asset for coordinate with id {assetCreateDto.CoordinateId}");
            }
            if (coordinate.IsDeleted)
            {
                _logger.LogWarning("Cannot create asset for coordinate with id {CoordinateId}",
                    assetCreateDto.CoordinateId);
                return new ServiceResponse($"Cannot create asset for coordinate with id {assetCreateDto.CoordinateId}");
            }

            if (coordinate.Assets.Any(a =>
                    Equals(a.Name.ToLower().Trim(), assetCreateDto.Name.ToLower().Trim())))
            {
                _logger.LogWarning("Asset with name {AssetName} already exists", assetCreateDto.Name);
                return new ServiceResponse($"Asset with name {assetCreateDto.Name} already exists");
            }

            var model = await context.Models.FirstOrDefaultAsync(m => m.ModelId == assetCreateDto.ModelId);
            if (model == null)
            {
                _logger.LogWarning("Cannot create asset for model with id {ModelId}", assetCreateDto.ModelId);
                return new ServiceResponse($"Cannot create asset for model with id {assetCreateDto.ModelId}");
            }
            if (model.IsDeleted)
            {
                _logger.LogWarning("Cannot create asset for model with id {ModelId}", assetCreateDto.ModelId);
                return new ServiceResponse($"Cannot create asset for model with id {assetCreateDto.ModelId}");
            }
            Asset asset = new()
            {
                Name = assetCreateDto.Name,
                ModelId = assetCreateDto.ModelId,
                CoordinateId = assetCreateDto.CoordinateId,
                UserId = userId
            };
            context.Assets.Add(asset);
            await context.SaveChangesAsync();
            await transaction.CommitAsync();
            return new ServiceResponse("Asset created", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating asset");
            await transaction.RollbackAsync();
            return new ServiceResponse("Error creating asset");
        }
    }

    /// <summary>
    /// Creates category if name is unique.
    /// </summary>
    /// <param name="categoryCreateDto"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    public async Task<ServiceResponse> CreateCategory(CategoryCreateDto categoryCreateDto, string userId)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            var exists = await context.Categories.AnyAsync(c =>
                c.Name.ToLower().Trim() == categoryCreateDto.Name.ToLower().Trim());
            if (exists)
            {
                _logger.LogWarning("Category with name {CategoryName} already exists", categoryCreateDto.Name);
                return new ServiceResponse($"Category with name {categoryCreateDto.Name} already exists");
            }

            Category category = new()
            {
                Name = categoryCreateDto.Name,
                Description = categoryCreateDto.Description,
                UserId = userId
            };
            context.Categories.Add(category);
            await context.SaveChangesAsync();
            await transaction.CommitAsync();
            return new ServiceResponse("Category created", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating category");
            await transaction.RollbackAsync();
            return new ServiceResponse("Error creating category");
        }
    }

    /// <summary>
    /// Creates communicate if name is unique.
    /// </summary>
    /// <param name="communicateCreateDto"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    public async Task<ServiceResponse> CreateCommunicate(CommunicateCreateDto communicateCreateDto, string userId)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            var exists = await context.Communicates.AnyAsync(c =>
                c.Name.ToLower().Trim() == communicateCreateDto.Name.ToLower().Trim());
            if (exists)
            {
                _logger.LogWarning("Communicate with name {CommunicateName} already exists",
                    communicateCreateDto.Name);
                return new ServiceResponse($"Communicate with name {communicateCreateDto.Name} already exists");
            }

            var communicate = new Communicate
            {
                UserId = userId,
                Name = communicateCreateDto.Name,
                Description = communicateCreateDto.Description
            };
            context.Communicates.Add(communicate);
            await context.SaveChangesAsync();
            await transaction.CommitAsync();
            _logger.LogInformation("Communicate created");
            return new ServiceResponse("Communicate created", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating communicate");
            await transaction.RollbackAsync();
            return new ServiceResponse($"Error creating communicate");
        }
    }

    /// <summary>
    /// Creates coordinate if name is unique or another coordinate with the name is marked as deleted.
    /// Requires that space exists and is not marked as deleted.
    /// </summary>
    /// <param name="spaceId"></param>
    /// <param name="coordinateCreateDto"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    public async Task<ServiceResponse> CreateCoordinate(int spaceId, CoordinateCreateDto coordinateCreateDto, string userId)
    {
        if (spaceId <= 0)
        {
            _logger.LogWarning("SpaceId is invalid");
            return new ServiceResponse("SpaceId is invalid");
        }
        await using var context = await _contextFactory.CreateDbContextAsync();
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            var space = await context.Spaces.Include(c => c.Coordinates).FirstOrDefaultAsync(s => s.SpaceId == spaceId);
            if (space == null)
            {
                _logger.LogWarning("Cannot create coordinate for space with id {SpaceId}", spaceId);
                return new ServiceResponse($"Cannot create coordinate for space with id {spaceId}");
            }
            if (space.IsDeleted)
            {
                _logger.LogWarning("Cannot create coordinate for space with id {SpaceId}", spaceId);
                return new ServiceResponse($"Cannot create coordinate for space with id {spaceId}");
            }

            if (space.Coordinates.Any(c =>
                    Equals(c.Name.ToLower().Trim(), coordinateCreateDto.Name.ToLower().Trim())))
            {
                _logger.LogWarning("Coordinate with {CoordinateName} already exists", coordinateCreateDto.Name);
                return new ServiceResponse($"Coordinate with name {coordinateCreateDto.Name} already exists");
            }

            Coordinate coordinate = new()
            {
                Name = coordinateCreateDto.Name,
                Description = coordinateCreateDto.Description,
                SpaceId = spaceId,
                UserId = userId
            };
            context.Coordinates.Add(coordinate);

            await context.SaveChangesAsync();
            await transaction.CommitAsync();
            return new ServiceResponse("Coordinate created", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating coordinate");
            await transaction.RollbackAsync();
            return new ServiceResponse("Error creating coordinate");
        }
    }

    /// <summary>
    /// Creates detail if name is unique.
    /// </summary>
    /// <param name="detailCreateDto"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    public async Task<ServiceResponse> CreateDetail(DetailCreateDto detailCreateDto, string userId)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            var exists = await context.Details.AnyAsync(d =>
                Equals(d.Name.ToLower().Trim() == detailCreateDto.Name.ToLower().Trim()));
            if (exists)
            {
                _logger.LogWarning("Detail with name {DetailName} already exists", detailCreateDto.Name);
                return new ServiceResponse($"Detail with name {detailCreateDto.Name} already exists");
            }

            var detail = new Detail
            {
                Name = detailCreateDto.Name,
                Description = detailCreateDto.Description,
                UserId = userId
            };
            context.Details.Add(detail);
            await context.SaveChangesAsync();
            await transaction.CommitAsync();
            _logger.LogInformation("Detail with id {DetailId} created", detail.DetailId);
            return new ServiceResponse($"Detail {detail.DetailId} created", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating detail");
            await transaction.RollbackAsync();
            return new ServiceResponse($"Error creating detail");
        }
    }

    /// <summary>
    /// Creates device if name is unique.
    /// </summary>
    /// <param name="deviceCreateDto"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    public async Task<ServiceResponse> CreateDevice(DeviceCreateDto deviceCreateDto, string userId)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            Device device = new();
            var exist = await context.Devices.AnyAsync(p =>
                Equals(p.Name.ToLower().Trim(), device.Name.ToLower().Trim()));
            if (exist)
            {
                return new ServiceResponse("Device already exists");
            }

            device.UserId = userId;
            device.Name = deviceCreateDto.Name;
            device.Description = deviceCreateDto.Description;
            context.Devices.Add(device);
            await context.SaveChangesAsync();
            await transaction.CommitAsync();
            _logger.LogInformation("Device with id {DeviceId} created", device.DeviceId);
            return new ServiceResponse("Device created", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating device");
            await transaction.RollbackAsync();
            return new ServiceResponse("Error creating device");
        }
    }

    /// <summary>
    /// Creates model if name is unique.
    /// Requires that device exists and is not marked as deleted.
    /// </summary>
    /// <param name="deviceId"></param>
    /// <param name="modelCreateDto"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    public async Task<ServiceResponse> CreateModel(int deviceId, ModelCreateDto modelCreateDto, string userId)
    {
        if (deviceId <= 0)
        {
            _logger.LogWarning("DeviceId is invalid");
            return new ServiceResponse("DeviceId is invalid");
        }
        await using var context = await _contextFactory.CreateDbContextAsync();
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            var device = await context.Devices.Include(d => d.Models).FirstOrDefaultAsync(d => d.DeviceId == deviceId);
            if (device == null)
            {
                _logger.LogWarning("Device not found");
                return new ServiceResponse("Device not found");
            }

            if (device.IsDeleted)
            {
                _logger.LogWarning("Device marked as deleted");
                return new ServiceResponse("Device marked as deleted");
            }

            var exists = device.Models.Any(m => Equals(m.Name.ToLower().Trim(), modelCreateDto.Name.ToLower().Trim()));
            if (exists)
            {
                _logger.LogWarning("Model with name {modelCreateDto.Name} already exists", modelCreateDto.Name);
                return new ServiceResponse($"Model with name {modelCreateDto.Name} already exists");
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
            await transaction.CommitAsync();
            _logger.LogInformation("Model with id {ModelId} created", model.ModelId);
            return new ServiceResponse($"Model {model.ModelId} created", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating model");
            await transaction.RollbackAsync();
            return new ServiceResponse("Error creating model");
        }
    }

    /// <summary>
    /// Creates parameter if name is unique.
    /// </summary>
    /// <param name="parameterCreateDto"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    public async Task<ServiceResponse> CreateParameter(ParameterCreateDto parameterCreateDto, string userId)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            var exists = await context.Parameters.AnyAsync(p =>
                Equals(p.Name.ToLower().Trim(), parameterCreateDto.Name.ToLower().Trim()));
            if (exists)
            {
                _logger.LogWarning("Parameter with name {parameterCreateDto.Name} already exists",
                    parameterCreateDto.Name);
                return new ServiceResponse($"Parameter with name {parameterCreateDto.Name} already exists");
            }

            var parameter = new Parameter
            {
                Name = parameterCreateDto.Name,
                Description = parameterCreateDto.Description,
                UserId = userId
            };
            context.Parameters.Add(parameter);
            await context.SaveChangesAsync();
            await transaction.CommitAsync();
            _logger.LogInformation("Parameter with id {ParameterId} created", parameter.ParameterId);
            return new ServiceResponse($"Parameter {parameter.ParameterId} created", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating parameter");
            await transaction.RollbackAsync();
            return new ServiceResponse("Error creating parameter");
        }
    }

    /// <summary>
    /// Creates plant if name is unique.
    /// </summary>
    /// <param name="plantCreateDto"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    public async Task<ServiceResponse> CreatePlant(PlantCreateDto plantCreateDto, string userId)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            var exist = await context.Plants.AnyAsync(p =>
                Equals(p.Name.ToLower().Trim(), plantCreateDto.Name.ToLower().Trim()));
            if (exist)
            {
                return new ServiceResponse($"Plant with name {plantCreateDto.Name} already exists");
            }

            Plant plant = new()
            {
                UserId = userId,
                Name = plantCreateDto.Name,
                Description = plantCreateDto.Description
            };
            context.Plants.Add(plant);
            await context.SaveChangesAsync();
            await transaction.CommitAsync();
            return new ServiceResponse("Plant created", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating plant");
            await transaction.RollbackAsync();
            return new ServiceResponse("Error creating plant");
        }
    }

    /// <summary>
    /// Creates Question if name is unique.
    /// </summary>
    /// <param name="questionCreateDto"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    public async Task<ServiceResponse> CreateQuestion(QuestionCreateDto questionCreateDto, string userId)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            var exists = await context.Questions.AnyAsync(q =>
                Equals(q.Name.ToLower().Trim(), questionCreateDto.Name.ToLower().Trim()));
            if (exists)
            {
                return new ServiceResponse($"Question with name {questionCreateDto.Name} already exists");
            }

            var question = new Question
            {
                Name = questionCreateDto.Name,
                UserId = userId,
            };

            context.Questions.Add(question);
            await context.SaveChangesAsync();
            _logger.LogInformation("QuestionId {id} created", question.QuestionId);
            return new ServiceResponse($"QuestionId {question.QuestionId} created", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating question");
            return new ServiceResponse($"Error creating question");
        }
    }

    /// <summary>
    /// Creates situation if name is unique.
    /// </summary>
    /// <param name="situationCreateDto"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    public async Task<ServiceResponse> CreateSituation(SituationCreateDto situationCreateDto, string userId)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            var exists = await context.Situations.AnyAsync(s =>
                Equals(s.Name.ToLower().Trim(), situationCreateDto.Name.ToLower().Trim()));
            if (exists)
            {
                _logger.LogWarning("Situation with name {situationCreateDto.Name} already exists",
                    situationCreateDto.Name);
                return new ServiceResponse($"Situation with name {situationCreateDto.Name} already exists");
            }

            var situation = new Situation
            {
                Name = situationCreateDto.Name,
                Description = situationCreateDto.Description,
                UserId = userId
            };
            context.Situations.Add(situation);
            await context.SaveChangesAsync();
            await transaction.CommitAsync();
            _logger.LogInformation("SituationId {SituationId} created", situation.SituationId);
            return new ServiceResponse($"Situation {situation.SituationId} created", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating situation");
            await transaction.RollbackAsync();
            return new ServiceResponse($"Error creating situation");
        }
    }

    /// <summary>
    /// Creates space if name is unique.
    /// Requires area exists and is not marked as deleted.
    /// </summary>
    /// <param name="areaId"></param>
    /// <param name="spaceCreateDto"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    public async Task<ServiceResponse> CreateSpace(int areaId, SpaceCreateDto spaceCreateDto, string userId)
    {
        if (areaId <= 0)
        {
            return new ServiceResponse("AreaId is invalid.");
        }
        await using var context = await _contextFactory.CreateDbContextAsync();
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            var area = await context.Areas.Include(s => s.Spaces).FirstOrDefaultAsync(a => a.AreaId == areaId);
            if (area == null)
            {
                _logger.LogWarning("Cannot create space for area with id {AreaId}", areaId);
                return new ServiceResponse($"Cannot create space for area with id {areaId}");
            }
            if (area.IsDeleted)
            {
                _logger.LogWarning("Cannot create space for area with id {AreaId}", areaId);
                return new ServiceResponse($"Cannot create space for area with id {areaId}");
            }

            if (area.Spaces.Any(s =>
                    Equals(s.Name.ToLower().Trim(), spaceCreateDto.Name.ToLower().Trim())))
            {
                _logger.LogWarning("Space with name {SpaceName} already exists", spaceCreateDto.Name);
                return new ServiceResponse($"Space with name {spaceCreateDto.Name} already exists");
            }

            Space space = new()
            {
                UserId = userId,
                AreaId = areaId,
                Name = spaceCreateDto.Name,
                Description = spaceCreateDto.Description
            };
            context.Spaces.Add(space);
            await context.SaveChangesAsync();
            await transaction.CommitAsync();
            return new ServiceResponse("Space created", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating space");
            await transaction.RollbackAsync();
            return new ServiceResponse("Error creating space");
        }
    }

    /// <summary>
    /// Deletes area if it is marked as deleted and exists.
    /// Deletion is permanent.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<ServiceResponse> DeleteArea(int id)
    {
        if (id <= 0)
        {
            return new ServiceResponse("AreaId is invalid.");
        }
        await using var context = await _contextFactory.CreateDbContextAsync();
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            var area = await context.Areas.FirstOrDefaultAsync(a => a.AreaId == id);
            if (area == null)
            {
                _logger.LogError("Area not found");
                return new ServiceResponse("Area not found");
            }

            if (area.IsDeleted)
            {
                context.Areas.Remove(area);
                await context.SaveChangesAsync();
                await transaction.CommitAsync();
                _logger.LogInformation("Area {AreaId} deleted", area.AreaId);
                return new ServiceResponse("Area deleted", true);
            }
            _logger.LogError("Area not marked as deleted");
            return new ServiceResponse("Area not marked as deleted");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting area");
            await transaction.RollbackAsync();
            return new ServiceResponse("Error deleting area");
        }
    }

    /// <summary>
    /// Deletes question if it is marked as deleted and exists.
    /// Deletion is pemanent
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<ServiceResponse> DeleteAsset(int id)
    {
        if (id <= 0)
        {
            return new ServiceResponse("AssetId is invalid.");
        }
        await using var context = await _contextFactory.CreateDbContextAsync();
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            var asset = await context.Assets.FirstOrDefaultAsync(a => a.AssetId == id);
            if (asset == null)
            {
                _logger.LogWarning("Asset not found");
                return new ServiceResponse("Asset not found");
            }
            if (asset.IsDeleted)
            {
                context.Assets.Remove(asset);
                _logger.LogInformation("Asset with id {AssetId} deleted", id);
                await context.SaveChangesAsync();
                await transaction.CommitAsync();
                return new ServiceResponse($"Asset {id} deleted", true);
            }
            _logger.LogWarning("Asset not marked as deleted");
            return new ServiceResponse("Asset not marked as deleted");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting asset with id {AssetId}", id);
            await transaction.RollbackAsync();
            return new ServiceResponse($"Error deleting asset with id {id}");
        }
    }

    /// <summary>
    /// Deletes category if it is marked as deleted and exists.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<ServiceResponse> DeleteCategory(int id)
    {
        if (id <= 0)
        {
            return new ServiceResponse("CategoryId is invalid.");
        }
        await using var context = await _contextFactory.CreateDbContextAsync();
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            var category = await context.Categories.Include(c => c.AssetCategories).Include(c => c.CommunicateCategories).Include(c => c.CategorySituations).FirstOrDefaultAsync(c => c.CategoryId == id);
            if (category == null)
            {
                _logger.LogWarning("Category not found");
                return new ServiceResponse("Category not found");
            }

            if (category.IsDeleted)
            {
                context.Categories.Remove(category);
                await context.SaveChangesAsync();
                await transaction.CommitAsync();
                _logger.LogInformation("Category {CategoryId} deleted", id);
                return new ServiceResponse("Category deleted", true);
            }
            _logger.LogWarning("Category not marked as deleted");
            return new ServiceResponse("Category not marked as deleted");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting category");
            await transaction.RollbackAsync();
            return new ServiceResponse("Error deleting category");
        }
    }

    /// <summary>
    /// Deletes communicate if it is marked as deleted and exists.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<ServiceResponse> DeleteCommunicate(int id)
    {
        if (id <= 0)
        {
            return new ServiceResponse("CommunicateId is invalid.");
        }
        await using var context = await _contextFactory.CreateDbContextAsync();
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            var communicate = await context.Communicates.FirstOrDefaultAsync(c => c.CommunicateId == id);
            if (communicate == null)
            {
                _logger.LogError("Communicate with id {Id} not found", id);
                return new ServiceResponse($"Communicate with id {id} not found");
            }
            if (communicate.IsDeleted)
            {
                context.Communicates.Remove(communicate);
                await context.SaveChangesAsync();
                await transaction.CommitAsync();
                _logger.LogInformation("Communicate with id {Id} deleted", id);
                return new ServiceResponse($"Communicate with id {id} deleted", true);
            }
            _logger.LogError("Communicate with id {Id} not marked as deleted", id);
            return new ServiceResponse($"Communicate with id {id} not marked as deleted");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting communicate with id {Id}", id);
            await transaction.RollbackAsync();
            return new ServiceResponse($"Error deleting communicate with id {id}");
        }
    }

    /// <summary>
    /// Deletes coordinate if it is marked as deleted and exists.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<ServiceResponse> DeleteCoordinate(int id)
    {
        if (id <= 0)
        {
            return new ServiceResponse("CoordinateId is invalid.");
        }
        await using var context = await _contextFactory.CreateDbContextAsync();
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            var coordinate = await context.Coordinates.FirstOrDefaultAsync(c => c.CoordinateId == id);
            if (coordinate == null)
            {
                _logger.LogError("Coordinate not found");
                return new ServiceResponse("Coordinate not found");
            }

            if (coordinate.IsDeleted)
            {
                context.Coordinates.Remove(coordinate);
                await context.SaveChangesAsync();
                await transaction.CommitAsync();
                _logger.LogInformation("Coordinate {CoordinateId} deleted", id);
                return new ServiceResponse("Coordinate deleted", true);
            }
            _logger.LogError("Coordinate not marked as deleted");
            return new ServiceResponse("Coordinate not marked as deleted");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting coordinate");
            await transaction.RollbackAsync();
            return new ServiceResponse("Error deleting coordinate");
        }
    }

    /// <summary>
    /// Deletes detail if it is marked as deleted and exists.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<ServiceResponse> DeleteDetail(int id)
    {
        if (id <= 0)
        {
            return new ServiceResponse("DetailId is invalid.");
        }
        await using var context = await _contextFactory.CreateDbContextAsync();
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            var detail = await context.Details.FirstOrDefaultAsync(d => d.DetailId == id);
            if (detail == null)
            {
                _logger.LogWarning("Detail not found");
                return new ServiceResponse("Detail not found");
            }
            if (detail.IsDeleted)
            {
                context.Details.Remove(detail);
                await context.SaveChangesAsync();
                await transaction.CommitAsync();
                _logger.LogInformation("Detail with id {DetailId} deleted", detail.DetailId);
                return new ServiceResponse($"Detail {detail.DetailId} deleted", true);
            }
            _logger.LogWarning("Detail not marked as deleted");
            return new ServiceResponse("Detail not marked as deleted");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting detail");
            await transaction.RollbackAsync();
            return new ServiceResponse($"Error deleting detail");
        }
    }

    /// <summary>
    /// Deletes device if it is marked as deleted and exists.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<ServiceResponse> DeleteDevice(int id)
    {
        if (id <= 0)
        {
            return new ServiceResponse("DeviceId is invalid.");
        }
        await using var context = await _contextFactory.CreateDbContextAsync();
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            var device = await context.Devices.FirstOrDefaultAsync(d => d.DeviceId == id);
            if (device == null)
            {
                _logger.LogWarning("Device not found");
                return new ServiceResponse("Device not found");
            }

            if (device.IsDeleted)
            {
                context.Devices.Remove(device);
                await context.SaveChangesAsync();
                await transaction.CommitAsync();
                _logger.LogInformation("Device with id {DeviceId} deleted", id);
                return new ServiceResponse($"Device {id} deleted", true);
            }
            _logger.LogWarning("Device not marked as deleted");
            return new ServiceResponse("Device not marked as deleted");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting device with id {DeviceId}", id);

            await transaction.RollbackAsync();

            return new ServiceResponse($"Error deleting device with id {id}");
        }
    }

    /// <summary>
    /// Deletes model if it is marked as deleted and exists.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<ServiceResponse> DeleteModel(int id)
    {
        if (id <= 0)
        {
            return new ServiceResponse("ModelId is invalid.");
        }
        await using var context = await _contextFactory.CreateDbContextAsync();
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            var model = await context.Models.FirstOrDefaultAsync(m => m.ModelId == id);
            if (model == null)
            {
                _logger.LogWarning("Model not found");
                return new ServiceResponse("Model not found");
            }

            if (model.IsDeleted)
            {
                context.Models.Remove(model);
                await context.SaveChangesAsync();
                await transaction.CommitAsync();
                _logger.LogInformation("Model with id {ModelId} deleted", model.ModelId);
                return new ServiceResponse($"Model {model.ModelId} deleted", true);
            }
            _logger.LogWarning("Model not marked as deleted");
            return new ServiceResponse("Model not marked as deleted");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting model with id {ModelId}", id);
            await transaction.RollbackAsync();
            return new ServiceResponse($"Error deleting model with id {id}");
        }
    }

    /// <summary>
    /// Deletes parameter if it is marked as deleted and exists.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<ServiceResponse> DeleteParameter(int id)
    {
        if (id <= 0)
        {
            return new ServiceResponse("ParameterId is invalid.");
        }
        await using var context = await _contextFactory.CreateDbContextAsync();
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            var parameter = await context.Parameters.FirstOrDefaultAsync(p => p.ParameterId == id);
            if (parameter == null)
            {
                _logger.LogWarning("Parameter not found");
                return new ServiceResponse("Parameter not found");
            }

            if (parameter.IsDeleted)
            {
                context.Remove(parameter);
                await context.SaveChangesAsync();
                await transaction.CommitAsync();
                _logger.LogInformation("Parameter with id {ParameterId} deleted", parameter.ParameterId);
                return new ServiceResponse($"Parameter {parameter.ParameterId} deleted", true);
            }
            _logger.LogWarning("Parameter with id {ParameterId} not marked as deleted", parameter.ParameterId);
            return new ServiceResponse($"Parameter with id {parameter.ParameterId} not marked as deleted");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting parameter with id {ParameterId}", id);
            await transaction.RollbackAsync();
            return new ServiceResponse($"Error deleting parameter with id {id}");
        }
    }

    /// <summary>
    /// Deletes plant if it is marked as deleted and exists.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<ServiceResponse> DeletePlant(int id)
    {
        if (id <= 0)
        {
            return new ServiceResponse("PlantId is invalid.");
        }
        await using var context = await _contextFactory.CreateDbContextAsync();
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            var plant = await context.Plants.FirstOrDefaultAsync(p => p.PlantId == id);
            if (plant == null)
            {
                _logger.LogError("Plant not found");
                return new ServiceResponse("Plant not found");
            }

            if (plant.IsDeleted)
            {
                context.Plants.Remove(plant);
                await context.SaveChangesAsync();
                await transaction.CommitAsync();
                _logger.LogInformation("Plant with id {PlantId} deleted", plant.PlantId);
                return new ServiceResponse("Plant deleted", true);
            }
            _logger.LogError("Plant not marked as deleted");
            return new ServiceResponse("Plant not marked as deleted");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting plant");
            await transaction.RollbackAsync();
            return new ServiceResponse("Error deleting plant");
        }
    }

    /// <summary>
    /// Deletes question if it is marked as deleted and exists.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<ServiceResponse> DeleteQuestion(int id)
    {
        if (id <= 0)
        {
            return new ServiceResponse("QuestionId is invalid.");
        }
        await using var context = await _contextFactory.CreateDbContextAsync();
        await using var transaction = await context.Database.BeginTransactionAsync();

        try
        {
            var question = await context.Questions.FirstOrDefaultAsync(q => q.QuestionId == id);
            if (question == null)
            {
                _logger.LogInformation("Question not found");
                return new ServiceResponse($"Question not found");
            }
            if (question.IsDeleted)
            {
                context.Questions.Remove(question);
                await context.SaveChangesAsync();
                await transaction.CommitAsync();
                _logger.LogInformation("Question with id {QuestionId} deleted", question.QuestionId);
                return new ServiceResponse($"Question {question.QuestionId} deleted", true);
            }
            _logger.LogInformation("Question not marked as deleted");
            return new ServiceResponse($"Question not marked as deleted");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting question");
            await transaction.RollbackAsync();
            return new ServiceResponse($"Error deleting question");
        }
    }

    /// <summary>
    /// Deletes situation if it is marked as deleted and exists.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<ServiceResponse> DeleteSituation(int id)
    {
        if (id <= 0)
        {
            return new ServiceResponse("SituationId is invalid.");
        }
        await using var context = await _contextFactory.CreateDbContextAsync();
        await using var transaction = await context.Database.BeginTransactionAsync();

        try
        {
            var situation = await context.Situations.Where(s => s.SituationId == id).FirstOrDefaultAsync();
            if (situation == null)
            {
                _logger.LogWarning("SituationId {id} not found", id);
                return new ServiceResponse($"SituationId {id} not found");
            }
            if (situation.IsDeleted)
            {
                context.Situations.Remove(situation);
                await context.SaveChangesAsync();
                await transaction.CommitAsync();
                _logger.LogInformation("SituationId {SituationId} deleted", situation.SituationId);
                return new ServiceResponse($"SituationId {situation.SituationId} deleted", true);
            }
            _logger.LogWarning("SituationId {SituationId} not marked as deleted", situation.SituationId);
            return new ServiceResponse($"SituationId {situation.SituationId} not marked as deleted");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting situation");
            await transaction.RollbackAsync();
            return new ServiceResponse($"Error deleting situation");
        }
    }

    /// <summary>
    /// Deletes space if it is marked as deleted and exists.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<ServiceResponse> DeleteSpace(int id)
    {
        if (id <= 0)
        {
            return new ServiceResponse("SpaceId is invalid.");
        }
        await using var context = await _contextFactory.CreateDbContextAsync();
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            var space = await context.Spaces.FirstOrDefaultAsync(s => s.SpaceId == id);
            if (space == null)
            {
                _logger.LogError("Space not found");
                return new ServiceResponse("Space not found");
            }
            if (space.IsDeleted)
            {
                context.Spaces.Remove(space);
                await context.SaveChangesAsync();
                await transaction.CommitAsync();
                _logger.LogInformation("Space with id {SpaceId} deleted", space.SpaceId);
                return new ServiceResponse("Space deleted", true);
            }
            _logger.LogError("Space not marked as deleted");
            return new ServiceResponse("Space not marked as deleted");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting space");
            await transaction.RollbackAsync();
            return new ServiceResponse("Error deleting space");
        }
    }

    /// <summary>
    /// Returns service response with area if it exists.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<ServiceResponse<AreaDto>> GetAreaById(int id)
    {
        if (id <= 0)
        {
            return new ServiceResponse<AreaDto>("AreaId is invalid.");
        }
        await using var context = await _contextFactory.CreateDbContextAsync();
        try
        {
            var area = await _mapper.ProjectTo<AreaDto>(context.Areas).FirstOrDefaultAsync(a => a.AreaId == id);
            if (area == null)
            {
                _logger.LogWarning("AreaId {id} not found", id);
                return new ServiceResponse<AreaDto>($"AreaId {id} not found");
            }

            return new ServiceResponse<AreaDto>(area, "Area returned");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting area by id");
            return new ServiceResponse<AreaDto>("Error getting area by id");
        }
    }

    /// <summary>
    /// Returns service response with areas if they exist.
    /// </summary>
    /// <returns></returns>
    public async Task<ServiceResponse<IEnumerable<AreaDto>>> GetAreas()
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        try
        {
            var areas = await _mapper.ProjectTo<AreaDto>(context.Areas).ToListAsync();
            if (areas.Count == 0)
            {
                _logger.LogWarning("No areas found");
                return new ServiceResponse<IEnumerable<AreaDto>>("No areas found");
            }
            return new ServiceResponse<IEnumerable<AreaDto>>(areas, "Areas returned");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all areas");
            return new ServiceResponse<IEnumerable<AreaDto>>("Error getting all areas");
        }
    }

    /// <summary>
    /// Returns service response with areas and spaces if they exist.
    /// </summary>
    /// <returns></returns>
    public async Task<ServiceResponse<IEnumerable<AreaDto>>> GetAreasWithSpaces()
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        try
        {
            var areas = await _mapper.ProjectTo<AreaDto>(context.Areas.Include(s => s.Spaces)).ToListAsync();
            if (areas.Count == 0)
            {
                return new ServiceResponse<IEnumerable<AreaDto>>("Areas not found");
            }
            return new ServiceResponse<IEnumerable<AreaDto>>(areas,
                "List of areas with spaces returned");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting areas with spaces");
            return new ServiceResponse<IEnumerable<AreaDto>>("Error getting areas with spaces");
        }
    }

    /// <summary>
    /// Returns service response with asset by id if it exists.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<ServiceResponse<AssetDto>> GetAssetById(int id)
    {
        if (id <= 0)
        {
            return new ServiceResponse<AssetDto>("AssetId is invalid.");
        }
        if (id <= 0)
        {
            _logger.LogWarning("AssetId {id} not valid", id);
            return new ServiceResponse<AssetDto>($"AssetId {id} not valid");
        }
        await using var context = await _contextFactory.CreateDbContextAsync();
        try
        {
            var asset = await _mapper.ProjectTo<AssetDto>(context.Assets.Include(a => a.AssetDetails).Include(a => a.AssetCategories))
                .FirstOrDefaultAsync(a => a.AssetId == id);
            if (asset == null)
            {
                _logger.LogWarning("AssetId {id} not found", id);
                return new ServiceResponse<AssetDto>("Asset not found");
            }

            return new ServiceResponse<AssetDto>(asset, "Asset returned");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting asset by id");
            return new ServiceResponse<AssetDto>("Error getting asset by id");
        }
    }

    /// <summary>
    /// Returns service response with assets if they exist.
    /// </summary>
    /// <returns></returns>
    public async Task<ServiceResponse<IEnumerable<AssetDto>>> GetAssets()
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        try
        {
            var assets = await _mapper.ProjectTo<AssetDto>(context.Assets.Include(a => a.AssetDetails).Include(a => a.AssetCategories))
                .ToListAsync();
            if (assets.Count == 0)
            {
                _logger.LogWarning("No assets found");
                return new ServiceResponse<IEnumerable<AssetDto>>("No assets found");
            }
            return new ServiceResponse<IEnumerable<AssetDto>>(assets, "List of assets returned");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all assets");
            return new ServiceResponse<IEnumerable<AssetDto>>("Error getting all assets");
        }
    }

    /// <summary>
    /// Returns service response with assets and all data if they exist.
    /// </summary>
    /// <returns></returns>
    public async Task<ServiceResponse<IEnumerable<AssetDto>>> GetAssetsWithAllData()
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        try
        {
            var assets = await _mapper.ProjectTo<AssetDto>(context.Assets.Include(a => a.AssetDetails).Include(a => a.AssetCategories)
                .Include(a => a.Coordinate).Include(c => c.CommunicateAssets).Include(a => a.Model)).ToListAsync();
            if (assets.Count == 0)
            {
                _logger.LogWarning("No assets found");
                return new ServiceResponse<IEnumerable<AssetDto>>("Assets not found");
            }
            return new ServiceResponse<IEnumerable<AssetDto>>(assets, "List of assets returned");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all assets");
            return new ServiceResponse<IEnumerable<AssetDto>>("Error getting all assets");
        }
    }

    /// <summary>
    /// Returns categories if they exist.
    /// </summary>
    /// <returns></returns>
    public async Task<ServiceResponse<IEnumerable<CategoryDto>>> GetCategories()
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        try
        {
            var categories = await _mapper.ProjectTo<CategoryDto>(context.Categories).ToListAsync();
            return new ServiceResponse<IEnumerable<CategoryDto>>(categories, "Categories found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting categories");
            return new ServiceResponse<IEnumerable<CategoryDto>>("Error getting categories");
        }
    }

    /// <summary>
    /// Returns service response with categories and assets if they exist.
    /// </summary>
    /// <returns></returns>
    public async Task<ServiceResponse<IEnumerable<CategoryWithAssetsDto>>> GetCategoriesWithAssets()
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        try
        {
            var categories = await _mapper.ProjectTo<CategoryWithAssetsDto>(context.Categories.Include(a => a.AssetCategories).ThenInclude(a => a.Asset))
                .ToListAsync();
            return new ServiceResponse<IEnumerable<CategoryWithAssetsDto>>(categories, "Categories found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting categories");
            return new ServiceResponse<IEnumerable<CategoryWithAssetsDto>>("Error getting categories");
        }
    }

    /// <summary>
    /// Returns service response with category by id if exists
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<ServiceResponse<CategoryDto>> GetCategoryById(int id)
    {
        if (id <= 0)
        {
            return new ServiceResponse<CategoryDto>("Invalid category id.");
        }
        await using var context = await _contextFactory.CreateDbContextAsync();
        try
        {
            var category = await _mapper.ProjectTo<CategoryDto>(context.Categories).FirstOrDefaultAsync(c => c.CategoryId == id);
            if (category == null)
            {
                return new ServiceResponse<CategoryDto>("Category not found");
            }

            return new ServiceResponse<CategoryDto>(category, "Category found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting category");
            return new ServiceResponse<CategoryDto>("Error getting category");
        }
    }

    /// <summary>
    /// Returns service response with category by id with assets if exists
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<ServiceResponse<CategoryWithAssetsDto>> GetCategoryByIdWithAssets(int id)
    {
        if (id <= 0)
        {
            return new ServiceResponse<CategoryWithAssetsDto>("Invalid category id");
        }
        await using var context = await _contextFactory.CreateDbContextAsync();
        try
        {
            var category = await _mapper.ProjectTo<CategoryWithAssetsDto>(context.Categories.Include(a => a.AssetCategories).ThenInclude(a => a.Asset))
                .FirstOrDefaultAsync(a => a.CategoryId == id);
            if (category == null)
            {
                _logger.LogWarning("CategoryId {id} not found", id);
                return new ServiceResponse<CategoryWithAssetsDto>("Category not found");
            }
            return new ServiceResponse<CategoryWithAssetsDto>(category, "Category returned");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting category");
            return new ServiceResponse<CategoryWithAssetsDto>("Error getting category with assets");
        }
    }

    /// <summary>
    /// Returns service response with communicate if exists
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<ServiceResponse<CommunicateDto>> GetCommunicateById(int id)
    {
        if (id <= 0)
        {
            return new ServiceResponse<CommunicateDto>("Invalid communicate id");
        }
        await using var context = await _contextFactory.CreateDbContextAsync();
        try
        {
            var communicate = await _mapper.ProjectTo<CommunicateDto>(context.Communicates).FirstOrDefaultAsync(c => c.CommunicateId == id);
            if (communicate == null)
            {
                _logger.LogInformation("Communicate not found");
                return new ServiceResponse<CommunicateDto>($"Communicate not found");
            }
            _logger.LogInformation("Communicate found");
            return new ServiceResponse<CommunicateDto>(communicate, "Communicate found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting communicate");
            return new ServiceResponse<CommunicateDto>($"Error getting communicate");
        }
    }

    /// <summary>
    /// Returns service response with communicates if they exists
    /// </summary>
    /// <returns></returns>
    public async Task<ServiceResponse<IEnumerable<CommunicateDto>>> GetCommunicates()
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        try
        {
            var communicates = await _mapper.ProjectTo<CommunicateDto>(context.Communicates).ToListAsync();
            if (communicates.Count == 0)
            {
                _logger.LogWarning("No communicates found");
                return new ServiceResponse<IEnumerable<CommunicateDto>>("No communicates found");
            }

            _logger.LogInformation("Communicates found");
            return new ServiceResponse<IEnumerable<CommunicateDto>>(communicates, "Communicates found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting communicates");
            return new ServiceResponse<IEnumerable<CommunicateDto>>($"Error getting communicates");
        }
    }

    /// <summary>
    /// Returns service response of communicates with assets if they exist
    /// </summary>
    /// <returns></returns>
    public async Task<ServiceResponse<IEnumerable<CommunicateWithAssetsDto>>> GetCommunicatesWithAssets()
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        try
        {
            var communicates = await _mapper.ProjectTo<CommunicateWithAssetsDto>(context.Communicates.Include(a => a.CommunicateAssets).ThenInclude(a => a.Asset)).ToListAsync();
            if (communicates.Count == 0)
            {
                _logger.LogWarning("No communicates found");
                return new ServiceResponse<IEnumerable<CommunicateWithAssetsDto>>("No communicates found");
            }

            _logger.LogInformation("Communicates found");
            return new ServiceResponse<IEnumerable<CommunicateWithAssetsDto>>(communicates, "Communicates found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting communicates");
            return new ServiceResponse<IEnumerable<CommunicateWithAssetsDto>>($"Error getting communicates");
        }
    }

    /// <summary>
    /// Returns service response with coordinate by id with assets if exists
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<ServiceResponse<CoordinateDto>> GetCoordinateByIdWithAssets(int id)
    {
        if (id <= 0)
        {
            return new ServiceResponse<CoordinateDto>("Invalid coordinate id");
        }
        await using var context = await _contextFactory.CreateDbContextAsync();
        try
        {
            var coordinate = await _mapper.ProjectTo<CoordinateDto>(context.Coordinates.Include(a => a.Assets)).FirstOrDefaultAsync(c => c.CoordinateId == id);
            if (coordinate == null)
            {
                _logger.LogWarning("Coordinate not found");
                return new ServiceResponse<CoordinateDto>("Coordinate not found");
            }

            return new ServiceResponse<CoordinateDto>(coordinate, "Coordinate returned");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting coordinate by id");
            return new ServiceResponse<CoordinateDto>("Error getting coordinate by id");
        }
    }

    /// <summary>
    /// Returns service response with coordinates if they exists
    /// </summary>
    /// <returns></returns>
    public async Task<ServiceResponse<IEnumerable<CoordinateDto>>> GetCoordinates()
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        try
        {
            var coordinates = await _mapper.ProjectTo<CoordinateDto>(context.Coordinates).ToListAsync();
            if (coordinates.Count == 0)
            {
                return new ServiceResponse<IEnumerable<CoordinateDto>>("Coordinates not found");
            }
            return new ServiceResponse<IEnumerable<CoordinateDto>>(coordinates, "List of coordinates returned");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all coordinates");
            return new ServiceResponse<IEnumerable<CoordinateDto>>("Error getting all coordinates");
        }
    }

    /// <summary>
    /// Returns service response with coordinates and assets if they exists
    /// </summary>
    /// <returns></returns>
    public async Task<ServiceResponse<IEnumerable<CoordinateDto>>> GetCoordinatesWithAssets()
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        try
        {
            var coordinates = await _mapper.ProjectTo<CoordinateDto>(context.Coordinates.Include(a => a.Assets)).ToListAsync();
            if (coordinates.Count == 0)
            {
                return new ServiceResponse<IEnumerable<CoordinateDto>>("Coordinates not found");
            }

            return new ServiceResponse<IEnumerable<CoordinateDto>>(coordinates, "List of coordinates with assets returned");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all coordinates with assets");
            return new ServiceResponse<IEnumerable<CoordinateDto>>("Error getting all coordinates with assets");
        }
    }

    /// <summary>
    /// Returns service response with detail by id if it exists
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<ServiceResponse<DetailDto>> GetDetailById(int id)
    {
        if (id <= 0)
        {
            return new ServiceResponse<DetailDto>("Invalid detail id");
        }
        await using var context = await _contextFactory.CreateDbContextAsync();
        try
        {
            var detail = await _mapper.ProjectTo<DetailDto>(context.Details).FirstOrDefaultAsync(d => d.DetailId == id);
            if (detail == null)
            {
                _logger.LogWarning("Detail not found");
                return new ServiceResponse<DetailDto>("Detail not found");
            }
            _logger.LogInformation("Detail with id {DetailId} found", detail.DetailId);
            return new ServiceResponse<DetailDto>(detail, "Detail found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting detail with id {DetailId}", id);
            return new ServiceResponse<DetailDto>($"Error getting detail with id {id}");
        }
    }

    /// <summary>
    /// Returns service response with details if they exist
    /// </summary>
    /// <returns></returns>
    public async Task<ServiceResponse<IEnumerable<DetailDto>>> GetDetails()
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        try
        {
            var details = await _mapper.ProjectTo<DetailDto>(context.Details).ToListAsync();
            _logger.LogInformation("{DetailCount} details found", details.Count);
            return new ServiceResponse<IEnumerable<DetailDto>>(details, $"{details.Count} details found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting details");
            return new ServiceResponse<IEnumerable<DetailDto>>("Error getting details");
        }
    }

    /// <summary>
    /// Returns service response with detail with assets if they exist
    /// </summary>
    /// <returns></returns>
    public async Task<ServiceResponse<IEnumerable<DetailWithAssetsDto>>> GetDetailsWithAssets()
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        try
        {
            var details = await _mapper.ProjectTo<DetailWithAssetsDto>(context.Details.Include(d => d.AssetDetails).ThenInclude(a => a.Asset)).ToListAsync();
            _logger.LogInformation("{DetailCount} details found", details.Count);
            return new ServiceResponse<IEnumerable<DetailWithAssetsDto>>(details, $"{details.Count} details found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting details");
            return new ServiceResponse<IEnumerable<DetailWithAssetsDto>>("Error getting details");
        }
    }

    /// <summary>
    /// Returns service response with device by id if exist
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<ServiceResponse<DeviceDto>> GetDeviceById(int id)
    {
        if (id <= 0)
        {
            return new ServiceResponse<DeviceDto>("Invalid device id");
        }
        await using var context = await _contextFactory.CreateDbContextAsync();
        try
        {
            var device = await _mapper.ProjectTo<DeviceDto>(context.Devices).FirstOrDefaultAsync(d => d.DeviceId == id);
            if (device == null)
            {
                return new ServiceResponse<DeviceDto>("Device not found");
            }

            return new ServiceResponse<DeviceDto>(device, "Device returned");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting device by id");
            return new ServiceResponse<DeviceDto>("Error getting device by id");
        }
    }

    /// <summary>
    /// Returns service response with devices if they exist
    /// </summary>
    /// <returns></returns>
    public async Task<ServiceResponse<IEnumerable<DeviceDto>>> GetDevices()
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        try
        {
            var devices = await _mapper.ProjectTo<DeviceDto>(context.Devices).ToListAsync();
            if (devices.Count == 0)
            {
                return new ServiceResponse<IEnumerable<DeviceDto>>("Devices not found");
            }

            return new ServiceResponse<IEnumerable<DeviceDto>>(devices, "Devices returned");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting devices");
            return new ServiceResponse<IEnumerable<DeviceDto>>("Error getting devices");
        }
    }

    /// <summary>
    /// Returns service response with device and models if they exist
    /// </summary>
    /// <returns></returns>
    public async Task<ServiceResponse<IEnumerable<DeviceDto>>> GetDevicesWithModels()
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        try
        {
            var devices = await _mapper.ProjectTo<DeviceDto>(context.Devices.Include(d => d.Models)).ToListAsync();
            if (devices.Count == 0)
            {
                return new ServiceResponse<IEnumerable<DeviceDto>>("Devices not found");
            }

            return new ServiceResponse<IEnumerable<DeviceDto>>(devices,
                "Devices returned");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting devices");
            return new ServiceResponse<IEnumerable<DeviceDto>>("Error getting devices");
        }
    }

    /// <summary>
    /// Returns service response with model if exist
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<ServiceResponse<ModelDto>> GetModelById(int id)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        try
        {
            var model = await _mapper.ProjectTo<ModelDto>(context.Models).FirstOrDefaultAsync(m => m.ModelId == id);
            if (model == null)
            {
                _logger.LogWarning("Model not found");
                return new ServiceResponse<ModelDto>("Model not found");
            }

            _logger.LogInformation("Model with id {ModelId} retrieved", model.ModelId);
            return new ServiceResponse<ModelDto>(model, $"Model with id {model.ModelId} retrieved");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving model with id {ModelId}", id);
            return new ServiceResponse<ModelDto>($"Error retrieving model with id {id}");
        }
    }

    /// <summary>
    /// Returns service response with models if they exist
    /// </summary>
    /// <returns></returns>
    public async Task<ServiceResponse<IEnumerable<ModelDto>>> GetModels()
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        try
        {
            var models = await _mapper.ProjectTo<ModelDto>(context.Models).ToListAsync();
            _logger.LogInformation("Models retrieved");
            return new ServiceResponse<IEnumerable<ModelDto>>(models, "Models retrieved");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving models");
            return new ServiceResponse<IEnumerable<ModelDto>>("Error retrieving models");
        }
    }

    /// <summary>
    /// Returns service response with model and assets if they exist
    /// </summary>
    /// <returns></returns>
    public async Task<ServiceResponse<IEnumerable<ModelDto>>> GetModelsWithAssets()
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        try
        {
            var models = await _mapper.ProjectTo<ModelDto>(context.Models.Include(m => m.Assets)).ToListAsync();
            _logger.LogInformation("Models retrieved");
            return new ServiceResponse<IEnumerable<ModelDto>>(models, "Models with assets retrieved");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving models");
            return new ServiceResponse<IEnumerable<ModelDto>>("Error retrieving models");
        }
    }

    /// <summary>
    /// Returns service response with parameter if it exist
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<ServiceResponse<ParameterDto>> GetParameterById(int id)
    {
        if (id <= 0)
        {
            return new ServiceResponse<ParameterDto>("Invalid parameter id");
        }
        await using var context = await _contextFactory.CreateDbContextAsync();
        try
        {
            var parameter = await _mapper.ProjectTo<ParameterDto>(context.Parameters)
                .FirstOrDefaultAsync(p => p.ParameterId == id);
            if (parameter == null)
            {
                _logger.LogWarning("Parameter not found");
                return new ServiceResponse<ParameterDto>("Parameter not found");
            }

            _logger.LogInformation("Parameter with id {ParameterId} retrieved", parameter.ParameterId);
            return new ServiceResponse<ParameterDto>(parameter, $"Parameter {parameter.ParameterId} retrieved");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving parameter with id {ParameterId}", id);
            return new ServiceResponse<ParameterDto>($"Error retrieving parameter with id {id}");
        }
    }

    /// <summary>
    /// Returns service response with parameters if they exist
    /// </summary>
    /// <returns></returns>
    public async Task<ServiceResponse<IEnumerable<ParameterDto>>> GetParameters()
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        try
        {
            var parameters = await _mapper.ProjectTo<ParameterDto>(context.Parameters).ToListAsync();
            if (parameters.Count == 0)
            {
                _logger.LogWarning("No parameters found");
                return new ServiceResponse<IEnumerable<ParameterDto>>("No parameters found");
            }

            _logger.LogInformation("{parameters.Count} parameters retrieved", parameters.Count);
            return new ServiceResponse<IEnumerable<ParameterDto>>(parameters,
                $"{parameters.Count} parameters retrieved");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving parameters");
            return new ServiceResponse<IEnumerable<ParameterDto>>("Error retrieving parameters");
        }
    }

    /// <summary>
    /// Returns service response with parameters and models if they exist
    /// </summary>
    /// <returns></returns>
    public async Task<ServiceResponse<IEnumerable<ParameterWithModelsDto>>> GetParametersWithModels()
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        try
        {
            var parameters = await _mapper.ProjectTo<ParameterWithModelsDto>(context.Parameters.Include(p => p.ModelParameters).ThenInclude(m => m.Model))
                .ToListAsync();
            if (parameters.Count == 0)
            {
                _logger.LogWarning("No parameters found");
                return new ServiceResponse<IEnumerable<ParameterWithModelsDto>>("No parameters found");
            }
            else
            {
                _logger.LogWarning("No parameters found");
                return new ServiceResponse<IEnumerable<ParameterWithModelsDto>>("No parameters found");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving parameters with models");
            return new ServiceResponse<IEnumerable<ParameterWithModelsDto>>("Error retrieving parameters with models");
        }
    }

    /// <summary>
    /// Returns service response with plant if it exist
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<ServiceResponse<PlantDto>> GetPlantById(int id)
    {
        if (id <= 0)
        {
            return new ServiceResponse<PlantDto>("Invalid plant id");
        }
        await using var context = await _contextFactory.CreateDbContextAsync();

        try
        {
            var plant = await _mapper.ProjectTo<PlantDto>(context.Plants).FirstOrDefaultAsync(p => p.PlantId == id);
            if (plant == null)
            {
                _logger.LogWarning("Plant not found");
                return new ServiceResponse<PlantDto>("Plant not found");
            }
            _logger.LogInformation("Plant with id {PlantId} retrieved", plant.PlantId);
            return new ServiceResponse<PlantDto>(plant, "Plant returned");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting plant by id");
            return new ServiceResponse<PlantDto>("Error getting plant by id");
        }
    }

    /// <summary>
    /// Returns service response with plants if they exist
    /// </summary>
    /// <returns></returns>
    public async Task<ServiceResponse<IEnumerable<PlantDto>>> GetPlants()
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        try
        {
            var plants = await _mapper.ProjectTo<PlantDto>(context.Plants).ToListAsync();
            if (plants.Count == 0)
            {
                _logger.LogWarning("No plants found");
                return new ServiceResponse<IEnumerable<PlantDto>>("No plants found");
            }
            _logger.LogInformation("{plants.Count} plants retrieved", plants.Count);
            return new ServiceResponse<IEnumerable<PlantDto>>(plants, "List of plants returned");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all plants");
            return new ServiceResponse<IEnumerable<PlantDto>>("Error getting all plants");
        }
    }

    /// <summary>
    /// Returns service response with plants contains areas if they exist
    /// </summary>
    /// <returns></returns>
    public async Task<ServiceResponse<IEnumerable<PlantDto>>> GetPlantsWithAreas()
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        try
        {
            var plants = await _mapper.ProjectTo<PlantDto>(context.Plants.Include(a => a.Areas)).ToListAsync();
            if (plants.Count == 0)
            {
                return new ServiceResponse<IEnumerable<PlantDto>>("Plants not found");
            }

            return new ServiceResponse<IEnumerable<PlantDto>>(plants, "List of plants returned");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting plants with areas");
            return new ServiceResponse<IEnumerable<PlantDto>>("Error getting plants with areas");
        }
    }

    /// <summary>
    /// Returns service response with question by id if it exist
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<ServiceResponse<QuestionDto>> GetQuestionById(int id)
    {
        if (id <= 0)
        {
            return new ServiceResponse<QuestionDto>("Invalid question id");
        }
        await using var context = await _contextFactory.CreateDbContextAsync();
        try
        {
            var question = await _mapper.ProjectTo<QuestionDto>(context.Questions.Where(q => q.QuestionId == id)).FirstOrDefaultAsync();
            if (question == null)
            {
                _logger.LogWarning("QuestionId {id} not found", id);
                return new ServiceResponse<QuestionDto>($"QuestionId {id} not found");
            }
            _logger.LogInformation("QuestionId {id} found", id);
            return new ServiceResponse<QuestionDto>(question, $"Question with Id {id} found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting question");
            return new ServiceResponse<QuestionDto>($"Error getting question");
        }
    }

    /// <summary>
    /// Returns service response with questions if they exist
    /// </summary>
    /// <returns></returns>
    public async Task<ServiceResponse<IEnumerable<QuestionDto>>> GetQuestions()
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        try
        {
            var questions = await _mapper.ProjectTo<QuestionDto>(context.Questions).ToListAsync();
            if (questions.Count == 0)
            {
                _logger.LogWarning("No questions found");
                return new ServiceResponse<IEnumerable<QuestionDto>>("No questions found");
            }

            _logger.LogInformation("Questions found");
            return new ServiceResponse<IEnumerable<QuestionDto>>(questions, "List of questions returned");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting questions");
            return new ServiceResponse<IEnumerable<QuestionDto>>($"Error getting questions");
        }
    }

    /// <summary>
    /// Returns service response with questions containing situations if they exist
    /// </summary>
    /// <returns></returns>
    public async Task<ServiceResponse<IEnumerable<QuestionWithSituationsDto>>> GetQuestionsWithSituations()
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        try
        {
            var questions = await _mapper.ProjectTo<QuestionWithSituationsDto>(context.Questions.Include(q => q.SituationQuestions).ThenInclude(s => s.Situation)).ToListAsync();
            if (questions.Count == 0)
            {
                _logger.LogWarning("No questions found");
                return new ServiceResponse<IEnumerable<QuestionWithSituationsDto>>("No questions found");
            }
            _logger.LogInformation("Questions found");
            return new ServiceResponse<IEnumerable<QuestionWithSituationsDto>>(questions, "List of questions with situations returned");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting questions");
            return new ServiceResponse<IEnumerable<QuestionWithSituationsDto>>($"Error getting questions");
        }
    }

    /// <summary>
    /// Returns service response with situation by id if it exist
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<ServiceResponse<SituationDto>> GetSituationById(int id)
    {
        if (id <= 0)
        {
            return new ServiceResponse<SituationDto>("Invalid situation id");
        }
        await using var context = await _contextFactory.CreateDbContextAsync();
        try
        {
            var situation = await _mapper.ProjectTo<SituationDto>(context.Situations).FirstOrDefaultAsync(s => s.SituationId == id);
            if (situation == null)
            {
                _logger.LogWarning("SituationId {id} not found", id);
                return new ServiceResponse<SituationDto>($"SituationId {id} not found");
            }
            _logger.LogInformation("SituationId {id} found", id);
            return new ServiceResponse<SituationDto>(situation, $"Situation with Id {id} found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting situation");
            return new ServiceResponse<SituationDto>("Error getting situation");
        }
    }

    /// <summary>
    /// Returns service response with situations if they exist
    /// </summary>
    /// <returns></returns>
    public async Task<ServiceResponse<IEnumerable<SituationDto>>> GetSituations()
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        try
        {
            var situations = await _mapper.ProjectTo<SituationDto>(context.Situations).ToListAsync();
            if (situations.Count == 0)
            {
                _logger.LogWarning("No situations found");
                return new ServiceResponse<IEnumerable<SituationDto>>("No situations found");
            }
            _logger.LogInformation("Situations found");
            return new ServiceResponse<IEnumerable<SituationDto>>(situations, "List of situations returned");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting situations");
            return new ServiceResponse<IEnumerable<SituationDto>>("Error getting situations");
        }
    }

    /// <summary>
    /// Returns service response with situations containing assets if they exist
    /// </summary>
    /// <returns></returns>
    public async Task<ServiceResponse<IEnumerable<SituationWithAssetsDto>>> GetSituationsWithAssets()
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        try
        {
            var situations = await _mapper.ProjectTo<SituationWithAssetsDto>(context.Situations.Include(d => d.AssetSituations).ThenInclude(a => a.Asset)).ToListAsync();
            if (situations.Count == 0)
            {
                _logger.LogWarning("No situations found");
                return new ServiceResponse<IEnumerable<SituationWithAssetsDto>>("No situations found");
            }
            _logger.LogInformation("Situations found");
            return new ServiceResponse<IEnumerable<SituationWithAssetsDto>>(situations, "List of situations with assets returned");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting situations");
            return new ServiceResponse<IEnumerable<SituationWithAssetsDto>>("Error getting situations");
        }
    }

    /// <summary>
    /// Returns service response with situations containing categories if they exist
    /// </summary>
    /// <returns></returns>
    public async Task<ServiceResponse<IEnumerable<SituationWithCategoriesDto>>> GetSituationsWithCategories()
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        try
        {
            var situations = await _mapper.ProjectTo<SituationWithCategoriesDto>(context.Situations.Include(a => a.CategorySituations).ThenInclude(c => c.Category)).ToListAsync();
            if (situations.Count == 0)
            {
                _logger.LogWarning("No situations found");
                return new ServiceResponse<IEnumerable<SituationWithCategoriesDto>>("No situations found");
            }
            _logger.LogInformation("Situations found");
            return new ServiceResponse<IEnumerable<SituationWithCategoriesDto>>(situations, "List of situations with categories returned");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting situations");
            return new ServiceResponse<IEnumerable<SituationWithCategoriesDto>>("Error getting situations");
        }
    }

    /// <summary>
    /// Returns service response with space by id if it exist
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<ServiceResponse<SpaceDto>> GetSpaceById(int id)
    {
        if (id <= 0)
        {
            return new ServiceResponse<SpaceDto>("Invalid space id");
        }
        await using var context = await _contextFactory.CreateDbContextAsync();
        try
        {
            var space = await _mapper.ProjectTo<SpaceDto>(context.Spaces).FirstOrDefaultAsync(s => s.SpaceId == id);
            if (space == null)
            {
                _logger.LogWarning("SpaceId {id} not found", id);
                return new ServiceResponse<SpaceDto>($"SpaceId {id} not found");
            }
            _logger.LogInformation("SpaceId {id} found", id);
            return new ServiceResponse<SpaceDto>(space, $"Space with Id {id} found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting space by id");
            return new ServiceResponse<SpaceDto>("Error getting space by id");
        }
    }

    /// <summary>
    /// Returns service response with spaces if they exist
    /// </summary>
    /// <returns></returns>
    public async Task<ServiceResponse<IEnumerable<SpaceDto>>> GetSpaces()
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        try
        {
            var spaces = await _mapper.ProjectTo<SpaceDto>(context.Spaces).ToListAsync();
            if (spaces.Count == 0)
            {
                _logger.LogWarning("No spaces found");
                return new ServiceResponse<IEnumerable<SpaceDto>>("No spaces found");
            }
            _logger.LogInformation("Spaces found");
            return new ServiceResponse<IEnumerable<SpaceDto>>(spaces, "List of spaces returned");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all spaces");
            return new ServiceResponse<IEnumerable<SpaceDto>>("Error getting all spaces");
        }
    }

    /// <summary>
    /// Returns service response with spaces containing coordinates if they exist
    /// </summary>
    /// <returns></returns>
    public async Task<ServiceResponse<IEnumerable<SpaceDto>>> GetSpacesWithCoordinates()
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        try
        {
            var spaces = await _mapper.ProjectTo<SpaceDto>(context.Spaces.Include(s => s.Coordinates)).ToListAsync();
            if (spaces.Count == 0)
            {
                _logger.LogWarning("No spaces found");
                return new ServiceResponse<IEnumerable<SpaceDto>>("No spaces found");
            }
            _logger.LogInformation("Spaces found");
            return new ServiceResponse<IEnumerable<SpaceDto>>(spaces, "List of spaces with coordinates returned");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting spaces with coordinates");
            return new ServiceResponse<IEnumerable<SpaceDto>>("Error getting spaces with coordinates");
        }
    }

    /// <summary>
    /// Returns service response with true if area by id is marked as deleted
    /// Requires all included spaces are marked as deleted or deleted.
    /// Marks all relations as deleted
    /// </summary>
    /// <param name="id"></param>
    /// <param name="userId"></param>
    /// <returns>bool</returns>
    public async Task<ServiceResponse> MarkDeleteArea(int id, string userId)
    {
        if (id <= 0)
        {
            return new ServiceResponse("Invalid area id");
        }
        await using var context = await _contextFactory.CreateDbContextAsync();
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            var area = await context.Areas.Include(s => s.Spaces).Include(a => a.CommunicateAreas).FirstOrDefaultAsync(a => a.AreaId == id);
            if (area == null)
            {
                return new ServiceResponse("Area not found");
            }

            if (area.Spaces.Any(s => s.IsDeleted == false))
            {
                return new ServiceResponse("Cannot delete area with spaces");
            }
            foreach (var communicateArea in area.CommunicateAreas)
            {
                communicateArea.IsDeleted = true;
                communicateArea.UserId = userId;
            }
            context.CommunicateAreas.UpdateRange(area.CommunicateAreas);
            area.IsDeleted = true;
            area.UserId = userId;

            context.Areas.Update(area);
            await context.SaveChangesAsync();
            await transaction.CommitAsync();
            return new ServiceResponse("Area marked as deleted", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking area as deleted");
            await transaction.RollbackAsync();
            return new ServiceResponse("Error deleting area");
        }
    }

    /// <summary>
    /// Returns service response with true if asset by id is marked as deleted
    /// Marks all relations as deleted
    /// </summary>
    /// <param name="id"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    public async Task<ServiceResponse> MarkDeleteAsset(int id, string userId)
    {
        if (id <= 0)
        {
            return new ServiceResponse("Invalid asset id");
        }
        await using var context = await _contextFactory.CreateDbContextAsync();
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            var asset = await context.Assets.Include(a => a.AssetDetails).Include(a => a.AssetCategories).Include(a => a.AssetSituations).Include(a => a.CommunicateAssets).FirstOrDefaultAsync(a => a.AssetId == id);
            if (asset == null)
            {
                _logger.LogWarning("Asset not found");
                return new ServiceResponse("Asset not found");
            }

            if (asset.IsDeleted)
            {
                _logger.LogWarning("Asset marked as deleted");
                return new ServiceResponse("Asset marked as deleted", true);
            }
            foreach (var communicateAsset in asset.CommunicateAssets)
            {
                communicateAsset.IsDeleted = true;
                communicateAsset.UserId = userId;
            }
            context.CommunicateAssets.UpdateRange(asset.CommunicateAssets);
            foreach (var assetSituation in asset.AssetSituations)
            {
                assetSituation.IsDeleted = true;
                assetSituation.UserId = userId;
            }
            context.AssetSituations.UpdateRange(asset.AssetSituations);
            foreach (var assetCategory in asset.AssetCategories)
            {
                assetCategory.IsDeleted = true;
                assetCategory.UserId = userId;
            }
            context.AssetCategories.UpdateRange(asset.AssetCategories);
            foreach (var assetDetail in asset.AssetDetails)
            {
                assetDetail.IsDeleted = true;
                assetDetail.UserId = userId;
            }
            context.AssetDetails.UpdateRange(asset.AssetDetails);

            asset.IsDeleted = true;
            asset.UserId = userId;
            context.Assets.Update(asset);
            await context.SaveChangesAsync();
            await transaction.CommitAsync();
            _logger.LogInformation("Asset with id {AssetId} marked as deleted", id);
            return new ServiceResponse($"Asset {id} marked as deleted", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking asset with id {AssetId} as deleted", id);
            await transaction.RollbackAsync();
            return new ServiceResponse($"Error marking asset with id {id} as deleted");
        }
    }

    /// <summary>
    /// Returns service response with true if category by id is marked as deleted
    /// </summary>
    /// <param name="id"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    public async Task<ServiceResponse> MarkDeleteCategory(int id, string userId)
    {
        if (id <= 0)
        {
            return new ServiceResponse("Invalid category id");
        }
        await using var context = await _contextFactory.CreateDbContextAsync();
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            var category = await context.Categories.Include(c => c.AssetCategories).Include(c => c.CategorySituations).Include(c => c.CommunicateCategories)
                .FirstOrDefaultAsync(c => c.CategoryId == id);
            if (category == null)
            {
                return new ServiceResponse("Category not found");
            }

            if (category.IsDeleted)
            {
                return new ServiceResponse("Category already marked as deleted");
            }

            foreach (var assetCategory in category.AssetCategories)
            {
                assetCategory.IsDeleted = true;
                assetCategory.UserId = userId;
            }
            context.AssetCategories.UpdateRange(category.AssetCategories);

            foreach (var communicateCategory in category.CommunicateCategories)
            {
                communicateCategory.IsDeleted = true;
                communicateCategory.UserId = userId;
            }
            context.CommunicateCategories.UpdateRange(category.CommunicateCategories);

            foreach (var categorySituation in category.CategorySituations)
            {
                categorySituation.IsDeleted = true;
                categorySituation.UserId = userId;
            }
            context.CategorySituations.UpdateRange(category.CategorySituations);
            category.IsDeleted = true;
            category.UserId = userId;
            context.Categories.Update(category);
            await context.SaveChangesAsync();
            await transaction.CommitAsync();
            return new ServiceResponse("Category marked as deleted", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking category as deleted");
            await transaction.RollbackAsync();
            return new ServiceResponse("Error marking category as deleted");
        }
    }

    /// <summary>
    /// Returns service response with true if communicate by id is marked as deleted
    /// Marks all relations as deleted
    /// </summary>
    /// <param name="id"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    public async Task<ServiceResponse> MarkDeleteCommunicate(int id, string userId)
    {
        if (id <= 0)
        {
            return new ServiceResponse("Invalid communicate id");
        }
        await using var context = await _contextFactory.CreateDbContextAsync();
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            var communicate = await context.Communicates.Include(c => c.CommunicateAssets).Include(c => c.CommunicateAreas).Include(c => c.CommunicateCoordinates).Include(c => c.CommunicateDevices).Include(c => c.CommunicateModels).Include(c => c.CommunicateSpaces).Include(c => c.CommunicateCategories).FirstOrDefaultAsync(c => c.CommunicateId == id);
            if (communicate == null)
            {
                _logger.LogError("Communicate with id {id} not found", id);
                return new ServiceResponse($"Communicate with id {id} not found");
            }
            foreach (var communicateAsset in communicate.CommunicateAssets)
            {
                communicateAsset.IsDeleted = true;
                communicateAsset.UserId = userId;
            }
            context.CommunicateAssets.UpdateRange(communicate.CommunicateAssets);

            foreach (var communicateArea in communicate.CommunicateAreas)
            {
                communicateArea.IsDeleted = true;
                communicateArea.UserId = userId;
            }
            context.CommunicateAreas.UpdateRange(communicate.CommunicateAreas);
            foreach (var communicateCoordinate in communicate.CommunicateCoordinates)
            {
                communicateCoordinate.IsDeleted = true;
                communicateCoordinate.UserId = userId;
            }
            context.CommunicateCoordinates.UpdateRange(communicate.CommunicateCoordinates);
            foreach (var communicateDevice in communicate.CommunicateDevices)
            {
                communicateDevice.IsDeleted = true;
                communicateDevice.UserId = userId;
            }
            context.CommunicateDevices.UpdateRange(communicate.CommunicateDevices);
            foreach (var communicateModel in communicate.CommunicateModels)
            {
                communicateModel.IsDeleted = true;
                communicateModel.UserId = userId;
            }
            context.CommunicateModels.UpdateRange(communicate.CommunicateModels);
            foreach (var communicateSpace in communicate.CommunicateSpaces)
            {
                communicateSpace.IsDeleted = true;
                communicateSpace.UserId = userId;
            }
            context.CommunicateSpaces.UpdateRange(communicate.CommunicateSpaces);

            foreach (var communicateCategory in communicate.CommunicateCategories)
            {
                communicateCategory.IsDeleted = true;
                communicateCategory.UserId = userId;
            }
            context.CommunicateCategories.UpdateRange(communicate.CommunicateCategories);

            communicate.IsDeleted = true;
            communicate.UserId = userId;
            context.Communicates.Update(communicate);
            await context.SaveChangesAsync();
            await transaction.CommitAsync();
            _logger.LogInformation("Communicate with id {CommunicateId} marked as deleted", id);
            return new ServiceResponse($"Communicate {id} marked as deleted", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking communicate with id {id} as deleted", id);
            await transaction.RollbackAsync();
            return new ServiceResponse($"Error marking communicate with id {id} as deleted");
        }
    }

    /// <summary>
    /// Returns service response with true if coordinate by id is marked as deleted
    /// Marks all relations as deleted
    /// </summary>
    /// <param name="id"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    public async Task<ServiceResponse> MarkDeleteCoordinate(int id, string userId)
    {
        if (id <= 0)
        {
            return new ServiceResponse("Invalid coordinate id");
        }
        await using var context = await _contextFactory.CreateDbContextAsync();
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            var coordinate = await context.Coordinates.Include(a => a.Assets).Include(c => c.CommunicateCoordinates)
                .FirstOrDefaultAsync(c => c.CoordinateId == id);
            if (coordinate == null)
            {
                return new ServiceResponse("Coordinate not found");
            }

            if (coordinate.Assets.Any(a => a.IsDeleted == false))
            {
                return new ServiceResponse("Cannot delete coordinate with assets");
            }

            if (coordinate.IsDeleted)
            {
                return new ServiceResponse("Coordinate already marked as deleted");
            }

            foreach (var communicateCoordinate in coordinate.CommunicateCoordinates)
            {
                communicateCoordinate.IsDeleted = true;
                communicateCoordinate.UserId = userId;
            }
            context.CommunicateCoordinates.UpdateRange(coordinate.CommunicateCoordinates);

            coordinate.IsDeleted = true;
            coordinate.UserId = userId;
            context.Coordinates.Update(coordinate);
            await context.SaveChangesAsync();
            await transaction.CommitAsync();
            return new ServiceResponse("Coordinate deleted", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting coordinate");
            await transaction.RollbackAsync();
            return new ServiceResponse("Error deleting coordinate");
        }
    }

    /// <summary>
    /// Returns service response with true if detail by id is marked as deleted
    /// Marks all relations as deleted
    /// </summary>
    /// <param name="id"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    public async Task<ServiceResponse> MarkDeleteDetail(int id, string userId)
    {
        if (id <= 0)
        {
            return new ServiceResponse("Invalid detail id");
        }
        await using var context = await _contextFactory.CreateDbContextAsync();
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            var detail = await context.Details.Include(d => d.AssetDetails).Include(d => d.SituationDetails).FirstOrDefaultAsync(d => d.DetailId == id);
            if (detail == null)
            {
                _logger.LogWarning("Detail not found");
                return new ServiceResponse("Detail not found");
            }

            detail.IsDeleted = true;
            detail.UserId = userId;
            context.Details.Update(detail);
            await context.SaveChangesAsync();
            await transaction.CommitAsync();
            _logger.LogInformation("Detail with id {DetailId} deleted", detail.DetailId);
            return new ServiceResponse($"Detail {detail.DetailId} deleted", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting detail");
            await transaction.RollbackAsync();
            return new ServiceResponse($"Error deleting detail");
        }
    }

    /// <summary>
    /// Returns service response with true if device by id is marked as deleted
    /// Marks all relations as deleted
    /// </summary>
    /// <param name="id"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    public async Task<ServiceResponse> MarkDeleteDevice(int id, string userId)
    {
        if (id <= 0)
        {
            return new ServiceResponse("Invalid device id");
        }
        await using var context = await _contextFactory.CreateDbContextAsync();
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            var device = await context.Devices.Include(d => d.CommunicateDevices).Include(d => d.DeviceSituations).Include(d => d.Models).FirstOrDefaultAsync(d => d.DeviceId == id);
            if (device == null)
            {
                _logger.LogWarning("Device not found");
                return new ServiceResponse("Device not found");
            }

            if (device.IsDeleted)
            {
                _logger.LogWarning("Device already marked as deleted");
                return new ServiceResponse("Device already marked as deleted");
            }
            if (device.Models.Any(m => m.IsDeleted == false))
            {
                _logger.LogWarning("Device has models");
                return new ServiceResponse("Device has models");
            }
            foreach (var communicateDevice in device.CommunicateDevices)
            {
                communicateDevice.IsDeleted = true;
                communicateDevice.UserId = userId;
            }
            context.CommunicateDevices.UpdateRange(device.CommunicateDevices);
            foreach (var deviceSituation in device.DeviceSituations)
            {
                deviceSituation.IsDeleted = true;
                deviceSituation.UserId = userId;
            }
            context.DeviceSituations.UpdateRange(device.DeviceSituations);

            device.UserId = userId;
            device.IsDeleted = true;
            context.Devices.Update(device);
            await context.SaveChangesAsync();
            await transaction.CommitAsync();
            _logger.LogInformation("Device with id {DeviceId} marked as deleted", id);
            return new ServiceResponse($"Device {id} marked as deleted", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking device as deleted with id {DeviceId}", id);
            await transaction.RollbackAsync();
            return new ServiceResponse($"Error marking device as deleted with id {id}");
        }
    }

    /// <summary>
    /// Returns service response with true if model by id is marked as deleted
    /// Marks all relations as deleted
    /// </summary>
    /// <param name="id"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    public async Task<ServiceResponse> MarkDeleteModel(int id, string userId)
    {
        if (id <= 0)
        {
            return new ServiceResponse("Invalid model id");
        }
        await using var context = await _contextFactory.CreateDbContextAsync();
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            var model = await context.Models.Include(m => m.Assets).Include(m => m.ModelParameters).Include(m => m.CommunicateModels).FirstOrDefaultAsync(m => m.ModelId == id);
            if (model == null)
            {
                _logger.LogWarning("Model not found");
                return new ServiceResponse("Model not found");
            }

            if (model.IsDeleted)
            {
                _logger.LogWarning("Model already marked as deleted");
                return new ServiceResponse("Model already marked as deleted");
            }

            if (model.Assets.Any(m => m.IsDeleted == false))
            {
                _logger.LogWarning("Model has assets");
                return new ServiceResponse("Model has assets");
            }

            model.IsDeleted = true;
            model.UserId = userId;
            context.Models.Update(model);
            await context.SaveChangesAsync();
            await transaction.CommitAsync();
            _logger.LogInformation("Model with id {ModelId} marked as deleted", model.ModelId);
            return new ServiceResponse($"Model {model.ModelId} marked as deleted", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking model with id {ModelId} as deleted", id);
            await transaction.RollbackAsync();
            return new ServiceResponse($"Error marking model with id {id} as deleted");
        }
    }

    /// <summary>
    /// Returns service response with true if parameter by id is marked as deleted
    /// </summary>
    /// <param name="id"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    public async Task<ServiceResponse> MarkDeleteParameter(int id, string userId)
    {
        if (id <= 0)
        {
            return new ServiceResponse("Invalid parameter id");
        }
        await using var context = await _contextFactory.CreateDbContextAsync();
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            var parameter = await context.Parameters.Include(p => p.ModelParameters).Include(p => p.SituationParameters).FirstOrDefaultAsync(p => p.ParameterId == id);
            if (parameter == null)
            {
                _logger.LogWarning("Parameter not found");
                return new ServiceResponse("Parameter not found");
            }

            if (parameter.IsDeleted)
            {
                _logger.LogWarning("Parameter with id {ParameterId} already marked as deleted", parameter.ParameterId);
                return new ServiceResponse($"Parameter with id {parameter.ParameterId} already deleted");
            }
            foreach (var modelParameter in parameter.ModelParameters)
            {
                modelParameter.IsDeleted = true;
                modelParameter.UserId = userId;
            }
            context.ModelParameters.UpdateRange(parameter.ModelParameters);
            foreach (var situationParameter in parameter.SituationParameters)
            {
                situationParameter.IsDeleted = true;
                situationParameter.UserId = userId;
            }
            context.SituationParameters.UpdateRange(parameter.SituationParameters);
            parameter.IsDeleted = true;
            parameter.UserId = userId;
            context.Parameters.Update(parameter);
            await context.SaveChangesAsync();
            await transaction.CommitAsync();
            _logger.LogInformation("Parameter with id {ParameterId} marked as deleted", parameter.ParameterId);
            return new ServiceResponse($"Parameter {parameter.ParameterId} marked as deleted", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking parameter with id {ParameterId} as deleted", id);
            await transaction.RollbackAsync();
            return new ServiceResponse($"Error marking parameter with id {id} as deleted");
        }
    }

    /// <summary>
    /// Returns service response with true if plant by id is marked as deleted
    /// </summary>
    /// <param name="id"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    public async Task<ServiceResponse> MarkDeletePlant(int id, string userId)
    {
        if (id <= 0)
        {
            return new ServiceResponse("Invalid plant id");
        }
        await using var context = await _contextFactory.CreateDbContextAsync();
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            var plant = await context.Plants.Include(a => a.Areas).FirstOrDefaultAsync(p => p.PlantId == id);
            if (plant == null)
            {
                return new ServiceResponse("Plant not found");
            }
            if (plant.IsDeleted)
            {
                _logger.LogWarning("Plant with id {PlantId} already marked as deleted", plant.PlantId);
                return new ServiceResponse("Plant already marked as deleted");
            }

            if (plant.Areas.Any(p => p.IsDeleted == false))
            {
                return new ServiceResponse("Plant has areas");
            }

            plant.IsDeleted = true;
            plant.UserId = userId;
            context.Plants.Update(plant);
            await context.SaveChangesAsync();
            await transaction.CommitAsync();
            return new ServiceResponse("Plant marked as deleted", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking plant as deleted");
            await transaction.RollbackAsync();
            return new ServiceResponse("Error marking plant as deleted");
        }
    }

    /// <summary>
    /// Returns service response with true if question by id is marked as deleted
    /// Marks all relations as deleted
    /// </summary>
    /// <param name="id"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    public async Task<ServiceResponse> MarkDeleteQuestion(int id, string userId)
    {
        if (id <= 0)
        {
            return new ServiceResponse("Invalid question id");
        }
        await using var context = await _contextFactory.CreateDbContextAsync();
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            var question = await context.Questions.Include(q => q.SituationQuestions).FirstOrDefaultAsync(q => q.QuestionId == id);
            if (question == null)
            {
                _logger.LogInformation("Question not found");
                return new ServiceResponse($"Question not found");
            }
            if (question.IsDeleted)
            {
                _logger.LogWarning("Question with id {QuestionId} already marked as deleted", question.QuestionId);
                return new ServiceResponse($"Question with id {question.QuestionId} already deleted");
            }
            foreach (var situationQuestion in question.SituationQuestions)
            {
                situationQuestion.IsDeleted = true;
                situationQuestion.UserId = userId;
            }
            context.SituationQuestions.UpdateRange(question.SituationQuestions);

            question.IsDeleted = true;
            question.UserId = userId;
            context.Questions.Update(question);
            await context.SaveChangesAsync();
            await transaction.CommitAsync();
            _logger.LogInformation("Question with id {QuestionId} marked as deleted", question.QuestionId);
            return new ServiceResponse($"Question {question.QuestionId} marked as deleted", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting question");
            await transaction.RollbackAsync();
            return new ServiceResponse($"Error deleting question");
        }
    }

    /// <summary>
    /// Returns service response with true if situation by id is marked as deleted
    /// Marks all relations as deleted
    /// </summary>
    /// <param name="id"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    public async Task<ServiceResponse> MarkDeleteSituation(int id, string userId)
    {
        if (id <= 0)
        {
            return new ServiceResponse("Invalid situation id");
        }
        await using var context = await _contextFactory.CreateDbContextAsync();
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            var situation = await context.Situations.Include(s => s.SituationQuestions).Include(s => s.SituationDetails).Include(s => s.SituationParameters).Include(s => s.CategorySituations).Include(s => s.DeviceSituations).Include(s => s.AssetSituations).FirstOrDefaultAsync(s => s.SituationId == id);
            if (situation == null)
            {
                _logger.LogWarning("SituationId {id} not found", id);
                return new ServiceResponse($"SituationId {id} not found");
            }
            if (situation.IsDeleted)
            {
                _logger.LogWarning("SituationId {id} already marked as deleted", id);
                return new ServiceResponse($"SituationId {id} already deleted");
            }
            foreach (var situationQuestion in situation.SituationQuestions)
            {
                situationQuestion.IsDeleted = true;
                situationQuestion.UserId = userId;
            }
            context.SituationQuestions.UpdateRange(situation.SituationQuestions);
            foreach (var situationDetail in situation.SituationDetails)
            {
                situationDetail.IsDeleted = true;
                situationDetail.UserId = userId;
            }
            context.SituationDetails.UpdateRange(situation.SituationDetails);
            foreach (var situationParameter in situation.SituationParameters)
            {
                situationParameter.IsDeleted = true;
                situationParameter.UserId = userId;
            }
            context.SituationParameters.UpdateRange(situation.SituationParameters);
            foreach (var categorySituation in situation.CategorySituations)
            {
                categorySituation.IsDeleted = true;
                categorySituation.UserId = userId;
            }
            context.CategorySituations.UpdateRange(situation.CategorySituations);
            foreach (var deviceSituation in situation.DeviceSituations)
            {
                deviceSituation.IsDeleted = true;
                deviceSituation.UserId = userId;
            }
            context.DeviceSituations.UpdateRange(situation.DeviceSituations);
            foreach (var assetSituation in situation.AssetSituations)
            {
                assetSituation.IsDeleted = true;
                assetSituation.UserId = userId;
            }
            context.AssetSituations.UpdateRange(situation.AssetSituations);

            situation.IsDeleted = true;
            situation.UserId = userId;

            await context.SaveChangesAsync();
            await transaction.CommitAsync();
            _logger.LogInformation("SituationId {id} marked as deleted", id);
            return new ServiceResponse($"Situation {id} marked as deleted", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting situation");
            await transaction.RollbackAsync();
            return new ServiceResponse($"Error deleting situation");
        }
    }

    /// <summary>
    /// Returns service response with true if space by id is marked as deleted
    /// Marks all relations as deleted
    /// </summary>
    /// <param name="id"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    public async Task<ServiceResponse> MarkDeleteSpace(int id, string userId)
    {
        if (id <= 0)
        {
            return new ServiceResponse("Invalid space id");
        }
        await using var context = await _contextFactory.CreateDbContextAsync();
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            var space = await context.Spaces.Include(c => c.Coordinates).Include(s => s.CommunicateSpaces).FirstOrDefaultAsync(s => s.SpaceId == id);
            if (space == null)
            {
                return new ServiceResponse("Space not found");
            }
            if (space.IsDeleted)
            {
                _logger.LogWarning("Space with id {SpaceId} already marked as deleted", space.SpaceId);
                return new ServiceResponse("Space already marked as deleted");
            }
            if (space.Coordinates.Any(s => s.IsDeleted == false))
            {
                return new ServiceResponse("Cannot delete space with coordinates");
            }
            foreach (var communicateSpace in space.CommunicateSpaces)
            {
                communicateSpace.IsDeleted = true;
                communicateSpace.UserId = userId;
            }
            context.CommunicateSpaces.UpdateRange(space.CommunicateSpaces);
            space.IsDeleted = true;
            space.UserId = userId;
            context.Spaces.Update(space);
            await context.SaveChangesAsync();
            await transaction.CommitAsync();
            return new ServiceResponse("Space marked as deleted", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking space as deleted");
            await transaction.RollbackAsync();
            return new ServiceResponse("Error marking space as deleted");
        }
    }

    /// <summary>
    /// Returns service response with true if area by id is not marked as deleted
    /// Marks all relations as not deleted if it is possible
    /// </summary>
    /// <param name="id"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    public async Task<ServiceResponse> MarkUnDeleteArea(int id, string userId)
    {
        //1 id<=0
        if (id <= 0)
        {
            return new ServiceResponse("Invalid area id");
        }
        //2 await DbContext
        await using var context = await _contextFactory.CreateDbContextAsync();
        //3 await transaction
        await using var transaction = await context.Database.BeginTransactionAsync();
        //4 try-catch
        try
        {
            //5 get entity by id including children and relations with parents
            var area = await context.Areas.Include(a => a.Spaces).Include(a => a.CommunicateAreas).ThenInclude(c => c.Communicate).FirstOrDefaultAsync(a => a.AreaId == id);
            //6 check if entity is null
            if (area == null)
            {
                return new ServiceResponse("Area not found");
            }
            //7 check if entity is marked as deleted
            if (area.IsDeleted == false)
            {
                _logger.LogWarning("AreaId {Id} already marked as not deleted", id);
                return new ServiceResponse($"Area with id {id} already marked as not deleted");
            }
            //8 get parent with entities
            var plant = await context.Plants.Include(p => p.Areas).FirstOrDefaultAsync(p => p.PlantId == area.PlantId);
            //9 check if parent is null
            if (plant == null)
            {
                return new ServiceResponse("Plant not found");
            }
            //10 check if parent is marked as deleted
            if (plant.IsDeleted)
            {
                _logger.LogWarning("PlantId {Id} also marked as deleted", id);
                return new ServiceResponse($"Plant with id {id} also marked as deleted");
            }
            //11 check if parent has not marked as deleted entities with the same name as the entity
            if (plant.Areas.Any(a => Equals(a.Name.ToLower().Trim(), area.Name.ToLower().Trim()) && a.AreaId != area.AreaId && a.IsDeleted == false))
            {
                _logger.LogWarning("Area with name {Name} already exists", area.Name);
                return new ServiceResponse("Area with the same name already exists");
            }
            //12 undelete entity
            area.IsDeleted = false;
            area.UserId = userId;
            //13 update entity
            context.Areas.Update(area);
            //14 check if related parents are not marked as deleted and exist
            //15 undelete relations
            foreach (var communicateArea in area.CommunicateAreas.Where(a => a.Communicate.IsDeleted == false))
            {
                communicateArea.IsDeleted = false;
                communicateArea.UserId = userId;
            }
            //16 update relations
            context.CommunicateAreas.UpdateRange(area.CommunicateAreas);

            //17 save changes, await transaction commit, log, return response
            await context.SaveChangesAsync();
            await transaction.CommitAsync();
            _logger.LogInformation("AreaId {Id} marked as not deleted", id);
            return new ServiceResponse($"Area {id} marked as not deleted", true);
        }
        catch
        {
            //18 await transaction rollback, log, return response
            await transaction.RollbackAsync();
            _logger.LogError("Error marking area as not deleted");
            return new ServiceResponse("Error marking area as not deleted");
        }
    }

    /// <summary>
    /// Returns service response with true if asset by id is not marked as deleted
    /// Marks all relations as not deleted if it is possible
    /// </summary>
    /// <param name="id"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    public async Task<ServiceResponse> MarkUnDeleteAsset(int id, string userId)
    {
        //1 	id<=0
        if (id <= 0)
        {
            return new ServiceResponse("Invalid asset id");
        }
        //2 	await DbContext
        await using var context = await _contextFactory.CreateDbContextAsync();
        //3 	await transaction
        await using var transaction = await context.Database.BeginTransactionAsync();
        //4 	try-catch
        try
        {
            //5	get entity by id including children and relations with parents
            var asset = await context.Assets.Include(a => a.AssetDetails).ThenInclude(d => d.Detail).Include(a => a.CommunicateAssets).ThenInclude(c => c.Communicate).Include(a => a.AssetCategories).ThenInclude(c => c.Category).Include(a => a.AssetSituations).ThenInclude(s => s.Situation).FirstOrDefaultAsync(a => a.AssetId == id);
            //6	check if entity is null
            if (asset == null)
            {
                return new ServiceResponse("Asset not found");
            }
            //7	check if entity is marked as deleted
            if (asset.IsDeleted == false)
            {
                _logger.LogWarning("AssetId {Id} already marked as not deleted", id);
                return new ServiceResponse($"Asset with id {id} already marked as not deleted");
            }
            //8 	get parent with entities
            var model = await context.Models.Include(m => m.Assets).FirstOrDefaultAsync(m => m.ModelId == asset.ModelId);
            var coordinate = await context.Coordinates.Include(c => c.Assets).FirstOrDefaultAsync(c => c.CoordinateId == asset.CoordinateId);
            //9 	check if parent is not null
            if (model == null)
            {
                return new ServiceResponse("Model not found");
            }
            if (coordinate == null)
            {
                return new ServiceResponse("Coordinate not found");
            }
            //10 	check if parent is marked as deleted
            if (model.IsDeleted)
            {
                _logger.LogWarning("ModelId {Id} also marked as deleted", id);
                return new ServiceResponse($"Model with id {id} also marked as deleted");
            }
            if (coordinate.IsDeleted)
            {
                _logger.LogWarning("CoordinateId {Id} also marked as deleted", id);
                return new ServiceResponse($"Coordinate with id {id} also marked as deleted");
            }
            //11	check if parent has not marked as deleted entities with the same name as the entity
            if (model.Assets.Any(a => Equals(a.Name.ToLower().Trim(), asset.Name.ToLower().Trim()) && a.AssetId != asset.AssetId && a.IsDeleted == false))
            {
                _logger.LogWarning("Asset with name {Name} already exists", asset.Name);
                return new ServiceResponse("Asset with the same name already exists");
            }
            //12	undelete entity
            asset.IsDeleted = false;
            asset.UserId = userId;
            //13	update entity
            context.Assets.Update(asset);
            //14	check if related parents are not marked as deleted and exist

            //15	undelete relations
            foreach (var assetDetail in asset.AssetDetails.Where(a => a.Detail.IsDeleted == false))
            {
                assetDetail.IsDeleted = false;
                assetDetail.UserId = userId;
            }

            //16	update relations
            context.AssetDetails.UpdateRange(asset.AssetDetails);

            foreach (var communicateAsset in asset.CommunicateAssets.Where(a => a.Communicate.IsDeleted == false))
            {
                communicateAsset.IsDeleted = false;
                communicateAsset.UserId = userId;
            }
            //16	update relations
            context.CommunicateAssets.UpdateRange(asset.CommunicateAssets);

            foreach (var assetCategory in asset.AssetCategories.Where(a => a.Category.IsDeleted == false))
            {
                assetCategory.IsDeleted = false;
                assetCategory.UserId = userId;
            }
            //16	update relations
            context.AssetCategories.UpdateRange(asset.AssetCategories);

            foreach (var assetSituation in asset.AssetSituations.Where(a => a.Situation.IsDeleted == false))
            {
                assetSituation.IsDeleted = false;
                assetSituation.UserId = userId;
            }
            //16	update relations
            context.AssetSituations.UpdateRange(asset.AssetSituations);

            //17	save changes, await transaction commit, log, return response
            await context.SaveChangesAsync();
            await transaction.CommitAsync();
            _logger.LogInformation("AssetId {Id} marked as not deleted", id);
            return new ServiceResponse($"Asset {id} marked as not deleted", true);
        }
        catch (Exception ex)
        {
            //18	catch await transaction rollback, log, return response
            _logger.LogError(ex, "Error marking asset as not deleted");
            await transaction.RollbackAsync();
            return new ServiceResponse("Error marking asset as not deleted");
        }
    }

    /// <summary>
    /// Returns service response with true if category by id is not marked as deleted
    /// Marks all relations as not deleted if it is possible
    /// </summary>
    /// <param name="id"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    public async Task<ServiceResponse> MarkUnDeleteCategory(int id, string userId)
    {
        //1 	id<=0
        if (id <= 0)
        {
            return new ServiceResponse("Invalid category id");
        }
        //2 	await DbContext
        await using var context = await _contextFactory.CreateDbContextAsync();
        //3 	await transaction
        await using var transaction = await context.Database.BeginTransactionAsync();
        //4 	try-catch
        try
        {
            //5	get entity by id including children and relations with parents
            var category = await context.Categories.Include(c => c.AssetCategories).ThenInclude(a => a.Asset).Include(c => c.CommunicateCategories).ThenInclude(c => c.Communicate).Include(c => c.CommunicateCategories).ThenInclude(c => c.Communicate).FirstOrDefaultAsync(c => c.CategoryId == id);
            //6	check if entity is null
            if (category == null)
            {
                return new ServiceResponse("Category not found");
            }
            //7	check if entity is marked as deleted
            if (category.IsDeleted == false)
            {
                _logger.LogWarning("CategoryId {Id} not marked as deleted", id);
                return new ServiceResponse($"Category with id {id} not marked as deleted");
            }
            //8 	get parent with entities or entities group
            var categories = await context.Categories.ToListAsync();
            //9 	check if parent is not null
            if (categories == null)
            {
                return new ServiceResponse("Categories not found");
            }
            //10 	check if parent is marked as deleted
            //11	check if parent has not marked as deleted entities with the same name as the entity
            var exist = categories.Any(c => Equals(c.Name.ToLower().Trim(), category.Name.ToLower().Trim()) && c.CategoryId != category.CategoryId && c.IsDeleted == false);
            if (exist)
            {
                _logger.LogWarning("Category with name {Name} already exists", category.Name);
                return new ServiceResponse("Category with the same name already exists");
            }
            //12	undelete entity
            category.IsDeleted = false;
            category.UserId = userId;
            //13	update entity
            context.Categories.Update(category);
            //14	check if related parents are not marked as deleted and exist

            //15-1	undelete relations
            foreach (var assetCategory in category.AssetCategories.Where(c => c.Asset.IsDeleted == false))
            {
                assetCategory.IsDeleted = false;
                assetCategory.UserId = userId;
            }

            //16-1	update relations
            context.AssetCategories.UpdateRange(category.AssetCategories);

            //15-2	undelete relations
            foreach (var communicateCategory in category.CommunicateCategories.Where(a => a.Communicate.IsDeleted == false))
            {
                communicateCategory.IsDeleted = false;
                communicateCategory.UserId = userId;
            }

            //16-2	update relations
            context.CommunicateCategories.UpdateRange(category.CommunicateCategories);
            //17	save changes, await transaction commit, log, return response
            await context.SaveChangesAsync();
            await transaction.CommitAsync();
            _logger.LogInformation("CategoryId {Id} marked as not deleted", id);
            return new ServiceResponse($"Category {id} marked as not deleted", true);
        }
        catch (Exception ex)
        {
            //18	catch await transaction rollback, log, return response
            _logger.LogError(ex, "Error marking category as not deleted");
            await transaction.RollbackAsync();
            return new ServiceResponse("Error marking category as not deleted");
        }
    }

    /// <summary>
    /// Returns service response with true if communicate by id is not marked as deleted
    /// Marks all relations as not deleted if it is possible
    /// </summary>
    /// <param name="id"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    public async Task<ServiceResponse> MarkUnDeleteCommunicate(int id, string userId)
    {
        //1 	id<=0
        if (id <= 0)
        {
            return new ServiceResponse("Invalid communicate id");
        }
        //2 	await DbContext
        await using var context = await _contextFactory.CreateDbContextAsync();
        //3 	await transaction
        await using var transaction = await context.Database.BeginTransactionAsync();
        //4 	try-catch
        try
        {
            //5	get entity by id including children and relations with parents
            var communicate = await context.Communicates.Include(c => c.CommunicateAreas).ThenInclude(a => a.Area).Include(c => c.CommunicateAssets).ThenInclude(c => c.Asset).Include(c => c.CommunicateCoordinates).ThenInclude(c => c.Coordinate).Include(c => c.CommunicateDevices).ThenInclude(c => c.Device).Include(c => c.CommunicateModels).ThenInclude(c => c.Model).Include(c => c.CommunicateSpaces).ThenInclude(c => c.Space).Include(c => c.CommunicateCategories).ThenInclude(c => c.Category).FirstOrDefaultAsync(c => c.CommunicateId == id);
            //6	check if entity is null
            if (communicate == null)
            {
                return new ServiceResponse("Communicate not found");
            }
            //7	check if entity is marked as deleted
            if (communicate.IsDeleted == false)
            {
                _logger.LogWarning("CommunicateId {Id} not marked as deleted", id);
                return new ServiceResponse($"Communicate with id {id} not marked as deleted");
            }
            //8 	get parent with entities or entities group
            var communicates = await context.Communicates.ToListAsync();
            //9 	check if parent is not null
            //10 	check if parent is marked as deleted
            //11	check if parent or entities group has not marked as deleted entities with the same name as the entity
            var exist = communicates.Any(c => Equals(c.Name.ToLower().Trim(), communicate.Name.ToLower().Trim()) && c.CommunicateId != communicate.CommunicateId && c.IsDeleted == false);
            //12	undelete entity
            communicate.IsDeleted = false;
            communicate.UserId = userId;
            //13	update entity
            context.Communicates.Update(communicate);
            //14	check if related parents are not marked as deleted and exist
            //15-1	undelete relations
            foreach (var communicateArea in communicate.CommunicateAreas.Where(c => c.Area.IsDeleted == false))
            {
                communicateArea.IsDeleted = false;
                communicateArea.UserId = userId;
            }
            //16-1	update relations
            context.CommunicateAreas.UpdateRange(communicate.CommunicateAreas);

            //15-2	undelete relations
            foreach (var communicateAsset in communicate.CommunicateAssets.Where(c => c.Asset.IsDeleted == false))
            {
                communicateAsset.IsDeleted = false;
                communicateAsset.UserId = userId;
            }
            //16-2	update relations
            context.CommunicateAssets.UpdateRange(communicate.CommunicateAssets);

            //15-3	undelete relations
            foreach (var communicateCoordinate in communicate.CommunicateCoordinates.Where(c => c.Coordinate.IsDeleted == false))
            {
                communicateCoordinate.IsDeleted = false;
                communicateCoordinate.UserId = userId;
            }
            //16-3	update relations
            context.CommunicateCoordinates.UpdateRange(communicate.CommunicateCoordinates.Where(c => c.Coordinate.IsDeleted == false));

            //15-4	undelete relations
            foreach (var communicateDevice in communicate.CommunicateDevices)
            {
                communicateDevice.IsDeleted = false;
                communicateDevice.UserId = userId;
            }
            //16-4	update relations
            context.CommunicateDevices.UpdateRange(communicate.CommunicateDevices);

            //15-5	undelete relations
            foreach (var communicateModel in communicate.CommunicateModels.Where(c => c.Model.IsDeleted == false))
            {
                communicateModel.IsDeleted = false;
                communicateModel.UserId = userId;
            }

            //16-5	update relations
            context.CommunicateModels.UpdateRange(communicate.CommunicateModels);

            //15-6	undelete relations
            foreach (var communicateSpace in communicate.CommunicateSpaces.Where(c => c.Space.IsDeleted == false))
            {
                communicateSpace.IsDeleted = false;
                communicateSpace.UserId = userId;
            }

            //16-6	update relations
            context.CommunicateSpaces.UpdateRange(communicate.CommunicateSpaces);

            foreach (var communicateCategory in communicate.CommunicateCategories.Where(c => c.Category.IsDeleted == false))
            {
                communicateCategory.IsDeleted = false;
                communicateCategory.UserId = userId;
            }
            //16-7	update relations
            context.CommunicateCategories.UpdateRange(communicate.CommunicateCategories);
            //17	save changes, await transaction commit, log, return response
            await context.SaveChangesAsync();
            await transaction.CommitAsync();
            _logger.LogInformation("CommunicateId {Id} marked as not deleted", id);
            return new ServiceResponse($"Communicate with id {id} marked as not deleted", true);
        }
        catch (Exception ex)
        {
            //18	catch await transaction rollback, log, return response
            await transaction.RollbackAsync();
            _logger.LogError(ex, "CommunicateId {Id} not marked as not deleted", id);
            return new ServiceResponse($"Communicate with id {id} not marked as not deleted");
        }
    }

    /// <summary>
    /// Returns service response with true if coordinate by id is not marked as deleted
    /// Marks all relations as not deleted if it is possible
    /// </summary>
    /// <param name="id"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    public async Task<ServiceResponse> MarkUnDeleteCoordinate(int id, string userId)
    {
        //1 	id<=0
        if (id <= 0)
        {
            _logger.LogWarning("Invalid coordinate id {Id}", id);
            return new ServiceResponse($"Invalid coordinate id {id}");
        }
        //2 	await DbContext
        await using var context = await _contextFactory.CreateDbContextAsync();
        //3 	await transaction
        await using var transaction = await context.Database.BeginTransactionAsync();
        //4 	try-catch
        try
        {
            //5	get entity by id including children and relations with parents
            var coordinate = await context.Coordinates.Include(c => c.Assets).Include(c => c.CommunicateCoordinates).ThenInclude(c => c.Communicate).FirstOrDefaultAsync(c => c.CoordinateId == id);
            //6	check if entity is null
            if (coordinate == null)
            {
                _logger.LogWarning("CoordinateId {Id} not found", id);
                return new ServiceResponse($"Coordinate with id {id} not found");
            }
            //7	check if entity is marked as deleted
            if (coordinate.IsDeleted == false)
            {
                _logger.LogWarning("CoordinateId {Id} not marked as deleted", id);
                return new ServiceResponse($"Coordinate with id {id} not marked as deleted");
            }
            //8 	get parent with entities or entities group
            var space = await context.Spaces.Include(c => c.Coordinates).FirstOrDefaultAsync(s => s.SpaceId == coordinate.SpaceId);
            //9 	check if parent is not null
            if (space == null)
            {
                _logger.LogWarning("Space with id {Id} not found", coordinate.SpaceId);
                return new ServiceResponse($"Space with id {coordinate.SpaceId} not found");
            }
            //10 	check if parent is marked as deleted
            if (space.IsDeleted == true)
            {
                _logger.LogWarning("Space with id {Id} marked as deleted", coordinate.SpaceId);
                return new ServiceResponse($"Space with id {coordinate.SpaceId} also marked as deleted");
            }
            //11	check if parent or entities group has not marked as deleted entities with the same name as the entity
            if (space.Coordinates.Any(c => c.CoordinateId != id && c.IsDeleted == false && Equals(c.Name.ToLower().Trim() == coordinate.Name.ToLower().Trim())))
            {
                _logger.LogWarning("Space with id {Id} has not marked as deleted coordinate with the same name", coordinate.SpaceId);
                return new ServiceResponse($"Space with id {coordinate.SpaceId} has not marked as deleted coordinate with the same name");
            }
            //12	undelete entity
            coordinate.IsDeleted = false;
            coordinate.UserId = userId;
            //13	update entity
            context.Coordinates.Update(coordinate);
            //14	check if related parents are not marked as deleted and exist
            foreach (var communicateCoordinate in coordinate.CommunicateCoordinates.Where(c => c.Communicate.IsDeleted == false))
            {
                //15	undelete relations
                communicateCoordinate.IsDeleted = false;
                communicateCoordinate.UserId = userId;
                //16	update relations
                context.CommunicateCoordinates.Update(communicateCoordinate);
            }

            //17	save changes, await transaction commit, log, return response
            await context.SaveChangesAsync();
            await transaction.CommitAsync();
            _logger.LogInformation("CoordinateId {Id} marked as not deleted", id);
            return new ServiceResponse($"Coordinate with id {id} marked as not deleted", true);
        }
        catch (Exception ex)
        {
            //18	catch await transaction rollback, log, return response
            await transaction.RollbackAsync();
            _logger.LogError(ex, "CoordinateId {Id} not marked as not deleted", id);
            return new ServiceResponse($"Coordinate with id {id} not marked as not deleted");
        }
    }

    /// <summary>
    /// Returns service response with true if detail by id is marked as deleted
    /// Marks all relations as not deleted if it is possible
    /// </summary>
    /// <param name="id"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    public async Task<ServiceResponse> MarkUnDeleteDetail(int id, string userId)
    {
        //1 	id<=0
        if (id <= 0)
        {
            _logger.LogWarning("Invalid detail id {Id}", id);
            return new ServiceResponse($"Invalid detail id {id}");
        }
        //2 	await DbContext
        await using var context = await _contextFactory.CreateDbContextAsync();
        //3 	await transaction
        await using var transaction = await context.Database.BeginTransactionAsync();
        //4 	try-catch
        try
        {
            //5	get entity by id including children and relations with parents
            var detail = await context.Details.Include(d => d.AssetDetails).ThenInclude(d => d.Asset).Include(d => d.SituationDetails).ThenInclude(s => s.Situation).FirstOrDefaultAsync(d => d.DetailId == id);
            //6	check if entity is null
            if (detail == null)
            {
                _logger.LogWarning("DetailId {Id} not found", id);
                return new ServiceResponse($"Detail with id {id} not found");
            }
            //7	check if entity is not marked as deleted
            if (detail.IsDeleted == false)
            {
                _logger.LogWarning("DetailId {Id} not marked as deleted", id);
                return new ServiceResponse($"Detail with id {id} not marked as deleted");
            }
            //8 check if duplicate name exists
            var exists = await context.Details.AnyAsync(d => Equals(d.Name.ToLower().Trim(), detail.Name.ToLower().Trim()) && d.DetailId != id && d.IsDeleted == false);
            if (exists)
            {
                _logger.LogWarning("Detail with name {Name} already exists", detail.Name);
                return new ServiceResponse($"Detail with name {detail.Name} already exists");
            }
            //9 undelete entity
            detail.IsDeleted = false;
            detail.UserId = userId;
            //10 update entity
            context.Details.Update(detail);
            //11 check if related parents are not marked as deleted and exist
            foreach (var assetDetail in detail.AssetDetails.Where(d => d.Asset.IsDeleted == false))
            {
                //12 undelete relations
                assetDetail.IsDeleted = false;
                assetDetail.UserId = userId;
                //13 update relations
                context.AssetDetails.Update(assetDetail);
            }
            foreach (var situationDetail in detail.SituationDetails.Where(d => d.Situation.IsDeleted == false))
            {
                //14 undelete relations
                situationDetail.IsDeleted = false;
                situationDetail.UserId = userId;
                //15 update relations
                context.SituationDetails.Update(situationDetail);
            }
            //14	save changes, await transaction commit, log, return response
            await context.SaveChangesAsync();
            await transaction.CommitAsync();
            _logger.LogInformation("DetailId {Id} marked as not deleted", id);
            return new ServiceResponse($"Detail with id {id} marked as not deleted", true);
        }
        catch
        {
            //15 catch await transaction rollback, log, return response
            _logger.LogError("Error marking detail with id {Id} as not deleted", id);
            await transaction.RollbackAsync();
            return new ServiceResponse($"Error marking detail with id {id} as not deleted");
        }
    }

    /// <summary>
    /// Returns service response with true if device by id is marked as deleted
    /// Marks all relations as not deleted if it is possible
    /// </summary>
    /// <param name="id"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    public async Task<ServiceResponse> MarkUnDeleteDevice(int id, string userId)
    {
        //1 id<=0
        if (id <= 0)
        {
            _logger.LogWarning("Invalid device id {Id}", id);
            return new ServiceResponse($"Invalid device id {id}");
        }
        //2 await DbContext
        await using var context = await _contextFactory.CreateDbContextAsync();
        //3 await transaction
        await using var transaction = await context.Database.BeginTransactionAsync();
        //4 try-catch
        try
        {
            //5 get entity by id including children and relations with parents
            var device = await context.Devices.Include(d => d.Models).Include(d => d.CommunicateDevices).ThenInclude(d => d.Communicate).Include(d => d.DeviceSituations).ThenInclude(d => d.Situation).FirstOrDefaultAsync(d => d.DeviceId == id);
            //6 check if entity is null
            if (device == null)
            {
                _logger.LogWarning("DeviceId {Id} not found", id);
                return new ServiceResponse($"Device with id {id} not found");
            }
            //7 check if entity is marked as deleted
            if (device.IsDeleted == false)
            {
                _logger.LogWarning("DeviceId {Id} not marked as deleted", id);
                return new ServiceResponse($"Device with id {id} not marked as deleted");
            }
            //8 check if duplicate name exists
            var exists = await context.Devices.AnyAsync(d => Equals(d.Name.ToLower().Trim(), device.Name.ToLower().Trim()) && d.DeviceId != id && d.IsDeleted == false);
            if (exists)
            {
                _logger.LogWarning("Device with name {Name} already exists", device.Name);
                return new ServiceResponse($"Device with name {device.Name} already exists");
            }
            //9 undelete entity
            device.IsDeleted = false;
            device.UserId = userId;
            //10 update entity
            context.Devices.Update(device);
            //11 check if related parents are not marked as deleted and exist
            foreach (var deviceSituation in device.DeviceSituations.Where(d => d.Situation.IsDeleted == false))
            {
                //12 undelete relations
                deviceSituation.IsDeleted = false;
                deviceSituation.UserId = userId;
                //13 update relations
                context.DeviceSituations.Update(deviceSituation);
            }
            foreach (var communicateDevice in device.CommunicateDevices.Where(d => d.Communicate.IsDeleted == false))
            {
                //14 undelete relations
                communicateDevice.IsDeleted = false;
                communicateDevice.UserId = userId;
                //15 update relations
                context.CommunicateDevices.Update(communicateDevice);
            }
            //14 save changes, await transaction commit, log, return response
            await context.SaveChangesAsync();
            await transaction.CommitAsync();
            _logger.LogInformation("DeviceId {Id} marked as not deleted", id);
            return new ServiceResponse($"Device with id {id} marked as not deleted", true);
        }
        catch (Exception ex)
        {
            //15 catch await transaction rollback, log, return response
            _logger.LogError(ex, "Error marking device with id {Id} as not deleted", id);
            await transaction.RollbackAsync();
            return new ServiceResponse($"Error marking device with id {id} as not deleted");
        }
    }

    /// <summary>
    /// Returns service response with true if model by id is marked as deleted
    /// Marks all relations as not deleted if it is possible
    /// </summary>
    /// <param name="id"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    public async Task<ServiceResponse> MarkUnDeleteModel(int id, string userId)
    {
        //1 id<=0
        if (id <= 0)
        {
            _logger.LogWarning("Invalid model id {Id}", id);
            return new ServiceResponse($"Invalid model id {id}");
        }
        //2 await DbContext
        await using var context = await _contextFactory.CreateDbContextAsync();
        //3 await transaction
        await using var transaction = await context.Database.BeginTransactionAsync();
        //4 try-catch
        try
        {
            //5 get entity by id including children and relations with parents
            var model = await context.Models.Include(d => d.Assets).Include(d => d.ModelParameters).ThenInclude(d => d.Parameter).Include(m => m.CommunicateModels).ThenInclude(m => m.Communicate).FirstOrDefaultAsync(d => d.ModelId == id);
            //6 check if entity is null
            if (model == null)
            {
                _logger.LogWarning("ModelId {Id} not found", id);
                return new ServiceResponse($"Model with id {id} not found");
            }
            //7 check if entity is marked as deleted
            if (model.IsDeleted == false)
            {
                _logger.LogWarning("ModelId {Id} not marked as deleted", id);
                return new ServiceResponse($"Model with id {id} not marked as deleted");
            }
            //8 check if duplicate name exists
            var exists = await context.Devices.Include(m => m.Models).AnyAsync(d => d.Models.Any(d => Equals(d.Name.ToLower().Trim(), model.Name.ToLower().Trim()) && d.ModelId != id && d.IsDeleted == false));
            if (exists)
            {
                _logger.LogWarning("Model with name {Name} already exists", model.Name);
                return new ServiceResponse($"Model with name {model.Name} already exists");
            }
            //9 undelete entity
            model.IsDeleted = false;
            model.UserId = userId;
            //10 update entity
            context.Models.Update(model);
            //11 check if related parents are not marked as deleted and exist
            foreach (var modelParameter in model.ModelParameters.Where(d => d.Parameter.IsDeleted == false))
            {
                //12 undelete relations
                modelParameter.IsDeleted = false;
                modelParameter.UserId = userId;
                //13 update relations
                context.ModelParameters.Update(modelParameter);
            }
            foreach (var communicateModel in model.CommunicateModels.Where(d => d.Communicate.IsDeleted == false))
            {
                //14 undelete relations
                communicateModel.IsDeleted = false;
                communicateModel.UserId = userId;
                //15 update relations
                context.CommunicateModels.Update(communicateModel);
            }
            //14 save changes, await transaction commit, log, return response
            await context.SaveChangesAsync();
            await transaction.CommitAsync();
            _logger.LogInformation("ModelId {Id} marked as not deleted", id);
            return new ServiceResponse($"Model with id {id} marked as not deleted", true);
        }
        catch (Exception ex)
        {
            //15 catch await transaction rollback, log, return response
            _logger.LogError(ex, "Error marking model with id {Id} as not deleted", id);
            await transaction.RollbackAsync();
            return new ServiceResponse($"Error marking model with id {id} as not deleted");
        }
    }

    /// <summary>
    /// Returns service response with true if parameter by id is marked as deleted
    /// Marks all relations as not deleted if it is possible
    /// </summary>
    /// <param name="id"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    public async Task<ServiceResponse> MarkUnDeleteParameter(int id, string userId)
    {
        //1 id<=0
        if (id <= 0)
        {
            _logger.LogWarning("Invalid parameter id {Id}", id);
            return new ServiceResponse($"Invalid parameter id {id}");
        }
        //2 await DbContext
        await using var context = await _contextFactory.CreateDbContextAsync();
        //3 await transaction
        await using var transaction = await context.Database.BeginTransactionAsync();
        //4 try-catch
        try
        {
            //5 get entity by id including children and relations with parents
            var parameter = await context.Parameters.Include(d => d.ModelParameters).ThenInclude(d => d.Model).Include(d => d.SituationParameters).ThenInclude(d => d.Situation).FirstOrDefaultAsync(d => d.ParameterId == id);
            //6 check if entity is null
            if (parameter == null)
            {
                _logger.LogWarning("ParameterId {Id} not found", id);
                return new ServiceResponse($"Parameter with id {id} not found");
            }
            //7 check if entity is marked as deleted
            if (parameter.IsDeleted == false)
            {
                _logger.LogWarning("ParameterId {Id} not marked as deleted", id);
                return new ServiceResponse($"Parameter with id {id} not marked as deleted");
            }
            //8 check if duplicate name exists
            var exists = await context.Parameters.AnyAsync(p => Equals(p.Name.ToLower().Trim(), parameter.Name.ToLower().Trim()) && p.ParameterId != id && p.IsDeleted == false);
            if (exists)
            {
                _logger.LogWarning("Parameter with name {Name} already exists", parameter.Name);
                return new ServiceResponse($"Parameter with name {parameter.Name} already exists");
            }
            //9 undelete entity
            parameter.IsDeleted = false;
            parameter.UserId = userId;
            //10 update entity
            context.Parameters.Update(parameter);
            //11 check if related parents are not marked as deleted and exist
            foreach (var modelParameter in parameter.ModelParameters.Where(d => d.Model.IsDeleted == false))
            {
                //12 undelete relations
                modelParameter.IsDeleted = false;
                modelParameter.UserId = userId;
                //13 update relations
                context.ModelParameters.Update(modelParameter);
            }
            foreach (var situationParameter in parameter.SituationParameters.Where(d => d.Situation.IsDeleted == false))
            {
                //14 undelete relations
                situationParameter.IsDeleted = false;
                situationParameter.UserId = userId;
                //15 update relations
                context.SituationParameters.Update(situationParameter);
            }
            //14 save changes, await transaction commit, log, return response
            await context.SaveChangesAsync();
            await transaction.CommitAsync();
            _logger.LogInformation("ParameterId {Id} marked as not deleted", id);
            return new ServiceResponse($"Parameter with id {id} marked as not deleted", true);
        }
        catch (Exception ex)
        {
            //15 catch await transaction rollback, log, return response
            _logger.LogError(ex, "Error marking parameter with id {Id} as not deleted", id);
            await transaction.RollbackAsync();
            return new ServiceResponse($"Error marking parameter with id {id} as not deleted");
        }
    }

    /// <summary>
    /// Returns service response with true if plant by id is marked as deleted
    /// </summary>
    /// <param name="id"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    public async Task<ServiceResponse> MarkUnDeletePlant(int id, string userId)
    {
        //1 id<=0
        if (id <= 0)
        {
            _logger.LogWarning("Invalid plant id {Id}", id);
            return new ServiceResponse($"Invalid plant id {id}");
        }
        //2 await DbContext
        await using var context = await _contextFactory.CreateDbContextAsync();
        //3 await transaction
        await using var transaction = await context.Database.BeginTransactionAsync();
        //4 try-catch
        try
        {
            //5 get entity by id including children and relations with parents
            var plant = await context.Plants.Include(p => p.Areas).FirstOrDefaultAsync(d => d.PlantId == id);
            //6 check if entity is null
            if (plant == null)
            {
                _logger.LogWarning("PlantId {Id} not found", id);
                return new ServiceResponse($"Plant with id {id} not found");
            }
            //7 check if entity is marked as deleted
            if (plant.IsDeleted == false)
            {
                _logger.LogWarning("PlantId {Id} not marked as deleted", id);
                return new ServiceResponse($"Plant with id {id} not marked as deleted");
            }
            //8 check if duplicate name exists
            var exists = await context.Plants.AnyAsync(p => Equals(p.Name.ToLower().Trim(), plant.Name.ToLower().Trim()) && p.PlantId != id && p.IsDeleted == false);
            if (exists)
            {
                _logger.LogWarning("Plant with name {Name} already exists", plant.Name);
                return new ServiceResponse($"Plant with name {plant.Name} already exists");
            }
            //9 undelete entity
            plant.IsDeleted = false;
            plant.UserId = userId;
            //10 update entity
            context.Plants.Update(plant);
            //11 check if related parents are not marked as deleted and exist
            //12 undelete relations
            //13 update relations
            //14 save changes, await transaction commit, log, return response with true
            await context.SaveChangesAsync();
            await transaction.CommitAsync();
            _logger.LogInformation("PlantId {Id} marked as not deleted", id);
            return new ServiceResponse($"Plant with id {id} marked as not deleted", true);
        }
        catch (Exception ex)
        {
            //15 catch await transaction rollback, log, return response
            _logger.LogError(ex, "Error marking plant with id {Id} as not deleted", id);
            await transaction.RollbackAsync();
            return new ServiceResponse($"Error marking plant with id {id} as not deleted");
        }
    }

    /// <summary>
    /// Returns service response with true if question by id is marked as deleted
    /// Marks all relations as not deleted if it is possible
    /// </summary>
    /// <param name="id"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    public async Task<ServiceResponse> MarkUnDeleteQuestion(int id, string userId)
    {
        //1 id<=0
        if (id <= 0)
        {
            _logger.LogWarning("Invalid question id {Id}", id);
            return new ServiceResponse($"Invalid question id {id}");
        }
        //2 await DbContext
        await using var context = await _contextFactory.CreateDbContextAsync();
        //3 await transaction
        await using var transaction = await context.Database.BeginTransactionAsync();
        //4 try-catch
        try
        {
            //5 get entity by id including children and relations with parents
            var question = await context.Questions.Include(q => q.SituationQuestions).ThenInclude(q => q.Situation).FirstOrDefaultAsync(d => d.QuestionId == id);
            //6 check if entity is null
            if (question == null)
            {
                _logger.LogWarning("QuestionId {Id} not found", id);
                return new ServiceResponse($"Question with id {id} not found");
            }
            //7 check if entity is marked as deleted
            if (question.IsDeleted == false)
            {
                _logger.LogWarning("QuestionId {Id} not marked as deleted", id);
                return new ServiceResponse($"Question with id {id} not marked as deleted");
            }
            //8 check if duplicate name exists
            var exists = await context.Questions.AnyAsync(q => Equals(q.Name.ToLower().Trim(), question.Name.ToLower().Trim()) && q.QuestionId != id && q.IsDeleted == false);
            if (exists)
            {
                _logger.LogWarning("Question with name {Name} already exists", question.Name);
                return new ServiceResponse($"Question with name {question.Name} already exists");
            }
            //9 undelete entity
            question.IsDeleted = false;
            question.UserId = userId;
            //10 update entity
            context.Questions.Update(question);
            //11 check if related parents are not marked as deleted and exist

            //12 undelete relations
            foreach (var situationQuestion in question.SituationQuestions.Where(s => s.Situation.IsDeleted == false))
            {
                situationQuestion.IsDeleted = false;
                situationQuestion.UserId = userId;
                context.SituationQuestions.Update(situationQuestion);
            }
            //14 save changes, await transaction commit, log, return response with true
            await context.SaveChangesAsync();
            await transaction.CommitAsync();
            _logger.LogInformation("QuestionId {Id} marked as not deleted", id);
            return new ServiceResponse($"Question with id {id} marked as not deleted", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking question with id {Id} as not deleted", id);
            await transaction.RollbackAsync();
            return new ServiceResponse($"Error marking question with id {id} as not deleted");
        }
    }

    /// <summary>
    /// Returns service response with true if situation by id is marked as deleted
    /// Marks all relations as not deleted if it is possible
    /// </summary>
    /// <param name="id"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    public async Task<ServiceResponse> MarkUnDeleteSituation(int id, string userId)
    {
        //1 id<=0
        if (id <= 0)
        {
            _logger.LogWarning("Invalid situation id {Id}", id);
            return new ServiceResponse($"Invalid situation id {id}");
        }
        //2 await DbContext
        await using var context = await _contextFactory.CreateDbContextAsync();
        //3 await transaction
        await using var transaction = await context.Database.BeginTransactionAsync();
        //4 try-catch
        try
        {
            //5 get entity by id including children and relations with parents
            var situation = await context.Situations.Include(s => s.SituationQuestions).ThenInclude(s => s.Question).Include(s => s.SituationDetails).ThenInclude(s => s.Detail).Include(s => s.SituationParameters).ThenInclude(s => s.Parameter).Include(s => s.CategorySituations).ThenInclude(s => s.Category).Include(s => s.DeviceSituations).ThenInclude(s => s.Device).Include(s => s.AssetSituations).ThenInclude(s => s.Asset).FirstOrDefaultAsync(d => d.SituationId == id);
            //6 check if entity is null
            if (situation == null)
            {
                _logger.LogWarning("SituationId {Id} not found", id);
                return new ServiceResponse($"Situation with id {id} not found");
            }
            //7 check if entity is marked as deleted
            if (situation.IsDeleted == false)
            {
                _logger.LogWarning("SituationId {Id} not marked as deleted", id);
                return new ServiceResponse($"Situation with id {id} not marked as deleted");
            }
            //8 check if duplicate name exists
            var exists = await context.Situations.AnyAsync(s => Equals(s.Name.ToLower().Trim(), situation.Name.ToLower().Trim()) && s.SituationId != id && s.IsDeleted == false);
            if (exists)
            {
                _logger.LogWarning("Situation with name {Name} already exists", situation.Name);
                return new ServiceResponse($"Situation with name {situation.Name} already exists");
            }
            //9 undelete entity
            situation.IsDeleted = false;
            situation.UserId = userId;
            //10 update entity
            context.Situations.Update(situation);
            //11 undelete relations if both parents are not marked as deleted
            foreach (var situationQuestion in situation.SituationQuestions.Where(s => s.Question.IsDeleted == false))
            {
                situationQuestion.IsDeleted = false;
                situationQuestion.UserId = userId;
                context.SituationQuestions.Update(situationQuestion);
            }
            foreach (var situationDetail in situation.SituationDetails.Where(s => s.Detail.IsDeleted == false))
            {
                situationDetail.IsDeleted = false;
                situationDetail.UserId = userId;
                context.SituationDetails.Update(situationDetail);
            }
            foreach (var situationParameter in situation.SituationParameters.Where(s => s.Parameter.IsDeleted == false))
            {
                situationParameter.IsDeleted = false;
                situationParameter.UserId = userId;
                context.SituationParameters.Update(situationParameter);
            }
            foreach (var categorySituation in situation.CategorySituations.Where(s => s.Category.IsDeleted == false))
            {
                categorySituation.IsDeleted = false;
                categorySituation.UserId = userId;
                context.CategorySituations.Update(categorySituation);
            }
            foreach (var deviceSituation in situation.DeviceSituations.Where(s => s.Device.IsDeleted == false))
            {
                deviceSituation.IsDeleted = false;
                deviceSituation.UserId = userId;
                context.DeviceSituations.Update(deviceSituation);
            }
            foreach (var assetSituation in situation.AssetSituations.Where(s => s.Asset.IsDeleted == false))
            {
                assetSituation.IsDeleted = false;
                assetSituation.UserId = userId;
                context.AssetSituations.Update(assetSituation);
            }
            //12 save changes, await transaction commit, log, return response with true
            await context.SaveChangesAsync();
            await transaction.CommitAsync();
            _logger.LogInformation("SituationId {Id} marked as not deleted", id);
            return new ServiceResponse($"Situation with id {id} marked as not deleted", true);
        }
        catch (Exception ex)
        {
            //13 catch await transaction rollback, log, return response
            _logger.LogError(ex, "Error marking situation with id {Id} as not deleted", id);
            await transaction.RollbackAsync();
            return new ServiceResponse($"Error marking situation with id {id} as not deleted");
        }
    }

    /// <summary>
    /// Returns service response with true if space by id is marked as deleted
    /// Marks all relations as not deleted if it is possible
    /// </summary>
    /// <param name="id"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    public async Task<ServiceResponse> MarkUnDeleteSpace(int id, string userId)
    {
        //1 id<=0
        if (id <= 0)
        {
            _logger.LogWarning("Invalid space id {Id}", id);
            return new ServiceResponse($"Invalid space id {id}");
        }
        //2 await DbContext
        await using var context = await _contextFactory.CreateDbContextAsync();
        //3 await transaction
        await using var transaction = await context.Database.BeginTransactionAsync();
        //4 try-catch
        try
        {
            //5 get entity by id including children and relations with parents
            var space = await context.Spaces.Include(s => s.CommunicateSpaces).ThenInclude(s => s.Communicate).FirstOrDefaultAsync(d => d.SpaceId == id);
            //6 check if entity is null
            if (space == null)
            {
                _logger.LogWarning("SpaceId {Id} not found", id);
                return new ServiceResponse($"Space with id {id} not found");
            }
            //7 check if entity is marked as deleted
            if (space.IsDeleted == false)
            {
                _logger.LogWarning("SpaceId {Id} not marked as deleted", id);
                return new ServiceResponse($"Space with id {id} not marked as deleted");
            }
            //8 check if duplicate name exists
            var exists = await context.Spaces.AnyAsync(s => Equals(s.Name.ToLower().Trim(), space.Name.ToLower().Trim()) && s.SpaceId != id && s.IsDeleted == false && s.AreaId == space.AreaId);
            if (exists)
            {
                _logger.LogWarning("Space with name {Name} already exists", space.Name);
                return new ServiceResponse($"Space with name {space.Name} already exists");
            }
            //9 undelete entity
            space.IsDeleted = false;
            space.UserId = userId;
            //10 update entity
            context.Spaces.Update(space);
            //11 undelete relations if both parents are not marked as deleted
            foreach (var communicateSpace in space.CommunicateSpaces.Where(s => s.Communicate.IsDeleted == false))
            {
                communicateSpace.IsDeleted = false;
                communicateSpace.UserId = userId;
                context.CommunicateSpaces.Update(communicateSpace);
            }
            //12 save changes, await transaction commit, log, return response with true
            await context.SaveChangesAsync();
            await transaction.CommitAsync();
            _logger.LogInformation("SpaceId {Id} marked as not deleted", id);
            return new ServiceResponse($"Space with id {id} marked as not deleted", true);
        }
        catch (Exception ex)
        {
            //13 catch await transaction rollback, log, return response
            _logger.LogError(ex, "Error marking space with id {Id} as not deleted", id);
            await transaction.RollbackAsync();
            return new ServiceResponse($"Error marking space with id {id} as not deleted");
        }
    }

    /// <summary>
    /// Returns service response with true if area by id is updated
    /// </summary>
    /// <param name="id"></param>
    /// <param name="userId"></param>
    /// <param name="areaUpdateDto"></param>
    /// <returns></returns>
    public async Task<ServiceResponse> UpdateArea(int id, string userId, AreaUpdateDto areaUpdateDto)
    {
        //1 id<=0
        if (id <= 0)
        {
            return new ServiceResponse("Invalid area id");
        }
        //2 await DbContext
        await using var context = await _contextFactory.CreateDbContextAsync();
        //3 await transaction
        await using var transaction = await context.Database.BeginTransactionAsync();
        //4 try-catch
        try
        {
            //5 get entity by id including children and relations with parents
            var area = await context.Areas.Include(a => a.CommunicateAreas).ThenInclude(a => a.Communicate).FirstOrDefaultAsync(a => a.AreaId == id);
            //6 check if entity is null
            if (area == null)
            {
                return new ServiceResponse("Area not found");
            }
            //7 duplicate names check
            var duplicate = await context.Areas.AnyAsync(a =>
                a.AreaId != id && a.PlantId == area.PlantId && a.IsDeleted == false &&
                Equals(a.Name.ToLower().Trim(), areaUpdateDto.Name.ToLower().Trim()));
            if (duplicate)
            {
                _logger.LogWarning("Area with name {Name} already exists", areaUpdateDto.Name);
                return new ServiceResponse($"Area with name {areaUpdateDto.Name} already exists");
            }
            //8 check if entity is marked as deleted
            if (area.IsDeleted)
            {
                _logger.LogWarning("AreaId {Id} is marked as deleted", id);
                return new ServiceResponse($"Area with id {id} is marked as deleted");
            }
            if (!Equals(area.Name.ToLower().Trim(), areaUpdateDto.Name.ToLower().Trim()))
            {
                //9 update name
                area.Name = areaUpdateDto.Name;
                area.UserId = userId;
                context.Areas.Update(area);
            }
            if (!Equals(area.Description.ToLower().Trim(), areaUpdateDto.Description.ToLower().Trim()))
            {
                //10 update description
                area.Description = areaUpdateDto.Description;
                area.UserId = userId;
                context.Areas.Update(area);
            }
            // update communicate areas
            foreach (var communicateAreaDto in areaUpdateDto.

            //11 save changes, await transaction commit, log, return response with true
            await context.SaveChangesAsync();
            await transaction.CommitAsync();
            return new ServiceResponse("Area updated", true);
        }
        //12 catch await transaction rollback, log, return response
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating area with id {Id}", id);
            await transaction.RollbackAsync();
            return new ServiceResponse("Error updating area");
        }
    }
    /// <summary>
    /// Returns service response with true if asset by id is updated
    /// Updates related entities
    /// </summary>
    /// <param name="id"></param>
    /// <param name="userId"></param>
    /// <param name="assetUpdateDto"></param>
    /// <returns></returns>
    public async Task<ServiceResponse> UpdateAsset(int id, string userId, AssetUpdateDto assetUpdateDto)
    {
        // id<=0
        if (id <= 0)
        {
            return new ServiceResponse("Invalid asset id");
        }
        // await DbContext
        await using var context = await _contextFactory.CreateDbContextAsync();
        // await transaction
        await using var transaction = await context.Database.BeginTransactionAsync();
        // try-catch
        try
        {
            // get entity by id including children and relations with parents: AssetCategories, AssetDetails, CommunicateAssets, AssetSituations
            var asset = await context.Assets.Include(a => a.AssetCategories).ThenInclude(a => a.Category).Include(a => a.AssetDetails).ThenInclude(a => a.Detail).Include(a => a.CommunicateAssets).ThenInclude(a => a.Communicate).Include(a => a.AssetSituations).ThenInclude(a => a.Situation).FirstOrDefaultAsync(a => a.AssetId == id);
            // check if entity is null
            if (asset == null)
            {
                return new ServiceResponse("Asset not found");
            }
            // duplicate names check
            var duplicate = await context.Assets.AnyAsync(a =>
                a.AssetId != id && a.IsDeleted == false &&
                Equals(a.Name.ToLower().Trim(), assetUpdateDto.Name.ToLower().Trim()));
            if (duplicate)
            {
                _logger.LogWarning("Asset with name {Name} already exists", assetUpdateDto.Name);
                return new ServiceResponse($"Asset with name {assetUpdateDto.Name} already exists");
            }
            // check if entity is marked as deleted
            if (asset.IsDeleted)
            {
                _logger.LogWarning("AssetId {Id} is marked as deleted", id);
                return new ServiceResponse($"Asset with id {id} is marked as deleted");
            }
            // update Name
            if (!Equals(asset.Name.ToLower().Trim(), assetUpdateDto.Name.ToLower().Trim()))
            {
                asset.Name = assetUpdateDto.Name;
                asset.UserId = userId;
                context.Assets.Update(asset);
            }
            // update Process
            if (!Equals(asset.Process.ToLower().Trim(), assetUpdateDto.Process.ToLower().Trim()))
            {
                asset.Process = assetUpdateDto.Process;
                asset.UserId = userId;
                context.Assets.Update(asset);
            }
            // update Status
            if (!Equals(asset.Status, assetUpdateDto.Status))
            {
                asset.Status = assetUpdateDto.Status;
                asset.UserId = userId;
                context.Assets.Update(asset);
            }
            // update AssetCategories
            foreach (var assetCategoryDto in assetUpdateDto.AssetCategories)
            {
                // check if parent entity is null
                var category = await context.Categories.FirstOrDefaultAsync(a => a.CategoryId == assetCategoryDto.CategoryId);
                if (category == null)
                {
                    _logger.LogWarning("Category with id {Id} not found", assetCategoryDto.CategoryId);
                    return new ServiceResponse("Category not found");
                }
                // get entity by id
                var assetCategory = await context.AssetCategories.Include(a => a.Category).FirstOrDefaultAsync(a => a.CategoryId == assetCategoryDto.CategoryId && a.AssetId == assetCategoryDto.AssetId);
                // check if entity is null
                if (assetCategory == null)
                {
                    assetCategory = new AssetCategory
                    {
                        AssetId = assetCategoryDto.AssetId,
                        CategoryId = assetCategoryDto.CategoryId,
                        UserId = userId,
                        IsDeleted = false
                    };
                    context.AssetCategories.Add(assetCategory);
                }
                else
                {
                    // check if entity is marked as deleted
                    if (assetCategory.IsDeleted != assetCategoryDto.IsDeleted)
                    {
                        assetCategory.IsDeleted = assetCategoryDto.IsDeleted;
                        assetCategory.UserId = userId;
                        context.AssetCategories.Update(assetCategory);
                    }
                }
            }
            // update AssetDetails
            foreach (var communicateAssetDto in assetUpdateDto.AssetDetails)
            {
                // check if parent entity is null
                var detail = await context.Details.FirstOrDefaultAsync(a => a.DetailId == communicateAssetDto.DetailId);
                if (detail == null)
                {
                    _logger.LogWarning("Detail with id {Id} not found", communicateAssetDto.DetailId);
                    return new ServiceResponse("Detail not found");
                }
                // get entity by id
                var assetDetail = await context.AssetDetails.FirstOrDefaultAsync(a => a.DetailId == communicateAssetDto.DetailId && a.AssetId == communicateAssetDto.AssetId);
                // check if entity is null
                if (assetDetail == null && detail)
                {
                    assetDetail = new AssetDetail
                    {
                        AssetId = communicateAssetDto.AssetId,
                        DetailId = communicateAssetDto.DetailId,
                        UserId = userId,
                        IsDeleted = false
                    };
                    context.AssetDetails.Add(assetDetail);
                }
                else
                {

                    //// check if entity is marked as deleted
                    //if (assetDetail.IsDeleted)
                    //{
                    //    assetDetail.IsDeleted = communicateAssetDto.IsDeleted;
                    //    assetDetail.UserId = userId;
                    //    context.AssetDetails.Update(assetDetail);
                    //}
                }
            }
            // update CommunicateAssets
            // update AssetSituations
            // save changes, await transaction commit, log, return ServiceResponse(string, true)
        }
        catch (Exception)
        {
            // catch await transaction rollback, log, return ServiceResponse(string, false)

        }

    }
    /// <summary>
    /// Returns service response with true if category by id is updated
    /// Updates related entities
    /// </summary>
    /// <param name="id"></param>
    /// <param name="userId"></param>
    /// <param name="categoryUpdateDto"></param>
    /// <returns></returns>
    public async Task<ServiceResponse> UpdateCategory(int id, string userId, CategoryUpdateDto categoryUpdateDto)
    {
        //1 check category id
        if (id <= 0)
        {
            return new ServiceResponse("Invalid category id");
        }
        //2 await using context
        await using var context = await _contextFactory.CreateDbContextAsync();
        //3 await using transaction
        await using var transaction = await context.Database.BeginTransactionAsync();
        //4 try-catch
        try
        {
            Category c;
            var category = await context.Categories.FirstOrDefaultAsync(c => c.CategoryId == id);
            if (category == null)
            {
                return new ServiceResponse("Category not found");
            }
            // check if category exists and is not marked as deleted
            var exists = await context.Categories.AnyAsync(c =>
                c.Name.ToLower().Trim() == categoryUpdateDto.Name.ToLower().Trim() && c.IsDeleted == false &&
                c.CategoryId != id);
            if (exists)
            {
                return new ServiceResponse($"Category with name {categoryUpdateDto.Name} already exists");
            }
            // update name
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
            await transaction.CommitAsync();
            return new ServiceResponse("Category updated", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating category");
            await transaction.RollbackAsync();
            return new ServiceResponse("Error updating category");
        }
    }

    public async Task<ServiceResponse> UpdateCommunicate(int id, string userId, CommunicateUpdateDto communicateUpdateDto)
    {
        if (id <= 0)
        {
            return new ServiceResponse("Invalid communicate id");
        }
        await using var context = await _contextFactory.CreateDbContextAsync();
        try
        {
            var communicate = await context.Communicates.FirstOrDefaultAsync(c => c.CommunicateId == id);
            if (communicate == null)
            {
                _logger.LogError($"Communicate with id {id} not found");
                return new ServiceResponse($"Communicate with id {id} not found");
            }

            communicate.Name = communicateUpdateDto.Name;
            communicate.Description = communicateUpdateDto.Description;
            communicate. = DateTime.UtcNow;
            communicate.UpdatedBy = userId;
            await context.SaveChangesAsync();
            _logger.LogInformation("Communicate with id {id} updated");
            return new ServiceResponse();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating communicate with id {id}");
            return new ServiceResponse($"Error updating communicate with id {id}");
        }
    }

    public async Task<ServiceResponse> UpdateCoordinate(int id, string userId, CoordinateUpdateDto coordinateUpdateDto)
    {
        if (id <= 0)
        {
            return new ServiceResponse("Invalid coordinate id");
        }
        await using var context = await _contextFactory.CreateDbContextAsync();
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            var coordinate = await context.Coordinates.FirstOrDefaultAsync(c => c.CoordinateId == id);
            if (coordinate == null)
            {
                return new ServiceResponse("Coordinate not found");
            }

            var exists = await context.Coordinates.Include(a => a.Space).ThenInclude(a => a.Area).AnyAsync(c =>
                Equals(c.Name.ToLower().Trim(), coordinateUpdateDto.Name.ToLower().Trim()) && c.CoordinateId != id &&
                c.IsDeleted == false && c.Space.Area.PlantId == coordinate.Space.Area.PlantId);
            if (exists)
            {
                return new ServiceResponse($"Coordinate with name {coordinateUpdateDto.Name} already exists");
            }

            coordinate.Name = coordinateUpdateDto.Name;
            coordinate.Description = coordinateUpdateDto.Description;
            coordinate.UserId = userId;
            context.Coordinates.Update(coordinate);
            await context.SaveChangesAsync();
            await transaction.CommitAsync();
            return new ServiceResponse("Coordinate updated", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating coordinate");
            await transaction.RollbackAsync();
            return new ServiceResponse("Error updating coordinate");
        }
    }

    public async Task<ServiceResponse> UpdateDetail(int id, string userId, DetailUpdateDto detailUpdateDto)
    {
        if (id <= 0)
        {
            return new ServiceResponse("Invalid detail id");
        }
        await using var context = await _contextFactory.CreateDbContextAsync();
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            var detail = await context.Details.FirstOrDefaultAsync(d => d.DetailId == id);
            if (detail == null)
            {
                _logger.LogWarning("Detail not found");
                return new ServiceResponse("Detail not found");
            }

            var exists = await context.Details.AnyAsync(d =>
                Equals(d.Name.ToLower().Trim(), detailUpdateDto.Name.ToLower().Trim()) && d.DetailId != id &&
                d.IsDeleted == false);
            if (exists)
            {
                _logger.LogWarning("Detail with name {Name} already exists", detailUpdateDto.Name);
                return new ServiceResponse($"Detail with name {detailUpdateDto.Name} already exists");
            }

            if (!Equals(detail.Name.ToLower().Trim(), detailUpdateDto.Name.ToLower().Trim()))
            {
                detail.Name = detailUpdateDto.Name;
                detail.UserId = userId;
            }

            if (!Equals(detail.Description.ToLower().Trim(), detailUpdateDto.Description.ToLower().Trim()))
            {
                detail.Description = detailUpdateDto.Description;
                detail.UserId = userId;
            }

            await context.SaveChangesAsync();
            await transaction.CommitAsync();
            _logger.LogInformation("Detail with id {DetailId} updated", detail.DetailId);
            return new ServiceResponse($"Detail {detail.DetailId} updated", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating detail");
            await transaction.RollbackAsync();
            return new ServiceResponse($"Error updating detail");
        }
    }

    public async Task<ServiceResponse> UpdateDevice(int id, string userId, DeviceUpdateDto deviceUpdateDto)
    {
        if (id <= 0)
        {
            return new ServiceResponse("Invalid device id");
        }
        await using var context = await _contextFactory.CreateDbContextAsync();
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            var device = await context.Devices.FirstOrDefaultAsync(d => d.DeviceId == id);
            if (device == null)
            {
                _logger.LogWarning("Device not found");
                return new ServiceResponse("Device not found");
            }

            if (device.IsDeleted)
            {
                _logger.LogWarning("Device marked as deleted");
                return new ServiceResponse("Device marked as deleted");
            }

            var exists = await context.Devices.AnyAsync(d =>
                d.DeviceId != id && Equals(d.Name.ToLower().Trim(), deviceUpdateDto.Name.ToLower().Trim()) &&
                !d.IsDeleted);
            if (exists)
            {
                _logger.LogWarning("Device with name {deviceUpdateDto.Name} already exists", deviceUpdateDto.Name);
                return new ServiceResponse($"Device with name {deviceUpdateDto.Name} already exists");
            }

            device.UserId = userId;
            device.Name = deviceUpdateDto.Name;
            device.Description = deviceUpdateDto.Description;
            context.Devices.Update(device);
            await context.SaveChangesAsync();
            await transaction.CommitAsync();
            _logger.LogInformation("Device with id {DeviceId}", id);
            return new ServiceResponse($"Device {id} updated", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating device with id {DeviceId}", id);
            await transaction.RollbackAsync();
            return new ServiceResponse($"Error updating device with id {id}");
        }
    }

    public async Task<ServiceResponse> UpdateModel(int id, string userId, ModelUpdateDto modelUpdateDto)
    {
        if (id <= 0)
        {
            return new ServiceResponse("Invalid model id");
        }
        await using var context = await _contextFactory.CreateDbContextAsync();
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            var model = await context.Models.FirstOrDefaultAsync(m => m.ModelId == id);
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

            var exists = await context.Models.AnyAsync(m =>
                m.DeviceId == model.DeviceId && Equals(m.Name.ToLower().Trim(), modelUpdateDto.Name.ToLower().Trim()) &&
                !m.IsDeleted && m.ModelId != id);
            if (exists)
            {
                _logger.LogWarning("Model with name {modelUpdateDto.Name} already exists", modelUpdateDto.Name);
                return new ServiceResponse($"Model with name {modelUpdateDto.Name} already exists");
            }

            if (!Equals(model.Name.ToLower().Trim(), modelUpdateDto.Name.ToLower().Trim()))
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
            await transaction.CommitAsync();
            _logger.LogInformation("Model with id {ModelId} updated", model.ModelId);
            return new ServiceResponse($"Model {model.ModelId} updated", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating model with id {ModelId}", id);
            await transaction.RollbackAsync();
            return new ServiceResponse($"Error updating model with id {id}");
        }
    }

    public async Task<ServiceResponse> UpdateParameter(int id, string userId, ParameterUpdateDto parameterUpdateDto)
    {
        if (id <= 0)
        {
            return new ServiceResponse("Invalid parameter id");
        }
        await using var context = await _contextFactory.CreateDbContextAsync();
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            var parameter = await context.Parameters.FirstOrDefaultAsync(p => p.ParameterId == id);
            if (parameter == null)
            {
                _logger.LogWarning("Parameter not found");
                return new ServiceResponse("Parameter not found");
            }

            var exists = await context.Parameters.AnyAsync(p =>
                Equals(p.Name.ToLower().Trim(), parameterUpdateDto.Name.ToLower().Trim()) && p.ParameterId != id &&
                p.IsDeleted == false);
            if (exists)
            {
                _logger.LogWarning("Parameter with name {Name} already exists", parameterUpdateDto.Name);
                return new ServiceResponse($"Parameter with name {parameterUpdateDto.Name} already exists");
            }

            if (!Equals(parameter.Name.ToLower().Trim(), parameterUpdateDto.Name.ToLower().Trim()))
            {
                parameter.Name = parameterUpdateDto.Name;
                parameter.UserId = userId;
            }

            if (!Equals(parameter.Description, parameterUpdateDto.Description))
            {
                parameter.Description = parameterUpdateDto.Description;
                parameter.UserId = userId;
            }

            await context.SaveChangesAsync();
            await transaction.CommitAsync();
            _logger.LogInformation("Parameter with id {ParameterId} updated", parameter.ParameterId);
            return new ServiceResponse($"Parameter {parameter.ParameterId} updated");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating parameter with id {ParameterId}", id);
            await transaction.RollbackAsync();
            return new ServiceResponse($"Error updating parameter with id {id}");
        }
    }

    public async Task<ServiceResponse> UpdatePlant(int id, string userId, PlantUpdateDto plantUpdateDto)
    {
        if (id <= 0)
        {
            return new ServiceResponse("Invalid plant id");
        }
        await using var context = await _contextFactory.CreateDbContextAsync();
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            var plant = await context.Plants.FirstOrDefaultAsync(p => p.PlantId == id);
            if (plant == null)
            {
                return new ServiceResponse("Plant not found");
            }

            var exists = await context.Plants.Where(p =>
                Equals(p.Name.ToLower().Trim(), plantUpdateDto.Name.ToLower().Trim()) && p.PlantId != id &&
                p.IsDeleted == false).ToListAsync();
            if (exists.Any())
            {
                return new ServiceResponse($"Plant with name {plantUpdateDto.Name} already exists");
            }

            if (!Equals(plant.Name.ToLower().Trim(), plantUpdateDto.Name.ToLower().Trim()))
            {
                plant.Name = plantUpdateDto.Name;
                plant.UserId = userId;
            }

            if (!Equals(plant.Description, plantUpdateDto.Description))
            {
                plant.Description = plantUpdateDto.Description;
                plant.UserId = userId;
            }

            plant.Description = plantUpdateDto.Description;
            plant.Name = plantUpdateDto.Name;
            plant.UserId = userId;
            context.Plants.Update(plant);
            await context.SaveChangesAsync();
            await transaction.CommitAsync();
            return new ServiceResponse("Plant updated", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating plant");
            await transaction.RollbackAsync();
            return new ServiceResponse("Error updating plant");
        }
    }

    public async Task<ServiceResponse> UpdateQuestion(int id, string userId, QuestionUpdateDto questionUpdateDto)
    {
        if (id <= 0)
        {
            return new ServiceResponse("Invalid question id");
        }
        await using var context = await _contextFactory.CreateDbContextAsync();
        try
        {
            var question = await context.Questions.FirstOrDefaultAsync(q => q.QuestionId == id);
            if (question == null)
            {
                _logger.LogInformation("Question not found");
                return new ServiceResponse($"Question not found");
            }

            if (question.)
            {
                _logger.LogInformation("Question not found");
                return new ServiceResponse($"Question not found");
            }

            question.Name = questionUpdateDto.Name;
            question.;
            await context.SaveChangesAsync();
            _logger.LogInformation("Question updated");
            return new ServiceResponse();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating question");
            return new ServiceResponse($"Error updating question");
        }
    }

    public async Task<ServiceResponse> UpdateSituation(int id, string userId, SituationUpdateDto situationUpdateDto)
    {
        if (id <= 0)
        {
            return new ServiceResponse("Invalid situation id");
        }
        await using var context = await _contextFactory.CreateDbContextAsync();
        try
        {
            var situation = await context.Situations.Where(s => s.SituationId == id && !s.IsDeleted)
                .FirstOrDefaultAsync();
            if (situation == null)
            {
                _logger.LogWarning("SituationId {id} not found", id);
                return new ServiceResponse($"SituationId {id} not found");
            }

            if (situation.)
            {
                _logger.LogWarning("UserId {userId} does not match situation userId {situationUserId}", userId,
                    situation.UserId);
                return new ServiceResponse($"UserId {userId} does not match situation userId {situation.UserId}");
            }

            situation.Name = situationUpdateDto.Name;
            situation.Description = situationUpdateDto.Description;
            situation.UserId = userId;

            await context.SaveChangesAsync();
            _logger.LogInformation("SituationId {id} updated", id);
            return new ServiceResponse($"SituationId {id} updated");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating situation");
            return new ServiceResponse($"Error updating situation");
        }
    }

    public async Task<ServiceResponse> UpdateSpace(int id, string userId, SpaceUpdateDto spaceUpdateDto)
    {
        if (id <= 0)
        {
            return new ServiceResponse("Invalid space id");
        }
        await using var context = await _contextFactory.CreateDbContextAsync();
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            var space = await context.Spaces.FirstOrDefaultAsync(s => s.SpaceId == id);
            if (space == null)
            {
                return new ServiceResponse("Space not found");
            }

            var exists = await context.Spaces
                .Where(s => s.IsDeleted == false)
                .Where(s => s.SpaceId != id)
                .Where(s => Equals(s.Name.ToLower().Trim(), spaceUpdateDto.Name.ToLower().Trim()))
                .ToListAsync();
            if (exists.Any())
            {
                return new ServiceResponse($"Space with name {spaceUpdateDto.Name} already exists");
            }

            space.Name = spaceUpdateDto.Name;
            space.Description = spaceUpdateDto.Description;
            space.SpaceType = spaceUpdateDto.SpaceType;
            space.UserId = userId;
            context.Spaces.Update(space);
            await context.SaveChangesAsync();
            await transaction.CommitAsync();
            return new ServiceResponse("Space updated", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating space");
            await transaction.RollbackAsync();
            return new ServiceResponse("Error updating space");
        }
    }

    public async Task<ServiceResponse> UpdateAssetCategory(AssetCategoryDto assetCategoryDto, string userId)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Updates or creates a new AssetDetail, returns ServiceResponse(string, true) if successful
    /// </summary>
    /// <param name="assetDetailDto"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    public async Task<ServiceResponse> UpdateAssetDetail(AssetDetailDto assetDetailDto, string userId)
    {
        // id validation
        if (assetDetailDto.AssetId <= 0)
        {
            return new ServiceResponse("Invalid asset id");
        }
        if (assetDetailDto.DetailId <= 0)
        {
            return new ServiceResponse("Invalid detail id");
        }
        if (assetDetailDto.AssetDetailId < 0)
        {
            return new ServiceResponse("Invalid AssetDetailId id");
        }

        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();
        // await using transaction
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            // get asset
            var asset = await context.Assets.FirstOrDefaultAsync(a => a.AssetId == assetDetailDto.AssetId);
            if (asset == null)
            {
                return new ServiceResponse("Asset not found");
            }

            // get detail
            var detail = await context.Details.FirstOrDefaultAsync(d => d.DetailId == assetDetailDto.DetailId);
            if (detail == null)
            {
                return new ServiceResponse("Detail not found");
            }

            // get assetDetail
            var assetDetail = await context.AssetDetails.FirstOrDefaultAsync(ad => ad.AssetDetailId == assetDetailDto.AssetDetailId);
            // if assetDetail is null, create new
            if (assetDetail == null)
            {
                // create new assetDetail
                assetDetail = new AssetDetail
                {
                    AssetId = assetDetailDto.AssetId,
                    DetailId = assetDetailDto.DetailId,
                    UserId = userId
                };
                context.AssetDetails.Add(assetDetail);
            }
            else
            {
                if (asset.IsDeleted)
                {
                    _logger.LogWarning("AssetId {assetId} is marked as deleted", asset.AssetId);
                    return new ServiceResponse("Asset is marked as deleted");
                }
                if (detail.IsDeleted)
                {
                    _logger.LogWarning("DetailId {detailId} is marked as deleted", detail.DetailId);
                    return new ServiceResponse("Detail is marked as deleted");
                }

                // update assetDetail
                if (assetDetail.IsDeleted != assetDetailDto.IsDeleted)
                {
                    assetDetail.IsDeleted = assetDetailDto.IsDeleted;
                    assetDetail.UserId = userId;
                }
                if (assetDetail.Value != assetDetailDto.Value)
                {
                    assetDetail.Value = assetDetailDto.Value;
                    assetDetail.UserId = userId;
                }
                context.AssetDetails.Update(assetDetail);
            }

            // save changes
            await context.SaveChangesAsync();
            await transaction.CommitAsync();
            return new ServiceResponse("AssetDetail updated", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating assetDetail");
            await transaction.RollbackAsync();
            return new ServiceResponse("Error updating assetDetail");
        }
    }
    /// <summary>
    /// Updates or creates a new AssetSituation, returns ServiceResponse(string, true) if successful
    /// </summary>
    /// <param name="assetSituationDto"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task<ServiceResponse> UpdateAssetSituation(AssetSituationDto assetSituationDto, string userId)
    {
        // id validation
        if (assetSituationDto.AssetId <= 0)
        {
            return new ServiceResponse("Invalid asset id");
        }
        if (assetSituationDto.SituationId <= 0)
        {
            return new ServiceResponse("Invalid situation id");
        }
        if (assetSituationDto.AssetSituationId < 0)
        {
            return new ServiceResponse("Invalid AssetSituationId id");
        }
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();
        // await using transaction
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            // get asset
            var asset = await context.Assets.FirstOrDefaultAsync(a => a.AssetId == assetSituationDto.AssetId);
            if (asset == null)
            {
                return new ServiceResponse("Asset not found");
            }
            if (asset.IsDeleted)
            {
                _logger.LogWarning("AssetId {assetId} is marked as deleted", asset.AssetId);
                return new ServiceResponse("Asset is marked as deleted");
            }
            // get situation
            var situation = await context.Situations.FirstOrDefaultAsync(s => s.SituationId == assetSituationDto.SituationId);
            if (situation == null)
            {
                return new ServiceResponse("Situation not found");
            }
            if (situation.IsDeleted)
            {
                _logger.LogWarning("SituationId {situationId} is marked as deleted", situation.SituationId);
                return new ServiceResponse("Situation is marked as deleted");
            }
            // get assetSituation
            var assetSituation = await context.AssetSituations.FirstOrDefaultAsync(asd => asd.AssetSituationId == assetSituationDto.AssetSituationId);
            // if assetSituation is null, create new
            if (assetSituation == null)
            {
                // create new assetSituation
                assetSituation = new AssetSituation
                {
                    AssetId = assetSituationDto.AssetId,
                    SituationId = assetSituationDto.SituationId,
                    UserId = userId
                };
                context.AssetSituations.Add(assetSituation);
            }
            else
            {
                // update assetSituation
                if (assetSituation.IsDeleted != assetSituationDto.IsDeleted)
                {
                    assetSituation.IsDeleted = assetSituationDto.IsDeleted;
                    assetSituation.UserId = userId;
                }
                    context.AssetSituations.Update(assetSituation);
            }
            // save changes
            await context.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating assetSituation");
            await transaction.RollbackAsync();
            return new ServiceResponse("Error updating assetSituation");
        }
    }

        public async Task<ServiceResponse> UpdateCategorySituation(CategorySituationDto categorySituationDto, string userId)
    {
        throw new NotImplementedException();
    }

    public async Task<ServiceResponse> UpdateCommunicateArea(CommunicateAreaDto communicateAreaDto, string userId)
    {
        throw new NotImplementedException();
    }

    public async Task<ServiceResponse> UpdateCommunicateAsset(CommunicateAssetDto communicateAssetDto, string userId)
    {
        throw new NotImplementedException();
    }

    public async Task<ServiceResponse> UpdateCommunicateCategory(CommunicateCategoryDto communicateCategoryDto, string userId)
    {
        throw new NotImplementedException();
    }

    public async Task<ServiceResponse> UpdateCommunicateCoordinate(CommunicateCoordinateDto communicateCoordinateDto, string userId)
    {
        throw new NotImplementedException();
    }

    public async Task<ServiceResponse> UpdateCommunicateDevice(CommunicateDeviceDto communicateDeviceDto, string userId)
    {
        throw new NotImplementedException();
    }

    public async Task<ServiceResponse> UpdateCommunicateModel(CommunicateModelDto communicateModelDto, string userId)
    {
        throw new NotImplementedException();
    }

    public async Task<ServiceResponse> UpdateCommunicateSpace(CommunicateSpaceDto communicateSpaceDto, string userId)
    {
        throw new NotImplementedException();
    }

    public async Task<ServiceResponse> UpdateDeviceSituation(DeviceSituationDto deviceSituationDto, string userId)
    {
        throw new NotImplementedException();
    }

    public async Task<ServiceResponse> UpdateModelParameter(ModelParameterDto modelParameterDto, string userId)
    {
        throw new NotImplementedException();
    }

    public async Task<ServiceResponse> UpdateSituationDetail(SituationDetailDto situationDetailDto, string userId)
    {
        throw new NotImplementedException();
    }

    public async Task<ServiceResponse> UpdateSituationParameter(SituationParameterDto situationParameterDto, string userId)
    {
        throw new NotImplementedException();
    }

    public async Task<ServiceResponse> UpdateSituationQuestion(SituationQuestionDto situationQuestionDto, string userId)
    {
        throw new NotImplementedException();
    }

    public async Task<ServiceResponse> DeleteAssetCategory(AssetCategoryDto assetCategoryDto, string userId)
    {
        throw new NotImplementedException();
    }

    public async Task<ServiceResponse> DeleteAssetDetail(AssetDetailDto assetDetailDto, string userId)
    {
        throw new NotImplementedException();
    }

    public async Task<ServiceResponse> DeleteAssetSituation(AssetSituationDto assetSituationDto, string userId)
    {
        throw new NotImplementedException();
    }

    public async Task<ServiceResponse> DeleteCategorySituation(CategorySituationDto categorySituationDto, string userId)
    {
        throw new NotImplementedException();
    }

    public async Task<ServiceResponse> DeleteCommunicateArea(CommunicateAreaDto communicateAreaDto, string userId)
    {
        throw new NotImplementedException();
    }

    public async Task<ServiceResponse> DeleteCommunicateAsset(CommunicateAssetDto communicateAssetDto, string userId)
    {
        throw new NotImplementedException();
    }

    public async Task<ServiceResponse> DeleteCommunicateCategory(CommunicateCategoryDto communicateCategoryDto, string userId)
    {
        throw new NotImplementedException();
    }

    public async Task<ServiceResponse> DeleteCommunicateCoordinate(CommunicateCoordinateDto communicateCoordinateDto, string userId)
    {
        throw new NotImplementedException();
    }

    public async Task<ServiceResponse> DeleteCommunicateDevice(CommunicateDeviceDto communicateDeviceDto, string userId)
    {
        throw new NotImplementedException();
    }

    public async Task<ServiceResponse> DeleteCommunicateModel(CommunicateModelDto communicateModelDto, string userId)
    {
        throw new NotImplementedException();
    }

    public async Task<ServiceResponse> DeleteCommunicateSpace(CommunicateSpaceDto communicateSpaceDto, string userId)
    {
        throw new NotImplementedException();
    }

    public async Task<ServiceResponse> DeleteDeviceSituation(DeviceSituationDto deviceSituationDto, string userId)
    {
        throw new NotImplementedException();
    }

    public async Task<ServiceResponse> DeleteModelParameter(ModelParameterDto modelParameterDto, string userId)
    {
        throw new NotImplementedException();
    }

    public async Task<ServiceResponse> DeleteSituationDetail(SituationDetailDto situationDetailDto, string userId)
    {
        throw new NotImplementedException();
    }

    public async Task<ServiceResponse> DeleteSituationParameter(SituationParameterDto situationParameterDto, string userId)
    {
        throw new NotImplementedException();
    }

    public async Task<ServiceResponse> DeleteSituationQuestion(SituationQuestionDto situationQuestionDto, string userId)
    {
        throw new NotImplementedException();
    }
}