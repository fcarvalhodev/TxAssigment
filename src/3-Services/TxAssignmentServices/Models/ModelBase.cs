namespace TxAssignmentServices.Models
{
    public class ModelBase
    {
        public Guid Id { get; set; }

        public ModelBase()
        {
            Id = Guid.NewGuid();
        }
    }
}
