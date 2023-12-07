using Moq;
using Newtonsoft.Json;
using StackExchange.Redis;
using TxAssignmentInfra.Entities;
using TxAssignmentInfra.Entities.Enumerators;
using TxAssignmentInfra.Repositories;

namespace TxAssigmentUnitTests.Repositories
{
    [TestClass]
    public class RepositoryProductTests
    {
        private Mock<IDatabase>? _mockDatabase;
        private RepositoryProduct? _repository;
        private Product? _testProduct;


        [TestInitialize]
        public void Setup()
        {
            _mockDatabase = new Mock<IDatabase>();
            _repository = new RepositoryProduct(_mockDatabase.Object);

            _testProduct = new Product
            {
                Id = Guid.NewGuid(),
                JanCode = $"{Guid.NewGuid()}",
                Depth = 0.308,
                Height = 0.097,
                Width = 0.097,
                Size = 1500,
                ImageUrl = "https://operationmanagerstorage.blob.core.windows.net/skus/4902102141109_1666091236.jpg",
                Name = "Coca -Cola 1500ml",
                Shape = EnumProductShape.Bottle,
                TimeStamp = 1659397548
            };
        }

        [TestMethod]
        public async Task CreateProduct_Successful()
        {
            // Arrange
            _mockDatabase.Setup(db => db.StringSetAsync(It.IsAny<RedisKey>(), It.IsAny<RedisValue>(), null, When.Always, CommandFlags.None))
                         .ReturnsAsync(true);

            // Act
            var response = await _repository.CreateProduct(_testProduct);

            // Assert
            Assert.IsTrue(response.Success);
            Assert.AreEqual("Product created successfully.", response.Message);
        }

        [TestMethod]
        public async Task GetProductById_ProductExists()
        {
            // Arrange
            var serializedProduct = JsonConvert.SerializeObject(_testProduct);
            _mockDatabase.Setup(db => db.StringGetAsync(It.IsAny<RedisKey>(), CommandFlags.None))
                         .ReturnsAsync(serializedProduct);

            // Act
            var response = await _repository.GetProductById(_testProduct.Id);

            // Assert
            Assert.IsTrue(response.Success);
            Assert.IsNotNull(response.Data);
            Assert.AreEqual(_testProduct.Id, response.Data.Id);
        }

        [TestMethod]
        public async Task GetProductById_ProductDoesNotExist()
        {
            // Arrange
            _mockDatabase.Setup(db => db.StringGetAsync(It.IsAny<RedisKey>(), CommandFlags.None))
                         .ReturnsAsync(RedisValue.Null);

            // Act
            var response = await _repository.GetProductById(Guid.NewGuid());

            // Assert
            Assert.IsFalse(response.Success);
            Assert.AreEqual("Product not found.", response.Message);
            Assert.IsNull(response.Data);
        }

        [TestMethod]
        public async Task DeleteProduct_Successful()
        {
            // Arrange
            _mockDatabase.Setup(db => db.KeyDeleteAsync(It.IsAny<RedisKey>(), CommandFlags.None))
                         .ReturnsAsync(true);

            // Act
            var response = await _repository.DeleteProduct(_testProduct.Id);

            // Assert
            Assert.IsTrue(response.Success);
            Assert.AreEqual("Product deleted successfully.", response.Message);
        }

        [TestMethod]
        public async Task DeleteProduct_Unsuccessful()
        {
            // Arrange
            _mockDatabase.Setup(db => db.KeyDeleteAsync(It.IsAny<RedisKey>(), CommandFlags.None))
                         .ReturnsAsync(false);

            // Act
            var response = await _repository.DeleteProduct(Guid.NewGuid());

            // Assert
            Assert.IsFalse(response.Success);
            Assert.AreEqual("Failed to delete product.", response.Message);
        }
    }
}
