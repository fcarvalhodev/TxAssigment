using TxAssignmentInfra.Entities.Enumerators;

namespace TxAssignmentInfra.Entities
{
    //Equivalent to SKU
    public class Product
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

        public Product()
        {
            Shape = EnumProductShape.Unknown; // Initialize with default Unknown value
        }
    }
}
