using TxAssignmentInfra.Entities;
using TxAssignmentServices.Models;

namespace TxAssignmentServices.Services
{
    public interface IServiceProduct
    {
        Task<ServiceResponse> CreateProduct(ModelProduct product);
        Task<ServiceResponse> UpdateProduct(string janCode, ModelProduct product);
        Task<ServiceResponse> DeleteProduct(string janCode);
        Task<ServiceResponse<ModelProduct>> GetProductByJanCode(string JanCode);
        Task<ServiceResponse<List<ModelProduct>>> GetAllProducts();
    }
}
