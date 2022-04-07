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

    public async Task<ServiceResponse> ChangeAssetsModel(int assetId, string userId, int modelId)
    {
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

    public async Task<ServiceResponse> CreateArea(int plantId, AreaCreateDto areaCreateDto, string userId)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            var plant = await context.Plants.Include(a => a.Areas).FirstOrDefaultAsync(p => p.PlantId == plantId);
            if (plant == null || plant.IsDeleted)
            {
                _logger.LogWarning("Cannot create area for plant with id {PlantId}", plantId);
                return new ServiceResponse($"Cannot create area for plant with id {plantId}");
            }

            if (plant.Areas.Any(a =>
                    !a.IsDeleted && Equals(a.Name.ToLower().Trim(), areaCreateDto.Name.ToLower().Trim())))
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

            if (coordinate.Assets.Any(a =>
                    !a.IsDeleted && Equals(a.Name.ToLower().Trim(), assetCreateDto.Name.ToLower().Trim())))
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

    public async Task<ServiceResponse> CreateCategory(CategoryCreateDto categoryCreateDto, string userId)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            var exists = await context.Categories.Where(c =>
                c.Name.ToLower().Trim() == categoryCreateDto.Name.ToLower().Trim() && c.IsDeleted == false).AnyAsync();
            if (exists)
            {
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

    public async Task<ServiceResponse> CreateCommunicate(CommunicateCreateDto communicateCreateDto, string userId)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        try
        {
            var communicate = new Communicate
            {
                UserId = userId,
                Name = communicateCreateDto.Name,
                Description = communicateCreateDto.Description,
                
                
            };
            context.Communicates.Add(communicate);
            await context.SaveChangesAsync();
            _logger.LogInformation("Communicate created");
            return new ServiceResponse();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating communicate");
            return new ServiceResponse($"Error creating communicate");
        }
    }

    public async Task<ServiceResponse> CreateCoordinate(int spaceId, CoordinateCreateDto coordinateCreateDto,
        string userId)
    {
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

            if (space.Coordinates.Any(c =>
                    !c.IsDeleted && Equals(c.Name.ToLower().Trim(), coordinateCreateDto.Name.ToLower().Trim())))
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

    public async Task<ServiceResponse> CreateDevice(DeviceCreateDto deviceCreateDto, string userId)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            Device device = new();
            var exist = await context.Devices.FirstOrDefaultAsync(p =>
                Equals(p.Name.ToLower().Trim(), device.Name.ToLower().Trim()));
            if (exist is { IsDeleted: false })
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

    public async Task<ServiceResponse> CreateModel(int deviceId, ModelCreateDto modelCreateDto, string userId)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            var device = await context.Devices.FirstOrDefaultAsync(d => d.DeviceId == deviceId);
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

            var exists = await context.Models.Where(m =>
                m.DeviceId == deviceId && Equals(m.Name.ToLower().Trim(), modelCreateDto.Name.ToLower().Trim()) &&
                !m.IsDeleted).AnyAsync();
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

    public async Task<ServiceResponse> CreateParameter(ParameterCreateDto parameterCreateDto, string userId)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            var exists = await context.Parameters.Where(p =>
                Equals(p.Name.ToLower().Trim(), parameterCreateDto.Name.ToLower().Trim()) && !p.IsDeleted).AnyAsync();
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

    public async Task<ServiceResponse> CreatePlant(PlantCreateDto plantCreateDto, string userId)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            Plant plant = new();
            var exist = await context.Plants.FirstOrDefaultAsync(p =>
                Equals(p.Name.ToLower().Trim(), plant.Name.ToLower().Trim()));
            if (exist is {IsDeleted: false})
            {
                return new ServiceResponse($"Plant with name {plantCreateDto.Name} already exists");
            }

            plant.UserId = userId;
            plant.Name = plantCreateDto.Name;
            plant.Description = plantCreateDto.Description;
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

    public async Task<ServiceResponse> CreateQuestion(QuestionCreateDto questionCreateDto, string userId)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        try
        {
            var question = new Question
            {
                Name = questionCreateDto.Name,
                UserId = userId,
            };

            context.Questions.Add(question);
            await context.SaveChangesAsync();
            _logger.LogInformation("QuestionId {id} created", question.QuestionId);
            return new ServiceResponse($"QuestionId {question.QuestionId} created");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating question");
            return new ServiceResponse($"Error creating question");
        }
    }

    public async Task<ServiceResponse> CreateSituation(SituationCreateDto situationCreateDto, string userId)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            var exists = await context.Situations.Where(s =>
                Equals(s.Name.ToLower().Trim(), situationCreateDto.Name.ToLower().Trim()) && !s.IsDeleted).AnyAsync();
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

    public async Task<ServiceResponse> CreateSpace(int areaId, SpaceCreateDto spaceCreateDto, string userId)
    {
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

            if (area.Spaces.Any(s =>
                    !s.IsDeleted && Equals(s.Name.ToLower().Trim(), spaceCreateDto.Name.ToLower().Trim())))
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

    public async Task<ServiceResponse> DeleteArea(int id)
    {
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

            if (area.IsDeleted == false)
            {
                _logger.LogError("Area not marked as deleted");
                return new ServiceResponse("Area not marked as deleted");
            }

            context.Areas.Remove(area);
            await context.SaveChangesAsync();
            await transaction.CommitAsync();
            return new ServiceResponse("Area deleted", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting area");
            await transaction.RollbackAsync();
            return new ServiceResponse("Error deleting area");
        }
    }

    public async Task<ServiceResponse> DeleteAsset(int id)
    {
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

            context.Assets.Remove(asset);
            _logger.LogInformation("Asset with id {AssetId} deleted", id);
            await context.SaveChangesAsync();
            await transaction.CommitAsync();
            return new ServiceResponse($"Asset {id} deleted", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting asset with id {AssetId}", id);
            await transaction.RollbackAsync();
            return new ServiceResponse($"Error deleting asset with id {id}");
        }
    }

    public async Task<ServiceResponse> DeleteCategory(int id)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            var category = await context.Categories.FirstOrDefaultAsync(c => c.CategoryId == id);
            if (category == null)
            {
                return new ServiceResponse("Category not found");
            }

            if (category.IsDeleted == false)
            {
                return new ServiceResponse("Category not marked as deleted");
            }

            context.Categories.Remove(category);
            await context.SaveChangesAsync();
            await transaction.CommitAsync();
            return new ServiceResponse("Category deleted", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting category");
            await transaction.RollbackAsync();
            return new ServiceResponse("Error deleting category");
        }
    }

    public async Task<ServiceResponse> DeleteCommunicate(int id)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        try
        {
            var communicate = await context.Communicates.FirstOrDefaultAsync(c => c.CommunicateId == id);
            if (communicate == null)
            {
                _logger.LogError("Communicate with id {Id} not found", id);
                return new ServiceResponse($"Communicate with id {id} not found");
            }

            context.Communicates.Remove(communicate);
            await context.SaveChangesAsync();
            _logger.LogInformation("Communicate with id {Id} deleted", id);
            return new ServiceResponse($"Communicate with id {id} deleted");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting communicate with id {Id}", id);
            return new ServiceResponse($"Error deleting communicate with id {id}");
        }
    }

    public async Task<ServiceResponse> DeleteCoordinate(int id)
    {
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

            if (coordinate.IsDeleted == false)
            {
                _logger.LogError("Coordinate not marked as deleted");
                return new ServiceResponse("Coordinate not marked as deleted");
            }

            context.Coordinates.Remove(coordinate);
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

    public async Task<ServiceResponse> DeleteDetail(int id)
    {
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

            detail.IsDeleted = true;
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

    public async Task<ServiceResponse> DeleteDevice(int id)
    {
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

            if (!device.IsDeleted)
            {
                _logger.LogWarning("Device not marked as deleted");
                return new ServiceResponse("Device not marked as deleted");
            }

            context.Devices.Remove(device);
            await context.SaveChangesAsync();
            await transaction.CommitAsync();
            _logger.LogInformation("Device with id {DeviceId} deleted", id);
            return new ServiceResponse($"Device {id} deleted", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting device with id {DeviceId}", id);
            await transaction.RollbackAsync();
            return new ServiceResponse($"Error deleting device with id {id}");
        }
    }

    public async Task<ServiceResponse> DeleteModel(int id)
    {
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

            if (!model.IsDeleted)
            {
                _logger.LogWarning("Model not marked as deleted");
                return new ServiceResponse("Model not marked as deleted");
            }

            context.Models.Remove(model);
            await context.SaveChangesAsync();
            await transaction.CommitAsync();
            _logger.LogInformation("Model with id {ModelId} deleted", model.ModelId);
            return new ServiceResponse($"Model {model.ModelId} deleted", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting model with id {ModelId}", id);
            await transaction.RollbackAsync();
            return new ServiceResponse($"Error deleting model with id {id}");
        }
    }

    public async Task<ServiceResponse> DeleteParameter(int id)
    {
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

            if (!parameter.IsDeleted)
            {
                _logger.LogWarning("Parameter with id {ParameterId} not marked as deleted", parameter.ParameterId);
                return new ServiceResponse($"Parameter with id {parameter.ParameterId} not marked as deleted");
            }

            context.Remove(parameter);
            await context.SaveChangesAsync();
            await transaction.CommitAsync();
            _logger.LogInformation("Parameter with id {ParameterId} deleted", parameter.ParameterId);
            return new ServiceResponse($"Parameter {parameter.ParameterId} deleted");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting parameter with id {ParameterId}", id);
            await transaction.RollbackAsync();
            return new ServiceResponse($"Error deleting parameter with id {id}");
        }
    }

    public async Task<ServiceResponse> DeletePlant(int id)
    {
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

            if (plant.IsDeleted == false)
            {
                _logger.LogError("Plant not marked as deleted");
                return new ServiceResponse("Plant not marked as deleted");
            }

            context.Plants.Remove(plant);

            await context.SaveChangesAsync();
            await transaction.CommitAsync();
            return new ServiceResponse("Plant deleted", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting plant");
            await transaction.RollbackAsync();
            return new ServiceResponse("Error deleting plant");
        }
    }

    public async Task<ServiceResponse> DeleteQuestion(int id)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        try
        {
            var question = await context.Questions.FirstOrDefaultAsync(q => q.QuestionId == id);
            if (question == null)
            {
                _logger.LogInformation("Question not found");
                return new ServiceResponse($"Question not found");
            }

            question.IsDeleted = true;
            question.;
            await context.SaveChangesAsync();
            _logger.LogInformation("Question deleted");
            return new ServiceResponse();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting question");
            return new ServiceResponse($"Error deleting question");
        }
    }

    public async Task<ServiceResponse> DeleteSituation(int id)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        try
        {
            var situation = await context.Situations.Where(s => s.SituationId == id).FirstOrDefaultAsync();
            if (situation == null)
            {
                _logger.LogWarning("SituationId {id} not found", id);
                return new ServiceResponse($"SituationId {id} not found");
            }

            situation.IsDeleted = true;
            situation.UpdatedAt = DateTime.UtcNow;

            await context.SaveChangesAsync();
            _logger.LogInformation("SituationId {id} deleted", id);
            return new ServiceResponse($"SituationId {id} deleted");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting situation");
            return new ServiceResponse($"Error deleting situation");
        }
    }

    public async Task<ServiceResponse> DeleteSpace(int id)
    {
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

            if (space.IsDeleted == false)
            {
                _logger.LogError("Space not marked as deleted");
                return new ServiceResponse("Space not marked as deleted");
            }

            context.Spaces.Remove(space);
            await context.SaveChangesAsync();
            await transaction.CommitAsync();
            return new ServiceResponse("Space deleted", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting space");
            await transaction.RollbackAsync();
            return new ServiceResponse("Error deleting space");
        }
    }

    public async Task<ServiceResponse<AreaDto>> GetAreaById(int id)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        try
        {
            var area = await context.Areas.FirstOrDefaultAsync(a => a.AreaId == id);
            if (area == null)
            {
                return new ServiceResponse<AreaDto>("Area not found");
            }

            return new ServiceResponse<AreaDto>( area, "Area returned");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting area by id");
            return new ServiceResponse<AreaDto>("Error getting area by id");
        }
    }

    public async Task<ServiceResponse<IEnumerable<AreaDto>>> GetAreas()
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        try
        {
            var areas = await context.Areas.ToListAsync();
            if (areas.Count == 0)
            {
                return new ServiceResponse<IEnumerable<AreaDto>>(new List<AreaDto>(), "No areas found");
            }
            else
            {
                return new ServiceResponse<IEnumerable<AreaDto>>("Areas not found");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all areas");
            return new ServiceResponse<IEnumerable<AreaDto>>("Error getting all areas");
        }
    }

    public async Task<ServiceResponse<IEnumerable<AreaDto>>> GetAreasWithSpaces()
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        try
        {
            var areas = await context.Areas.Include(s => s.Spaces).ToListAsync();
            if (areas.Count == 0)
            {
                return new ServiceResponse<IEnumerable<AreaDto>>("Areas not found");
            }

            return new ServiceResponse<IEnumerable<AreaDto>>( < IEnumerable < AreaDto >> (areas),
            "List of areas with spaces returned");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting areas with spaces");
            return new ServiceResponse<IEnumerable<AreaDto>>("Error getting areas with spaces");
        }
    }

    public async Task<ServiceResponse<AssetDto>> GetAssetById(int id)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        try
        {
            var asset = await context.Assets.Include(a => a.AssetDetails).Include(a => a.AssetCategories)
                .FirstOrDefaultAsync(a => a.AssetId == id);
            if (asset == null)
            {
                return new ServiceResponse<AssetDto>("Asset not found");
            }

            return new ServiceResponse<AssetDto>( < AssetDto > (asset), "Asset returned");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting asset by id");
            return new ServiceResponse<AssetDto>("Error getting asset by id");
        }
    }

    public async Task<ServiceResponse<IEnumerable<AssetDto>>> GetAssets()
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        try
        {
            var assets = await context.Assets.Include(a => a.AssetDetails).Include(a => a.AssetCategories)
                .ToListAsync();
            if (assets.Count == 0)
            {
                return new ServiceResponse<IEnumerable<AssetDto>>(new List<AssetDto>(), "No assets found");
            }

            return new ServiceResponse<IEnumerable<AssetDto>>( < IEnumerable < AssetDto >> (assets),
            "List of assets returned");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all assets");
            return new ServiceResponse<IEnumerable<AssetDto>>("Error getting all assets");
        }
    }

    public async Task<ServiceResponse<IEnumerable<AssetDto>>> GetAssetsWithAllData()
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        try
        {
            var assets = await context.Assets.Include(a => a.AssetDetails).Include(a => a.AssetCategories)
                .Include(a => a.Coordinate).Include(c => c.CommunicateAssets).Include(a => a.Model).ToListAsync();
            if (assets.Count == 0)
            {
                return new ServiceResponse<IEnumerable<AssetDto>>("Assets not found");
            }

            return new ServiceResponse<IEnumerable<AssetDto>>( < IEnumerable < AssetDto >> (assets),
            "List of assets returned");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all assets");
            return new ServiceResponse<IEnumerable<AssetDto>>("Error getting all assets");
        }
    }

    public async Task<ServiceResponse<IEnumerable<CategoryDto>>> GetCategories()
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        try
        {
            var categories = await context.Categories.ToListAsync();
            return new ServiceResponse<IEnumerable<CategoryDto>>( < IEnumerable < CategoryDto >> (categories),
            "Categories found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting categories");
            return new ServiceResponse<IEnumerable<CategoryDto>>("Error getting categories");
        }
    }

    public async Task<ServiceResponse<IEnumerable<CategoryDto>>> GetCategoriesWithAssets()
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        try
        {
            var categories = await context.Categories.Include(a => a.AssetCategories).ThenInclude(a => a.Asset)
                .ToListAsync();
            return new ServiceResponse<IEnumerable<CategoryDto>>( < IEnumerable < CategoryDto >> (categories),
            "Categories found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting categories");
            return new ServiceResponse<IEnumerable<CategoryDto>>("Error getting categories");
        }
    }

    public async Task<ServiceResponse<CategoryDto>> GetCategoryById(int id)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        try
        {
            var category = await context.Categories.FirstOrDefaultAsync(c => c.CategoryId == id);
            if (category == null)
            {
                return new ServiceResponse<CategoryDto>("Category not found");
            }

            return new ServiceResponse<CategoryDto>( < CategoryDto > (category), "Category found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting category");
            return new ServiceResponse<CategoryDto>("Error getting category");
        }
    }

    public async Task<ServiceResponse<CommunicateDto>> GetCommunicateById(int id)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        try
        {
            var communicate = await context.Communicates.FirstOrDefaultAsync(c => c.CommunicateId == id);
            if (communicate == null)
            {
                _logger.LogInformation("Communicate not found");
                return new ServiceResponse<CommunicateDto>($"Communicate not found");
            }

            var communicateDto = new CommunicateDto
            {
                CommunicateId = communicate.CommunicateId,
                Name = communicate.Name,
                Description = communicate.Description,
                CreatedAt = communicate.CreatedAt,
                UpdatedAt = communicate.UpdatedAt
            };
            _logger.LogInformation("Communicate found");
            return new ServiceResponse<CommunicateDto>(communicateDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting communicate");
            return new ServiceResponse<CommunicateDto>($"Error getting communicate");
        }
    }

    public async Task<ServiceResponse<IEnumerable<CommunicateDto>>> GetCommunicates()
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        try
        {
            var communicates = await context.Communicates.Where(c => c.IsDeleted == false).ToListAsync();
            var communicateDtos = new List<CommunicateDto>();
            foreach (var communicate in communicates)
            {
                var communicateDto = new CommunicateDto
                {
                    CommunicateId = communicate.CommunicateId,
                    Name = communicate.Name,
                    Description = communicate.Description,
                    CreatedAt = communicate.CreatedAt,
                    UpdatedAt = communicate.UpdatedAt
                };
                communicateDtos.Add(communicateDto);
            }

            _logger.LogInformation("Communicates found");
            return new ServiceResponse<IEnumerable<CommunicateDto>>(communicateDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting communicates");
            return new ServiceResponse<IEnumerable<CommunicateDto>>($"Error getting communicates");
        }
    }

    public async Task<ServiceResponse<IEnumerable<CommunicateDto>>> GetCommunicatesWithAssets()
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        try
        {
            var communicates = await context.Communicates.Where(c => c.IsDeleted == false).ToListAsync();
            var communicateDtos = new List<CommunicateDto>();
            foreach (var communicate in communicates)
            {
                var communicateDto = new CommunicateDto
                {
                    CommunicateId = communicate.CommunicateId,
                    Name = communicate.Name,
                    Description = communicate.Description,
                    CreatedAt = communicate.CreatedAt,
                    UpdatedAt = communicate.UpdatedAt
                };
                var assets = await context.Assets.Where(a => a.CommunicateId == communicate.CommunicateId)
                    .ToListAsync();
                foreach (var asset in assets)
                {
                    var assetDto = new AssetDto
                    {
                        AssetId = asset.AssetId,
                        Name = asset.Name,
                        Description = asset.Description,
                        CreatedAt = asset.CreatedAt,
                        UpdatedAt = asset.UpdatedAt
                    };
                    communicateDto.Assets.Add(assetDto);
                }

                communicateDtos.Add(communicateDto);
            }

            _logger.LogInformation("Communicates found");
            return new ServiceResponse<IEnumerable<CommunicateDto>>(communicateDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting communicates");
            return new ServiceResponse<IEnumerable<CommunicateDto>>($"Error getting communicates");
        }
    }

    public async Task<ServiceResponse<CoordinateDto>> GetCoordinateById(int id)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        try
        {
            var coordinate = await context.Coordinates.FirstOrDefaultAsync(c => c.CoordinateId == id);
            if (coordinate == null)
            {
                return new ServiceResponse<CoordinateDto>("Coordinate not found");
            }

            return new ServiceResponse<CoordinateDto>( < CoordinateDto > (coordinate), "Coordinate returned");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting coordinate by id");
            return new ServiceResponse<CoordinateDto>("Error getting coordinate by id");
        }
    }

    public async Task<ServiceResponse<IEnumerable<CoordinateDto>>> GetCoordinates()
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        try
        {
            var coordinates = await context.Coordinates.ToListAsync();
            if (coordinates.Count == 0)
            {
                return new ServiceResponse<IEnumerable<CoordinateDto>>("Coordinates not found");
            }

            return new ServiceResponse<IEnumerable<CoordinateDto>>( < IEnumerable < CoordinateDto >> (coordinates),
            "List of coordinates returned");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all coordinates");
            return new ServiceResponse<IEnumerable<CoordinateDto>>("Error getting all coordinates");
        }
    }

    public async Task<ServiceResponse<IEnumerable<CoordinateDto>>> GetCoordinatesWithAssets()
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        try
        {
            var coordinates = await context.Coordinates.Include(a => a.Assets).ToListAsync();
            if (coordinates.Count == 0)
            {
                return new ServiceResponse<IEnumerable<CoordinateDto>>("Coordinates not found");
            }

            return new ServiceResponse<IEnumerable<CoordinateDto>>( < IEnumerable < CoordinateDto >> (coordinates),
            "List of coordinates with assets returned");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all coordinates with assets");
            return new ServiceResponse<IEnumerable<CoordinateDto>>("Error getting all coordinates with assets");
        }
    }

    public async Task<ServiceResponse<DetailDto>> GetDetailById(int id)
    {
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

    public async Task<ServiceResponse<IEnumerable<DetailDto>>> GetDetailsWithAssets()
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        try
        {
            var details = await _mapper.ProjectTo<DetailDto>(context.Details.Include(d => d.AssetDetails))
                .ToListAsync();
            _logger.LogInformation("{DetailCount} details found", details.Count);
            return new ServiceResponse<IEnumerable<DetailDto>>(details, $"{details.Count} details found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting details");
            return new ServiceResponse<IEnumerable<DetailDto>>("Error getting details");
        }
    }

    public async Task<ServiceResponse<DeviceDto>> GetDeviceById(int id)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        try
        {
            var device = await context.Devices.FirstOrDefaultAsync(d => d.DeviceId == id);
            if (device == null)
            {
                return new ServiceResponse<DeviceDto>("Plant not found");
            }

            return new ServiceResponse<DeviceDto>( < DeviceDto > (device), "Device returned");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting device by id");
            return new ServiceResponse<DeviceDto>("Error getting device by id");
        }
    }

    public async Task<ServiceResponse<IEnumerable<DeviceDto>>> GetDevices()
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        try
        {
            var devices = await context.Devices.ToListAsync();
            if (devices.Count == 0)
            {
                return new ServiceResponse<IEnumerable<DeviceDto>>("Devices not found");
            }

            return new ServiceResponse<IEnumerable<DeviceDto>>( < IEnumerable < DeviceDto >> (devices),
            "Devices returned");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting devices");
            return new ServiceResponse<IEnumerable<DeviceDto>>("Error getting devices");
        }
    }

    public async Task<ServiceResponse<IEnumerable<DeviceDto>>> GetDevicesWithModels()
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        try
        {
            var devices = await context.Devices.Include(d => d.Models).ToListAsync();
            if (devices.Count == 0)
            {
                return new ServiceResponse<IEnumerable<DeviceDto>>("Devices not found");
            }

            return new ServiceResponse<IEnumerable<DeviceDto>>( < IEnumerable < DeviceDto >> (devices),
            "Devices returned");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting devices");
            return new ServiceResponse<IEnumerable<DeviceDto>>("Error getting devices");
        }
    }

    public async Task<ServiceResponse<ModelDto>> GetModelById(int id)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        try
        {
            var model = await context.Models.FirstOrDefaultAsync(m => m.ModelId == id);
            if (model == null)
            {
                _logger.LogWarning("Model not found");
                return new ServiceResponse<ModelDto>("Model not found");
            }

            _logger.LogInformation("Model with id {ModelId} retrieved", model.ModelId);
            return new ServiceResponse<ModelDto>( < ModelDto > (model),
            $"Model with id {model.ModelId} retrieved");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving model with id {ModelId}", id);
            return new ServiceResponse<ModelDto>($"Error retrieving model with id {id}");
        }
    }

    public async Task<ServiceResponse<IEnumerable<ModelDto>>> GetModels()
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        try
        {
            var models = await context.Models.ToListAsync();
            _logger.LogInformation("Models retrieved");
            return new ServiceResponse<IEnumerable<ModelDto>>( < IEnumerable < ModelDto >> (models),
            "Models retrieved");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving models");
            return new ServiceResponse<IEnumerable<ModelDto>>("Error retrieving models");
        }
    }

    public async Task<ServiceResponse<IEnumerable<ModelDto>>> GetModelsWithAssets()
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        try
        {
            var models = await context.Models.Include(m => m.Assets).ToListAsync();
            _logger.LogInformation("Models retrieved");
            return new ServiceResponse<IEnumerable<ModelDto>>( < IEnumerable < ModelDto >> (models),
            "Models retrieved");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving models");
            return new ServiceResponse<IEnumerable<ModelDto>>("Error retrieving models");
        }
    }

    public async Task<ServiceResponse<ParameterDto>> GetParameterById(int id)
    {
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

            var parameterDto = new ParameterDto
            {
                ParameterId = parameter.ParameterId,
                Name = parameter.Name,
                Description = parameter.Description
            };
            _logger.LogInformation("Parameter with id {ParameterId} retrieved", parameter.ParameterId);
            return new ServiceResponse<ParameterDto>(parameterDto, $"Parameter {parameter.ParameterId} retrieved");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving parameter with id {ParameterId}", id);
            return new ServiceResponse<ParameterDto>($"Error retrieving parameter with id {id}");
        }
    }

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

    public async Task<ServiceResponse<IEnumerable<ParameterDto>>> GetParametersWithModels()
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        try
        {
            var parameters = await _mapper.ProjectTo<ParameterDto>(context.Parameters.Include(p => p.ModelParameters))
                .ToListAsync();
            if (parameters.Count == 0)
            {
                _logger.LogWarning("No parameters found");
                return new ServiceResponse<IEnumerable<ParameterDto>>("No parameters found");
            }
            else
            {
                _logger.LogWarning("No parameters found");
                return new ServiceResponse<IEnumerable<ParameterDto>>("No parameters found");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving parameters with models");
            return new ServiceResponse<IEnumerable<ParameterDto>>("Error retrieving parameters with models");
        }
    }

    public async Task<ServiceResponse<PlantDto>> GetPlantById(int id)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        try
        {
            var plant = await _mapper.ProjectTo<PlantDto>(context.Plants).FirstOrDefaultAsync(p => p.PlantId == id);
            return plant == null
                ? new ServiceResponse<PlantDto>("Plant not found")
                : new ServiceResponse<PlantDto>(plant, "Plant returned");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting plant by id");
            return new ServiceResponse<PlantDto>("Error getting plant by id");
        }
    }

    public async Task<ServiceResponse<IEnumerable<PlantDto>>> GetPlants()
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        try
        {
            var plants = await _mapper.ProjectTo<PlantDto>(context.Plants).ToListAsync();
            return plants.Count == 0
                ? new ServiceResponse<IEnumerable<PlantDto>>("No plants found")
                : new ServiceResponse<IEnumerable<PlantDto>>(plants, "List of plants returned");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all plants");
            return new ServiceResponse<IEnumerable<PlantDto>>("Error getting all plants");
        }
    }

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

            return new ServiceResponse<IEnumerable<PlantDto>>(plants,
                "List of plants returned");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting plants with areas");
            return new ServiceResponse<IEnumerable<PlantDto>>("Error getting plants with areas");
        }
    }

    public async Task<ServiceResponse<QuestionDto>> GetQuestionById(int id)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        try
        {
            var question = await context.Questions.Where(q => q.QuestionId == id).FirstOrDefaultAsync();
            if (question == null)
            {
                _logger.LogWarning("QuestionId {id} not found", id);
                return new ServiceResponse<QuestionDto>($"QuestionId {id} not found");
            }

            var questionDto = new QuestionDto
            {
                QuestionId = question.QuestionId,
                Name = question.Name,
                CreatedAt = question.CreatedAt,
                UpdatedAt = question.UpdatedAt,
                IsDeleted = question.IsDeleted,
                UserId = question.UserId
            };

            _logger.LogInformation("QuestionId {id} found", id);
            return new ServiceResponse<QuestionDto>(questionDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting question");
            return new ServiceResponse<QuestionDto>($"Error getting question");
        }
    }

    public async Task<ServiceResponse<IEnumerable<QuestionDto>>> GetQuestions()
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        try
        {
            var questions = await context.Questions.Where(q => q.IsDeleted == false).ToListAsync();
            var questionsDto = new List<QuestionDto>();
            foreach (var question in questions)
            {
                var questionDto = new QuestionDto
                {
                    QuestionId = question.QuestionId,
                    Name = question.Name,
                    CreatedAt = question.CreatedAt,
                    UpdatedAt = question.UpdatedAt,
                    IsDeleted = question.IsDeleted,
                    UserId = question.UserId
                };
                questionsDto.Add(questionDto);
            }

            _logger.LogInformation("Questions found");
            return new ServiceResponse<IEnumerable<QuestionDto>>(questionsDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting questions");
            return new ServiceResponse<IEnumerable<QuestionDto>>($"Error getting questions");
        }
    }

    public async Task<ServiceResponse<IEnumerable<QuestionDto>>> GetQuestionsWithSituations()
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        try
        {
            var questions = await context.Questions.Where(q => q.IsDeleted == false).ToListAsync();
            var questionsDto = new List<QuestionDto>();
            foreach (var question in questions)
            {
                var questionDto = new QuestionDto
                {
                    QuestionId = question.QuestionId,
                    Name = question.Name,
                    CreatedAt = question.CreatedAt,
                    UpdatedAt = question.UpdatedAt,
                    IsDeleted = question.IsDeleted,
                    UserId = question.UserId
                };
                var questionSituations = await context.QuestionSituations
                    .Where(qs => qs.QuestionId == question.QuestionId).ToListAsync();
                var questionSituationsDto = new List<QuestionSituationDto>();
                foreach (var questionSituation in questionSituations)
                {
                    var questionSituationDto = new QuestionSituationDto
                    {
                        QuestionId = questionSituation.QuestionId,
                        SituationId = questionSituation.SituationId,
                        CreatedAt = questionSituation.CreatedAt,
                        UpdatedAt = questionSituation.UpdatedAt,
                        IsDeleted = questionSituation.IsDeleted
                    };
                    questionSituationsDto.Add(questionSituationDto);
                }

                questionDto.QuestionSituations = questionSituationsDto;
                questionsDto.Add(questionDto);
            }

            _logger.LogInformation("Questions found");
            return new ServiceResponse<IEnumerable<QuestionDto>>(questionsDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting questions");
            return new ServiceResponse<IEnumerable<QuestionDto>>($"Error getting questions");
        }
    }

    public async Task<ServiceResponse<SituationDto>> GetSituationById(int id)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        try
        {
            var situation = await context.Situations.Where(s => s.SituationId == id)
                .FirstOrDefaultAsync();
            if (situation == null)
            {
                _logger.LogWarning("SituationId {Id} not found", id);
                return new ServiceResponse<SituationDto>($"SituationId {id} not found");
            }

            var situationDto = < SituationDto > (situation);
            _logger.LogInformation("SituationId {Id} found", id);
            return new ServiceResponse<SituationDto>(situationDto, $"SituationId {id} found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting situation");
            return new ServiceResponse<SituationDto>("Error getting situation");
        }
    }

    public async Task<ServiceResponse<IEnumerable<SituationDto>>> GetSituations()
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        try
        {
            var situations = await context.Situations.Where(s => !s.IsDeleted).ToListAsync();
            var situationsDto = < IEnumerable < SituationDto >> (situations);
            _logger.LogInformation("{SituationCount} situations found", situations.Count);
            return new ServiceResponse<IEnumerable<SituationDto>>(situationsDto,
                $"{situations.Count} situations found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting situations");
            return new ServiceResponse<IEnumerable<SituationDto>>("Error getting situations");
        }
    }

    public async Task<ServiceResponse<IEnumerable<SituationDto>>> GetSituationsWithAssets()
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        try
        {
            var situations = await context.Situations.Where(s => !s.IsDeleted).ToListAsync();
            var situationsDto = < IEnumerable < SituationDto >> (situations);
            _logger.LogInformation("{SituationCount} situations found", situations.Count);
            return new ServiceResponse<IEnumerable<SituationDto>>(situationsDto,
                $"{situations.Count} situations found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting situations");
            return new ServiceResponse<IEnumerable<SituationDto>>("Error getting situations");
        }
    }

    public async Task<ServiceResponse<IEnumerable<SituationDto>>> GetSituationsWithCategories()
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        try
        {
            var situations = await context.Situations.Where(s => !s.IsDeleted).ToListAsync();
            var situationsDto = < IEnumerable < SituationDto >> (situations);
            _logger.LogInformation("{SituationCount} situations found", situations.Count);
            return new ServiceResponse<IEnumerable<SituationDto>>(situationsDto,
                $"{situations.Count} situations found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting situations");
            return new ServiceResponse<IEnumerable<SituationDto>>("Error getting situations");
        }
    }

    public async Task<ServiceResponse<SpaceDto>> GetSpaceById(int id)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        try
        {
            var space = await context.Spaces.FirstOrDefaultAsync(s => s.SpaceId == id);
            if (space == null)
            {
                return new ServiceResponse<SpaceDto>("Space not found");
            }

            return new ServiceResponse<SpaceDto>( < SpaceDto > (space), "Space returned");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting space by id");
            return new ServiceResponse<SpaceDto>("Error getting space by id");
        }
    }

    public async Task<ServiceResponse<IEnumerable<SpaceDto>>> GetSpaces()
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        try
        {
            var spaces = await context.Spaces.ToListAsync();
            if (spaces.Count == 0)
            {
                return new ServiceResponse<IEnumerable<SpaceDto>>("Spaces not found");
            }

            return new ServiceResponse<IEnumerable<SpaceDto>>( < IEnumerable < SpaceDto >> (spaces),
            "List of spaces returned");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all spaces");
            return new ServiceResponse<IEnumerable<SpaceDto>>("Error getting all spaces");
        }
    }

    public async Task<ServiceResponse<IEnumerable<SpaceDto>>> GetSpacesWithCoordinates()
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        try
        {
            var spaces = await context.Spaces.Include(c => c.Coordinates).ToListAsync();
            if (spaces.Count == 0)
            {
                return new ServiceResponse<IEnumerable<SpaceDto>>("Spaces not found");
            }

            return new ServiceResponse<IEnumerable<SpaceDto>>( < IEnumerable < SpaceDto >> (spaces),
            "List of spaces with coordinates returned");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting spaces with coordinates");
            return new ServiceResponse<IEnumerable<SpaceDto>>("Error getting spaces with coordinates");
        }
    }

    public async Task<ServiceResponse> MarkDeleteArea(int id, string userId)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            var area = await context.Areas.Include(s => s.Spaces).FirstOrDefaultAsync(a => a.AreaId == id);
            if (area == null)
            {
                return new ServiceResponse("Area not found");
            }

            if (area.Spaces.Count > 0)
            {
                return new ServiceResponse("Cannot delete area with spaces");
            }

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

    public async Task<ServiceResponse> MarkDeleteAsset(int id, string userId)
    {
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
                _logger.LogWarning("Asset marked as deleted");
                return new ServiceResponse("Asset marked as deleted");
            }

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

    public async Task<ServiceResponse> MarkDeleteCategory(int id, string userId)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            var category = await context.Categories.Include(c => c.AssetCategories)
                .FirstOrDefaultAsync(c => c.CategoryId == id);
            if (category == null)
            {
                return new ServiceResponse("Category not found");
            }

            if (category.IsDeleted)
            {
                return new ServiceResponse("Category already marked as deleted");
            }

            if (category.AssetCategories.Count > 0)
            {
                return new ServiceResponse("Category has assets assigned to it");
            }

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

    public async Task<ServiceResponse> MarkDeleteCommunicate(int id, string userId)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        try
        {
            var communicate = await context.Communicates.FirstOrDefaultAsync(c => c.CommunicateId == id);
            if (communicate == null)
            {
                _logger.LogError($"Communicate with id {id} not found");
                return new ServiceResponse($"Communicate with id {id} not found");
            }

            communicate.IsDeleted = true;
            communicate.UpdatedAt = DateTime.UtcNow;
            communicate.UpdatedBy = userId;
            await context.SaveChangesAsync();
            _logger.LogInformation("Communicate with id {id} marked as deleted");
            return new ServiceResponse();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking communicate with id {id} as deleted");
            return new ServiceResponse($"Error marking communicate with id {id} as deleted");
        }
    }

    public async Task<ServiceResponse> MarkDeleteCoordinate(int id, string userId)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            var coordinate = await context.Coordinates.Include(a => a.Assets)
                .FirstOrDefaultAsync(c => c.CoordinateId == id);
            if (coordinate == null)
            {
                return new ServiceResponse("Coordinate not found");
            }

            if (coordinate.Assets.Any())
            {
                return new ServiceResponse("Cannot delete coordinate with assets");
            }

            if (coordinate.IsDeleted)
            {
                return new ServiceResponse("Coordinate already marked as deleted");
            }

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

    public async Task<ServiceResponse> MarkDeleteDetail(int id, string userId)
    {
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


            detail.IsDeleted = true;
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

    public async Task<ServiceResponse> MarkDeleteDevice(int id, string userId)
    {
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

    public async Task<ServiceResponse> MarkDeleteModel(int id, string userId)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            var model = await context.Models.Include(m => m.Assets).FirstOrDefaultAsync(m => m.ModelId == id);
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

            if (model.Assets.Count > 0)
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

    public async Task<ServiceResponse> MarkDeleteParameter(int id, string userId)
    {
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
                _logger.LogWarning("Parameter with id {ParameterId} already marked as deleted", parameter.ParameterId);
                return new ServiceResponse($"Parameter with id {parameter.ParameterId} already deleted");
            }

            parameter.IsDeleted = true;
            parameter.UserId = userId;
            await context.SaveChangesAsync();
            await transaction.CommitAsync();
            _logger.LogInformation("Parameter with id {ParameterId} marked as deleted", parameter.ParameterId);
            return new ServiceResponse($"Parameter {parameter.ParameterId} marked as deleted");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking parameter with id {ParameterId} as deleted", id);
            await transaction.RollbackAsync();
            return new ServiceResponse($"Error marking parameter with id {id} as deleted");
        }
    }

    public async Task<ServiceResponse> MarkDeletePlant(int id, string userId)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            var plant = await context.Plants.Include(a => a.Areas).FirstOrDefaultAsync(p => p.PlantId == id);
            if (plant == null)
            {
                return new ServiceResponse("Plant not found");
            }

            if (plant.Areas.Count > 0)
            {
                return new ServiceResponse("Plant has areas, can't delete");
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

    public async Task<ServiceResponse> MarkDeleteQuestion(int id, string userId)
    {
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

            question.IsDeleted = true;
            question.;
            await context.SaveChangesAsync();
            _logger.LogInformation("Question deleted");
            return new ServiceResponse();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting question");
            return new ServiceResponse($"Error deleting question");
        }
    }

    public async Task<ServiceResponse> MarkDeleteSituation(int id, string userId)
    {
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

            situation.IsDeleted = true;
            situation.UpdatedAt = DateTime.UtcNow;

            await context.SaveChangesAsync();
            _logger.LogInformation("SituationId {id} deleted", id);
            return new ServiceResponse($"SituationId {id} deleted");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting situation");
            return new ServiceResponse($"Error deleting situation");
        }
    }

    public async Task<ServiceResponse> MarkDeleteSpace(int id, string userId)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            var space = await context.Spaces.Include(c => c.Coordinates).FirstOrDefaultAsync(s => s.SpaceId == id);
            if (space == null)
            {
                return new ServiceResponse("Space not found");
            }

            if (space.Coordinates.Count > 0)
            {
                return new ServiceResponse("Cannot delete space with coordinates");
            }

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

    public async Task<ServiceResponse> MarkUnDeleteArea(int id, string userId)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            var area = await context.Areas.FirstOrDefaultAsync(a => a.AreaId == id);
            if (area == null)
            {
                return new ServiceResponse("Area not found");
            }

            var exists = await context.Areas.FirstOrDefaultAsync(a =>
                a.AreaId != id && a.PlantId == area.PlantId && a.IsDeleted == false &&
                Equals(a.Name.ToLower().Trim(), area.Name.ToLower().Trim()));
            if (exists != null)
            {
                return new ServiceResponse($"Area with name {area.Name} already exists");
            }

            area.IsDeleted = false;
            area.UserId = userId;

            context.Areas.Update(area);

            await context.SaveChangesAsync();
            await transaction.CommitAsync();
            return new ServiceResponse("Area marked as undeleted", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking area as undeleted");
            await transaction.RollbackAsync();
            return new ServiceResponse("Error deleting area");
        }
    }

    public async Task<ServiceResponse> MarkUnDeleteAsset(int id, string userId)
    {
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

            if (!asset.IsDeleted)
            {
                _logger.LogWarning("Asset not marked as deleted");
                return new ServiceResponse("Asset not marked as deleted");
            }

            asset.IsDeleted = false;
            asset.UserId = userId;
            context.Assets.Update(asset);
            await context.SaveChangesAsync();
            await transaction.CommitAsync();
            _logger.LogInformation("Asset with id {AssetId} marked as not deleted", id);
            return new ServiceResponse($"Asset {id} marked as not deleted", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking asset with id {AssetId} as not deleted", id);
            await transaction.RollbackAsync();
            return new ServiceResponse($"Error marking asset with id {id} as not deleted");
        }
    }

    public async Task<ServiceResponse> MarkUnDeleteCategory(int id, string userId)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            var category = await context.Categories.FirstOrDefaultAsync(c => c.CategoryId == id);
            if (category == null)
            {
                return new ServiceResponse("Category not found");
            }

            if (category.IsDeleted == false)
            {
                return new ServiceResponse("Category already marked as not deleted");
            }

            if (await context.Categories.Where(c =>
                    Equals(category.Name.ToLower().Trim(), c.Name.ToLower().Trim()) &&
                    c.CategoryId != category.CategoryId && c.IsDeleted == false).AnyAsync())
            {
                return new ServiceResponse($"Category with name {category.Name} already exists");
            }

            category.IsDeleted = false;
            category.UserId = userId;
            context.Categories.Update(category);
            await context.SaveChangesAsync();
            await transaction.CommitAsync();
            return new ServiceResponse("Category marked as not deleted", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking category as not deleted");
            await transaction.RollbackAsync();
            return new ServiceResponse("Error marking category as not deleted");
        }
    }

    public async Task<ServiceResponse> MarkUnDeleteCommunicate(int id, string userId)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        try
        {
            var communicate = await context.Communicates.FirstOrDefaultAsync(c => c.CommunicateId == id);
            if (communicate == null)
            {
                _logger.LogError($"Communicate with id {id} not found");
                return new ServiceResponse($"Communicate with id {id} not found");
            }

            communicate.IsDeleted = false;
            communicate.UpdatedAt = DateTime.UtcNow;
            communicate.UpdatedBy = userId;
            await context.SaveChangesAsync();
            _logger.LogInformation("Communicate with id {id} marked as un-deleted");
            return new ServiceResponse();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking communicate with id {id} as un-deleted");
            return new ServiceResponse($"Error marking communicate with id {id} as un-deleted");
        }
    }

    public async Task<ServiceResponse> MarkUnDeleteCoordinate(int id, string userId)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            var coordinate = await context.Coordinates.FirstOrDefaultAsync(c => c.CoordinateId == id);
            if (coordinate == null)
            {
                return new ServiceResponse("Coordinate not found");
            }

            if (!coordinate.IsDeleted)
            {
                return new ServiceResponse("Coordinate already marked as not deleted");
            }

            var exists = await context.Coordinates.Include(a => a.Space).ThenInclude(a => a.Area).Where(c =>
                Equals(c.Name.ToLower().Trim(), coordinate.Name.ToLower().Trim()) && c.CoordinateId != id &&
                c.IsDeleted == false && c.Space.Area.PlantId == coordinate.Space.Area.PlantId).AnyAsync();
            if (exists)
            {
                return new ServiceResponse($"Coordinate with name {coordinate.Name} already exists");
            }

            coordinate.IsDeleted = false;
            coordinate.UserId = userId;
            context.Coordinates.Update(coordinate);
            await context.SaveChangesAsync();
            await transaction.CommitAsync();
            return new ServiceResponse("Coordinate un-deleted", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error un-deleting coordinate");
            await transaction.RollbackAsync();
            return new ServiceResponse("Error un-deleting coordinate");
        }
    }

    public async Task<ServiceResponse> MarkUnDeleteDetail(int id, string userId)
    {
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


            detail.IsDeleted = false;
            await context.SaveChangesAsync();
            await transaction.CommitAsync();
            _logger.LogInformation("Detail with id {DetailId} undeleted", detail.DetailId);
            return new ServiceResponse($"Detail {detail.DetailId} undeleted", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error undeleting detail");
            await transaction.RollbackAsync();
            return new ServiceResponse($"Error undeleting detail");
        }
    }

    public async Task<ServiceResponse> MarkUnDeleteDevice(int id, string userId)
    {
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

            if (!device.IsDeleted)
            {
                _logger.LogWarning("Device not marked as deleted");
                return new ServiceResponse("Device not marked as deleted");
            }

            device.UserId = userId;
            device.IsDeleted = false;
            context.Devices.Update(device);
            await context.SaveChangesAsync();
            await transaction.CommitAsync();
            _logger.LogInformation("Device with id {DeviceId} marked as undeleted", id);
            return new ServiceResponse($"Device {id} marked as undeleted", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking device as undeleted with id {DeviceId}", id);
            await transaction.RollbackAsync();
            return new ServiceResponse($"Error marking device as undeleted with id {id}");
        }
    }

    public async Task<ServiceResponse> MarkUnDeleteModel(int id, string userId)
    {
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

            if (!model.IsDeleted)
            {
                _logger.LogWarning("Model not marked as deleted");
                return new ServiceResponse("Model not marked as deleted");
            }

            model.IsDeleted = false;
            model.UserId = userId;
            context.Models.Update(model);
            await context.SaveChangesAsync();
            await transaction.CommitAsync();
            _logger.LogInformation("Model with id {ModelId} marked as undeleted", model.ModelId);
            return new ServiceResponse($"Model {model.ModelId} marked as undeleted", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking model with id {ModelId} as undeleted", id);
            await transaction.RollbackAsync();
            return new ServiceResponse($"Error marking model with id {id} as undeleted");
        }
    }

    public async Task<ServiceResponse> MarkUnDeleteParameter(int id, string userId)
    {
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

            if (!parameter.IsDeleted)
            {
                _logger.LogWarning("Parameter with id {ParameterId} already marked as not deleted",
                    parameter.ParameterId);
                return new ServiceResponse($"Parameter with id {parameter.ParameterId} already not deleted");
            }


            parameter.IsDeleted = false;
            parameter.UserId = userId;
            await context.SaveChangesAsync();
            await transaction.CommitAsync();
            _logger.LogInformation("Parameter with id {ParameterId} marked as not deleted", parameter.ParameterId);
            return new ServiceResponse($"Parameter {parameter.ParameterId} marked as not deleted");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking parameter with id {ParameterId} as not deleted", id);
            await transaction.RollbackAsync();
            return new ServiceResponse($"Error marking parameter with id {id} as not deleted");
        }
    }

    public async Task<ServiceResponse> MarkUnDeletePlant(int id, string userId)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            var plant = await context.Plants.FirstOrDefaultAsync(p => p.PlantId == id);
            if (plant == null)
            {
                return new ServiceResponse("Plant not found");
            }

            if (plant.IsDeleted == false)
            {
                return new ServiceResponse("Plant is not marked as deleted");
            }

            var exists = await context.Plants.Where(p =>
                    Equals(p.Name.ToLower().Trim(), plant.Name.ToLower().Trim()) && p.PlantId != id &&
                    p.IsDeleted == false)
                .ToListAsync();
            if (exists.Any())
            {
                return new ServiceResponse($"Plant with name {plant.Name} already exists");
            }

            plant.IsDeleted = false;
            plant.UserId = userId;
            context.Plants.Update(plant);

            await context.SaveChangesAsync();
            await transaction.CommitAsync();
            return new ServiceResponse("Plant marked as undeleted", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error un-deleting plant");
            await transaction.RollbackAsync();
            return new ServiceResponse("Error un-deleting plant");
        }
    }

    public async Task<ServiceResponse> MarkUnDeleteQuestion(int id, string userId)
    {
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

            question.IsDeleted = false;
            question.;
            await context.SaveChangesAsync();
            _logger.LogInformation("Question un-deleted");
            return new ServiceResponse();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error un-deleting question");
            return new ServiceResponse($"Error un-deleting question");
        }
    }

    public async Task<ServiceResponse> MarkUnDeleteSituation(int id, string userId)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        try
        {
            var situation = await context.Situations.Where(s => s.SituationId == id && s.IsDeleted)
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

            situation.IsDeleted = false;
            situation.UpdatedAt = DateTime.UtcNow;

            await context.SaveChangesAsync();
            _logger.LogInformation("SituationId {id} un-deleted", id);
            return new ServiceResponse($"SituationId {id} un-deleted");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error un-deleting situation");
            return new ServiceResponse($"Error un-deleting situation");
        }
    }

    public async Task<ServiceResponse> MarkUnDeleteSpace(int id, string userId)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            var space = await context.Spaces.FirstOrDefaultAsync(s => s.SpaceId == id);
            if (space == null)
            {
                return new ServiceResponse("Space not found");
            }

            if (!space.IsDeleted)
            {
                return new ServiceResponse("Space is not marked as deleted");
            }

            var exists = await context.Spaces.Where(s =>
                    s.IsDeleted == false && s.SpaceId != id &&
                    Equals(s.Name.ToLower().Trim(), space.Name.ToLower().Trim()))
                .ToListAsync();
            if (exists.Any())
            {
                return new ServiceResponse($"Space with name {space.Name} already exists");
            }

            space.IsDeleted = false;
            space.UserId = userId;
            context.Spaces.Update(space);
            await context.SaveChangesAsync();
            await transaction.CommitAsync();
            return new ServiceResponse("Space marked as undeleted", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking space as undeleted");
            await transaction.RollbackAsync();
            return new ServiceResponse("Error undeleting space");
        }
    }

    public async Task<ServiceResponse> UpdateArea(int id, string userId, AreaUpdateDto areaUpdateDto)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            var area = await context.Areas.FirstOrDefaultAsync(a => a.AreaId == id);
            if (area == null)
            {
                return new ServiceResponse("Area not found");
            }

            var exists = await context.Areas.FirstOrDefaultAsync(a =>
                a.AreaId != id && a.PlantId == area.PlantId && a.IsDeleted == false &&
                Equals(a.Name.ToLower().Trim(), areaUpdateDto.Name.ToLower().Trim()));
            if (exists != null)
            {
                return new ServiceResponse($"Area with name {areaUpdateDto.Name} already exists");
            }

            area.Description = areaUpdateDto.Description;
            area.Name = areaUpdateDto.Name;
            area.UserId = userId;
            context.Areas.Update(area);

            await context.SaveChangesAsync();
            await transaction.CommitAsync();
            return new ServiceResponse("Area updated", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating area");
            await transaction.RollbackAsync();
            return new ServiceResponse("Error updating area");
        }
    }

    public async Task<ServiceResponse> UpdateAsset(int id, string userId, AssetUpdateDto assetUpdateDto)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            var asset = await context.Assets.Include(a => a.AssetDetails).Include(a => a.AssetCategories)
                .FirstOrDefaultAsync(a => a.AssetId == id);
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

            // check if asset name is unique
            var exists = await context.Assets.Where(a =>
                Equals(a.Name.ToLower().Trim(), assetUpdateDto.Name.ToLower().Trim()) && a.AssetId != id &&
                a.IsDeleted == false).AnyAsync();
            if (exists)
            {
                _logger.LogWarning("Asset with name {AssetName} already exists", assetUpdateDto.Name);
                return new ServiceResponse($"Asset with name {assetUpdateDto.Name} already exists");
            }

            var coordinate =
                await context.Coordinates.FirstOrDefaultAsync(c => c.CoordinateId == assetUpdateDto.CoordinateId);
            if (coordinate == null || coordinate.IsDeleted)
            {
                _logger.LogWarning("Cannot update asset to coordinate with id {CoordinateId}",
                    assetUpdateDto.CoordinateId);
                return new ServiceResponse($"Cannot update asset to coordinate with id {assetUpdateDto.CoordinateId}");
            }

            asset.Status = assetUpdateDto.Status;
            asset.Name = assetUpdateDto.Name;
            asset.Process = assetUpdateDto.Process;
            asset.UserId = userId;
            asset.AssetDetails = asset.AssetDetails
                .Where(ad => assetUpdateDto.AssetDetails.Any(a => a.AssetDetailId == ad.AssetDetailId)).ToList();
            asset.AssetCategories = asset.AssetCategories.Where(ac =>
                assetUpdateDto.AssetCategories.Any(a => a.AssetCategoryId == ac.AssetCategoryId)).ToList();
            foreach (var assetDetailDto in assetUpdateDto.AssetDetails)
            {
                var assetDetailToUpdate =
                    asset.AssetDetails.FirstOrDefault(ad => ad.AssetDetailId == assetDetailDto.AssetDetailId);
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

                if (assetDetailToUpdate == null || Equals(assetDetailToUpdate.Value = assetDetailDto.Value)) continue;
                assetDetailToUpdate.Value = assetDetailDto.Value;
                assetDetailToUpdate.UserId = userId;
            }

            foreach (var newAssetCategory in from assetCategoryDto in assetUpdateDto.AssetCategories
                                             let assetCategoryToUpdate =
                                                 asset.AssetCategories.FirstOrDefault(ac =>
                                                     ac.AssetCategoryId == assetCategoryDto.AssetCategoryId)
                                             where assetCategoryToUpdate == null
                                             select new AssetCategory()
                                             {
                                                 AssetId = asset.AssetId,
                                                 CategoryId = assetCategoryDto.CategoryId,
                                                 UserId = userId
                                             })
            {
                asset.AssetCategories.Add(newAssetCategory);
            }

            context.Assets.Update(asset);
            await context.SaveChangesAsync();
            await transaction.CommitAsync();
            _logger.LogInformation("Asset with id {AssetId} updated", id);
            return new ServiceResponse($"Asset {id} updated", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating asset with id {AssetId}", id);
            await transaction.RollbackAsync();
            return new ServiceResponse($"Error updating asset with id {id}");
        }
    }

    public async Task<ServiceResponse> UpdateCategory(int id, string userId, CategoryUpdateDto categoryUpdateDto)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            var category = await context.Categories.FirstOrDefaultAsync(c => c.CategoryId == id);
            if (category == null)
            {
                return new ServiceResponse("Category not found");
            }

            var exists = await context.Categories.Where(c =>
                c.Name.ToLower().Trim() == categoryUpdateDto.Name.ToLower().Trim() && c.IsDeleted == false &&
                c.CategoryId != id).AnyAsync();
            if (exists)
            {
                return new ServiceResponse($"Category with name {categoryUpdateDto.Name} already exists");
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

    public async Task<ServiceResponse> UpdateCommunicate(int id, string userId,
        CommunicateUpdateDto communicateUpdateDto)
    {
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
            communicate.UpdatedAt = DateTime.UtcNow;
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
        await using var context = await _contextFactory.CreateDbContextAsync();
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            var coordinate = await context.Coordinates.FirstOrDefaultAsync(c => c.CoordinateId == id);
            if (coordinate == null)
            {
                return new ServiceResponse("Coordinate not found");
            }

            var exists = await context.Coordinates.Include(a => a.Space).ThenInclude(a => a.Area).Where(c =>
                Equals(c.Name.ToLower().Trim(), coordinateUpdateDto.Name.ToLower().Trim()) && c.CoordinateId != id &&
                c.IsDeleted == false && c.Space.Area.PlantId == coordinate.Space.Area.PlantId).AnyAsync();
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

            var exists = await context.Devices.Where(d =>
                d.DeviceId != id && Equals(d.Name.ToLower().Trim(), deviceUpdateDto.Name.ToLower().Trim()) &&
                !d.IsDeleted).AnyAsync();
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

            var exists = await context.Models.Where(m =>
                m.DeviceId == model.DeviceId && Equals(m.Name.ToLower().Trim(), modelUpdateDto.Name.ToLower().Trim()) &&
                !m.IsDeleted && m.ModelId != id).AnyAsync();
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
            situation.CategoryId = situationUpdateDto.CategoryId;
            situation.AssetId = situationUpdateDto.AssetId;
            situation.IsDeleted = situationUpdateDto.IsDeleted;
            situation.UpdatedAt = DateTime.UtcNow;

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
}