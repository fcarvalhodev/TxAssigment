namespace TxAssignmentInfra.Entities
{
    public class Lane : BaseEntity
    {
        public int Number { get; set; }
        public List<Product> Products { get; set; }

        public Lane()
        {
            Products = new List<Product>();
        }
    }
}
