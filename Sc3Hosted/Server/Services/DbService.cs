using AutoMapper;

using Microsoft.EntityFrameworkCore;

using Sc3Hosted.Server.Entities;
using Sc3Hosted.Server.Repositories;
using Sc3Hosted.Shared.Dtos;

namespace Sc3Hosted.Server.Services;

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

    public async Task<IEnumerable<PlantDto>> GetPlantsWithAreas()
    {
       try
         {
              var plants = await _unitOfWork.Plants.Get( include:p => p.Include(a => a.Areas));
              if (plants == null)
              {
                return null!;
              }
              return _mapper.Map<IEnumerable<PlantDto>>(plants);
         }
         catch (Exception ex)
         {
              _logger.LogError(ex, "Error getting plants with areas");
              return null!;
         }
    }
#endregion
#region area service
    public async Task<bool> CreateArea(AreaCreateDto areaCreateDto, string userId)
    {
        try
        {
            var area = _mapper.Map<Area>(areaCreateDto);
            area.UserId = userId;
            var result = await _unitOfWork.Areas.Create(area);
            if (result)
            {
                await _unitOfWork.SaveChangesAsync();
              return  true;
            }
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating area");
            return false;
        }
    }

    public async Task<AreaDto> GetAreaById(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<AreaDto>> GetAreas()
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<AreaDto>> GetAreasWithSpaces()
    {
        throw new NotImplementedException();
    }

    public async Task<bool> UpdateArea(int id, string userId, AreaUpdateDto areaUpdateDto)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> MarkDeleteArea(int id, string userId)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> DeleteArea(int id)
    {
        throw new NotImplementedException();
    }
#endregion
#region space service
    public async Task<bool> CreateSpace(SpaceCreateDto spaceCreateDto, string userId)
    {
        throw new NotImplementedException();
    }

    public async Task<SpaceDto> GetSpaceById(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<SpaceDto>> GetSpaces()
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<SpaceDto>> GetSpacesWithCoordinates()
    {
        throw new NotImplementedException();
    }

    public async Task<bool> UpdateSpace(int id, string userId, SpaceUpdateDto spaceUpdateDto)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> MarkDeleteSpace(int id, string userId)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> DeleteSpace(int id)
    {
        throw new NotImplementedException();
    }
#endregion
#region coordinate service
    public async Task<bool> CreateCoordinate(CoordinateCreateDto coordinateCreateDto, string userId)
    {
        throw new NotImplementedException();
    }

    public async Task<CoordinateDto> GetCoordinateById(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<CoordinateDto>> GetCoordinates()
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<CoordinateDto>> GetCoordinatesWithAssets()
    {
        throw new NotImplementedException();
    }

    public async Task<bool> UpdateCoordinate(int id, string userId, CoordinateUpdateDto coordinateUpdateDto)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> MarkDeleteCoordinate(int id, string userId)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> DeleteCoordinate(int id)
    {
        throw new NotImplementedException();
    }
#endregion
#region asset service
    public async Task<bool> CreateAsset(AssetCreateDto assetCreateDto, string userId)
    {
        throw new NotImplementedException();
    }

    public async Task<AssetDto> GetAssetById(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<AssetDto>> GetAssets()
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<AssetDto>> GetAssetsWithAllData()
    {
        throw new NotImplementedException();
    }

    public async Task<bool> UpdateAsset(int id, string userId, AssetUpdateDto assetUpdateDto)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> MarkDeleteAsset(int id, string userId)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> DeleteAsset(int id)
    {
        throw new NotImplementedException();
    }
#endregion
#region device service
    public async Task<bool> CreateDevice(DeviceCreateDto deviceCreateDto, string userId)
    {
        throw new NotImplementedException();
    }

    public async Task<DeviceDto> GetDeviceById(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<DeviceDto>> GetDevices()
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<DeviceDto>> GetDevicesWithModels()
    {
        throw new NotImplementedException();
    }

    public async Task<bool> UpdateDevice(int id, string userId, DeviceUpdateDto deviceUpdateDto)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> MarkDeleteDevice(int id, string userId)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> DeleteDevice(int id)
    {
        throw new NotImplementedException();
    }
#endregion
#region model service
    public async Task<bool> CreateModel(ModelCreateDto modelCreateDto, string userId)
    {
        throw new NotImplementedException();
    }

    public async Task<ModelDto> GetModelById(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<ModelDto>> GetModels()
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<ModelDto>> GetModelsWithAssets()
    {
        throw new NotImplementedException();
    }

    public async Task<bool> UpdateModel(int id, string userId, ModelUpdateDto modelUpdateDto)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> MarkDeleteModel(int id, string userId)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> DeleteModel(int id)
    {
        throw new NotImplementedException();
    }
#endregion
#region situation service
    public async Task<bool> CreateSituation(SituationCreateDto situationCreateDto, string userId)
    {
        throw new NotImplementedException();
    }

    public async Task<SituationDto> GetSituationById(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<SituationDto>> GetSituations()
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<SituationDto>> GetSituationsWithAssets()
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<SituationDto>> GetSituationsWithCategories()
    {
        throw new NotImplementedException();
    }

    public async Task<bool> UpdateSituation(int id, string userId, SituationUpdateDto situationUpdateDto)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> MarkDeleteSituation(int id, string userId)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> DeleteSituation(int id)
    {
        throw new NotImplementedException();
    }
#endregion
#region category service
    public async Task<bool> CreateCategory(CategoryCreateDto categoryCreateDto, string userId)
    {
        throw new NotImplementedException();
    }

    public async Task<CategoryDto> GetCategoryById(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<CategoryDto>> GetCategories()
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<CategoryDto>> GetCategoriesWithAssets()
    {
        throw new NotImplementedException();
    }

    public async Task<bool> UpdateCategory(int id, string userId, CategoryUpdateDto categoryUpdateDto)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> MarkDeleteCategory(int id, string userId)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> DeleteCategory(int id)
    {
        throw new NotImplementedException();
    }
#endregion
#region communicate service
    public async Task<bool> CreateCommunicate(CommunicateCreateDto communicateCreateDto, string userId)
    {
        throw new NotImplementedException();
    }

    public async Task<CommunicateDto> GetCommunicateById(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<CommunicateDto>> GetCommunicates()
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<CommunicateDto>> GetCommunicatesWithAssets()
    {
        throw new NotImplementedException();
    }

    public async Task<bool> UpdateCommunicate(int id, string userId, CommunicateUpdateDto communicateUpdateDto)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> MarkDeleteCommunicate(int id, string userId)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> DeleteCommunicate(int id)
    {
        throw new NotImplementedException();
    }
#endregion
#region detail service
    public async Task<bool> CreateDetail(DetailCreateDto detailCreateDto, string userId)
    {
        throw new NotImplementedException();
    }

    public async Task<DetailDto> GetDetailById(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<DetailDto>> GetDetails()
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<DetailDto>> GetDetailsWithAssets()
    {
        throw new NotImplementedException();
    }

    public async Task<bool> UpdateDetail(int id, string userId, DetailUpdateDto detailUpdateDto)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> MarkDeleteDetail(int id, string userId)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> DeleteDetail(int id)
    {
        throw new NotImplementedException();
    }
#endregion
#region parameter service
    public async Task<bool> CreateParameter(ParameterCreateDto parameterCreateDto, string userId)
    {
        throw new NotImplementedException();
    }

    public async Task<ParameterDto> GetParameterById(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<ParameterDto>> GetParameters()
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<ParameterDto>> GetParametersWithModels()
    {
        throw new NotImplementedException();
    }

    public async Task<bool> UpdateParameter(int id, string userId, ParameterUpdateDto parameterUpdateDto)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> MarkDeleteParameter(int id, string userId)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> DeleteParameter(int id)
    {
        throw new NotImplementedException();
    }
#endregion
#region question service
    public async Task<bool> CreateQuestion(QuestionCreateDto questionCreateDto, string userId)
    {
        throw new NotImplementedException();
    }

    public async Task<QuestionDto> GetQuestionById(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<QuestionDto>> GetQuestions()
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<QuestionDto>> GetQuestionsWithSituations()
    {
        throw new NotImplementedException();
    }

    public async Task<bool> UpdateQuestion(int id, string userId, QuestionUpdateDto questionUpdateDto)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> MarkDeleteQuestion(int id, string userId)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> DeleteQuestion(int id)
    {
        throw new NotImplementedException();
    }
    #endregion
}