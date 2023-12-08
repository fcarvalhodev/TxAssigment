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

    }
}
