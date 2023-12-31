﻿using AutoMapper;
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
using TxAssignmentServices.Strategies.Cabinets;
using TxAssignmentServices.Strategies.Products;

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

        private Cabinet _cabinet;

        private Product _product3102;
        private Product _product2594;

        [TestInitialize]
        public void Setup()
        {
            _database = RedisConnectorHelper.Connection.GetDatabase();

            _repositoryCabinet = new RepositoryCabinet(_database);
            _repositoryProduct = new RepositoryProduct(_database);

            var mapperConfiguration = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new ProfileCabinet());
                mc.AddProfile(new ProfileProduct());
                mc.AddProfile(new ProfileUser());
            });
            _mapper = mapperConfiguration.CreateMapper();
            _mockLogger = new Mock<ILogger<ServiceCabinet>>();

            // Mock loggers for each strategy
            var mockLoggerCreate = new Mock<ILogger<StrategyCreateCabinetOperation>>();
            var mockLoggerUpdate = new Mock<ILogger<StrategyUpdateCabinetOperation>>();
            var mockLoggerDelete = new Mock<ILogger<StrategyDeleteCabinetOperation>>();

            // Instantiate strategies
            var createCabinetStrategy = new StrategyCreateCabinetOperation(_repositoryCabinet, _repositoryProduct, _mapper, mockLoggerCreate.Object);
            var updateCabinetStrategy = new StrategyUpdateCabinetOperation(_repositoryCabinet, _repositoryProduct, _mapper, mockLoggerUpdate.Object);
            var deleteCabinetStrategy = new StrategyDeleteCabinetOperation(_repositoryCabinet, mockLoggerDelete.Object);

            _serviceCabinet = new ServiceCabinet(_repositoryCabinet, _mapper, _mockLogger.Object, createCabinetStrategy, updateCabinetStrategy, deleteCabinetStrategy);

            _cabinet = new MockBuilderCabinet()
                                .WithNumber(1)
                                .WithPosition(new Position { X = 10, Y = 20, Z = 0 })
                                .WithSize(new Size { Width = 100, Depth = 50, Height = 200 })
                                .BuildRow(1, 50, new Size { Height = 40 })
                                .BuildLane(1, 5, "4902102113102", 20)
                                .BuildLane(2, 10, "4902102112594", 13)
                                .Build();

            _product3102 = new MockBuilderProduct()
                  .WithJanCode("4902102113102")
                  .WithName("アクエリアス/Aquarius 950ml")
                  .WithDimensions(0.097, 0.308, 0.097)
                  .WithSize(1500)
                  .WithImageUrl("https://operationmanagerstorage.blob.core.windows.net/skus/4902102141109_1666091236.jpg")
                  .WithTimeStamp(1659397548)
                  .WithShape("Bottle")
                  .Build();

            _product2594 = new MockBuilderProduct()
                  .WithJanCode("4902102112594")
                  .WithName("アクエリアス/Aquarius 950ml")
                  .WithDimensions(0.097, 0.308, 0.097)
                  .WithSize(1500)
                  .WithImageUrl("https://operationmanagerstorage.blob.core.windows.net/skus/4902102141109_1666091236.jpg")
                  .WithTimeStamp(1659397548)
                  .WithShape("Bottle")
                  .Build();
        }

        [TestMethod]
        public async Task CreateCabinet_WhenValid_ShouldCreateCabinet()
        {
            // Act
            await InitializeProducts();
            var response = await _serviceCabinet.CreateCabinet(_mapper.Map<ModelCabinet>(_cabinet));
            var cabinetRegistered = await _repositoryCabinet.GetCabinetById(_cabinet.Id);

            // Assert
            Assert.IsTrue(response.Success);
            Assert.IsNotNull(cabinetRegistered);

            //clean
            await _repositoryCabinet.DeleteCabinet(_cabinet.Id);
        }

        [TestMethod]
        public async Task UpdateCabinet_WhenCabinetExists_ShouldUpdateCabinet()
        {
            // Arrange
            await InitializeProducts();
            var createCabinetReponse = await _serviceCabinet.CreateCabinet(_mapper.Map<ModelCabinet>(_cabinet));
            Assert.IsTrue(createCabinetReponse.Success);
            var newCabinetModel = new MockBuilderCabinet()
              .WithId(_cabinet.Id)
              .WithNumber(2)
              .WithPosition(new Position { X = 20, Y = 40, Z = 10 })
              .WithSize(new Size { Width = 100, Depth = 50, Height = 160 })
              .BuildRow(1, 50, new Size { Height = 40 })
              .BuildLane(1, 5, "4902102113102", 20)
              .BuildLane(2, 10, "4902102112594", 13)
              .Build();

            // Act
            var response = await _serviceCabinet.UpdateCabinet(_cabinet.Id, _mapper.Map<ModelCabinet>(newCabinetModel));
            var cabinetRegistered = await _repositoryCabinet.GetCabinetById(_cabinet.Id);
            Assert.IsTrue(cabinetRegistered.Success);

            // Assert
            Assert.IsTrue(response.Success);
            Assert.AreNotEqual(_cabinet.Number, cabinetRegistered.Data.Number);
            Assert.AreNotEqual(_cabinet.Position.X, cabinetRegistered.Data.Position.X);
            Assert.AreNotEqual(_cabinet.Position.Y, cabinetRegistered.Data.Position.Y);

            await _repositoryCabinet.DeleteCabinet(_cabinet.Id);
        }

        [TestMethod]
        public async Task DeleteCabinet_WhenCabinetExists_ShouldDeleteCabinet()
        {
            //Arrange
            await InitializeProducts();
            var createCabinetReponse = await _serviceCabinet.CreateCabinet(_mapper.Map<ModelCabinet>(_cabinet));
            Assert.IsTrue(createCabinetReponse.Success);

            // Act
            var response = await _serviceCabinet.DeleteCabinet(_cabinet.Id);

            // Assert
            Assert.IsTrue(response.Success);

            //Clean
            await _repositoryCabinet.DeleteCabinet(_cabinet.Id);
        }

        [TestMethod]
        public async Task GetCabinetById_WhenCabinetExists_ShouldReturnCabinet()
        {
            // Arrange
            await InitializeProducts();
            var createCabinetReponse = await _serviceCabinet.CreateCabinet(_mapper.Map<ModelCabinet>(_cabinet));
            Assert.IsTrue(createCabinetReponse.Success);

            // Act
            var response = await _serviceCabinet.GetCabinetById(_cabinet.Id);

            // Assert
            Assert.IsTrue(response.Success);
            Assert.IsNotNull(response.Data);

            //Clean
            await _repositoryCabinet.DeleteCabinet(_cabinet.Id);
        }

        private async Task InitializeProducts()
        {
            var product3102 = await _repositoryProduct.GetProductByJanCode(_product3102.JanCode);
            var product2594 = await _repositoryProduct.GetProductByJanCode(_product3102.JanCode);

            if (product3102.Data == null)
                await _repositoryProduct.CreateProduct(_product3102);

            if (product2594.Data == null)
                await _repositoryProduct.CreateProduct(_product2594);
        }


        [TestCleanup]
        public async Task Cleanup()
        {
            if (_cabinet != null && _cabinet.Id != Guid.Empty)
            {
                await _repositoryCabinet.DeleteCabinet(_cabinet.Id);
            }

            if (_product3102 != null)
            {
                await _repositoryProduct.DeleteProduct(_product3102.JanCode);
            }

            if (_product2594 != null)
            {
                await _repositoryProduct.DeleteProduct(_product2594.JanCode);
            }
        }
    }
}
