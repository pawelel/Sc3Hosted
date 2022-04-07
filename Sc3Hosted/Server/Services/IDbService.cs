
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
    Task<ServiceResponse> MarkUnDeletePlant(int id, string userId);
    Task<ServiceResponse> DeletePlant(int id);
    #endregion
    # region area interface
    Task<ServiceResponse> CreateArea(int plantId, AreaCreateDto areaCreateDto, string userId);
    Task<ServiceResponse<AreaDto>> GetAreaById(int id);
    Task<ServiceResponse<IEnumerable<AreaDto>>> GetAreas();
    Task<ServiceResponse<IEnumerable<AreaDto>>> GetAreasWithSpaces();
    Task<ServiceResponse> UpdateArea(int id, string userId, AreaUpdateDto areaUpdateDto);
    Task<ServiceResponse> MarkDeleteArea(int id, string userId);
    Task<ServiceResponse> MarkUnDeleteArea(int id, string userId);
    Task<ServiceResponse> DeleteArea(int id);
    #endregion
    #region space interface
    Task<ServiceResponse> CreateSpace(int areaId, SpaceCreateDto spaceCreateDto, string userId);
    Task<ServiceResponse<SpaceDto>> GetSpaceById(int id);
    Task<ServiceResponse<IEnumerable<SpaceDto>>> GetSpaces();
    Task<ServiceResponse<IEnumerable<SpaceDto>>> GetSpacesWithCoordinates();
    Task<ServiceResponse> UpdateSpace(int id, string userId, SpaceUpdateDto spaceUpdateDto);
    Task<ServiceResponse> MarkDeleteSpace(int id, string userId);
    Task<ServiceResponse> MarkUnDeleteSpace(int id, string userId);
    Task<ServiceResponse> DeleteSpace(int id);
    #endregion
    #region coordinate interface
    Task<ServiceResponse> CreateCoordinate(int spaceId, CoordinateCreateDto coordinateCreateDto, string userId);
    Task<ServiceResponse<CoordinateDto>> GetCoordinateById(int id);
    Task<ServiceResponse<IEnumerable<CoordinateDto>>> GetCoordinates();
    Task<ServiceResponse<IEnumerable<CoordinateDto>>> GetCoordinatesWithAssets();
    Task<ServiceResponse> UpdateCoordinate(int id, string userId, CoordinateUpdateDto coordinateUpdateDto);
    Task<ServiceResponse> MarkDeleteCoordinate(int id, string userId);
    Task<ServiceResponse> MarkUnDeleteCoordinate(int id, string userId);
    Task<ServiceResponse> DeleteCoordinate(int id);
    #endregion
    #region asset interface
    Task<ServiceResponse> CreateAsset(AssetCreateDto assetCreateDto, string userId);
    Task<ServiceResponse<AssetDto>> GetAssetById(int id);
    Task<ServiceResponse<IEnumerable<AssetDto>>> GetAssets();
    Task<ServiceResponse<IEnumerable<AssetDto>>> GetAssetsWithAllData();
    Task<ServiceResponse> UpdateAsset(int id, string userId, AssetUpdateDto assetUpdateDto);
    Task<ServiceResponse> ChangeAssetsModel(int assetId, string userId, int modelId);
    Task<ServiceResponse> MarkDeleteAsset(int id, string userId);
    Task<ServiceResponse> MarkUnDeleteAsset(int id, string userId);
    Task<ServiceResponse> DeleteAsset(int id);
    #endregion
    #region device interface
    Task<ServiceResponse> CreateDevice(DeviceCreateDto deviceCreateDto, string userId);
    Task<ServiceResponse<DeviceDto>> GetDeviceById(int id);
    Task<ServiceResponse<IEnumerable<DeviceDto>>> GetDevices();
    Task<ServiceResponse<IEnumerable<DeviceDto>>> GetDevicesWithModels();
    Task<ServiceResponse> UpdateDevice(int id, string userId, DeviceUpdateDto deviceUpdateDto);
    Task<ServiceResponse> MarkDeleteDevice(int id, string userId);
    Task<ServiceResponse> MarkUnDeleteDevice(int id, string userId);
    Task<ServiceResponse> DeleteDevice(int id);
    #endregion
    #region model interface
    Task<ServiceResponse> CreateModel(int deviceId, ModelCreateDto modelCreateDto, string userId);
    Task<ServiceResponse<ModelDto>> GetModelById(int id);
    Task<ServiceResponse<IEnumerable<ModelDto>>> GetModels();
    Task<ServiceResponse<IEnumerable<ModelDto>>> GetModelsWithAssets();
    Task<ServiceResponse> UpdateModel(int id, string userId, ModelUpdateDto modelUpdateDto);
    Task<ServiceResponse> MarkDeleteModel(int id, string userId);
    Task<ServiceResponse> MarkUnDeleteModel(int id, string userId);
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
    Task<ServiceResponse> MarkUnDeleteSituation(int id, string userId);
    Task<ServiceResponse> DeleteSituation(int id);
    #endregion
    #region category interface
    Task<ServiceResponse> CreateCategory(CategoryCreateDto categoryCreateDto, string userId);
    Task<ServiceResponse<CategoryDto>> GetCategoryById(int id);
    Task<ServiceResponse<IEnumerable<CategoryDto>>> GetCategories();
    Task<ServiceResponse<IEnumerable<CategoryDto>>> GetCategoriesWithAssets();
    Task<ServiceResponse> UpdateCategory(int id, string userId, CategoryUpdateDto categoryUpdateDto);
    Task<ServiceResponse> MarkDeleteCategory(int id, string userId);
    Task<ServiceResponse> MarkUnDeleteCategory(int id, string userId);
    Task<ServiceResponse> DeleteCategory(int id);
    #endregion
    #region communicate interface
    Task<ServiceResponse> CreateCommunicate(CommunicateCreateDto communicateCreateDto, string userId);
    Task<ServiceResponse<CommunicateDto>> GetCommunicateById(int id);
    Task<ServiceResponse<IEnumerable<CommunicateDto>>> GetCommunicates();
    Task<ServiceResponse<IEnumerable<CommunicateDto>>> GetCommunicatesWithAssets();
    Task<ServiceResponse> UpdateCommunicate(int id, string userId, CommunicateUpdateDto communicateUpdateDto);
    Task<ServiceResponse> MarkDeleteCommunicate(int id, string userId);
    Task<ServiceResponse> MarkUnDeleteCommunicate(int id, string userId);
    Task<ServiceResponse> DeleteCommunicate(int id);
    #endregion
    #region detail interface
    Task<ServiceResponse> CreateDetail(DetailCreateDto detailCreateDto, string userId);
    Task<ServiceResponse<DetailDto>> GetDetailById(int id);
    Task<ServiceResponse<IEnumerable<DetailDto>>> GetDetails();
    Task<ServiceResponse<IEnumerable<DetailDto>>> GetDetailsWithAssets();
    Task<ServiceResponse> UpdateDetail(int id, string userId, DetailUpdateDto detailUpdateDto);
    Task<ServiceResponse> MarkDeleteDetail(int id, string userId);
    Task<ServiceResponse> MarkUnDeleteDetail(int id, string userId);
    Task<ServiceResponse> DeleteDetail(int id);
    #endregion
    #region parameter interface
    Task<ServiceResponse> CreateParameter(ParameterCreateDto parameterCreateDto, string userId);
    Task<ServiceResponse<ParameterDto>> GetParameterById(int id);
    Task<ServiceResponse<IEnumerable<ParameterDto>>> GetParameters();
    Task<ServiceResponse<IEnumerable<ParameterDto>>> GetParametersWithModels();
    Task<ServiceResponse> UpdateParameter(int id, string userId, ParameterUpdateDto parameterUpdateDto);
    Task<ServiceResponse> MarkDeleteParameter(int id, string userId);
    Task<ServiceResponse> MarkUnDeleteParameter(int id, string userId);
    Task<ServiceResponse> DeleteParameter(int id);
    #endregion
    #region question interface
    Task<ServiceResponse> CreateQuestion(QuestionCreateDto questionCreateDto, string userId);
    Task<ServiceResponse<QuestionDto>> GetQuestionById(int id);
    Task<ServiceResponse<IEnumerable<QuestionDto>>> GetQuestions();
    Task<ServiceResponse<IEnumerable<QuestionDto>>> GetQuestionsWithSituations();
    Task<ServiceResponse> UpdateQuestion(int id, string userId, QuestionUpdateDto questionUpdateDto);
    Task<ServiceResponse> MarkDeleteQuestion(int id, string userId);
    Task<ServiceResponse> MarkUnDeleteQuestion(int id, string userId);
    Task<ServiceResponse> DeleteQuestion(int id);
    #endregion
}
