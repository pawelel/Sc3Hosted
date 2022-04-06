using AutoMapper;

using Bogus;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Moq;

using Sc3Hosted.Server.Data;
using Sc3Hosted.Server.Services;
using Sc3Hosted.Shared.Dtos;

using System;
using System.Threading.Tasks;

using Xunit;

namespace Sc3Hosted.UnitTests.Services
{
    public class DbServiceTests
    {
        private MockRepository mockRepository;

        private Mock<IMapper> mockMapper;
        private Mock<ILogger<DbService>> mockLogger;
        private Mock<IDbContextFactory<Sc3HostedDbContext>> mockDbContextFactory;

        public DbServiceTests()
        {
            this.mockRepository = new MockRepository(MockBehavior.Loose);

            this.mockMapper = this.mockRepository.Create<IMapper>();
            this.mockLogger = this.mockRepository.Create<ILogger<DbService>>();
            this.mockDbContextFactory = this.mockRepository.Create<IDbContextFactory<Sc3HostedDbContext>>();
        }

        private DbService CreateService()
        {
            return new DbService(
                this.mockMapper.Object,
                this.mockLogger.Object,
                this.mockDbContextFactory.Object);
        }

        [Fact]
        public async Task CreatePlant_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var service = this.CreateService();
            PlantCreateDto plantCreateDto = new Faker<PlantCreateDto>().
        RuleFor(o => o.Name, f => f.Lorem.Word()).
        RuleFor(o => o.Description, f => f.Lorem.Paragraph()).
        Generate();
            string userId = Guid.NewGuid().ToString();

            // Act
            var result = await service.CreatePlant(
                plantCreateDto,
                userId);

            // Assert
            Assert.True(false);
            this.mockRepository.VerifyAll();
        }

        [Fact]
        public async Task GetPlantById_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var service = this.CreateService();
            int id = 0;

            // Act
            var result = await service.GetPlantById(
                id);

