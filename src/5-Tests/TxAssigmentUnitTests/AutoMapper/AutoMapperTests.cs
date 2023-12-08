using AutoMapper;
using TxAssignmentInfra.Entities;
using TxAssignmentServices.Models;
using TxAssignmentServices.Profiles;

namespace TxAssigmentUnitTests.AutoMapper
{
    [TestClass]
    public class AutoMapperTests
    {
        private readonly IConfigurationProvider _configuration;
        private readonly IMapper _mapper;

        public AutoMapperTests()
        {
            _configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<ProfileCabinet>();
            });

            _mapper = _configuration.CreateMapper();
        }

        [TestMethod]
        public void AutoMapper_Configuration_IsValid()
        {
            _configuration.AssertConfigurationIsValid();
        }

        [TestMethod]
        public void AutoMapper_CanMap_ModelCabinet_To_Cabinet()
        {
            // Arrange
            var modelCabinet = new ModelCabinet
            {
                Id = Guid.NewGuid(),
                Number = 1,
                Position = new ModelPosition { X = 10, Y = 20, Z = 0 },
                Size = new ModelSize { Width = 100, Depth = 50, Height = 200 },
                Rows = new List<ModelRow>
                {
                    new ModelRow
                    {
                        Number = 1,
                        PositionZ = 50,
                        Size = new ModelSize { Height = 40 },
                        Lanes = new List<ModelLane>
                        {
                            new ModelLane
                            {
                                Number = 1,
                                Products = new List<ModelProduct>(),
                                PositionX = 0
                            },
                        }
                    },
                }
            };

            // Act
            var cabinet = _mapper.Map<Cabinet>(modelCabinet);

            // Assert
            Assert.IsNotNull(cabinet);
            // Add more asserts to validate that important properties are mapped correctly
        }
    }
}
