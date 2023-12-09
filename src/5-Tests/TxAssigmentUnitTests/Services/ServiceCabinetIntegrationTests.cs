using AutoMapper;
using Castle.Core.Logging;
using Microsoft.Extensions.Logging;
using Moq;
using StackExchange.Redis;
using TxAssigmentUnitTests.Mocks;
using TxAssignmentInfra.Connectors;
using TxAssignmentInfra.Entities;
using TxAssignmentInfra.Repositories;
using TxAssignmentServices.Models;
using TxAssignmentServices.Profiles;
using TxAssignmentServices.Services;

namespace TxAssigmentUnitTests.Services
{
    [TestClass]
    public class ServiceCabinetIntegrationTests
    {
        private ServiceCabinet _serviceCabinet;

        private IRepositoryCabinet _repositoryCabinet;
        private IRepositoryProduct _repositoryProduct;
        private IMapper _mapper;
        private Mock<ILogger<ServiceCabinet>> _mockLogger;
        private IDatabase? _database;

        [TestInitialize]
        public void Setup()
        {
            _database = RedisConnectorHelper.Connection.GetDatabase();

            // Initialize repositories with the database context
            _repositoryCabinet = new RepositoryCabinet(_database);
            _repositoryProduct = new RepositoryProduct(_database);

            // Initialize AutoMapper
            var mapperConfiguration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<ProfileCabinet>(); // Ensure this profile is correctly set up
            });
            _mapper = mapperConfiguration.CreateMapper();

            _mockLogger = new Mock<ILogger<ServiceCabinet>>();

            // Initialize ServiceCabinet with actual instances
            _serviceCabinet = new ServiceCabinet(_repositoryCabinet, _repositoryProduct, _mapper, _mockLogger.Object);
        }

        [TestMethod]
        public async Task CreateCabinet_WhenValid_ShouldCreateCabinet()
        {
            // Arrange
            var cabinet = new MockBuilderCabinet()
                                .WithNumber(1)
                                .WithPosition(new Position { X = 10, Y = 20, Z = 0 })
                                .WithSize(new Size { Width = 100, Depth = 50, Height = 200 })
                                .BuildRow(1, 50, new Size { Height = 40 })
                                .BuildLane(1, 5, "4902102113102", 20)
                                .BuildLane(2, 10, "4902102112594", 13)
                                .Build();

            // Act
            var response = await _serviceCabinet.CreateCabinet(_mapper.Map<ModelCabinet>(cabinet));
            var cabinetRegistered = await _repositoryCabinet.GetCabinetById(cabinet.Id);

            // Assert
            Assert.IsTrue(response.Success);
            Assert.IsNotNull(cabinetRegistered);

            //clean
            await _repositoryCabinet.DeleteCabinet(cabinet.Id);
        }

        [TestMethod]
        public async Task UpdateCabinet_WhenCabinetExists_ShouldUpdateCabinet()
        {
            // Arrange
            var cabinet = new MockBuilderCabinet()
                       .WithNumber(1)
                       .WithPosition(new Position { X = 10, Y = 20, Z = 0 })
                       .WithSize(new Size { Width = 100, Depth = 50, Height = 200 })
                       .BuildRow(1, 50, new Size { Height = 40 })
                       .BuildLane(1, 5, "4902102113102", 20)
                       .BuildLane(2, 10, "4902102112594", 13)
                       .Build();

            var createCabinetReponse = await _serviceCabinet.CreateCabinet(_mapper.Map<ModelCabinet>(cabinet));

            Assert.IsTrue(createCabinetReponse.Success);
            var newCabinetModel = new MockBuilderCabinet()
              .WithId(cabinet.Id)
              .WithNumber(2)
              .WithPosition(new Position { X = 20, Y = 40, Z = 10})
              .WithSize(new Size { Width = 100, Depth = 50, Height = 160 })
              .BuildRow(1, 50, new Size { Height = 40 })
              .BuildLane(1, 5, "4902102112570", 20)
              .BuildLane(2, 10, "4902102112846", 13)
              .Build();

            // Act
            var response = await _serviceCabinet.UpdateCabinet(cabinet.Id, _mapper.Map<ModelCabinet>(newCabinetModel));
            var cabinetRegistered = await _repositoryCabinet.GetCabinetById(cabinet.Id);
            Assert.IsTrue(cabinetRegistered.Success);

            // Assert
            Assert.IsTrue(response.Success);
            Assert.AreNotEqual(cabinet.Number, cabinetRegistered.Data.Number);
            Assert.AreNotEqual(cabinet.Position.X, cabinetRegistered.Data.Position.X);
            Assert.AreNotEqual(cabinet.Position.Y, cabinetRegistered.Data.Position.Y);

            await _repositoryCabinet.DeleteCabinet(cabinet.Id);
        }

        [TestMethod]
        public async Task DeleteCabinet_WhenCabinetExists_ShouldDeleteCabinet()
        {
            // Arrange: Assuming a cabinet exists in the database
            Guid cabinetId = Guid.NewGuid();

            // Act
            var response = await _serviceCabinet.DeleteCabinet(cabinetId);

            // Assert
            Assert.IsTrue(response.Success);
            // Additional assertions to verify the cabinet was deleted from the database
        }

        [TestMethod]
        public async Task GetCabinetById_WhenCabinetExists_ShouldReturnCabinet()
        {
            // Arrange: Assuming a cabinet exists in the database
            Guid cabinetId = Guid.NewGuid();

            // Act
            var response = await _serviceCabinet.GetCabinetById(cabinetId);

            // Assert
            Assert.IsTrue(response.Success);
            Assert.IsNotNull(response.Data);
            // Additional assertions to verify the correct cabinet was retrieved
        }
    }
}