            // Assert
            Assert.True(false);
            this.mockRepository.VerifyAll();
        }

        [Fact]
        public async Task GetPlants_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var service = this.CreateService();

            // Act
            var result = await service.GetPlants();

            // Assert
            Assert.True(false);
            this.mockRepository.VerifyAll();
        }

        [Fact]
        public async Task GetPlantsWithAreas_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var service = this.CreateService();

            // Act
            var result = await service.GetPlantsWithAreas();

            // Assert
            Assert.True(false);
            this.mockRepository.VerifyAll();
        }

        [Fact]
        public async Task UpdatePlant_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var service = this.CreateService();
            int id = 0;
            string userId = null;
            PlantUpdateDto plantUpdateDto = null;

            // Act
            var result = await service.UpdatePlant(
                id,
                userId,
                plantUpdateDto);

            // Assert
            Assert.True(false);
            this.mockRepository.VerifyAll();
        }

        [Fact]
        public async Task MarkDeletePlant_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var service = this.CreateService();
            int id = 0;
            string userId = null;

            // Act
            var result = await service.MarkDeletePlant(
                id,
                userId);

            // Assert
            Assert.True(false);
            this.mockRepository.VerifyAll();
        }

        [Fact]
        public async Task MarkUnDeletePlant_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var service = this.CreateService();
            int id = 0;
            string userId = null;

            // Act
            var result = await service.MarkUnDeletePlant(
                id,
                userId);

            // Assert
            Assert.True(false);
            this.mockRepository.VerifyAll();
        }

        [Fact]
        public async Task DeletePlant_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var service = this.CreateService();
            int id = 0;

            // Act
            var result = await service.DeletePlant(
                id);

            // Assert
            Assert.True(false);
            this.mockRepository.VerifyAll();
        }

        [Fact]
        public async Task CreateArea_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var service = this.CreateService();
            int plantId = 0;
            AreaCreateDto areaCreateDto = null;
            string userId = null;

            // Act
            var result = await service.CreateArea(
                plantId,
                areaCreateDto,
                userId);

            // Assert
            Assert.True(false);
            this.mockRepository.VerifyAll();
        }

        [Fact]
        public async Task GetAreaById_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var service = this.CreateService();
            int id = 0;

            // Act
            var result = await service.GetAreaById(
                id);

            // Assert
            Assert.True(false);
            this.mockRepository.VerifyAll();
        }

        [Fact]
        public async Task GetAreas_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var service = this.CreateService();

            // Act
            var result = await service.GetAreas();

            // Assert
            Assert.True(false);
            this.mockRepository.VerifyAll();
        }

        [Fact]
        public async Task GetAreasWithSpaces_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var service = this.CreateService();

            // Act
            var result = await service.GetAreasWithSpaces();

            // Assert
            Assert.True(false);
            this.mockRepository.VerifyAll();
        }

        [Fact]
        public async Task UpdateArea_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var service = this.CreateService();
            int id = 0;
            string userId = null;
            AreaUpdateDto areaUpdateDto = null;

            // Act
            var result = await service.UpdateArea(
                id,
                userId,
                areaUpdateDto);

            // Assert
            Assert.True(false);
            this.mockRepository.VerifyAll();
        }

        [Fact]
        public async Task MarkDeleteArea_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var service = this.CreateService();
            int id = 0;
            string userId = null;

            // Act
            var result = await service.MarkDeleteArea(
                id,
                userId);

            // Assert
            Assert.True(false);
            this.mockRepository.VerifyAll();
        }

        [Fact]
        public async Task MarkUnDeleteArea_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var service = this.CreateService();
            int id = 0;
            string userId = null;

            // Act
            var result = await service.MarkUnDeleteArea(
                id,
                userId);

            // Assert
            Assert.True(false);
            this.mockRepository.VerifyAll();
        }

        [Fact]
        public async Task DeleteArea_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var service = this.CreateService();
            int id = 0;

            // Act
            var result = await service.DeleteArea(
                id);

            // Assert
            Assert.True(false);
            this.mockRepository.VerifyAll();
        }

        [Fact]
        public async Task CreateSpace_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var service = this.CreateService();
            int areaId = 0;
            SpaceCreateDto spaceCreateDto = null;
            string userId = null;

            // Act
            var result = await service.CreateSpace(
                areaId,
                spaceCreateDto,
                userId);

            // Assert
            Assert.True(false);
            this.mockRepository.VerifyAll();
        }

        [Fact]
        public async Task GetSpaceById_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var service = this.CreateService();
            int id = 0;

            // Act
            var result = await service.GetSpaceById(
                id);

            // Assert
            Assert.True(false);
            this.mockRepository.VerifyAll();
        }

        [Fact]
        public async Task GetSpaces_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var service = this.CreateService();

            // Act
            var result = await service.GetSpaces();

            // Assert
            Assert.True(false);
            this.mockRepository.VerifyAll();
        }

        [Fact]
        public async Task GetSpacesWithCoordinates_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var service = this.CreateService();

            // Act
            var result = await service.GetSpacesWithCoordinates();

            // Assert
            Assert.True(false);
            this.mockRepository.VerifyAll();
        }

        [Fact]
        public async Task UpdateSpace_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var service = this.CreateService();
            int id = 0;
            string userId = null;
            SpaceUpdateDto spaceUpdateDto = null;

            // Act
            var result = await service.UpdateSpace(
                id,
                userId,
                spaceUpdateDto);

            // Assert
            Assert.True(false);
            this.mockRepository.VerifyAll();
        }

        [Fact]
        public async Task MarkDeleteSpace_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var service = this.CreateService();
            int id = 0;
            string userId = null;

            // Act
            var result = await service.MarkDeleteSpace(
                id,
                userId);

            // Assert
            Assert.True(false);
            this.mockRepository.VerifyAll();
        }

        [Fact]
        public async Task MarkUnDeleteSpace_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var service = this.CreateService();
            int id = 0;
            string userId = null;

            // Act
            var result = await service.MarkUnDeleteSpace(
                id,
                userId);

            // Assert
            Assert.True(false);
            this.mockRepository.VerifyAll();
        }

        [Fact]
        public async Task DeleteSpace_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var service = this.CreateService();
            int id = 0;

            // Act
            var result = await service.DeleteSpace(
                id);

            // Assert
            Assert.True(false);
            this.mockRepository.VerifyAll();
        }

        [Fact]
        public async Task CreateCoordinate_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var service = this.CreateService();
            int spaceId = 0;
            CoordinateCreateDto coordinateCreateDto = null;
            string userId = null;

            // Act
            var result = await service.CreateCoordinate(
                spaceId,
                coordinateCreateDto,
                userId);

            // Assert
            Assert.True(false);
            this.mockRepository.VerifyAll();
        }

        [Fact]
        public async Task GetCoordinateById_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var service = this.CreateService();
            int id = 0;

            // Act
            var result = await service.GetCoordinateById(
                id);

            // Assert
            Assert.True(false);
            this.mockRepository.VerifyAll();
        }

        [Fact]
        public async Task GetCoordinates_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var service = this.CreateService();

            // Act
            var result = await service.GetCoordinates();

            // Assert
            Assert.True(false);
            this.mockRepository.VerifyAll();
        }

        [Fact]
        public async Task GetCoordinatesWithAssets_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var service = this.CreateService();

            // Act
            var result = await service.GetCoordinatesWithAssets();

            // Assert
            Assert.True(false);
            this.mockRepository.VerifyAll();
        }

        [Fact]
        public async Task UpdateCoordinate_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var service = this.CreateService();
            int id = 0;
            string userId = null;
            CoordinateUpdateDto coordinateUpdateDto = null;

            // Act
            var result = await service.UpdateCoordinate(
                id,
                userId,
                coordinateUpdateDto);

            // Assert
            Assert.True(false);
            this.mockRepository.VerifyAll();
        }

        [Fact]
        public async Task MarkDeleteCoordinate_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var service = this.CreateService();
            int id = 0;
            string userId = null;

            // Act
            var result = await service.MarkDeleteCoordinate(
                id,
                userId);

            // Assert
            Assert.True(false);
            this.mockRepository.VerifyAll();
        }

        [Fact]
        public async Task MarkUnDeleteCoordinate_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var service = this.CreateService();
            int id = 0;
            string userId = null;

            // Act
            var result = await service.MarkUnDeleteCoordinate(
                id,
                userId);

            // Assert
            Assert.True(false);
            this.mockRepository.VerifyAll();
        }

        [Fact]
        public async Task DeleteCoordinate_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var service = this.CreateService();
            int id = 0;

            // Act
            var result = await service.DeleteCoordinate(
                id);

            // Assert
            Assert.True(false);
            this.mockRepository.VerifyAll();
        }

        [Fact]
        public async Task CreateCategory_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var service = this.CreateService();
            CategoryCreateDto categoryCreateDto = null;
            string userId = null;

            // Act
            var result = await service.CreateCategory(
                categoryCreateDto,
                userId);

            // Assert
            Assert.True(false);
            this.mockRepository.VerifyAll();
        }

        [Fact]
        public async Task GetCategoryById_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var service = this.CreateService();
            int id = 0;

            // Act
            var result = await service.GetCategoryById(
                id);

            // Assert
            Assert.True(false);
            this.mockRepository.VerifyAll();
        }

        [Fact]
        public async Task GetCategories_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var service = this.CreateService();

            // Act
            var result = await service.GetCategories();

            // Assert
            Assert.True(false);
            this.mockRepository.VerifyAll();
        }

        [Fact]
        public async Task GetCategoriesWithAssets_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var service = this.CreateService();

            // Act
            var result = await service.GetCategoriesWithAssets();

            // Assert
            Assert.True(false);
            this.mockRepository.VerifyAll();
        }

        [Fact]
        public async Task UpdateCategory_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var service = this.CreateService();
            int id = 0;
            string userId = null;
            CategoryUpdateDto categoryUpdateDto = null;

            // Act
            var result = await service.UpdateCategory(
                id,
                userId,
                categoryUpdateDto);

            // Assert
            Assert.True(false);
            this.mockRepository.VerifyAll();
        }

        [Fact]
        public async Task MarkDeleteCategory_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var service = this.CreateService();
            int id = 0;
            string userId = null;

            // Act
            var result = await service.MarkDeleteCategory(
                id,
                userId);

            // Assert
            Assert.True(false);
            this.mockRepository.VerifyAll();
        }

        [Fact]
        public async Task MarkUnDeleteCategory_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var service = this.CreateService();
            int id = 0;
            string userId = null;

            // Act
            var result = await service.MarkUnDeleteCategory(
                id,
                userId);

            // Assert
            Assert.True(false);
            this.mockRepository.VerifyAll();
        }

        [Fact]
        public async Task DeleteCategory_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var service = this.CreateService();
            int id = 0;

            // Act
            var result = await service.DeleteCategory(
                id);

            // Assert
            Assert.True(false);
            this.mockRepository.VerifyAll();
        }

        [Fact]
        public async Task CreateDevice_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var service = this.CreateService();
            DeviceCreateDto deviceCreateDto = null;
            string userId = null;

            // Act
            var result = await service.CreateDevice(
                deviceCreateDto,
                userId);

            // Assert
            Assert.True(false);
            this.mockRepository.VerifyAll();
        }

        [Fact]
        public async Task GetDeviceById_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var service = this.CreateService();
            int id = 0;

            // Act
            var result = await service.GetDeviceById(
                id);

            // Assert
            Assert.True(false);
            this.mockRepository.VerifyAll();
        }

        [Fact]
        public async Task GetDevices_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var service = this.CreateService();

            // Act
            var result = await service.GetDevices();

            // Assert
            Assert.True(false);
            this.mockRepository.VerifyAll();
        }

        [Fact]
        public async Task GetDevicesWithModels_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var service = this.CreateService();

            // Act
            var result = await service.GetDevicesWithModels();

            // Assert
            Assert.True(false);
            this.mockRepository.VerifyAll();
        }

        [Fact]
        public async Task UpdateDevice_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var service = this.CreateService();
            int id = 0;
            string userId = null;
            DeviceUpdateDto deviceUpdateDto = null;

            // Act
            var result = await service.UpdateDevice(
                id,
                userId,
                deviceUpdateDto);

            // Assert
            Assert.True(false);
            this.mockRepository.VerifyAll();
        }

        [Fact]
        public async Task MarkDeleteDevice_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var service = this.CreateService();
            int id = 0;
            string userId = null;

            // Act
            var result = await service.MarkDeleteDevice(
                id,
                userId);

            // Assert
            Assert.True(false);
            this.mockRepository.VerifyAll();
        }

        [Fact]
        public async Task MarkUnDeleteDevice_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var service = this.CreateService();
            int id = 0;
            string userId = null;

            // Act
            var result = await service.MarkUnDeleteDevice(
                id,
                userId);

            // Assert
            Assert.True(false);
            this.mockRepository.VerifyAll();
        }

        [Fact]
        public async Task DeleteDevice_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var service = this.CreateService();
            int id = 0;

            // Act
            var result = await service.DeleteDevice(
                id);

            // Assert
            Assert.True(false);
            this.mockRepository.VerifyAll();
        }

        [Fact]
        public async Task CreateModel_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var service = this.CreateService();
            int deviceId = 0;
            ModelCreateDto modelCreateDto = null;
            string userId = null;

            // Act
            var result = await service.CreateModel(
                deviceId,
                modelCreateDto,
                userId);

            // Assert
            Assert.True(false);
            this.mockRepository.VerifyAll();
        }

        [Fact]
        public async Task GetModelById_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var service = this.CreateService();
            int id = 0;

            // Act
            var result = await service.GetModelById(
                id);

            // Assert
            Assert.True(false);
            this.mockRepository.VerifyAll();
        }

        [Fact]
        public async Task GetModels_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var service = this.CreateService();

            // Act
            var result = await service.GetModels();

            // Assert
            Assert.True(false);
            this.mockRepository.VerifyAll();
        }

        [Fact]
        public async Task GetModelsWithAssets_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var service = this.CreateService();

            // Act
            var result = await service.GetModelsWithAssets();

            // Assert
            Assert.True(false);
            this.mockRepository.VerifyAll();
        }

        [Fact]
        public async Task UpdateModel_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var service = this.CreateService();
            int id = 0;
            string userId = null;
            ModelUpdateDto modelUpdateDto = null;

            // Act
            var result = await service.UpdateModel(
                id,
                userId,
                modelUpdateDto);

            // Assert
            Assert.True(false);
            this.mockRepository.VerifyAll();
        }

        [Fact]
        public async Task MarkDeleteModel_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var service = this.CreateService();
            int id = 0;
            string userId = null;

            // Act
            var result = await service.MarkDeleteModel(
                id,
                userId);

            // Assert
            Assert.True(false);
            this.mockRepository.VerifyAll();
        }

        [Fact]
        public async Task MarkUnDeleteModel_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var service = this.CreateService();
            int id = 0;
            string userId = null;

            // Act
            var result = await service.MarkUnDeleteModel(
                id,
                userId);

            // Assert
            Assert.True(false);
            this.mockRepository.VerifyAll();
        }

        [Fact]
        public async Task DeleteModel_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var service = this.CreateService();
            int id = 0;

            // Act
            var result = await service.DeleteModel(
                id);

            // Assert
            Assert.True(false);
            this.mockRepository.VerifyAll();
        }

        [Fact]
        public async Task CreateParameter_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var service = this.CreateService();
            ParameterCreateDto parameterCreateDto = null;
            string userId = null;

            // Act
            var result = await service.CreateParameter(
                parameterCreateDto,
                userId);

            // Assert
            Assert.True(false);
            this.mockRepository.VerifyAll();
        }

        [Fact]
        public async Task GetParameterById_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var service = this.CreateService();
            int id = 0;

            // Act
            var result = await service.GetParameterById(
                id);

            // Assert
            Assert.True(false);
            this.mockRepository.VerifyAll();
        }

        [Fact]
        public async Task GetParameters_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var service = this.CreateService();

            // Act
            var result = await service.GetParameters();

            // Assert
            Assert.True(false);
            this.mockRepository.VerifyAll();
        }

        [Fact]
        public async Task GetParametersWithModels_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var service = this.CreateService();

            // Act
            var result = await service.GetParametersWithModels();

            // Assert
            Assert.True(false);
            this.mockRepository.VerifyAll();
        }

        [Fact]
        public async Task UpdateParameter_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var service = this.CreateService();
            int id = 0;
            string userId = null;
            ParameterUpdateDto parameterUpdateDto = null;

            // Act
            var result = await service.UpdateParameter(
                id,
                userId,
                parameterUpdateDto);

            // Assert
            Assert.True(false);
            this.mockRepository.VerifyAll();
        }

        [Fact]
        public async Task MarkDeleteParameter_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var service = this.CreateService();
            int id = 0;
            string userId = null;

            // Act
            var result = await service.MarkDeleteParameter(
                id,
                userId);

            // Assert
            Assert.True(false);
            this.mockRepository.VerifyAll();
        }

        [Fact]
        public async Task MarkUnDeleteParameter_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var service = this.CreateService();
            int id = 0;
            string userId = null;

            // Act
            var result = await service.MarkUnDeleteParameter(
                id,
                userId);

            // Assert
            Assert.True(false);
            this.mockRepository.VerifyAll();
        }

        [Fact]
        public async Task DeleteParameter_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var service = this.CreateService();
            int id = 0;

            // Act
            var result = await service.DeleteParameter(
                id);

            // Assert
            Assert.True(false);
            this.mockRepository.VerifyAll();
        }

        [Fact]
        public async Task CreateAsset_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var service = this.CreateService();
            AssetCreateDto assetCreateDto = null;
            string userId = null;

            // Act
            var result = await service.CreateAsset(
                assetCreateDto,
                userId);

            // Assert
            Assert.True(false);
            this.mockRepository.VerifyAll();
        }

        [Fact]
        public async Task GetAssetById_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var service = this.CreateService();
            int id = 0;

            // Act
            var result = await service.GetAssetById(
                id);

            // Assert
            Assert.True(false);
            this.mockRepository.VerifyAll();
        }

        [Fact]
        public async Task GetAssets_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var service = this.CreateService();

            // Act
            var result = await service.GetAssets();

            // Assert
            Assert.True(false);
            this.mockRepository.VerifyAll();
        }

        [Fact]
        public async Task GetAssetsWithAllData_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var service = this.CreateService();

            // Act
            var result = await service.GetAssetsWithAllData();

            // Assert
            Assert.True(false);
            this.mockRepository.VerifyAll();
        }

        [Fact]
        public async Task UpdateAsset_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var service = this.CreateService();
            int id = 0;
            string userId = null;
            AssetUpdateDto assetUpdateDto = null;

            // Act
            var result = await service.UpdateAsset(
                id,
                userId,
                assetUpdateDto);

            // Assert
            Assert.True(false);
            this.mockRepository.VerifyAll();
        }

        [Fact]
        public async Task AssetChangeModel_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var service = this.CreateService();
            int assetId = 0;
            string userId = null;
            int modelId = 0;

            // Act
            var result = await service.AssetChangeModel(
                assetId,
                userId,
                modelId);

            // Assert
            Assert.True(false);
            this.mockRepository.VerifyAll();
        }

        [Fact]
        public async Task MarkDeleteAsset_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var service = this.CreateService();
            int id = 0;
            string userId = null;

            // Act
            var result = await service.MarkDeleteAsset(
                id,
                userId);

            // Assert
            Assert.True(false);
            this.mockRepository.VerifyAll();
        }

        [Fact]
        public async Task MarkUnDeleteAsset_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var service = this.CreateService();
            int id = 0;
            string userId = null;

            // Act
            var result = await service.MarkUnDeleteAsset(
                id,
                userId);

            // Assert
            Assert.True(false);
            this.mockRepository.VerifyAll();
        }

        [Fact]
        public async Task DeleteAsset_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var service = this.CreateService();
            int id = 0;

            // Act
            var result = await service.DeleteAsset(
                id);

            // Assert
            Assert.True(false);
            this.mockRepository.VerifyAll();
        }

        [Fact]
        public async Task CreateDetail_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var service = this.CreateService();
            DetailCreateDto detailCreateDto = null;
            string userId = null;

            // Act
            var result = await service.CreateDetail(
                detailCreateDto,
                userId);

            // Assert
            Assert.True(false);
            this.mockRepository.VerifyAll();
        }

        [Fact]
        public async Task GetDetailById_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var service = this.CreateService();
            int id = 0;

            // Act
            var result = await service.GetDetailById(
                id);

            // Assert
            Assert.True(false);
            this.mockRepository.VerifyAll();
        }

        [Fact]
        public async Task GetDetails_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var service = this.CreateService();

            // Act
            var result = await service.GetDetails();

            // Assert
            Assert.True(false);
            this.mockRepository.VerifyAll();
        }

        [Fact]
        public async Task GetDetailsWithAssets_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var service = this.CreateService();

            // Act
            var result = await service.GetDetailsWithAssets();

            // Assert
            Assert.True(false);
            this.mockRepository.VerifyAll();
        }

        [Fact]
        public async Task UpdateDetail_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var service = this.CreateService();
            int id = 0;
            string userId = null;
            DetailUpdateDto detailUpdateDto = null;

            // Act
            var result = await service.UpdateDetail(
                id,
                userId,
                detailUpdateDto);

            // Assert
            Assert.True(false);
            this.mockRepository.VerifyAll();
        }

        [Fact]
        public async Task MarkDeleteDetail_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var service = this.CreateService();
            int id = 0;
            string userId = null;

            // Act
            var result = await service.MarkDeleteDetail(
                id,
                userId);

            // Assert
            Assert.True(false);
            this.mockRepository.VerifyAll();
        }

        [Fact]
        public async Task MarkUnDeleteDetail_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var service = this.CreateService();
            int id = 0;
            string userId = null;

            // Act
            var result = await service.MarkUnDeleteDetail(
                id,
                userId);

            // Assert
            Assert.True(false);
            this.mockRepository.VerifyAll();
        }

        [Fact]
        public async Task DeleteDetail_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var service = this.CreateService();
            int id = 0;

            // Act
            var result = await service.DeleteDetail(
                id);

            // Assert
            Assert.True(false);
            this.mockRepository.VerifyAll();
        }

        [Fact]
        public async Task CreateSituation_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var service = this.CreateService();
            SituationCreateDto situationCreateDto = null;
            string userId = null;

            // Act
            var result = await service.CreateSituation(
                situationCreateDto,
                userId);

            // Assert
            Assert.True(false);
            this.mockRepository.VerifyAll();
        }

        [Fact]
        public async Task GetSituationById_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var service = this.CreateService();
            int id = 0;

            // Act
            var result = await service.GetSituationById(
                id);

            // Assert
            Assert.True(false);
            this.mockRepository.VerifyAll();
        }

        [Fact]
        public async Task GetSituations_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var service = this.CreateService();

            // Act
            var result = await service.GetSituations();

            // Assert
            Assert.True(false);
            this.mockRepository.VerifyAll();
        }

        [Fact]
        public async Task GetSituationsWithAssets_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var service = this.CreateService();

            // Act
            var result = await service.GetSituationsWithAssets();

            // Assert
            Assert.True(false);
            this.mockRepository.VerifyAll();
        }

        [Fact]
        public async Task GetSituationsWithCategories_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var service = this.CreateService();

            // Act
            var result = await service.GetSituationsWithCategories();

            // Assert
            Assert.True(false);
            this.mockRepository.VerifyAll();
        }

        [Fact]
        public async Task UpdateSituation_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var service = this.CreateService();
            int id = 0;
            string userId = null;
            SituationUpdateDto situationUpdateDto = null;

            // Act
            var result = await service.UpdateSituation(
                id,
                userId,
                situationUpdateDto);

            // Assert
            Assert.True(false);
            this.mockRepository.VerifyAll();
        }

        [Fact]
        public async Task MarkDeleteSituation_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var service = this.CreateService();
            int id = 0;
            string userId = null;

            // Act
            var result = await service.MarkDeleteSituation(
                id,
                userId);

            // Assert
            Assert.True(false);
            this.mockRepository.VerifyAll();
        }

        [Fact]
        public async Task MarkUnDeleteSituation_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var service = this.CreateService();
            int id = 0;
            string userId = null;

            // Act
            var result = await service.MarkUnDeleteSituation(
                id,
                userId);

            // Assert
            Assert.True(false);
            this.mockRepository.VerifyAll();
        }

        [Fact]
        public async Task DeleteSituation_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var service = this.CreateService();
            int id = 0;

            // Act
            var result = await service.DeleteSituation(
                id);

            // Assert
            Assert.True(false);
            this.mockRepository.VerifyAll();
        }

        [Fact]
        public async Task CreateQuestion_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var service = this.CreateService();
            QuestionCreateDto questionCreateDto = null;
            string userId = null;

            // Act
            var result = await service.CreateQuestion(
                questionCreateDto,
                userId);

            // Assert
            Assert.True(false);
            this.mockRepository.VerifyAll();
        }

        [Fact]
        public async Task GetQuestionById_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var service = this.CreateService();
            int id = 0;

            // Act
            var result = await service.GetQuestionById(
                id);

            // Assert
            Assert.True(false);
            this.mockRepository.VerifyAll();
        }

        [Fact]
        public async Task GetQuestions_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var service = this.CreateService();

            // Act
            var result = await service.GetQuestions();

            // Assert
            Assert.True(false);
            this.mockRepository.VerifyAll();
        }

        [Fact]
        public async Task GetQuestionsWithSituations_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var service = this.CreateService();

            // Act
            var result = await service.GetQuestionsWithSituations();

            // Assert
            Assert.True(false);
            this.mockRepository.VerifyAll();
        }

        [Fact]
        public async Task UpdateQuestion_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var service = this.CreateService();
            int id = 0;
            string userId = null;
            QuestionUpdateDto questionUpdateDto = null;

            // Act
            var result = await service.UpdateQuestion(
                id,
                userId,
                questionUpdateDto);

            // Assert
            Assert.True(false);
            this.mockRepository.VerifyAll();
        }

        [Fact]
        public async Task MarkDeleteQuestion_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var service = this.CreateService();
            int id = 0;
            string userId = null;

            // Act
            var result = await service.MarkDeleteQuestion(
                id,
                userId);

            // Assert
            Assert.True(false);
            this.mockRepository.VerifyAll();
        }

        [Fact]
        public async Task MarkUnDeleteQuestion_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var service = this.CreateService();
            int id = 0;
            string userId = null;

            // Act
            var result = await service.MarkUnDeleteQuestion(
                id,
                userId);

            // Assert
            Assert.True(false);
            this.mockRepository.VerifyAll();
        }

        [Fact]
        public async Task DeleteQuestion_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var service = this.CreateService();
            int id = 0;

            // Act
            var result = await service.DeleteQuestion(
                id);

            // Assert
            Assert.True(false);
            this.mockRepository.VerifyAll();
        }

        [Fact]
        public async Task CreateCommunicate_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var service = this.CreateService();
            CommunicateCreateDto communicateCreateDto = null;
            string userId = null;

            // Act
            var result = await service.CreateCommunicate(
                communicateCreateDto,
                userId);

            // Assert
            Assert.True(false);
            this.mockRepository.VerifyAll();
        }

        [Fact]
        public async Task GetCommunicateById_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var service = this.CreateService();
            int id = 0;

            // Act
            var result = await service.GetCommunicateById(
                id);

            // Assert
            Assert.True(false);
            this.mockRepository.VerifyAll();
        }

        [Fact]
        public async Task GetCommunicates_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var service = this.CreateService();

            // Act
            var result = await service.GetCommunicates();

            // Assert
            Assert.True(false);
            this.mockRepository.VerifyAll();
        }

        [Fact]
        public async Task GetCommunicatesWithAssets_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var service = this.CreateService();

            // Act
            var result = await service.GetCommunicatesWithAssets();

            // Assert
            Assert.True(false);
            this.mockRepository.VerifyAll();
        }

        [Fact]
        public async Task UpdateCommunicate_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var service = this.CreateService();
            int id = 0;
            string userId = null;
            CommunicateUpdateDto communicateUpdateDto = null;

            // Act
            var result = await service.UpdateCommunicate(
                id,
                userId,
                communicateUpdateDto);

            // Assert
            Assert.True(false);
            this.mockRepository.VerifyAll();
        }

        [Fact]
        public async Task MarkDeleteCommunicate_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var service = this.CreateService();
            int id = 0;
            string userId = null;

            // Act
            var result = await service.MarkDeleteCommunicate(
                id,
                userId);

            // Assert
            Assert.True(false);
            this.mockRepository.VerifyAll();
        }

        [Fact]
        public async Task MarkUnDeleteCommunicate_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var service = this.CreateService();
            int id = 0;
            string userId = null;

            // Act
            var result = await service.MarkUnDeleteCommunicate(
                id,
                userId);

            // Assert
            Assert.True(false);
            this.mockRepository.VerifyAll();
        }

        [Fact]
        public async Task DeleteCommunicate_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var service = this.CreateService();
            int id = 0;

            // Act
            var result = await service.DeleteCommunicate(
                id);

            // Assert
            Assert.True(false);
            this.mockRepository.VerifyAll();
        }
    }
}
