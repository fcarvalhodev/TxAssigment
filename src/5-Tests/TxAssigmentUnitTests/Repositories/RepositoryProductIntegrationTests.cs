using StackExchange.Redis;
using TxAssignmentInfra.Connectors;
using TxAssignmentInfra.Entities;
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
            // Replace these with your Redis configuration
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
                Quantity = 1,
                JanCode  = "SOMEC02038743248327"
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
