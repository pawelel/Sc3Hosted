using FluentAssertions;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Moq;

using Sc3Hosted.Server.Data;
using Sc3Hosted.Server.Entities;
using Sc3Hosted.Server.Exceptions;
using Sc3Hosted.Server.Services;
using Sc3Hosted.Shared.Dtos;

using System;
using System.Threading.Tasks;

using Xunit;

namespace Sc3Hosted.UnitTests.Services
{
    public class AssetServiceTests
    {
        private readonly ApplicationDbContext _context;
        private readonly Mock<ILogger<AssetService>> _loggerStub;
        public AssetServiceTests()
        {
            var dbOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .EnableSensitiveDataLogging();
            _context = new ApplicationDbContext(dbOptions.Options);
            _loggerStub = new Mock<ILogger<AssetService>>();
        }
        private void AddModelsWithCoordinatesAnd1Asset()
        {
            var model1 = new Model { ModelId = 1, Name = "Model1" };
            _context.Add(model1);
            _context.SaveChanges();
            var model2 = new Model { ModelId = 2, Name = "Model2" };
            _context.Add(model2);
            _context.SaveChanges();
            var coordinate1 = new Coordinate { CoordinateId = 1, Name = "Coordinate1" };
            _context.Add(coordinate1);
            _context.SaveChanges();
            var coordinate2 = new Coordinate { CoordinateId = 2, Name = "Coordinate2" };
            _context.Add(coordinate2);
            _context.SaveChanges();
            _context.Assets.Add(new Asset { AssetId = 1, Model = model1, Coordinate = coordinate1, Name = "Asset1" });
            _context.SaveChanges();
        }


        [Fact]
        public async Task ChangeModelOfAsset_WithUnexistingAsset_ThrowsNotFound()
        {
            // Arrange
            var sut = new AssetService(_context, _loggerStub.Object);

            var assetId = 1;
            var modelId = 2;

            //// Act
            Func<Task> act = () => sut.ChangeModelOfAsset(assetId, modelId);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>();

        }
        [Fact]
        public async Task ChangeModelOfAsset_WithMatchingAsset_ReturnsNoContent()
        {
            // Arrange
            AddModelsWithCoordinatesAnd1Asset();

            var sut = new AssetService(_context, _loggerStub.Object);


            var assetId = 1;
            var modelId = 2;

            //// Act
            await sut.ChangeModelOfAsset(assetId, modelId);
            var asset = await _context.Assets.FirstOrDefaultAsync(a => a.AssetId == assetId);
            // Assert

            asset?.ModelId.Should().Be(modelId);

        }



        [Fact]
        public async Task CreateAsset_WhenAssetExists_Throws400BadRequst()
        {
            // Arrange
            var sut = new AssetService(_context, _loggerStub.Object);
            AddModelsWithCoordinatesAnd1Asset();
            AssetCreateDto assetCreateDto = new()
            {
                Name = "Asset1",
                CoordinateId = 1,
                ModelId = 1
            };

            // Act
            Func<Task> act = () => sut.CreateAsset(assetCreateDto);

            // Assert
            await act.Should().ThrowAsync<BadRequestException>();

        }

