using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using TxAssigmentUnitTests.Mocks;
using TxAssignmentInfra.Entities;
using TxAssignmentInfra.Entities.Enumerators;
using TxAssignmentInfra.Repositories;
using TxAssignmentServices.Models;
using TxAssignmentServices.Profiles;
using TxAssignmentServices.Services;

namespace TxAssigmentUnitTests.Services
{
    [TestClass]
    public class ServiceCabinetTests
    {
        private Mock<IRepositoryCabinet> _mockRepo;
        private Mock<IMapper> _mockMapper;
        private Mock<ILogger<ServiceCabinet>> _mockLogger;
        private ServiceCabinet _service;

        private readonly IConfigurationProvider _configuration;
        private readonly IMapper _mapper;

        public ServiceCabinetTests()
        {
            _configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<ProfileCabinet>();
            });

            _mapper = _configuration.CreateMapper();
        }


        [TestInitialize]
        public void Setup()
        {
            _mockRepo = new Mock<IRepositoryCabinet>();
            _mockMapper = new Mock<IMapper>();
            _mockLogger = new Mock<ILogger<ServiceCabinet>>();
            _service = new ServiceCabinet(_mockRepo.Object, _mockMapper.Object, _mockLogger.Object);
        }

        [TestMethod]
        public async Task CreateCabinet_ShouldReturnSuccess_WhenCabinetIsValid()
        {
            // Arrange
            var cabinetEntity = new MockBuilderCabinet()
                                .BuildRow(1, 50, new Size { Height = 40 })
                                .BuildLane(1, 0)
                                .BuildProduct("4902102112778", "午後の紅茶レモンティー/Afternoon tea lemon 500ml", 0.059, 0.059, 0.217, "https://operationmanagerstorage.blob.core.windows.net/skus/4909411084875_1685061792.jpg", 500, 1659397548, EnumProductShape.Bottle)
                                .BuildProduct("4902102112761", "午後の紅茶ミルクティー/Afternoon tea milk 500ml", 0.059, 0.059, 0.217, "https://operationmanagerstorage.blob.core.windows.net/skus/4909411084950_1685061819.jpg", 500, 1659397548, EnumProductShape.Bottle)
                                .BuildProduct("4902102112754", "紅茶花伝ロイヤルミルクティー/Kouchakaden Royal Milk 440ml", 0.069, 0.069, 0.174, "https://operationmanagerstorage.blob.core.windows.net/skus/4902102136716_1659397560.jpg", 440, 1659397548, EnumProductShape.Bottle)
                                .BuildProduct("4902102112747", "伊右衛門カフェジャスミンティーラテ/Cafe Jasmine Tea latte 500ml", 0.072, 0.072, 0.177, "https://operationmanagerstorage.blob.core.windows.net/skus/4901777381254_1659397560.jpg", 500, 1659397548, EnumProductShape.Bottle)
                                .BuildProduct("4902102112730", "ウェルチ１房分のぶどう/welch 1 bunch 470ml", 0.072, 0.072, 0.173, "https://operationmanagerstorage.blob.core.windows.net/skus/4901340064041_1659397560.jpg", 470, 1659397548, EnumProductShape.Bottle)
                                .BuildProduct("4902102112723", "トロピカーナ100%オレンジ/Tropicana 100%orange 330ml", 0.06, 0.06, 0.184, "https://operationmanagerstorage.blob.core.windows.net/skus/4909411073114_1659397561.jpg", 330, 1659397548, EnumProductShape.Bottle)
                                .BuildProduct("4902102112716", "サントリー天然水/Suntory Natural Water 2000ml", 0.091, 0.106, 0.311, "https://operationmanagerstorage.blob.core.windows.net/skus/4901777018686_1679809773.jpg", 2000, 1659397548, EnumProductShape.Bottle)
                                .BuildProduct("4902102112709", "エビアン/Evian750ml", 0.082, 0.082, 0.21, "https://operationmanagerstorage.blob.core.windows.net/skus/3068320116266_1659397561.jpg", 750, 1659397548, EnumProductShape.Bottle)
                                .BuildProduct("4902102112693", "天然水きりっと果実オレンジ＆マンゴー/Dried fruit orange & mango Water600ml", 0.068, 0.068, 0.224, "https://operationmanagerstorage.blob.core.windows.net/skus/4901777375901_1678168618.jpg", 600, 1659397548, EnumProductShape.Bottle)
                                .Build();

            var cabinetModel = _mapper.Map<ModelCabinet>(cabinetEntity);

            _mockMapper.Setup(m => m.Map<Cabinet>(It.IsAny<ModelCabinet>())).Returns(cabinetEntity);
            _mockRepo.Setup(r => r.CreateCabinet(cabinetEntity)).ReturnsAsync(new RepositoryResponse { Success = true });

            // Act
            var result = await _service.CreateCabinet(cabinetModel);

            // Assert
            cabinetEntity.Should().BeEquivalentTo(cabinetModel, options => options.ComparingByMembers<ModelCabinet>());
            Assert.IsTrue(result.Success);
            Assert.AreEqual("Cabinet created successfully.", result.Message);
            _mockRepo.Verify(r => r.CreateCabinet(cabinetEntity), Times.Once);
        }

        [TestMethod]
        public async Task DeleteCabinet_ShouldReturnSuccess_WhenCabinetExists()
        {
            // Arrange
            var cabinetId = Guid.NewGuid();
            _mockRepo.Setup(r => r.DeleteCabinet(cabinetId)).ReturnsAsync(new RepositoryResponse { Success = true });

            // Act
            var result = await _service.DeleteCabinet(cabinetId);

            // Assert
            Assert.IsTrue(result.Success);
            Assert.AreEqual("Cabinet deleted successfully.", result.Message);
            _mockRepo.Verify(r => r.DeleteCabinet(cabinetId), Times.Once);
        }


        [TestMethod]
        public async Task GetCabinetById_ShouldReturnCabinet_WhenCabinetExists()
        {
            // Arrange
            var cabinetId = Guid.NewGuid();
            var cabinetEntity = new Cabinet { /* Initialize properties */ };
            var cabinetModel = new ModelCabinet { /* Initialize properties */ };

            _mockRepo.Setup(r => r.GetCabinetById(cabinetId)).ReturnsAsync(new RepositoryResponse<Cabinet> { Success = true, Data = cabinetEntity });
            _mockMapper.Setup(m => m.Map<ModelCabinet>(cabinetEntity)).Returns(cabinetModel);

            // Act
            var result = await _service.GetCabinetById(cabinetId);

            // Assert
            Assert.IsTrue(result.Success);
            Assert.AreEqual(cabinetModel, result.Data);
            Assert.AreEqual("Cabinet retrieved successfully.", result.Message);
        }


        [TestMethod]
        public async Task UpdateCabinet_ShouldReturnSuccess_WhenCabinetIsUpdated()
        {
            // Arrange
            var cabinetId = Guid.NewGuid();
            var cabinetEntity = new MockBuilderCabinet()
                                .BuildRow(1, 50, new Size { Height = 40 })
                                .BuildLane(1, 0)
                                .BuildProduct("4902102112778", "午後の紅茶レモンティー/Afternoon tea lemon 500ml", 0.059, 0.059, 0.217, "https://operationmanagerstorage.blob.core.windows.net/skus/4909411084875_1685061792.jpg", 500, 1659397548, EnumProductShape.Bottle)
                                .BuildProduct("4902102112761", "午後の紅茶ミルクティー/Afternoon tea milk 500ml", 0.059, 0.059, 0.217, "https://operationmanagerstorage.blob.core.windows.net/skus/4909411084950_1685061819.jpg", 500, 1659397548, EnumProductShape.Bottle)
                                .BuildProduct("4902102112754", "紅茶花伝ロイヤルミルクティー/Kouchakaden Royal Milk 440ml", 0.069, 0.069, 0.174, "https://operationmanagerstorage.blob.core.windows.net/skus/4902102136716_1659397560.jpg", 440, 1659397548, EnumProductShape.Bottle)
                                .BuildProduct("4902102112747", "伊右衛門カフェジャスミンティーラテ/Cafe Jasmine Tea latte 500ml", 0.072, 0.072, 0.177, "https://operationmanagerstorage.blob.core.windows.net/skus/4901777381254_1659397560.jpg", 500, 1659397548, EnumProductShape.Bottle)
                                .BuildProduct("4902102112730", "ウェルチ１房分のぶどう/welch 1 bunch 470ml", 0.072, 0.072, 0.173, "https://operationmanagerstorage.blob.core.windows.net/skus/4901340064041_1659397560.jpg", 470, 1659397548, EnumProductShape.Bottle)
                                .BuildProduct("4902102112723", "トロピカーナ100%オレンジ/Tropicana 100%orange 330ml", 0.06, 0.06, 0.184, "https://operationmanagerstorage.blob.core.windows.net/skus/4909411073114_1659397561.jpg", 330, 1659397548, EnumProductShape.Bottle)
                                .BuildProduct("4902102112716", "サントリー天然水/Suntory Natural Water 2000ml", 0.091, 0.106, 0.311, "https://operationmanagerstorage.blob.core.windows.net/skus/4901777018686_1679809773.jpg", 2000, 1659397548, EnumProductShape.Bottle)
                                .BuildProduct("4902102112709", "エビアン/Evian750ml", 0.082, 0.082, 0.21, "https://operationmanagerstorage.blob.core.windows.net/skus/3068320116266_1659397561.jpg", 750, 1659397548, EnumProductShape.Bottle)
                                .BuildProduct("4902102112693", "天然水きりっと果実オレンジ＆マンゴー/Dried fruit orange & mango Water600ml", 0.068, 0.068, 0.224, "https://operationmanagerstorage.blob.core.windows.net/skus/4901777375901_1678168618.jpg", 600, 1659397548, EnumProductShape.Bottle)
                                .Build();

            var cabinetModel = _mapper.Map<ModelCabinet>(cabinetEntity);

            _mockMapper.Setup(m => m.Map<ModelCabinet>(It.IsAny<Cabinet>())).Returns(cabinetModel);
            _mockRepo.Setup(r => r.GetCabinetById(cabinetId))
                     .ReturnsAsync(new RepositoryResponse<Cabinet> { Success = true, Data = cabinetEntity });

            _mockMapper.Setup(m => m.Map<Cabinet>(cabinetModel)).Returns(cabinetEntity);
            _mockRepo.Setup(r => r.UpdateCabinet(cabinetId, cabinetEntity)).ReturnsAsync(new RepositoryResponse { Success = true });

            // Act
            var result = await _service.UpdateCabinet(cabinetId, cabinetModel);

            // Assert
            Assert.IsTrue(result.Success);
            Assert.AreEqual("Cabinet updated successfully.", result.Message);
        }

    }
}
