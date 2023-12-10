using AutoMapper;
using StackExchange.Redis;
using TxAssigmentUnitTests.Mocks;
using TxAssignmentInfra.Connectors;
using TxAssignmentInfra.Entities;
using TxAssignmentInfra.Repositories;
using TxAssignmentServices.Profiles;

namespace TxAssigmentUnitTests.Repositories
{
    [TestClass]
    public class RepositoryCabinetIntegrationTests
    {
        private RepositoryCabinet? _repository;
        private IDatabase? _database;

        private Cabinet _cabinet;

        [TestInitialize]
        public void Setup()
        {
            _database = RedisConnectorHelper.Connection.GetDatabase();
            _repository = new RepositoryCabinet(_database);

            var mapperConfiguration = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new ProfileCabinet());
                mc.AddProfile(new ProfileProduct());
                mc.AddProfile(new ProfileUser());
            });

            _cabinet = new MockBuilderCabinet()
                       .WithNumber(1)
                       .WithPosition(new Position { X = 10, Y = 20, Z = 0 })
                       .WithSize(new Size { Width = 100, Depth = 50, Height = 200 })
                       .BuildRow(1, 50, new Size { Height = 40 })
                       .BuildLane(1, 5, "4902102113102", 20)
                       .BuildLane(2, 10, "4902102112594", 13)
                       .Build();
        }

        [TestMethod]
        public async Task CreateCabinet_IntegrationTest()
        {
            // Act
            var createResponse = await _repository.CreateCabinet(_cabinet);

            var retrieveResponse = await _repository.GetCabinetById(_cabinet.Id);
            Assert.IsTrue(retrieveResponse.Success);

            // Assert
            Assert.IsTrue(createResponse.Success);
            Assert.AreEqual("Cabinet created successfully.", createResponse.Message);

            // Cleanup
            await _repository.DeleteCabinet(_cabinet.Id);
        }

        [TestMethod]
        public async Task UpdateCabinet_WhenCabinetExists_ShouldUpdateCabinet()
        {
            // Arrange: Create and store a cabinet in the database
            await _repository.CreateCabinet(_cabinet);
            var updatedCabinet = new MockBuilderCabinet()
                .WithId(_cabinet.Id) // Use the same ID
                .WithNumber(2) // Change some properties
                .Build();

            // Act
            var updateResponse = await _repository.UpdateCabinet(_cabinet.Id, updatedCabinet);

            // Assert
            Assert.IsTrue(updateResponse.Success);
            Assert.AreEqual("Cabinet updated successfully.", updateResponse.Message);

            // Cleanup
            await _repository.DeleteCabinet(_cabinet.Id);
        }

        [TestMethod]
        public async Task DeleteCabinet_WhenCabinetExists_ShouldDeleteCabinet()
        {
            // Arrange: Create and store a cabinet in the database
            await _repository.CreateCabinet(_cabinet);

            // Act
            var deleteResponse = await _repository.DeleteCabinet(_cabinet.Id);

            // Assert
            Assert.IsTrue(deleteResponse.Success);
            Assert.AreEqual("Cabinet deleted successfully.", deleteResponse.Message);
        }


        [TestMethod]
        public async Task GetCabinetById_WhenCabinetExists_ShouldReturnCabinet()
        {
            // Arrange: Create and store a cabinet in the database
            await _repository.CreateCabinet(_cabinet);

            // Act
            var getCabinetResponse = await _repository.GetCabinetById(_cabinet.Id);

            // Assert
            Assert.IsTrue(getCabinetResponse.Success);
            Assert.IsNotNull(getCabinetResponse.Data);
            Assert.AreEqual(_cabinet.Id, getCabinetResponse.Data.Id);

            // Cleanup
            await _repository.DeleteCabinet(_cabinet.Id);
        }
    }
}