        [Fact]
        public async Task CreateAssetCategory_WhenAssetAndCategoryExistsAndThereIsNoDuplicate_ShouldAddAssetCategory()
        {
            // Arrange
            var sut = new AssetService(_context, _loggerStub.Object);
            AddModelsWithCoordinatesAnd1Asset();
            var category = new Category { CategoryId = 1, Name = "Category1" };
            _context.Add(category);
            _context.SaveChanges();
            int assetId = 1;
            int categoryId = 1;

            // Act
            var result = await sut.CreateAssetCategory(
                assetId,
                categoryId);

            // Assert
            Assert.Equal(1, result.Item1);
            Assert.Equal(1, result.Item2);
        }
        [Fact]
        public async Task CreateAssetCategory_WhenAssetCategoryExists_Throws400BadRequest()
        {
            // Arrange
            var sut = new AssetService(_context, _loggerStub.Object);
            AddModelsWithCoordinatesAnd1Asset();
            var category = new Category { CategoryId = 1, Name = "Category1" };
            _context.Add(category);
            await _context.SaveChangesAsync();
            var assetCategory = new AssetCategory { IsDeleted = false, Category = category, Asset = _context.Assets.Find(1)! };

            _context.Add(assetCategory);
            await _context.SaveChangesAsync();
            int assetId = 1;
            int categoryId = 1;

            // Act
          var act = async () => await  sut.CreateAssetCategory(
                assetId,
                categoryId);
            
            // Assert
            BadRequestException ex = await Assert.ThrowsAsync<BadRequestException>(act);
            Assert.Equal("AssetCategory already exists", ex.Message);
        }

        [Fact]
        public async Task CreateAssetDetail_ProperAssetDetail_ReturnsNoContentResult()
        {
            // Arrange
            AddModelsWithCoordinatesAnd1Asset();
            var detail = new Detail { DetailId = 1, Name = "Detail1" };
            _context.Add(detail);
            await _context.SaveChangesAsync();
            var sut = new AssetService(_context, _loggerStub.Object);
            AssetDetailDto assetDetailDto = new() { AssetId = 1, DetailId = 1, Value = "Value1" };

            // Act
            var result = await sut.CreateAssetDetail(
                assetDetailDto);

            // Assert
            Assert.Equal(1, result.Item1);
            Assert.Equal(1, result.Item2);
        }

        //[Fact]
        //public async Task CreateCategory_StateUnderTest_ExpectedBehavior()
        //{
        //    // Arrange
        //    var sut = new AssetService(_context, _loggerStub.Object);
        //    CategoryCreateDto categoryCreateDto = null;

        //    // Act
        //    var result = await sut.CreateCategory(
        //        categoryCreateDto);

        //    // Assert
        //    Assert.True(false);
        //}

        //[Fact]
        //public async Task CreateDetail_StateUnderTest_ExpectedBehavior()
        //{
        //    // Arrange
        //    var sut = new AssetService(_context, _loggerStub.Object);
        //    DetailCreateDto detailCreateDto = null;

        //    // Act
        //    var result = await sut.CreateDetail(
        //        detailCreateDto);

        //    // Assert
        //    Assert.True(false);
        //}

        //[Fact]
        //public async Task CreateDevice_StateUnderTest_ExpectedBehavior()
        //{
        //    // Arrange
        //    var sut = new AssetService(_context, _loggerStub.Object);
        //    DeviceCreateDto deviceCreateDto = null;

        //    // Act
        //    var result = await sut.CreateDevice(
        //        deviceCreateDto);

        //    // Assert
        //    Assert.True(false);
        //}

        //[Fact]
        //public async Task CreateModel_StateUnderTest_ExpectedBehavior()
        //{
        //    // Arrange
        //    var sut = new AssetService(_context, _loggerStub.Object);
        //    int deviceId = 0;
        //    ModelCreateDto modelCreateDto = null;

        //    // Act
        //    var result = await sut.CreateModel(
        //        deviceId,
        //        modelCreateDto);

        //    // Assert
        //    Assert.True(false);
        //}

        //[Fact]
        //public async Task CreateModelParameter_StateUnderTest_ExpectedBehavior()
        //{
        //    // Arrange
        //    var sut = new AssetService(_context, _loggerStub.Object);
        //    ModelParameterDto modelParameterDto = null;

        //    // Act
        //    var result = await sut.CreateModelParameter(
        //        modelParameterDto);

        //    // Assert
        //    Assert.True(false);
        //}

        //[Fact]
        //public async Task CreateParameter_StateUnderTest_ExpectedBehavior()
        //{
        //    // Arrange
        //    var sut = new AssetService(_context, _loggerStub.Object);
        //    ParameterCreateDto parameterCreateDto = null;

