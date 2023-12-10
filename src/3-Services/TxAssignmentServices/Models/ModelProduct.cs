using TxAssignmentInfra.Entities.Enumerators;

namespace TxAssignmentServices.Models
{
    public class ModelProduct
    {
        public string? JanCode { get; set; }
        public string? Name { get; set; }
        public double Width { get; set; }
        public double Depth { get; set; }
        public double Height { get; set; }
        public string? ImageUrl { get; set; }
        public int Size { get; set; }
        public long TimeStamp { get; set; }
        public EnumProductShape Shape { get; set; }

        public ModelProduct()
        {
            
        }

        public ModelProduct(string janCode)
        {
            this.JanCode = janCode;
            Shape = EnumProductShape.Unknown;
        }
    }
}
