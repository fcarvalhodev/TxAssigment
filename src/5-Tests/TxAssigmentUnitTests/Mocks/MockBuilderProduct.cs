using TxAssignmentInfra.Entities.Enumerators;
using TxAssignmentServices.Models;

namespace TxAssigmentUnitTests.Mocks
{
    public class MockBuilderProduct
    {
        private ModelProduct _product;

        public MockBuilderProduct()
        {
            _product = new ModelProduct();
        }

        public MockBuilderProduct WithJanCode(string janCode)
        {
            _product.JanCode = janCode;
            return this;
        }

        public MockBuilderProduct WithName(string name)
        {
            _product.Name = name;
            return this;
        }

        public MockBuilderProduct WithDimensions(double width, double depth, double height)
        {
            _product.Width = width;
            _product.Depth = depth;
            _product.Height = height;
            return this;
        }

        public MockBuilderProduct WithImageUrl(string imageUrl)
        {
            _product.ImageUrl = imageUrl;
            return this;
        }

        public MockBuilderProduct WithSize(int size)
        {
            _product.Size = size;
            return this;
        }

        public MockBuilderProduct WithTimeStamp(long timeStamp)
        {
            _product.TimeStamp = timeStamp;
            return this;
        }

        public MockBuilderProduct WithShape(EnumProductShape shape)
        {
            _product.Shape = shape;
            return this;
        }

        public ModelProduct Build()
        {
            return _product;
        }
    }

}
