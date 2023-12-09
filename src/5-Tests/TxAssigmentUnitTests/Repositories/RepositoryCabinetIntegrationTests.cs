using StackExchange.Redis;
using TxAssigmentUnitTests.Mocks;
using TxAssignmentInfra.Connectors;
using TxAssignmentInfra.Entities;
using TxAssignmentInfra.Repositories;

namespace TxAssigmentUnitTests.Repositories
{
    [TestClass]
    public class RepositoryCabinetIntegrationTests
    {
        private RepositoryCabinet? _repository;
        private IDatabase? _database;

        [TestInitialize]
        public void Setup()
        {
            _database = RedisConnectorHelper.Connection.GetDatabase();
            _repository = new RepositoryCabinet(_database);
        }

        [TestMethod]
        public async Task CreateCabinet_IntegrationTest()
        {
            // Arrange
            var Cabinet = new MockBuilderCabinet()
                                .BuildRow(1, 50, new Size { Height = 40 })
                                .BuildLane(1, 0)
                                .BuildProduct("4012391230", "Coca-Cola", 10, 5, 5)
                                .BuildProduct("4012391212", "あおいおちゃ", 15, 5, 5)
                                .Build();

            // Act
            var createResponse = await _repository.CreateCabinet(Cabinet);

            var retrieveResponse = await _repository.GetCabinetById(Cabinet.Id);

            // Assert
            Assert.IsTrue(createResponse.Success);
            Assert.AreEqual("Cabinet created successfully.", createResponse.Message);

            // Cleanup
            await _repository.DeleteCabinet(Cabinet.Id);
        }

        [TestMethod]
        public async Task AddNewProductToCabinet_Test()
        {
            // Arrange
            var cabinet = new MockBuilderCabinet()
                                .BuildRow(1, 50, new Size { Height = 40 })
                                .BuildLane(1, 0)
                                .BuildProduct("4012391230", "Coca-Cola", 10, 5, 5)
                                .BuildProduct("4012391212", "あおいおちゃ", 15, 5, 5)
                                .Build();

            var newProduct = new MockBuilderProduct()
                            .WithJanCode("1238172783910921")
                            .WithDimensions(0.097, 0.308, 0.097)
                            .WithSize(1500)
                            .WithImageUrl("https://operationmanagerstorage.blob.core.windows.net/skus/4902102141109_1666091236.jpg")
                            .WithTimeStamp(1659397548)
                            .Build();

            //Act
            await _repository.CreateCabinet(cabinet);
            var cabinetResponse = await _repository.GetCabinetById(cabinet.Id);
            
            //Save the state for the cabinet
            var oldCabinet = cabinetResponse.Data;
            //Add the new product
            cabinet.Rows[0].Lanes[0].Products.Add(newProduct);
            //update cabinet
            var updateCabinet = await _repository.UpdateCabinet(cabinet.Id, cabinet);
            //Get cabinet updated
            cabinetResponse = await _repository.GetCabinetById(cabinet.Id);
            cabinet = cabinetResponse.Data;

            // Assert
            Assert.IsTrue(cabinetResponse.Success);
            Assert.IsTrue(updateCabinet.Success);
            Assert.IsNotNull(cabinet.Rows);
            Assert.IsNotNull(cabinet.Rows[0].Lanes);
            Assert.AreEqual(oldCabinet.Rows[0].Lanes[0].Products.Count, 2);
            Assert.AreEqual(cabinet.Rows[0].Lanes[0].Products.Count, 3);

            // Cleanup
            await _repository.DeleteCabinet(cabinet.Id);
        }

    }
}
