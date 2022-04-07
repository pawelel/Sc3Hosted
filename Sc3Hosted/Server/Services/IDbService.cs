
using Sc3Hosted.Shared.Dtos;
using Sc3Hosted.Shared.Helpers;

namespace Sc3Hosted.Server.Services;

public interface IDbService
{
    Task<ServiceResponse> ChangeAssetsModel(int assetId, string userId, int modelId);

    Task<ServiceResponse> CreateArea(int plantId, AreaCreateDto areaCreateDto, string userId);

    Task<ServiceResponse> CreateAsset(AssetCreateDto assetCreateDto, string userId);

    Task<ServiceResponse> CreateCategory(CategoryCreateDto categoryCreateDto, string userId);

    Task<ServiceResponse> CreateCommunicate(CommunicateCreateDto communicateCreateDto, string userId);

    Task<ServiceResponse> CreateCoordinate(int spaceId, CoordinateCreateDto coordinateCreateDto, string userId);

    Task<ServiceResponse> CreateDetail(DetailCreateDto detailCreateDto, string userId);

    Task<ServiceResponse> CreateDevice(DeviceCreateDto deviceCreateDto, string userId);

    Task<ServiceResponse> CreateModel(int deviceId, ModelCreateDto modelCreateDto, string userId);

    Task<ServiceResponse> CreateParameter(ParameterCreateDto parameterCreateDto, string userId);

    Task<ServiceResponse> CreatePlant(PlantCreateDto plantCreateDto, string userId);
    Task<ServiceResponse> CreateQuestion(QuestionCreateDto questionCreateDto, string userId);

    Task<ServiceResponse> CreateSituation(SituationCreateDto situationCreateDto, string userId);

    Task<ServiceResponse> CreateSpace(int areaId, SpaceCreateDto spaceCreateDto, string userId);

    Task<ServiceResponse> DeleteArea(int id);

    Task<ServiceResponse> DeleteAsset(int id);

    Task<ServiceResponse> DeleteCategory(int id);

    Task<ServiceResponse> DeleteCommunicate(int id);

    Task<ServiceResponse> DeleteCoordinate(int id);

    Task<ServiceResponse> DeleteDetail(int id);

    Task<ServiceResponse> DeleteDevice(int id);

    Task<ServiceResponse> DeleteModel(int id);

    Task<ServiceResponse> DeleteParameter(int id);

    Task<ServiceResponse> DeletePlant(int id);

    Task<ServiceResponse> DeleteQuestion(int id);

    Task<ServiceResponse> DeleteSituation(int id);

    Task<ServiceResponse> DeleteSpace(int id);

    Task<ServiceResponse<AreaDto>> GetAreaById(int id);

    Task<ServiceResponse<IEnumerable<AreaDto>>> GetAreas();

    Task<ServiceResponse<IEnumerable<AreaDto>>> GetAreasWithSpaces();

    Task<ServiceResponse<AssetDto>> GetAssetById(int id);

    Task<ServiceResponse<IEnumerable<AssetDto>>> GetAssets();

    Task<ServiceResponse<IEnumerable<AssetDto>>> GetAssetsWithAllData();

    Task<ServiceResponse<IEnumerable<CategoryDto>>> GetCategories();

    Task<ServiceResponse<IEnumerable<CategoryWithAssetsDto>>> GetCategoriesWithAssets();

    Task<ServiceResponse<CategoryDto>> GetCategoryById(int id);
    Task<ServiceResponse<CategoryWithAssetsDto>> GetCategoryByIdWithAssets(int id);
    Task<ServiceResponse<CommunicateDto>> GetCommunicateById(int id);

    Task<ServiceResponse<IEnumerable<CommunicateDto>>> GetCommunicates();

    Task<ServiceResponse<IEnumerable<CommunicateWithAssetsDto>>> GetCommunicatesWithAssets();

    Task<ServiceResponse<CoordinateDto>> GetCoordinateByIdWithAssets(int id);

    Task<ServiceResponse<IEnumerable<CoordinateDto>>> GetCoordinates();

    Task<ServiceResponse<IEnumerable<CoordinateDto>>> GetCoordinatesWithAssets();

    Task<ServiceResponse<DetailDto>> GetDetailById(int id);

    Task<ServiceResponse<IEnumerable<DetailDto>>> GetDetails();

    Task<ServiceResponse<IEnumerable<DetailWithAssetsDto>>> GetDetailsWithAssets();

    Task<ServiceResponse<DeviceDto>> GetDeviceById(int id);

    Task<ServiceResponse<IEnumerable<DeviceDto>>> GetDevices();

    Task<ServiceResponse<IEnumerable<DeviceDto>>> GetDevicesWithModels();

    Task<ServiceResponse<ModelDto>> GetModelById(int id);

    Task<ServiceResponse<IEnumerable<ModelDto>>> GetModels();

    Task<ServiceResponse<IEnumerable<ModelDto>>> GetModelsWithAssets();

    Task<ServiceResponse<ParameterDto>> GetParameterById(int id);

