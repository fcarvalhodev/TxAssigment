using TxAssignmentInfra.Entities;

namespace TxAssignmentInfra.Repositories
{
    public interface IRepositoryProduct
    {
        Task<RepositoryResponse> CreateProduct(Product product);
        Task<RepositoryResponse> UpdateProduct(string janCode, Product product);
        Task<RepositoryResponse> DeleteProduct(string janCode);
        Task<RepositoryResponse<Product>> GetProductByJanCode(string JanCode);
        Task<RepositoryResponse<List<Product>>> GetAllProducts();
    }
}
