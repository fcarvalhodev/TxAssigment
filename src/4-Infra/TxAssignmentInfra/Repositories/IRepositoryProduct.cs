using TxAssignmentInfra.Entities;

namespace TxAssignmentInfra.Repositories
{
    public interface IRepositoryProduct
    {
        Task<RepositoryResponse> CreateProduct(Product product);
        Task<RepositoryResponse> UpdateProduct(Guid IdProduct, Product product);
        Task<RepositoryResponse> DeleteProduct(Guid IdProduct);
        Task<RepositoryResponse<Product>> GetProductById(Guid IdProduct);
        Task<RepositoryResponse<Product>> GetProductByJanCode(string janCode);
    }
}
