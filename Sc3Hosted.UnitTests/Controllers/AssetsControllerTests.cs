using FluentAssertions;
using Microsoft.AspNetCore.Mvc;

using Moq;

using Sc3Hosted.Server.Controllers;
using Sc3Hosted.Server.Services;
using Sc3Hosted.Shared.Dtos;
using Sc3Hosted.Shared.Enumerations;

using System.Threading.Tasks;

using Xunit;

namespace Sc3Hosted.UnitTests.Controllers
{
    public class AssetsControllerTests
    {
        private readonly MockRepository _mockRepository;

        private readonly Mock<IAssetService> _mockAssetService;

        public AssetsControllerTests()
        {
            this._mockRepository = new MockRepository(MockBehavior.Default);

            this._mockAssetService = this._mockRepository.Create<IAssetService>();
        }

        private AssetsController CreateAssetsController()
        {
            return new AssetsController(
                this._mockAssetService.Object);
        }

        [Fact]
        public async Task CreateAsset_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var assetsController = this.CreateAssetsController();
            
            
            AssetCreateDto assetCreateDto = new()
            {
                Status = Status.InUse,
                CoordinateId = 1,
                Name = "Test Asset",
                Description = "Test Asset Description",
                ModelId = 1,
                Process = "Test Process"
            };
            _mockAssetService.Setup(x => x.CreateAsset(assetCreateDto).Result).Returns(1);
            
            // Act
            var result = await assetsController.CreateAsset(
                assetCreateDto);

            // Assert
            result.Should().BeOfType<CreatedResult>();
            assetsController.ModelState.IsValid.Should().BeTrue();
        }

        [Fact]
        public async Task DeleteAsset_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var assetsController = this.CreateAssetsController();
            int assetId = 0;

            // Act
            var result = await assetsController.DeleteAsset(
                assetId);

            // Assert
            result.Should().BeOfType<NoContentResult>();
            this._mockRepository.VerifyAll();
        }

        [Fact]
        public async Task GetAssetById_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var assetsController = this.CreateAssetsController();
            int assetId = 0;

            // Act
            var result = await assetsController.GetAssetById(
                assetId);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            this._mockRepository.VerifyAll();
        }

        [Fact]
        public async Task GetAssets_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var assetsController = this.CreateAssetsController();

            // Act
            var result = await assetsController.GetAssets();

            // Assert
                        result.Should().BeOfType<OkObjectResult>();
            this._mockRepository.VerifyAll();
        }

        [Fact]
        public async Task GetAssetDisplays_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var assetsController = this.CreateAssetsController();

            // Act
            var result = await assetsController.GetAssetDisplays();

            // Assert
                        result.Should().BeOfType<OkObjectResult>();
            this._mockRepository.VerifyAll();
        }

        [Fact]
        public async Task ChangeModelOfAsset_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var assetsController = this.CreateAssetsController();
            int assetId = 0;
            int modelId = 0;

            // Act
            var result = await assetsController.ChangeModelOfAsset(
                assetId,
                modelId);

            // Assert
                        result.Should().BeOfType<NoContentResult>();
            this._mockRepository.VerifyAll();
        }

        [Fact]
        public async Task UpdateAssetName_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var assetsController = this.CreateAssetsController();
            int assetId = 1;
            string name = "Test";

            // Act
            var result = await assetsController.UpdateAssetName(
                assetId,
                name);

            // Assert
                        result.Should().BeOfType<NoContentResult>();
            this._mockRepository.VerifyAll();
        }

        [Fact]
        public async Task CreateAssetCategory_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var assetsController = this.CreateAssetsController();
            int assetId = 0;
            int categoryId = 0;

            // Act
            var result = await assetsController.CreateAssetCategory(
                assetId,
                categoryId);

            // Assert
                        result.Should().BeOfType<CreatedResult>();
            this._mockRepository.VerifyAll();
        }

        [Fact]
        public async Task DeleteAssetCategory_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var assetsController = this.CreateAssetsController();
            int assetId = 0;
            int categoryId = 0;

            // Act
            var result = await assetsController.DeleteAssetCategory(
                assetId,
                categoryId);

            // Assert
                        result.Should().BeOfType<NoContentResult>();
            this._mockRepository.VerifyAll();
        }

        [Fact]
        public async Task MarkDeleteAssetCategory_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var assetsController = this.CreateAssetsController();
            int assetId = 0;
            int categoryId = 0;

            // Act
            var result = await assetsController.MarkDeleteAssetCategory(
                assetId,
                categoryId);

            // Assert
                        result.Should().BeOfType<NoContentResult>();
            this._mockRepository.VerifyAll();
        }

        [Fact]
        public async Task UpdateAssetCategory_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var assetsController = this.CreateAssetsController();
            int assetId = 0;
            int categoryId = 0;

            // Act
            var result = await assetsController.UpdateAssetCategory(
                assetId,
                categoryId);

            // Assert
                        result.Should().BeOfType<NoContentResult>();
            this._mockRepository.VerifyAll();
        }

        [Fact]
        public async Task CreateAssetDetail_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var assetsController = this.CreateAssetsController();
            AssetDetailDto assetDetailDto = new()
            {
                AssetId = 1,
                Value = "Test Asset Detail Value",
                DetailId = 1
            };

            // Act
            var result = await assetsController.CreateAssetDetail(
                assetDetailDto);

            // Assert
                        result.Should().BeOfType<CreatedResult>();
            this._mockRepository.VerifyAll();
        }

        [Fact]
        public async Task DeleteAssetDetail_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var assetsController = this.CreateAssetsController();
            int assetId = 0;
            int detailId = 0;

            // Act
            var result = await assetsController.DeleteAssetDetail(
                assetId,
                detailId);

            // Assert
                        result.Should().BeOfType<NoContentResult>();
            this._mockRepository.VerifyAll();
        }

        [Fact]
        public async Task MarkDeleteAssetDetail_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var assetsController = this.CreateAssetsController();
            int assetId = 0;
            int detailId = 0;

            // Act
            var result = await assetsController.MarkDeleteAssetDetail(
                assetId,
                detailId);

            // Assert
                        result.Should().BeOfType<NoContentResult>();
            this._mockRepository.VerifyAll();
        }

        [Fact]
        public async Task UpdateAssetDetail_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var assetsController = this.CreateAssetsController();
            AssetDetailDto assetDetailDto = new()
            {
                AssetId = 1,
                Value = "Test Asset Detail Value",
                DetailId = 1
            };

            // Act
            var result = await assetsController.UpdateAssetDetail(
                assetDetailDto);

            // Assert
                        result.Should().BeOfType<NoContentResult>();
            this._mockRepository.VerifyAll();
        }

        [Fact]
        public async Task MarkDeleteAsset_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var assetsController = this.CreateAssetsController();
            int assetId = 0;

            // Act
            var result = await assetsController.MarkDeleteAsset(
                assetId);

            // Assert
                        result.Should().BeOfType<NoContentResult>();
            this._mockRepository.VerifyAll();
        }

        [Fact]
        public async Task UpdateAsset_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var assetsController = this.CreateAssetsController();
            int assetId = 0;
            AssetUpdateDto assetUpdateDto = new()
            {
                CoordinateId = 1,
                Description = "Test Asset Description",
                Process = "Test Asset Process",
                Status = Status.InUse
            };

            // Act
            var result = await assetsController.UpdateAsset(
                assetId,
                assetUpdateDto);

            // Assert
                        result.Should().BeOfType<NoContentResult>();
            this._mockRepository.VerifyAll();
        }
    }
}
