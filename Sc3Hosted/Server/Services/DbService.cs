using AutoMapper;

using Microsoft.EntityFrameworkCore;

using Sc3Hosted.Server.Entities;
using Sc3Hosted.Server.Repositories;
using Sc3Hosted.Shared.Dtos;
using Sc3Hosted.Shared.Helpers;

namespace Sc3Hosted.Server.Services;

public interface IDbService
{
    #region plant interface
    Task<ServiceResponse> CreatePlant(PlantCreateDto plantCreateDto, string userId);
    Task<ServiceResponse<PlantDto>> GetPlantById(int id);
    Task<ServiceResponse<IEnumerable<PlantDto>>> GetPlants();
    Task<ServiceResponse<IEnumerable<PlantDto>>> GetPlantsWithAreas();
    Task<ServiceResponse> UpdatePlant(int id, string userId, PlantUpdateDto plantUpdateDto);
    Task<ServiceResponse> MarkDeletePlant(int id, string userId);
    Task<ServiceResponse> DeletePlant(int id);
    #endregion
    # region area interface
    Task<ServiceResponse> CreateArea(int plantId, AreaCreateDto areaCreateDto, string userId);
    Task<ServiceResponse<AreaDto>> GetAreaById(int id);
    Task<ServiceResponse<IEnumerable<AreaDto>>> GetAreas();
    Task<ServiceResponse<IEnumerable<AreaDto>>> GetAreasWithSpaces();
    Task<ServiceResponse> UpdateArea(int id, string userId, AreaUpdateDto areaUpdateDto);
    Task<ServiceResponse> MarkDeleteArea(int id, string userId);
    Task<ServiceResponse> DeleteArea(int id);
    #endregion
    #region space interface
    Task<ServiceResponse> CreateSpace(int areaId, SpaceCreateDto spaceCreateDto, string userId);
    Task<ServiceResponse<SpaceDto>> GetSpaceById(int id);
    Task<ServiceResponse<IEnumerable<SpaceDto>>> GetSpaces();
    Task<ServiceResponse<IEnumerable<SpaceDto>>> GetSpacesWithCoordinates();
    Task<ServiceResponse> UpdateSpace(int id, string userId, SpaceUpdateDto spaceUpdateDto);
    Task<ServiceResponse> MarkDeleteSpace(int id, string userId);
    Task<ServiceResponse> DeleteSpace(int id);
    #endregion
    #region coordinate interface
    Task<ServiceResponse> CreateCoordinate(int spaceId, CoordinateCreateDto coordinateCreateDto, string userId);
    Task<ServiceResponse<CoordinateDto>> GetCoordinateById(int id);
    Task<ServiceResponse<IEnumerable<CoordinateDto>>> GetCoordinates();
    Task<ServiceResponse<IEnumerable<CoordinateDto>>> GetCoordinatesWithAssets();
    Task<ServiceResponse> UpdateCoordinate(int id, string userId, CoordinateUpdateDto coordinateUpdateDto);
    Task<ServiceResponse> MarkDeleteCoordinate(int id, string userId);
    Task<ServiceResponse> DeleteCoordinate(int id);
    #endregion
    #region asset interface
    Task<ServiceResponse> CreateAsset(AssetCreateDto assetCreateDto, string userId);
    Task<ServiceResponse<AssetDto>> GetAssetById(int id);
    Task<ServiceResponse<IEnumerable<AssetDto>>> GetAssets();
    Task<ServiceResponse<IEnumerable<AssetDto>>> GetAssetsWithAllData();
    Task<ServiceResponse> UpdateAsset(int id, string userId, AssetUpdateDto assetUpdateDto);
    Task<ServiceResponse> MarkDeleteAsset(int id, string userId);
    Task<ServiceResponse> DeleteAsset(int id);
    #endregion
    #region device interface
    Task<ServiceResponse> CreateDevice(DeviceCreateDto deviceCreateDto, string userId);
    Task<ServiceResponse<DeviceDto>> GetDeviceById(int id);
    Task<ServiceResponse<IEnumerable<DeviceDto>>> GetDevices();
    Task<ServiceResponse<IEnumerable<DeviceDto>>> GetDevicesWithModels();
    Task<ServiceResponse> UpdateDevice(int id, string userId, DeviceUpdateDto deviceUpdateDto);
    Task<ServiceResponse> MarkDeleteDevice(int id, string userId);
    Task<ServiceResponse> DeleteDevice(int id);
    #endregion
    #region model interface
    Task<ServiceResponse> CreateModel(int deviceId, ModelCreateDto modelCreateDto, string userId);
    Task<ServiceResponse<ModelDto>> GetModelById(int id);
    Task<ServiceResponse<IEnumerable<ModelDto>>> GetModels();
    Task<ServiceResponse<IEnumerable<ModelDto>>> GetModelsWithAssets();
    Task<ServiceResponse> UpdateModel(int id, string userId, ModelUpdateDto modelUpdateDto);
    Task<ServiceResponse> MarkDeleteModel(int id, string userId);
    Task<ServiceResponse> DeleteModel(int id);
    #endregion
    #region situation interface
    Task<ServiceResponse> CreateSituation(SituationCreateDto situationCreateDto, string userId);
    Task<ServiceResponse<SituationDto>> GetSituationById(int id);
    Task<ServiceResponse<IEnumerable<SituationDto>>> GetSituations();
    Task<ServiceResponse<IEnumerable<SituationDto>>> GetSituationsWithAssets();
    Task<ServiceResponse<IEnumerable<SituationDto>>> GetSituationsWithCategories();
    Task<ServiceResponse> UpdateSituation(int id, string userId, SituationUpdateDto situationUpdateDto);
    Task<ServiceResponse> MarkDeleteSituation(int id, string userId);
    Task<ServiceResponse> DeleteSituation(int id);
    #endregion
    #region category interface
    Task<ServiceResponse> CreateCategory(CategoryCreateDto categoryCreateDto, string userId);
    Task<ServiceResponse<CategoryDto>> GetCategoryById(int id);
    Task<ServiceResponse<IEnumerable<CategoryDto>>> GetCategories();
    Task<ServiceResponse<IEnumerable<CategoryDto>>> GetCategoriesWithAssets();
    Task<ServiceResponse> UpdateCategory(int id, string userId, CategoryUpdateDto categoryUpdateDto);
    Task<ServiceResponse> MarkDeleteCategory(int id, string userId);
    Task<ServiceResponse> DeleteCategory(int id);
    #endregion
    #region communicate interface
    Task<ServiceResponse> CreateCommunicate(CommunicateCreateDto communicateCreateDto, string userId);
    Task<ServiceResponse<CommunicateDto>> GetCommunicateById(int id);
    Task<ServiceResponse<IEnumerable<CommunicateDto>>> GetCommunicates();
    Task<ServiceResponse<IEnumerable<CommunicateDto>>> GetCommunicatesWithAssets();
    Task<ServiceResponse> UpdateCommunicate(int id, string userId, CommunicateUpdateDto communicateUpdateDto);
    Task<ServiceResponse> MarkDeleteCommunicate(int id, string userId);
    Task<ServiceResponse> DeleteCommunicate(int id);
    #endregion
    #region detail interface
    Task<ServiceResponse> CreateDetail(DetailCreateDto detailCreateDto, string userId);
    Task<ServiceResponse<DetailDto>> GetDetailById(int id);
    Task<ServiceResponse<IEnumerable<DetailDto>>> GetDetails();
    Task<ServiceResponse<IEnumerable<DetailDto>>> GetDetailsWithAssets();
    Task<ServiceResponse> UpdateDetail(int id, string userId, DetailUpdateDto detailUpdateDto);
    Task<ServiceResponse> MarkDeleteDetail(int id, string userId);
    Task<ServiceResponse> DeleteDetail(int id);
    #endregion
    #region parameter interface
    Task<ServiceResponse> CreateParameter(ParameterCreateDto parameterCreateDto, string userId);
    Task<ServiceResponse<ParameterDto>> GetParameterById(int id);
    Task<ServiceResponse<IEnumerable<ParameterDto>>> GetParameters();
    Task<ServiceResponse<IEnumerable<ParameterDto>>> GetParametersWithModels();
    Task<ServiceResponse> UpdateParameter(int id, string userId, ParameterUpdateDto parameterUpdateDto);
    Task<ServiceResponse> MarkDeleteParameter(int id, string userId);
    Task<ServiceResponse> DeleteParameter(int id);
    #endregion
    #region question interface
    Task<ServiceResponse> CreateQuestion(QuestionCreateDto questionCreateDto, string userId);
    Task<ServiceResponse<QuestionDto>> GetQuestionById(int id);
    Task<ServiceResponse<IEnumerable<QuestionDto>>> GetQuestions();
    Task<ServiceResponse<IEnumerable<QuestionDto>>> GetQuestionsWithSituations();
    Task<ServiceResponse> UpdateQuestion(int id, string userId, QuestionUpdateDto questionUpdateDto);
    Task<ServiceResponse> MarkDeleteQuestion(int id, string userId);
    Task<ServiceResponse> DeleteQuestion(int id);
    #endregion


}


