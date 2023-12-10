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
                            .WithJanCode("4902102113133")
                            .WithName("アクエリアス/Aquarius 950ml")
                            .WithDimensions(0.097, 0.308, 0.097)
                            .WithSize(1500)
                            .WithImageUrl("https://operationmanagerstorage.blob.core.windows.net/skus/4902102141109_1666091236.jpg")
                            .WithTimeStamp(1659397548)
                            .WithShape("Bottle")
                            .Build();

            // Act
            var retrieveResponse = await _repository.GetProductByJanCode(product.JanCode);
            if (retrieveResponse.Data == null)
            {
                var createResponse = await _repository.CreateProduct(product);



                // Assert
                Assert.IsTrue(createResponse.Success);
                Assert.AreEqual("Product created successfully.", createResponse.Message);
                Assert.AreEqual(product.JanCode, "4902102113133");
            }
            else
            {
                Assert.IsTrue(retrieveResponse.Success);
                Assert.AreEqual(product.JanCode, retrieveResponse.Data.JanCode);
            }
            // Cleanup
            await _repository.DeleteProduct(product.JanCode);

        }

        [TestMethod]
        public async Task CreateProduct_WhenProductExists_ShouldFail()
        {
            // Arrange
            var existingProduct = new MockBuilderProduct()
                                .WithJanCode("4902102113058")
                                .WithName("小岩井theカフェオレ/Koiwai THE Cafe au lait 500ml")
                                .WithDimensions(0.097, 0.308, 0.097)
                                .WithSize(1500)
                                .WithImageUrl("https://operationmanagerstorage.blob.core.windows.net/skus/4902102141109_1666091236.jpg")
                                .WithTimeStamp(1659397548)
                                .WithShape("Bottle")
                                .Build();

            // First, create a product to simulate an existing product in the database
            await _repository.CreateProduct(existingProduct);

            // Create a new product with the same JanCode to test for duplication
            var duplicateProduct = new MockBuilderProduct()
                                .WithJanCode("4902102113058")
                                .WithName("小岩井theカフェオレ/Koiwai THE Cafe au lait 500ml")
                                .WithDimensions(0.097, 0.308, 0.097)
                                .WithSize(1500)
                                .WithImageUrl("https://operationmanagerstorage.blob.core.windows.net/skus/4902102141109_1666091236.jpg")
                                .WithTimeStamp(1659397548)
                                .WithShape("Bottle")
                                .Build();

            // Act
            var createResponse = await _repository.CreateProduct(duplicateProduct);

            // Assert
            Assert.IsFalse(createResponse.Success);
            Assert.AreEqual($"The product with JanCode {duplicateProduct.JanCode} already exists in the database.", createResponse.Message);

            // Cleanup
            await _repository.DeleteProduct(existingProduct.JanCode);
        }
    }
}