    Task<ServiceResponse<IEnumerable<ParameterDto>>> GetParameters();

    Task<ServiceResponse<IEnumerable<ParameterDto>>> GetParametersWithModels();

    Task<ServiceResponse<PlantDto>> GetPlantById(int id);
    Task<ServiceResponse<IEnumerable<PlantDto>>> GetPlants();
    Task<ServiceResponse<IEnumerable<PlantDto>>> GetPlantsWithAreas();
    Task<ServiceResponse<QuestionDto>> GetQuestionById(int id);

    Task<ServiceResponse<IEnumerable<QuestionDto>>> GetQuestions();

    Task<ServiceResponse<IEnumerable<QuestionDto>>> GetQuestionsWithSituations();

    Task<ServiceResponse<SituationDto>> GetSituationById(int id);

    Task<ServiceResponse<IEnumerable<SituationDto>>> GetSituations();

    Task<ServiceResponse<IEnumerable<SituationDto>>> GetSituationsWithAssets();

    Task<ServiceResponse<IEnumerable<SituationDto>>> GetSituationsWithCategories();

    Task<ServiceResponse<SpaceDto>> GetSpaceById(int id);

    Task<ServiceResponse<IEnumerable<SpaceDto>>> GetSpaces();

    Task<ServiceResponse<IEnumerable<SpaceDto>>> GetSpacesWithCoordinates();

    Task<ServiceResponse> MarkDeleteArea(int id, string userId);

    Task<ServiceResponse> MarkDeleteAsset(int id, string userId);

    Task<ServiceResponse> MarkDeleteCategory(int id, string userId);

    Task<ServiceResponse> MarkDeleteCommunicate(int id, string userId);

    Task<ServiceResponse> MarkDeleteCoordinate(int id, string userId);

    Task<ServiceResponse> MarkDeleteDetail(int id, string userId);

    Task<ServiceResponse> MarkDeleteDevice(int id, string userId);

    Task<ServiceResponse> MarkDeleteModel(int id, string userId);

    Task<ServiceResponse> MarkDeleteParameter(int id, string userId);

    Task<ServiceResponse> MarkDeletePlant(int id, string userId);

    Task<ServiceResponse> MarkDeleteQuestion(int id, string userId);

    Task<ServiceResponse> MarkDeleteSituation(int id, string userId);

    Task<ServiceResponse> MarkDeleteSpace(int id, string userId);

    Task<ServiceResponse> MarkUnDeleteArea(int id, string userId);

    Task<ServiceResponse> MarkUnDeleteAsset(int id, string userId);

    Task<ServiceResponse> MarkUnDeleteCategory(int id, string userId);

    Task<ServiceResponse> MarkUnDeleteCommunicate(int id, string userId);

    Task<ServiceResponse> MarkUnDeleteCoordinate(int id, string userId);

    Task<ServiceResponse> MarkUnDeleteDetail(int id, string userId);

    Task<ServiceResponse> MarkUnDeleteDevice(int id, string userId);

    Task<ServiceResponse> MarkUnDeleteModel(int id, string userId);

    Task<ServiceResponse> MarkUnDeleteParameter(int id, string userId);

    Task<ServiceResponse> MarkUnDeletePlant(int id, string userId);

    Task<ServiceResponse> MarkUnDeleteQuestion(int id, string userId);

    Task<ServiceResponse> MarkUnDeleteSituation(int id, string userId);

    Task<ServiceResponse> MarkUnDeleteSpace(int id, string userId);

    Task<ServiceResponse> UpdateArea(int id, string userId, AreaUpdateDto areaUpdateDto);

    Task<ServiceResponse> UpdateAsset(int id, string userId, AssetUpdateDto assetUpdateDto);

    Task<ServiceResponse> UpdateCategory(int id, string userId, CategoryUpdateDto categoryUpdateDto);

    Task<ServiceResponse> UpdateCommunicate(int id, string userId, CommunicateUpdateDto communicateUpdateDto);

    Task<ServiceResponse> UpdateCoordinate(int id, string userId, CoordinateUpdateDto coordinateUpdateDto);

    Task<ServiceResponse> UpdateDetail(int id, string userId, DetailUpdateDto detailUpdateDto);

    Task<ServiceResponse> UpdateDevice(int id, string userId, DeviceUpdateDto deviceUpdateDto);

    Task<ServiceResponse> UpdateModel(int id, string userId, ModelUpdateDto modelUpdateDto);

    Task<ServiceResponse> UpdateParameter(int id, string userId, ParameterUpdateDto parameterUpdateDto);

    Task<ServiceResponse> UpdatePlant(int id, string userId, PlantUpdateDto plantUpdateDto);

    Task<ServiceResponse> UpdateQuestion(int id, string userId, QuestionUpdateDto questionUpdateDto);

    Task<ServiceResponse> UpdateSituation(int id, string userId, SituationUpdateDto situationUpdateDto);

    Task<ServiceResponse> UpdateSpace(int id, string userId, SpaceUpdateDto spaceUpdateDto);

}