public class DbService : IDbService
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DbService> _logger;

    public DbService(IMapper mapper, IUnitOfWork unitOfWork, ILogger<DbService> logger)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }


    #region plant service
    public async Task<ServiceResponse<IEnumerable<PlantDto>>> GetPlants()
    {
        try
        {
            var plants = await _unitOfWork.Plants.Get();
            if (plants == null)
            {
                return new( "Plants not found", false);
            }
            return new(_mapper.Map<IEnumerable<PlantDto>>(plants), "List of plants returned", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all plants");
            return new("Error getting all plants", false);
        }
    }
    public async Task<ServiceResponse<PlantDto>> GetPlantById(int id)
    {
        try
        {
            var plant = await _unitOfWork.Plants.GetById(id);
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

    public async Task<ServiceResponse> UpdatePlant(int id, string userId, PlantUpdateDto plantUpdateDto)
    {
        try
        {
            var plant = await _unitOfWork.Plants.GetById(id);
            if (plant == null)
            {
                return new("Plant not found", false);
            }
            plant.Description = plantUpdateDto.Description;
            plant.Name = plantUpdateDto.Name;
            plant.UserId = userId;
            await _unitOfWork.SaveChangesAsync();
            return new("Plant updated", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating plant");
            return new("Error updating plant", false);
        }
    }
    public async Task<ServiceResponse> MarkDeletePlant(int id, string userId)
    {
        try
        {
            var plant = await _unitOfWork.Plants.GetById(id);
            if (plant == null)
            {
                return new("Plant not found", false);
            }
          plant = await  _unitOfWork.Plants.GetOne(p=>p.PlantId==plant.PlantId, p=>p.Include(a=>a.Areas));
            if (plant.Areas.Count > 0)
            {
                return new("Plant has areas, can't delete", false);
            }
            plant.IsDeleted = true;
            plant.UserId = userId;
            await _unitOfWork.SaveChangesAsync();
            return new("Plant marked as deleted", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting plant");
            return new("Error deleting plant", false);
        }
    }

    public async Task<ServiceResponse> DeletePlant(int id)
    {
        try
        {
            var plant = await _unitOfWork.Plants.GetById(id);
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
            await _unitOfWork.Plants.Delete(plant);
            await _unitOfWork.SaveChangesAsync();
            return new("Plant marked as deleted", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting plant");
            return new("Error deleting plant", false);
        }
    }
    public async Task<ServiceResponse> CreatePlant(PlantCreateDto plantCreateDto, string userId)
    {
        try
        {
            var plant = _mapper.Map<Plant>(plantCreateDto);
            var exist = await _unitOfWork.Plants.GetOne(p => p.Name.ToLower().Trim() == plant.Name.ToLower().Trim());
            if (exist != null && exist.IsDeleted == false)
            {
                return new("Plant already exists", false);
            }
            plant.UserId = userId;
            var result = await _unitOfWork.Plants.Create(plant);
            if (result)
            {
                await _unitOfWork.SaveChangesAsync();
                return new("Plant created", true);
            }
            return new("Plant not created", false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating plant");
            return new("Error creating plant", false);
        }
    }

    public async Task<ServiceResponse<IEnumerable<PlantDto>>> GetPlantsWithAreas()
    {
        try
        {
            var plants = await _unitOfWork.Plants.Get(include: p => p.Include(a => a.Areas));
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
    #endregion
    #region area service
    public async Task<ServiceResponse> CreateArea(int plantId, AreaCreateDto areaCreateDto, string userId)
    {
        var plant = await _unitOfWork.Plants.GetOne(p => p.PlantId == plantId, p => p.Include(a => a.Areas));
        if (plant == null || plant.IsDeleted)
        {
            _logger.LogWarning("Cannot create area for plant with id {plantId}", plantId);
            return new($"Cannot create area for plant with id {plantId}", false);
        }
        if (plant.Areas.Any(a => !a.IsDeleted&&a.Name.ToLower().Trim() == areaCreateDto.Name.ToLower().Trim()))
        {
            _logger.LogWarning("Area with name {areaName} already exists", areaCreateDto.Name);
            return new($"Area with name {areaCreateDto.Name} already exists", false);
        }
        try
        {
            var area = _mapper.Map<Area>(areaCreateDto);
            area.UserId = userId;
            var result = await _unitOfWork.Areas.Create(area);
            if (result)
            {
                await _unitOfWork.SaveChangesAsync();
                return new("Area created", true);
            }
            return new("Area not created", false);
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
            var area = await _unitOfWork.Areas.GetById(id);
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
            var areas = await _unitOfWork.Areas.Get();
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
            var areas = await _unitOfWork.Areas.Get(include: a => a.Include(s => s.Spaces));
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
            var area = await _unitOfWork.Areas.GetById(id);
            if (area == null)
            {
                return new("Area not found", false);
            }
            area.Description = areaUpdateDto.Description;
            area.Name = areaUpdateDto.Name;
            area.UserId = userId;
            await _unitOfWork.SaveChangesAsync();
            return new("Area updated", true);
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
            var area = await _unitOfWork.Areas.GetOne(a => a.AreaId == id, a=>a.Include(s => s.Spaces));
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
            await _unitOfWork.SaveChangesAsync();
            return new("Area marked as deleted", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking area as deleted");
            return new("Error deleting area", false);
        }
    }

    public async Task<ServiceResponse> DeleteArea(int id)
    {
        try
        {
            var area = await _unitOfWork.Areas.GetById(id);
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
            await _unitOfWork.Areas.Delete(area);
            await _unitOfWork.SaveChangesAsync();
            return new("Area deleted", true);
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
        var area = await _unitOfWork.Areas.GetOne(a => a.AreaId == areaId, a => a.Include(s => s.Spaces));
        if (area == null)
        {
            _logger.LogWarning("Cannot create space for area with id {areaId}", areaId);
            return new($"Cannot create space for area with id {areaId}", false);
        }
        if (area.Spaces.Any(s => !s.IsDeleted && s.Name.ToLower().Trim() == spaceCreateDto.Name.ToLower().Trim()))
        {
            _logger.LogWarning("Space with name {spaceName} already exists", spaceCreateDto.Name);
            return new($"Space with name {spaceCreateDto.Name} already exists", false);
        }
        try
        {
            var space = _mapper.Map<Space>(spaceCreateDto);            
            space.UserId = userId;
            var result = await _unitOfWork.Spaces.Create(space);
            if (result)
            {
                await _unitOfWork.SaveChangesAsync();
                return new("Space created", true);
            }
            return new("Space not created", false);
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
            var space = await _unitOfWork.Spaces.GetById(id);
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
            var spaces = await _unitOfWork.Spaces.Get();
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
            var spaces = await _unitOfWork.Spaces.Get(include: s => s.Include(c => c.Coordinates));
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
            var space = await _unitOfWork.Spaces.GetById(id);
            if (space == null)
            {
                return new("Space not found", false);
            }
            space.Description = spaceUpdateDto.Description;
            space.Name = spaceUpdateDto.Name;
            space.UserId = userId;
            await _unitOfWork.SaveChangesAsync();
            return new("Space updated", true);
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
            var space = await _unitOfWork.Spaces.GetOne(s => s.SpaceId == id, s => s.Include(c => c.Coordinates));
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
            await _unitOfWork.SaveChangesAsync();
            return new("Space marked as deleted", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking space as deleted");
            return new("Error deleting space", false);
        }
    }

    public async Task<ServiceResponse> DeleteSpace(int id)
    {
        try
        {
            var space = await _unitOfWork.Spaces.GetById(id);
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
            await _unitOfWork.Spaces.Delete(space);
            await _unitOfWork.SaveChangesAsync();
            return new("Space deleted", true);
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
        var space = await _unitOfWork.Spaces.GetOne(s => s.SpaceId == spaceId, s => s.Include(c => c.Coordinates));
        if (space == null)
        {
            _logger.LogWarning("Cannot create coordinate for space with id {spaceId}", spaceId);
            return new($"Cannot create coordinate for space with id {spaceId}", false);
        }
        if (space.Coordinates.Any(c => !c.IsDeleted && c.Name.ToLower().Trim() == coordinateCreateDto.Name.ToLower().Trim()))
        {
            _logger.LogWarning("Coordinate with name {coordinateName} already exists", coordinateCreateDto.Name);
            return new($"Coordinate with name {coordinateCreateDto.Name} already exists", false);
        }

        try
        {
            var coordinate = _mapper.Map<Coordinate>(coordinateCreateDto);
            coordinate.UserId = userId;
            var result = await _unitOfWork.Coordinates.Create(coordinate);
            if (result)
            {
                await _unitOfWork.SaveChangesAsync();
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
        throw new NotImplementedException();
    }

    public async Task<ServiceResponse<IEnumerable<CoordinateDto>>> GetCoordinates()
    {
        throw new NotImplementedException();
    }

    public async Task<ServiceResponse<IEnumerable<CoordinateDto>>> GetCoordinatesWithAssets()
    {
        throw new NotImplementedException();
    }

    public async Task<ServiceResponse> UpdateCoordinate(int id, string userId, CoordinateUpdateDto coordinateUpdateDto)
    {
        throw new NotImplementedException();
    }

    public async Task<ServiceResponse> MarkDeleteCoordinate(int id, string userId)
    {
        throw new NotImplementedException();
    }

    public async Task<ServiceResponse> DeleteCoordinate(int id)
    {
        throw new NotImplementedException();
    }
    #endregion
    #region asset service
    public async Task<ServiceResponse> CreateAsset(AssetCreateDto assetCreateDto, string userId)
    {
        throw new NotImplementedException();
    }

    public async Task<ServiceResponse<AssetDto>> GetAssetById(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<ServiceResponse<IEnumerable<AssetDto>>> GetAssets()
    {
        throw new NotImplementedException();
    }

    public async Task<ServiceResponse<IEnumerable<AssetDto>>> GetAssetsWithAllData()
    {
        throw new NotImplementedException();
    }

    public async Task<ServiceResponse> UpdateAsset(int id, string userId, AssetUpdateDto assetUpdateDto)
    {
        throw new NotImplementedException();
    }

    public async Task<ServiceResponse> MarkDeleteAsset(int id, string userId)
    {
        throw new NotImplementedException();
    }

    public async Task<ServiceResponse> DeleteAsset(int id)
    {
        throw new NotImplementedException();
    }
    #endregion
    #region device service
    public async Task<ServiceResponse> CreateDevice(DeviceCreateDto deviceCreateDto, string userId)
    {
        throw new NotImplementedException();
    }

    public async Task<ServiceResponse<DeviceDto>> GetDeviceById(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<ServiceResponse<IEnumerable<DeviceDto>>> GetDevices()
    {
        throw new NotImplementedException();
    }

    public async Task<ServiceResponse<IEnumerable<DeviceDto>>> GetDevicesWithModels()
    {
        throw new NotImplementedException();
    }

    public async Task<ServiceResponse> UpdateDevice(int id, string userId, DeviceUpdateDto deviceUpdateDto)
    {
        throw new NotImplementedException();
    }

    public async Task<ServiceResponse> MarkDeleteDevice(int id, string userId)
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

    public async Task<ServiceResponse> DeleteQuestion(int id)
    {
        throw new NotImplementedException();
    }
    #endregion
}