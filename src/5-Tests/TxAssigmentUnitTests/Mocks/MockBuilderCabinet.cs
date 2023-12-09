using TxAssignmentInfra.Entities;

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

        public MockBuilderCabinet WithId(Guid Id)
        {
            _cabinet.Id = Id;
            return this;
        }

        public MockBuilderCabinet WithNumber(int number)
        {
            _cabinet.Number = number;
            return this;
        }

        public MockBuilderCabinet WithPosition(Position position)
        {
            _cabinet.Position = position;
            return this;
        }

        public MockBuilderCabinet WithSize(Size size)
        {
            _cabinet.Size = size;
            return this;
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

        public MockBuilderCabinet BuildLane(int laneNumber, int positionX, string janCode, int quantity)
        {
            var lastRow = _cabinet.Rows.LastOrDefault();
            if (lastRow != null)
            {
                var lane = new Lane
                {
                    Number = laneNumber,
                    PositionX = positionX,
                    Quantity = quantity,
                    JanCode = janCode
                };
                lastRow.Lanes.Add(lane);
            }
            return this;
        }

        public Cabinet Build()
        {
            return _cabinet;
        }
    }
}
