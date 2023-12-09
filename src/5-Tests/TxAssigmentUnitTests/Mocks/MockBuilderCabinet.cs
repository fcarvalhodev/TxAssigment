using TxAssignmentInfra.Entities;
using TxAssignmentInfra.Entities.Enumerators;

namespace TxAssigmentUnitTests.Mocks
{
    public class MockBuilderCabinet
    {
        private Cabinet _cabinet;

        public MockBuilderCabinet()
        {
            _cabinet = new Cabinet
            {
                Id = Guid.NewGuid(),
                Number = 1,
                Position = new Position(),
                Size = new Size(),
                Rows = new List<Row>()
            };
        }

        public MockBuilderCabinet BuildRow(int rowNumber, int positionZ, Size size)
        {
            var row = new Row
            {
                Number = rowNumber,
                PositionZ = positionZ,
                Size = size,
                Lanes = new List<Lane>()
            };
            _cabinet.Rows.Add(row);
            return this;
        }

        public MockBuilderCabinet BuildLane(int laneNumber, int positionX)
        {
            var lastRow = _cabinet.Rows.LastOrDefault();
            if (lastRow != null)
            {
                var lane = new Lane
                {
                    Number = laneNumber,
                    Products = new List<Product>(),
                };
                lastRow.Lanes.Add(lane);
            }
            return this;
        }

        public MockBuilderCabinet BuildProduct(string janCode, string name, double width, double depth, double height, string ImageUrl, int size, long timeStamp, EnumProductShape shape)
        {
            var lastLane = _cabinet.Rows.LastOrDefault()?.Lanes.LastOrDefault();
            if (lastLane != null)
            {
                var product = new Product
                {
                    JanCode = janCode,
                    Name = name,
                    Width = width,
                    Depth = depth,
                    Height = height,
                    ImageUrl = ImageUrl,
                    Size = size,
                    TimeStamp = timeStamp,
                    Shape = shape,
                };
                lastLane.Products.Add(product);
            }
            return this;
        }

        public Cabinet Build()
        {
            return _cabinet;
        }
    }
}
