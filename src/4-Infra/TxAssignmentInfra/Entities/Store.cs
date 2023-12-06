namespace TxAssignmentInfra.Entities
{
    public class Store : BaseEntity
    {
        public List<Cabinet> Cabinets { get; set; }

        public Store()
        {
            Cabinets = new List<Cabinet>();
        }
    }
}
