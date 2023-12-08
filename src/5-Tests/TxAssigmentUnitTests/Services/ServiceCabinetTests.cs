using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using TxAssignmentInfra.Entities;
using TxAssignmentInfra.Repositories;
using TxAssignmentServices.Models;
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
            var cabinetModel = new ModelCabinet
            {
                Id = Guid.NewGuid(),
                Number = 1,
                Position = new ModelPosition { X = 10, Y = 20, Z = 0 },
                Size = new ModelSize { Width = 100, Depth = 50, Height = 200 },
                Rows = new List<ModelRow>
                {
                    new ModelRow
                    {
                        Number = 1,
                        PositionZ = 50,
                        Size = new ModelSize { Height = 40 },
                        Lanes = new List<ModelLane>
                        {
                            new ModelLane
                            {
                                Number = 1,
                                Products = new List<ModelProduct>(),
                                PositionX = 0
                            },
                        }
                    },
                }
            };

            var cabinetEntity = new Cabinet
            {
                Id = Guid.NewGuid(),
                Number = 1,
                Position = new Position { X = 10, Y = 20, Z = 0 },
                Size = new Size { Width = 100, Depth = 50, Height = 200 },
                Rows = new List<Row>
                {
                    new Row
                    {
                        Number = 1,
                        PositionZ = 50,
                        Size = new Size { Height = 40 },
                        Lanes = new List<Lane>
                        {
                            new Lane
                            {
                                Number = 1,
                                Products = new List<Product>(),
                                PositionX = 0
                            },
                        }
                    },
                }
            };


           

            _mockMapper.Setup(m => m.Map<Cabinet>(It.IsAny<ModelCabinet>())).Returns(cabinetEntity);
            _mockRepo.Setup(r => r.CreateCabinet(cabinetEntity)).ReturnsAsync(new RepositoryResponse { Success = true });

            // Act
            var result = await _service.CreateCabinet(cabinetModel);

            // Assert
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
            var cabinetModel = new ModelCabinet { /* Initialize properties */ };
            var cabinetEntity = new Cabinet { /* Initialize properties */ };

            _mockMapper.Setup(m => m.Map<Cabinet>(cabinetModel)).Returns(cabinetEntity);
            _mockRepo.Setup(r => r.UpadteCabinet(cabinetId, cabinetEntity)).ReturnsAsync(new RepositoryResponse { Success = true });

            // Act
            var result = await _service.UpadteCabinet(cabinetId, cabinetModel);

            // Assert
            Assert.IsTrue(result.Success);
            Assert.AreEqual("Cabinet updated successfully.", result.Message);
        }

        [TestMethod]
        public async Task GetEmptySpaceOnCabinet_ShouldReturnSuccess_WhenEmptySpaceIsFound()
        {
            // Arrange
            var cabinetId = Guid.NewGuid();
            var newProduct = new ModelProduct { /* Initialize properties */ };
            var cabinetEntity = new Cabinet { /* Initialize properties */ };
            var cabinetModel = new ModelCabinet { /* Initialize properties */ };

            _mockRepo.Setup(r => r.GetCabinetById(cabinetId)).ReturnsAsync(new RepositoryResponse<Cabinet> { Success = true, Data = cabinetEntity });
            _mockMapper.Setup(m => m.Map<ModelCabinet>(cabinetEntity)).Returns(cabinetModel);
            // Add more setup to mock the behavior of FindRowForProduct and FindSpaceInLaneForProduct

            // Act
            var result = await _service.GetEmptySpaceOnCabinet(cabinetId, newProduct);

            // Assert
            // Add assertions based on the expected outcome of GetEmptySpaceOnCabinet
        }


    }
}