        //    // Act
        //    var result = await sut.CreateParameter(
        //        parameterCreateDto);

        //    // Assert
        //    Assert.True(false);
        //}

        //[Fact]
        //public async Task DeleteAsset_StateUnderTest_ExpectedBehavior()
        //{
        //    // Arrange
        //    var sut = new AssetService(_context, _loggerStub.Object);
        //    int assetId = 0;

        //    // Act
        //    await sut.DeleteAsset(
        //        assetId);

        //    // Assert
        //    Assert.True(false);
        //}

        //[Fact]
        //public async Task DeleteAssetCategory_StateUnderTest_ExpectedBehavior()
        //{
        //    // Arrange
        //    var sut = new AssetService(_context, _loggerStub.Object);
        //    int assetId = 0;
        //    int categoryId = 0;

        //    // Act
        //    await sut.DeleteAssetCategory(
        //        assetId,
        //        categoryId);

        //    // Assert
        //    Assert.True(false);
        //}

        //[Fact]
        //public async Task DeleteAssetDetail_StateUnderTest_ExpectedBehavior()
        //{
        //    // Arrange
        //    var sut = new AssetService(_context, _loggerStub.Object);
        //    int assetId = 0;
        //    int detailId = 0;

        //    // Act
        //    await sut.DeleteAssetDetail(
        //        assetId,
        //        detailId);

        //    // Assert
        //    Assert.True(false);
        //}

        //[Fact]
        //public async Task DeleteCategory_StateUnderTest_ExpectedBehavior()
        //{
        //    // Arrange
        //    var sut = new AssetService(_context, _loggerStub.Object);
        //    int categoryId = 0;

        //    // Act
        //    await sut.DeleteCategory(
        //        categoryId);

        //    // Assert
        //    Assert.True(false);
        //}

        //[Fact]
        //public async Task DeleteDetail_StateUnderTest_ExpectedBehavior()
        //{
        //    // Arrange
        //    var sut = new AssetService(_context, _loggerStub.Object);
        //    int detailId = 0;

        //    // Act
        //    await sut.DeleteDetail(
        //        detailId);

        //    // Assert
        //    Assert.True(false);
        //}

        //[Fact]
        //public async Task DeleteDevice_StateUnderTest_ExpectedBehavior()
        //{
        //    // Arrange
        //    var sut = new AssetService(_context, _loggerStub.Object);
        //    int deviceId = 0;

        //    // Act
        //    await sut.DeleteDevice(
        //        deviceId);

        //    // Assert
        //    Assert.True(false);
        //}

        //[Fact]
        //public async Task DeleteModel_StateUnderTest_ExpectedBehavior()
        //{
        //    // Arrange
        //    var sut = new AssetService(_context, _loggerStub.Object);
        //    int modelId = 0;

        //    // Act
        //    await sut.DeleteModel(
        //        modelId);

        //    // Assert
        //    Assert.True(false);
        //}

        //[Fact]
        //public async Task DeleteModelParameter_StateUnderTest_ExpectedBehavior()
        //{
        //    // Arrange
        //    var sut = new AssetService(_context, _loggerStub.Object);
        //    int modelId = 0;
        //    int parameterId = 0;

        //    // Act
        //    await sut.DeleteModelParameter(
        //        modelId,
        //        parameterId);

        //    // Assert
        //    Assert.True(false);
        //}

        //[Fact]
        //public async Task DeleteParameter_StateUnderTest_ExpectedBehavior()
        //{
        //    // Arrange
        //    var sut = new AssetService(_context, _loggerStub.Object);
        //    int parameterId = 0;

        //    // Act
        //    await sut.DeleteParameter(
        //        parameterId);

        //    // Assert
        //    Assert.True(false);
        //}

