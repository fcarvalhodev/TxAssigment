namespace TxAssignmentServices.Models
{
    public class ModelLane : ModelBase
    {
        public int Number { get; set; }
        public List<ModelProduct> Products { get; set; }

        public ModelLane()
        {
            Products = new List<ModelProduct>();
        }
    }
}
