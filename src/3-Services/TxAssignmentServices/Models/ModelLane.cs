namespace TxAssignmentServices.Models
{
    public class ModelLane
    {
        public int Number { get; set; }
        public List<ModelProduct> Products { get; set; }
        public int PositionX { get; set; }

        public ModelLane()
        {
            Products = new List<ModelProduct>();
        }
    }
}
