using Moq;
using Newtonsoft.Json;
using StackExchange.Redis;
using TxAssignmentInfra.Entities.Enumerators;
using TxAssignmentInfra.Entities;
using TxAssignmentInfra.Repositories;
using TxAssigmentUnitTests.Mocks;

namespace TxAssigmentUnitTests.Repositories
{
    [TestClass]
    public class RepositoryCabinetTests
    {
        private Mock<IDatabase>? _mockDatabase;
        private RepositoryCabinet? _repository;
        private Cabinet? _testCabinet;


        [TestInitialize]
        public void Setup()
        {
            _mockDatabase = new Mock<IDatabase>();
            _repository = new RepositoryCabinet(_mockDatabase.Object);

            _testCabinet = new MockBuilderCabinet()
                                .BuildRow(1, 50, new Size { Height = 40 })
                                .BuildLane(1, 5, "4902102113102", 20)
                                .BuildLane(2, 10,"4902102112594", 13)
                                .Build();
        }

        [TestMethod]
        public async Task CreateCabinet_Successful()
        {
            // Arrange
            _mockDatabase.Setup(db => db.StringSetAsync(It.IsAny<RedisKey>(), It.IsAny<RedisValue>(), null, When.Always, CommandFlags.None))
                         .ReturnsAsync(true);

            // Act
            var response = await _repository.CreateCabinet(_testCabinet);

            // Assert
            Assert.IsTrue(response.Success);
            Assert.AreEqual("Cabinet created successfully.", response.Message);
        }

        [TestMethod]
        public async Task GetCabinetById_CabinetExists()
        {
            // Arrange
            var serializedCabinet = JsonConvert.SerializeObject(_testCabinet);
            _mockDatabase.Setup(db => db.StringGetAsync(It.IsAny<RedisKey>(), CommandFlags.None))
                         .ReturnsAsync(serializedCabinet);

            // Act
            var response = await _repository.GetCabinetById(_testCabinet.Id);

            // Assert
            Assert.IsTrue(response.Success);
            Assert.IsNotNull(response.Data);
            Assert.AreEqual(_testCabinet.Id, response.Data.Id);
        }

        [TestMethod]
        public async Task GetCabinetById_CabinetDoesNotExist()
        {
            // Arrange
            _mockDatabase.Setup(db => db.StringGetAsync(It.IsAny<RedisKey>(), CommandFlags.None))
                         .ReturnsAsync(RedisValue.Null);

            // Act
            var response = await _repository.GetCabinetById(Guid.NewGuid());

            // Assert
            Assert.IsFalse(response.Success);
            Assert.AreEqual("Cabinet not found.", response.Message);
            Assert.IsNull(response.Data);
        }

        [TestMethod]
        public async Task DeleteCabinet_Successful()
        {
            // Arrange
            _mockDatabase.Setup(db => db.KeyDeleteAsync(It.IsAny<RedisKey>(), CommandFlags.None))
                         .ReturnsAsync(true);

            // Act
            var response = await _repository.DeleteCabinet(_testCabinet.Id);

            // Assert
            Assert.IsTrue(response.Success);
            Assert.AreEqual("Cabinet deleted successfully.", response.Message);
        }

        [TestMethod]
        public async Task DeleteCabinet_Unsuccessful()
        {
            // Arrange
            _mockDatabase.Setup(db => db.KeyDeleteAsync(It.IsAny<RedisKey>(), CommandFlags.None))
                         .ReturnsAsync(false);

            // Act
            var response = await _repository.DeleteCabinet(Guid.NewGuid());

            // Assert
            Assert.IsFalse(response.Success);
            Assert.AreEqual("Failed to delete cabinet.", response.Message);
        }
    }
}