        //[Fact]
        //public async Task GetAssetById_StateUnderTest_ExpectedBehavior()
        //{
        //    // Arrange
        //    var sut = new AssetService(_context, _loggerStub.Object);
        //    int assetId = 0;

        //    // Act
        //    var result = await sut.GetAssetById(
        //        assetId);

        //    // Assert
        //    Assert.True(false);
        //}

        //[Fact]
        //public async Task GetAssetDisplays_StateUnderTest_ExpectedBehavior()
        //{
        //    // Arrange
        //    var sut = new AssetService(_context, _loggerStub.Object);

        //    // Act
        //    var result = await sut.GetAssetDisplays();

        //    // Assert
        //    Assert.True(false);
        //}

        //[Fact]
        //public async Task GetAssets_StateUnderTest_ExpectedBehavior()
        //{
        //    // Arrange
        //    var sut = new AssetService(_context, _loggerStub.Object);

        //    // Act
        //    var result = await sut.GetAssets();

        //    // Assert
        //    Assert.True(false);
        //}

        //[Fact]
        //public async Task GetCategories_StateUnderTest_ExpectedBehavior()
        //{
        //    // Arrange
        //    var sut = new AssetService(_context, _loggerStub.Object);

        //    // Act
        //    var result = await sut.GetCategories();

        //    // Assert
        //    Assert.True(false);
        //}

        //[Fact]
        //public async Task GetCategoriesWithAssets_StateUnderTest_ExpectedBehavior()
        //{
        //    // Arrange
        //    var sut = new AssetService(_context, _loggerStub.Object);

        //    // Act
        //    var result = await sut.GetCategoriesWithAssets();

        //    // Assert
        //    Assert.True(false);
        //}

        //[Fact]
        //public async Task GetCategoryById_StateUnderTest_ExpectedBehavior()
        //{
        //    // Arrange
        //    var sut = new AssetService(_context, _loggerStub.Object);
        //    int categoryId = 0;

        //    // Act
        //    var result = await sut.GetCategoryById(
        //        categoryId);

        //    // Assert
        //    Assert.True(false);
        //}

        //[Fact]
        //public async Task GetCategoryByIdWithAssets_StateUnderTest_ExpectedBehavior()
        //{
        //    // Arrange
        //    var sut = new AssetService(_context, _loggerStub.Object);
        //    int categoryId = 0;

        //    // Act
        //    var result = await sut.GetCategoryByIdWithAssets(
        //        categoryId);

        //    // Assert
        //    Assert.True(false);
        //}

        //[Fact]
        //public async Task GetDetailById_StateUnderTest_ExpectedBehavior()
        //{
        //    // Arrange
        //    var sut = new AssetService(_context, _loggerStub.Object);
        //    int detailId = 0;

        //    // Act
        //    var result = await sut.GetDetailById(
        //        detailId);

        //    // Assert
        //    Assert.True(false);
        //}

        //[Fact]
        //public async Task GetDetails_StateUnderTest_ExpectedBehavior()
        //{
        //    // Arrange
        //    var sut = new AssetService(_context, _loggerStub.Object);

        //    // Act
        //    var result = await sut.GetDetails();

        //    // Assert
        //    Assert.True(false);
        //}

        //[Fact]
        //public async Task GetDetailsWithAssets_StateUnderTest_ExpectedBehavior()
        //{
        //    // Arrange
        //    var sut = new AssetService(_context, _loggerStub.Object);

        //    // Act
        //    var result = await sut.GetDetailsWithAssets();

        //    // Assert
        //    Assert.True(false);
        //}

        //[Fact]
        //public async Task GetDeviceById_StateUnderTest_ExpectedBehavior()
        //{
        //    // Arrange
        //    var sut = new AssetService(_context, _loggerStub.Object);
        //    int deviceId = 0;

        //    // Act
        //    var result = await sut.GetDeviceById(
        //        deviceId);

        //    // Assert
        //    Assert.True(false);
        //}

