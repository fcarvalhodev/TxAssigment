namespace TxAssignmentServices.Models
{
    public class ModelRow
    {
        public int Number { get; set; }
        public List<ModelLane> Lanes { get; set; }
        public int PositionZ { get; set; }
        public ModelSize Size { get; set; }

        public ModelRow()
        {
            Lanes = new List<ModelLane>();
            Size = new ModelSize();
        }
    }
}
