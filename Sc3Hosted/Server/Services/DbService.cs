using AutoMapper;

using Sc3Hosted.Server.Entities;
using Sc3Hosted.Server.Repositories;
using Sc3Hosted.Shared.Dtos;

namespace Sc3Hosted.Server.Services;

#region IDbService interface
public interface IDbService
{
    #region plant interface
    Task<bool> CreatePlant(PlantCreateDto plantCreateDto, string userId);
    Task<PlantDto> GetPlantById(int id);
    Task<IEnumerable<PlantDto>> GetPlants();
    Task<IEnumerable<PlantDto>> GetPlantsWithAreas();
    Task<bool> UpdatePlant(int id, string userId, PlantUpdateDto plantUpdateDto);
    Task<bool> MarkDeletePlant(int id, string userId);
    Task<bool> DeletePlant(int id);
    #endregion
    # region area interface
    Task<bool> CreateArea(AreaCreateDto areaCreateDto, string userId);
    Task<AreaDto> GetAreaById(int id);
    Task<IEnumerable<AreaDto>> GetAreas();
    Task<IEnumerable<AreaDto>> GetAreasWithSpaces();
    Task<bool> UpdateArea(int id, string userId, AreaUpdateDto areaUpdateDto);
    Task<bool> MarkDeleteArea(int id, string userId);
    Task<bool> DeleteArea(int id);
    #endregion
    #region space interface
    Task<bool> CreateSpace(SpaceCreateDto spaceCreateDto, string userId);
    Task<SpaceDto> GetSpaceById(int id);
    Task<IEnumerable<SpaceDto>> GetSpaces();
    Task<IEnumerable<SpaceDto>> GetSpacesWithCoordinates();
    Task<bool> UpdateSpace(int id, string userId, SpaceUpdateDto spaceUpdateDto);
    Task<bool> MarkDeleteSpace(int id, string userId);
    Task<bool> DeleteSpace(int id);
    #endregion
    #region coordinate interface
    Task<bool> CreateCoordinate(CoordinateCreateDto coordinateCreateDto, string userId);
    Task<CoordinateDto> GetCoordinateById(int id);
    Task<IEnumerable<CoordinateDto>> GetCoordinates();
    Task<IEnumerable<CoordinateDto>> GetCoordinatesWithAssets();
    Task<bool> UpdateCoordinate(int id, string userId, CoordinateUpdateDto coordinateUpdateDto);
    Task<bool> MarkDeleteCoordinate(int id, string userId);
    Task<bool> DeleteCoordinate(int id);
    #endregion
    #region asset interface
    Task<bool> CreateAsset(AssetCreateDto assetCreateDto, string userId);
    Task<AssetDto> GetAssetById(int id);
    Task<IEnumerable<AssetDto>> GetAssets();
    Task<IEnumerable<AssetDto>> GetAssetsWithAllData();
    Task<bool> UpdateAsset(int id, string userId, AssetUpdateDto assetUpdateDto);
    Task<bool> MarkDeleteAsset(int id, string userId);
    Task<bool> DeleteAsset(int id);
    #endregion
    #region device interface
    Task<bool> CreateDevice(DeviceCreateDto deviceCreateDto, string userId);
    Task<DeviceDto> GetDeviceById(int id);
    Task<IEnumerable<DeviceDto>> GetDevices();
    Task<IEnumerable<DeviceDto>> GetDevicesWithModels();
    Task<bool> UpdateDevice(int id, string userId, DeviceUpdateDto deviceUpdateDto);
    Task<bool> MarkDeleteDevice(int id, string userId);
    Task<bool> DeleteDevice(int id);
    #endregion
    #region model interface
    Task<bool> CreateModel(ModelCreateDto modelCreateDto, string userId);
    Task<ModelDto> GetModelById(int id);
    Task<IEnumerable<ModelDto>> GetModels();
    Task<IEnumerable<ModelDto>> GetModelsWithAssets();
    Task<bool> UpdateModel(int id, string userId, ModelUpdateDto modelUpdateDto);
    Task<bool> MarkDeleteModel(int id, string userId);
    Task<bool> DeleteModel(int id);
    #endregion
    #region situation interface
    Task<bool> CreateSituation(SituationCreateDto situationCreateDto, string userId);
    Task<SituationDto> GetSituationById(int id);
    Task<IEnumerable<SituationDto>> GetSituations();
    Task<IEnumerable<SituationDto>> GetSituationsWithAssets();
    Task<IEnumerable<SituationDto>> GetSituationsWithCategories();
    Task<bool> UpdateSituation(int id, string userId, SituationUpdateDto situationUpdateDto);
    Task<bool> MarkDeleteSituation(int id, string userId);
    Task<bool> DeleteSituation(int id);
    #endregion
    #region category interface
    Task<bool> CreateCategory(CategoryCreateDto categoryCreateDto, string userId);
    Task<CategoryDto> GetCategoryById(int id);
    Task<IEnumerable<CategoryDto>> GetCategories();
    Task<IEnumerable<CategoryDto>> GetCategoriesWithAssets();
    Task<bool> UpdateCategory(int id, string userId, CategoryUpdateDto categoryUpdateDto);
    Task<bool> MarkDeleteCategory(int id, string userId);
    Task<bool> DeleteCategory(int id);
    #endregion
    #region communicate interface
    Task<bool> CreateCommunicate(CommunicateCreateDto communicateCreateDto, string userId);
    Task<CommunicateDto> GetCommunicateById(int id);
    Task<IEnumerable<CommunicateDto>> GetCommunicates();
    Task<IEnumerable<CommunicateDto>> GetCommunicatesWithAssets();
    Task<bool> UpdateCommunicate(int id, string userId, CommunicateUpdateDto communicateUpdateDto);
    Task<bool> MarkDeleteCommunicate(int id, string userId);
    Task<bool> DeleteCommunicate(int id);
    #endregion
    #region detail interface
    Task<bool> CreateDetail(DetailCreateDto detailCreateDto, string userId);
    Task<DetailDto> GetDetailById(int id);
    Task<IEnumerable<DetailDto>> GetDetails();
    Task<IEnumerable<DetailDto>> GetDetailsWithAssets();
    Task<bool> UpdateDetail(int id, string userId, DetailUpdateDto detailUpdateDto);
    Task<bool> MarkDeleteDetail(int id, string userId);
    Task<bool> DeleteDetail(int id);
    #endregion
    #region parameter interface
    Task<bool> CreateParameter(ParameterCreateDto parameterCreateDto, string userId);
    Task<ParameterDto> GetParameterById(int id);
    Task<IEnumerable<ParameterDto>> GetParameters();
    Task<IEnumerable<ParameterDto>> GetParametersWithModels();
    Task<bool> UpdateParameter(int id, string userId, ParameterUpdateDto parameterUpdateDto);
    Task<bool> MarkDeleteParameter(int id, string userId);
    Task<bool> DeleteParameter(int id);
    #endregion
    #region question interface
    Task<bool> CreateQuestion(QuestionCreateDto questionCreateDto, string userId);
    Task<QuestionDto> GetQuestionById(int id);
    Task<IEnumerable<QuestionDto>> GetQuestions();
    Task<IEnumerable<QuestionDto>> GetQuestionsWithSituations();
    Task<bool> UpdateQuestion(int id, string userId, QuestionUpdateDto questionUpdateDto);
    Task<bool> MarkDeleteQuestion(int id, string userId);
    Task<bool> DeleteQuestion(int id);
    #endregion
    

}
#endregion

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


    #region plants
    public async Task<IEnumerable<PlantDto>> GetPlants()
    {
        try
        {
            var plants = await _unitOfWork.Plants.Get();
            if (plants == null)
            {
                return null!;
            }
            return _mapper.Map<IEnumerable<PlantDto>>(plants);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all plants");
            return null!;
        }
    }
    public async Task<PlantDto> GetPlantById(int id)
    {
        try
        {
            var plant = await _unitOfWork.Plants.GetById(id);
            if (plant == null)
            {
                return null!;
            }
            return _mapper.Map<PlantDto>(plant);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting plant by id");
            return null!;
        }
    }

    public async Task<bool> UpdatePlant(int id, string userId, PlantUpdateDto plantUpdateDto)
    {
        try
        {
            var plant = await _unitOfWork.Plants.GetById(id);
            if (plant == null)
            {
                return false;
            }
            _mapper.Map(plantUpdateDto, plant);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating plant");
            return false;
        }
    }
    public async Task<bool> MarkDeletePlant(int id, string userId)
    {
        try
        {
            var plant = await _unitOfWork.Plants.GetById(id);
            if (plant == null)
            {
                return false;
            }
            plant.IsDeleted = true;
            plant.UserId = userId;
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting plant");
            return false;
        }
    }

    public async Task<bool> DeletePlant(int id)
    {
        try
        {
            var plant = await _unitOfWork.Plants.GetById(id);
            if (plant == null)
            {
                _logger.LogError("Plant not found");
                return false;
            }
            if (plant.IsDeleted == false)
            {
                _logger.LogError("Plant not marked as deleted");
                return false;
            }
            await _unitOfWork.Plants.Delete(plant);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting plant");
            return false;
        }
    }
    public async Task<bool> CreatePlant(PlantCreateDto plantCreateDto, string userId)
    {
        try
        {
            var plant = _mapper.Map<Plant>(plantCreateDto);
var exist = await _unitOfWork.Plants.GetOne(p=>p.Name.ToLower().Trim() == plant.Name.ToLower().Trim());
if(exist != null&& exist.IsDeleted == false)
return false;
            plant.UserId = userId;
            var result =await _unitOfWork.Plants.Create(plant);
            if (result)
            {
            await _unitOfWork.SaveChangesAsync();
            return true;
            }
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating plant");
            return false;
        }
    }
    #endregion plants

    #region areas

    #endregion

}