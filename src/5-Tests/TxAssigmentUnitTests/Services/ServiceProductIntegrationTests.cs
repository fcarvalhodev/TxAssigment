using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using StackExchange.Redis;
using TxAssigmentUnitTests.Mocks;
using TxAssignmentInfra.Connectors;
using TxAssignmentInfra.Repositories;
using TxAssignmentServices.Models;
using TxAssignmentServices.Profiles;
using TxAssignmentServices.Services;
using TxAssignmentInfra.Entities;
using TxAssignmentServices.Strategies.Products;

namespace TxAssigmentUnitTests.Services
{
    [TestClass]
    public class ServiceProductIntegrationTests
    {
        private ServiceProduct _serviceProduct;
        private IRepositoryProduct _repositoryProduct;
        private IMapper _mapper;
        private Mock<ILogger<ServiceProduct>> _mockLogger;
        private IDatabase? _database;
        private Product _product;

        [TestInitialize]
        public void Setup()
        {
            _database = RedisConnectorHelper.Connection.GetDatabase();
            _repositoryProduct = new RepositoryProduct(_database);

            var mapperConfiguration = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new ProfileCabinet());
                mc.AddProfile(new ProfileProduct());
                mc.AddProfile(new ProfileUser());
            });
            _mapper = mapperConfiguration.CreateMapper();
            _mockLogger = new Mock<ILogger<ServiceProduct>>();

            // Mock loggers for each strategy
            var mockLoggerCreate = new Mock<ILogger<StrategyCreateProductOperation>>();
            var mockLoggerUpdate = new Mock<ILogger<StrategyUpdateProductOperation>>();
            var mockLoggerDelete = new Mock<ILogger<StrategyDeleteProductOperation>>();

            // Instantiate strategies
            var createProductStrategy = new StrategyCreateProductOperation(_repositoryProduct, _mapper, mockLoggerCreate.Object);
            var updateProductStrategy = new StrategyUpdateProductOperation(_repositoryProduct, _mapper, mockLoggerUpdate.Object);
            var deleteProductStrategy = new StrategyDeleteProductOperation(_repositoryProduct, mockLoggerDelete.Object);

            _serviceProduct = new ServiceProduct(_repositoryProduct, _mapper, _mockLogger.Object, createProductStrategy, updateProductStrategy, deleteProductStrategy);

            _product = new MockBuilderProduct()
                            .WithName("Black sugar -free 185g")
                            .WithJanCode("4902102112976")
                            .WithDimensions(0.097, 0.308, 0.097)
                            .WithSize(1500)
                            .WithImageUrl("https://operationmanagerstorage.blob.core.windows.net/skus/4902102141109_1666091236.jpg")
                            .WithTimeStamp(1659397548)
                            .WithShape("Bottle")
                            .Build();
        }

        [TestMethod]
        public async Task CreateProduct_WhenValid_ShouldCreateProduct()
        {

            // Act
            var response = await _serviceProduct.CreateProduct(_mapper.Map<ModelProduct>(_product));

            // Assert
            Assert.IsTrue(response.Success);

            // Cleanup
            await _repositoryProduct.DeleteProduct(_product.JanCode);
        }

        [TestMethod]
        public async Task UpdateProduct_WhenProductExists_ShouldUpdateProduct()
        {
            // Arrange
            var responseCreate = await _serviceProduct.CreateProduct(_mapper.Map<ModelProduct>(_product));
            Assert.IsTrue(responseCreate.Success);

            var updatedProduct = new MockBuilderProduct()
                                    .WithName("Black sugar -free 185g")
                                    .WithJanCode(_product.JanCode)
                                    .WithDimensions(0.200, 0.318, 0.099)
                                    .WithSize(1550)
                                    .WithImageUrl("https://operationmanagerstorage.blob.core.windows.net/skus/4902102141109_1666091236.jpg")
                                    .WithTimeStamp(1659397548)
                                    .WithShape("Bottle")
                                    .Build();

            // Act
            var responseUpdate = await _serviceProduct.UpdateProduct(_product.JanCode, _mapper.Map<ModelProduct>(updatedProduct));
            var updatedProductResponse = await _serviceProduct.GetProductByJanCode(_product.JanCode);

            // Assert
            Assert.IsTrue(updatedProductResponse.Success);
            Assert.IsTrue(responseUpdate.Success);
            Assert.AreNotEqual(updatedProductResponse.Data.Depth, _product.Depth);
            Assert.AreNotEqual(updatedProductResponse.Data.Width, _product.Width);
            Assert.AreNotEqual(updatedProductResponse.Data.Height, _product.Height);

            // Cleanup
            await _repositoryProduct.DeleteProduct(_product.JanCode);
        }

        [TestMethod]
        public async Task DeleteProduct_WhenProductExists_ShouldDeleteProduct()
        {
            // Arrange
            var responseCreate = await _serviceProduct.CreateProduct(_mapper.Map<ModelProduct>(_product));
            Assert.IsTrue(responseCreate.Success);

            // Act
            var responseDelete = await _serviceProduct.DeleteProduct(_product.JanCode);

            // Assert
            Assert.IsTrue(responseDelete.Success);
        }

        [TestMethod]
        public async Task GetProductByJanCode_WhenProductExists_ShouldReturnProduct()
        {
            // Arrange
            var responseCreate = await _serviceProduct.CreateProduct(_mapper.Map<ModelProduct>(_product));
            Assert.IsTrue(responseCreate.Success);

            // Act
            var responseGet = await _serviceProduct.GetProductByJanCode(_product.JanCode);

            // Assert
            Assert.IsTrue(responseGet.Success);
            Assert.IsNotNull(responseGet.Data);

            // Cleanup
            await _repositoryProduct.DeleteProduct(_product.JanCode);
        }

        [TestMethod]
        public async Task GetAllProducts_ShouldReturnProducts()
        {
            // Arrange
            var responseCreate = await _serviceProduct.CreateProduct(_mapper.Map<ModelProduct>(_product));
            Assert.IsTrue(responseCreate.Success);

            // Act
            var responseGetAll = await _serviceProduct.GetAllProducts();

            // Assert
            Assert.IsTrue(responseGetAll.Success);
            Assert.IsTrue(responseGetAll.Data.Count > 0);

            // Cleanup
            await _repositoryProduct.DeleteProduct(_product.JanCode);
        }
    }
}
