namespace TxAssignmentInfra.Entities
{
    public class Row
    {
        public int Number { get; set; }
        public List<Lane> Lanes { get; set; }
        public int PositionZ { get; set; }
        public Size Size { get; set; }

        public Row()
        {
            Lanes = new List<Lane>();
            Size = new Size();
        }
    }
}
