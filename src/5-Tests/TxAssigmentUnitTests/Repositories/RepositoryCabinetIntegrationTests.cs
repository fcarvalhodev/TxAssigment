using StackExchange.Redis;
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
            var Cabinet = new Cabinet
            {
                Id = Guid.NewGuid(),
                Number = 1,
                Position = new Position { X = 10, Y = 20, Z = 0 },
                Size = new Size { Width = 100, Depth = 50, Height = 200 },
                Rows = new List<Row>
                {
                    new Row
                    {
                        Number = 1,
                        PositionZ = 50,
                        Size = new Size { Height = 40 },
                        Lanes = new List<Lane>
                        {
                            new Lane
                            {
                                Number = 1,
                                Products = new List<Product>(),
                                PositionX = 0
                            },
                        }
                    },
                }
            };

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
