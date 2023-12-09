using StackExchange.Redis;
using TxAssigmentUnitTests.Mocks;
using TxAssignmentInfra.Connectors;
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
            var product = new MockBuilderProduct()
                            .WithJanCode("1238172783910921")
                            .WithDimensions(0.097, 0.308, 0.097)
                            .WithSize(1500)
                            .WithImageUrl("https://operationmanagerstorage.blob.core.windows.net/skus/4902102141109_1666091236.jpg")
                            .WithTimeStamp(1659397548)
                            .Build();

            // Act
            var createResponse = await _repository.CreateProduct(product);

            var retrieveResponse = await _repository.GetProductById(product.Id);

            // Assert
            Assert.IsTrue(createResponse.Success);
            Assert.AreEqual("Product created successfully.", createResponse.Message);
            Assert.AreEqual(product.JanCode, retrieveResponse.Data.JanCode); ;

            // Cleanup
            await _repository.DeleteProduct(product.Id);
        }

        [TestMethod]
        public async Task CreateProduct_WhenProductExists_ShouldFail()
        {
            // Arrange
            var existingProduct = new MockBuilderProduct()
                                .WithJanCode("1238172783910921")
                                .WithDimensions(0.097, 0.308, 0.097)
                                .WithSize(1500)
                                .WithImageUrl("https://operationmanagerstorage.blob.core.windows.net/skus/4902102141109_1666091236.jpg")
                                .WithTimeStamp(1659397548)
                                .Build();

            // First, create a product to simulate an existing product in the database
            await _repository.CreateProduct(existingProduct);

            // Create a new product with the same JanCode to test for duplication
            var duplicateProduct = new MockBuilderProduct()
                                .WithJanCode("1238172783910921") // Same JanCode as existingProduct
                                .WithDimensions(0.097, 0.308, 0.097)
                                .WithSize(1500)
                                .WithImageUrl("https://operationmanagerstorage.blob.core.windows.net/skus/4902102141109_1666091236.jpg")
                                .WithTimeStamp(1659397548)
                                .Build();

            // Act
            var createResponse = await _repository.CreateProduct(duplicateProduct);

            // Assert
            Assert.IsFalse(createResponse.Success);
            Assert.AreEqual("The product already exists on the database", createResponse.Message);

            // Cleanup
            await _repository.DeleteProduct(existingProduct.Id);
        }
    }
}