        //[Fact]
        //public async Task GetDevices_StateUnderTest_ExpectedBehavior()
        //{
        //    // Arrange
        //    var sut = new AssetService(_context, _loggerStub.Object);

        //    // Act
        //    var result = await sut.GetDevices();

        //    // Assert
        //    Assert.True(false);
        //}

        //[Fact]
        //public async Task GetDevicesWithModels_StateUnderTest_ExpectedBehavior()
        //{
        //    // Arrange
        //    var sut = new AssetService(_context, _loggerStub.Object);

        //    // Act
        //    var result = await sut.GetDevicesWithModels();

        //    // Assert
        //    Assert.True(false);
        //}

        //[Fact]
        //public async Task GetDevicesWithAssets_StateUnderTest_ExpectedBehavior()
        //{
        //    // Arrange
        //    var sut = new AssetService(_context, _loggerStub.Object);

        //    // Act
        //    var result = await sut.GetDevicesWithAssets();

        //    // Assert
        //    Assert.True(false);
        //}

        //[Fact]
        //public async Task GetDeviceWithAssets_StateUnderTest_ExpectedBehavior()
        //{
        //    // Arrange
        //    var sut = new AssetService(_context, _loggerStub.Object);
        //    int deviceId = 0;

        //    // Act
        //    var result = await sut.GetDeviceWithAssets(
        //        deviceId);

        //    // Assert
        //    Assert.True(false);
        //}

        //[Fact]
        //public async Task GetModelById_StateUnderTest_ExpectedBehavior()
        //{
        //    // Arrange
        //    var sut = new AssetService(_context, _loggerStub.Object);
        //    int modelId = 0;

        //    // Act
        //    var result = await sut.GetModelById(
        //        modelId);

        //    // Assert
        //    Assert.True(false);
        //}

        //[Fact]
        //public async Task GetModels_StateUnderTest_ExpectedBehavior()
        //{
        //    // Arrange
        //    var sut = new AssetService(_context, _loggerStub.Object);

        //    // Act
        //    var result = await sut.GetModels();

        //    // Assert
        //    Assert.True(false);
        //}

        //[Fact]
        //public async Task GetModelsWithAssets_StateUnderTest_ExpectedBehavior()
        //{
        //    // Arrange
        //    var sut = new AssetService(_context, _loggerStub.Object);

        //    // Act
        //    var result = await sut.GetModelsWithAssets();

        //    // Assert
        //    Assert.True(false);
        //}

        //[Fact]
        //public async Task GetParameterById_StateUnderTest_ExpectedBehavior()
        //{
        //    // Arrange
        //    var sut = new AssetService(_context, _loggerStub.Object);
        //    int parameterId = 0;

        //    // Act
        //    var result = await sut.GetParameterById(
        //        parameterId);

        //    // Assert
        //    Assert.True(false);
        //}

        //[Fact]
        //public async Task GetParameters_StateUnderTest_ExpectedBehavior()
        //{
        //    // Arrange
        //    var sut = new AssetService(_context, _loggerStub.Object);

        //    // Act
        //    var result = await sut.GetParameters();

        //    // Assert
        //    Assert.True(false);
        //}

        //[Fact]
        //public async Task GetParametersWithModels_StateUnderTest_ExpectedBehavior()
        //{
        //    // Arrange
        //    var sut = new AssetService(_context, _loggerStub.Object);

        //    // Act
        //    var result = await sut.GetParametersWithModels();

        //    // Assert
        //    Assert.True(false);
        //}

        //[Fact]
        //public async Task MarkDeleteAsset_StateUnderTest_ExpectedBehavior()
        //{
        //    // Arrange
        //    var sut = new AssetService(_context, _loggerStub.Object);
        //    int assetId = 0;

        //    // Act
        //    await sut.MarkDeleteAsset(
        //        assetId);

        //    // Assert
        //    Assert.True(false);
        //}

