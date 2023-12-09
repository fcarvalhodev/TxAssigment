using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using TxAssigmentUnitTests.Mocks;
using TxAssignmentInfra.Entities;
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
                                .BuildProduct("4012391230", "Coca-Cola", 10, 5, 5)
                                .BuildProduct("4012391212", "あおいおちゃ", 15, 5, 5)
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
                                .BuildProduct("4012391230", "Coca-Cola", 10, 5, 5)
                                .BuildProduct("4012391212", "あおいおちゃ", 15, 5, 5)
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
