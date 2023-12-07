using StackExchange.Redis;
using TxAssignmentInfra.Connectors;
using TxAssignmentInfra.Entities;
using TxAssignmentInfra.Entities.Enumerators;
using TxAssignmentInfra.Repositories;


namespace TxAssigmentUnitTests.Repositories
{
    [TestClass]
    public class RepositoryProductIntegrationTests
    {
        private RepositoryProduct? _repository;
        private IDatabase? _database;

        [TestInitialize]
        public void Setup()
        {
            _database = RedisConnectorHelper.Connection.GetDatabase();
            _repository = new RepositoryProduct(_database);
        }

        [TestMethod]
        public async Task CreateProduct_IntegrationTest()
        {
            // Arrange
            var product = new Product
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

            // Act
            var createResponse = await _repository.CreateProduct(product);

            var retrieveResponse = await _repository.GetProductById(product.Id);

            // Assert
            Assert.IsTrue(createResponse.Success);
            Assert.AreEqual("Product created successfully.", createResponse.Message);

            // Cleanup
            await _repository.DeleteProduct(product.Id);
        }
    }
}
