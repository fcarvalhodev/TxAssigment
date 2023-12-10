 namespace TxAssignmentInfra.Entities
{
    public class Cabinet 
    {
        public Guid Id { get; set; }
        public int Number { get; set; }
        public List<Row> Rows { get; set; }
        public Position Position { get; set; }
        public Size Size { get; set; }

        public Cabinet()
        {
            Rows = new List<Row>();
            Position = new Position();
            Size = new Size();
            Id = Guid.NewGuid();
        }
    }
}
