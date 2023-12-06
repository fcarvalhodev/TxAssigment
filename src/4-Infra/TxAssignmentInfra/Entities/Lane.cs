namespace TxAssignmentInfra.Entities
{
    public class Lane
    {
        public int Number { get; set; }
        public List<Product> Products { get; set; }
        public int PositionX { get; set; }

        public Lane()
        {
            Products = new List<Product>();
        }
    }
}