        //[Fact]
        //public async Task MarkDeleteAssetCategory_StateUnderTest_ExpectedBehavior()
        //{
        //    // Arrange
        //    var sut = new AssetService(_context, _loggerStub.Object);
        //    int assetId = 0;
        //    int categoryId = 0;

        //    // Act
        //    await sut.MarkDeleteAssetCategory(
        //        assetId,
        //        categoryId);

        //    // Assert
        //    Assert.True(false);
        //}

        //[Fact]
        //public async Task MarkDeleteAssetDetail_StateUnderTest_ExpectedBehavior()
        //{
        //    // Arrange
        //    var sut = new AssetService(_context, _loggerStub.Object);
        //    int assetId = 0;
        //    int detailId = 0;

        //    // Act
        //    await sut.MarkDeleteAssetDetail(
        //        assetId,
        //        detailId);

        //    // Assert
        //    Assert.True(false);
        //}

        //[Fact]
        //public async Task MarkDeleteCategory_StateUnderTest_ExpectedBehavior()
        //{
        //    // Arrange
        //    var sut = new AssetService(_context, _loggerStub.Object);
        //    int categoryId = 0;

        //    // Act
        //    await sut.MarkDeleteCategory(
        //        categoryId);

        //    // Assert
        //    Assert.True(false);
        //}

        //[Fact]
        //public async Task MarkDeleteDetail_StateUnderTest_ExpectedBehavior()
        //{
        //    // Arrange
        //    var sut = new AssetService(_context, _loggerStub.Object);
        //    int detailId = 0;

        //    // Act
        //    await sut.MarkDeleteDetail(
        //        detailId);

        //    // Assert
        //    Assert.True(false);
        //}

        //[Fact]
        //public async Task MarkDeleteDevice_StateUnderTest_ExpectedBehavior()
        //{
        //    // Arrange
        //    var sut = new AssetService(_context, _loggerStub.Object);
        //    int deviceId = 0;

        //    // Act
        //    await sut.MarkDeleteDevice(
        //        deviceId);

        //    // Assert
        //    Assert.True(false);
        //}

        //[Fact]
        //public async Task MarkDeleteModel_StateUnderTest_ExpectedBehavior()
        //{
        //    // Arrange
        //    var sut = new AssetService(_context, _loggerStub.Object);
        //    int modelId = 0;

        //    // Act
        //    await sut.MarkDeleteModel(
        //        modelId);

        //    // Assert
        //    Assert.True(false);
        //}

        //[Fact]
        //public async Task MarkDeleteModelParameter_StateUnderTest_ExpectedBehavior()
        //{
        //    // Arrange
        //    var sut = new AssetService(_context, _loggerStub.Object);
        //    int modelId = 0;
        //    int parameterId = 0;

        //    // Act
        //    await sut.MarkDeleteModelParameter(
        //        modelId,
        //        parameterId);

        //    // Assert
        //    Assert.True(false);
        //}

        //[Fact]
        //public async Task MarkDeleteParameter_StateUnderTest_ExpectedBehavior()
        //{
        //    // Arrange
        //    var sut = new AssetService(_context, _loggerStub.Object);
        //    int parameterId = 0;

        //    // Act
        //    await sut.MarkDeleteParameter(
        //        parameterId);

        //    // Assert
        //    Assert.True(false);
        //}

        //[Fact]
        //public async Task UpdateAsset_StateUnderTest_ExpectedBehavior()
        //{
        //    // Arrange
        //    var sut = new AssetService(_context, _loggerStub.Object);
        //    int assetId = 0;
        //    AssetUpdateDto assetUpdateDto = null;

        //    // Act
        //    await sut.UpdateAsset(
        //        assetId,
        //        assetUpdateDto);

        //    // Assert
        //    Assert.True(false);
        //}

        //[Fact]
        //public async Task UpdateAssetName_StateUnderTest_ExpectedBehavior()
        //{
        //    // Arrange
        //    var sut = new AssetService(_context, _loggerStub.Object);
        //    int assetId = 0;
        //    string name = null;

        //    // Act
        //    await sut.UpdateAssetName(
        //        assetId,
        //        name);

        //    // Assert
        //    Assert.True(false);
        //}

        //[Fact]
        //public async Task UpdateAssetCategory_StateUnderTest_ExpectedBehavior()
        //{
        //    // Arrange
        //    var sut = new AssetService(_context, _loggerStub.Object);
        //    int assetId = 0;
        //    int categoryId = 0;

        //    // Act
        //    await sut.UpdateAssetCategory(
        //        assetId,
        //        categoryId);

        //    // Assert
        //    Assert.True(false);
        //}

        //[Fact]
        //public async Task UpdateAssetDetail_StateUnderTest_ExpectedBehavior()
        //{
        //    // Arrange
        //    var sut = new AssetService(_context, _loggerStub.Object);
        //    AssetDetailDto assetDetailDto = null;

        //    // Act
        //    await sut.UpdateAssetDetail(
        //        assetDetailDto);

        //    // Assert
        //    Assert.True(false);
        //}

        //[Fact]
        //public async Task UpdateCategory_StateUnderTest_ExpectedBehavior()
        //{
        //    // Arrange
        //    var sut = new AssetService(_context, _loggerStub.Object);
        //    int categoryId = 0;
        //    CategoryUpdateDto categoryUpdateDto = null;

        //    // Act
        //    await sut.UpdateCategory(
        //        categoryId,
        //        categoryUpdateDto);

        //    // Assert
        //    Assert.True(false);
        //}

        //[Fact]
        //public async Task UpdateDetail_StateUnderTest_ExpectedBehavior()
        //{
        //    // Arrange
        //    var sut = new AssetService(_context, _loggerStub.Object);
        //    int detailId = 0;
        //    DetailUpdateDto detailUpdateDto = null;

        //    // Act
        //    await sut.UpdateDetail(
        //        detailId,
        //        detailUpdateDto);

        //    // Assert
        //    Assert.True(false);
        //}

        //[Fact]
        //public async Task UpdateDevice_StateUnderTest_ExpectedBehavior()
        //{
        //    // Arrange
        //    var sut = new AssetService(_context, _loggerStub.Object);
        //    int deviceId = 0;
        //    DeviceUpdateDto deviceUpdateDto = null;

        //    // Act
        //    await sut.UpdateDevice(
        //        deviceId,
        //        deviceUpdateDto);

        //    // Assert
        //    Assert.True(false);
        //}

        //[Fact]
        //public async Task UpdateModel_StateUnderTest_ExpectedBehavior()
        //{
        //    // Arrange
        //    var sut = new AssetService(_context, _loggerStub.Object);
        //    int modelId = 0;
        //    ModelUpdateDto modelUpdateDto = null;

        //    // Act
        //    await sut.UpdateModel(
        //        modelId,
        //        modelUpdateDto);

        //    // Assert
        //    Assert.True(false);
        //}

        //[Fact]
        //public async Task UpdateModelParameter_StateUnderTest_ExpectedBehavior()
        //{
        //    // Arrange
        //    var sut = new AssetService(_context, _loggerStub.Object);
        //    ModelParameterDto modelParameterDto = null;

        //    // Act
        //    await sut.UpdateModelParameter(
        //        modelParameterDto);

        //    // Assert
        //    Assert.True(false);
        //}

        //[Fact]
        //public async Task UpdateParameter_StateUnderTest_ExpectedBehavior()
        //{
        //    // Arrange
        //    var sut = new AssetService(_context, _loggerStub.Object);
        //    int parameterId = 0;
        //    ParameterUpdateDto parameterUpdateDto = null;

        //    // Act
        //    await sut.UpdateParameter(
        //        parameterId,
        //        parameterUpdateDto);

        //    // Assert
        //    Assert.True(false);
        //}
    }
}

