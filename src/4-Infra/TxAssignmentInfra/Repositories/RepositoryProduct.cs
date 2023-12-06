using Newtonsoft.Json;
using StackExchange.Redis;
using TxAssignmentInfra.Entities;

namespace TxAssignmentInfra.Repositories
{
    public class RepositoryProduct : IRepositoryProduct
    {

        private readonly IDatabase _database;

        public RepositoryProduct(IDatabase database)
        {
            _database = database;
        }

        public async Task<RepositoryResponse<Product>> GetProductById(Guid IdProduct)
        {
            try
            {
                var serializedProduct = await _database.StringGetAsync(IdProduct.ToString());

                if (serializedProduct.IsNullOrEmpty)
                {
                    return new RepositoryResponse<Product> { Success = false, Message = "Product not found." };

                }

                var product = JsonConvert.DeserializeObject<Product>(serializedProduct);
                return new RepositoryResponse<Product> { Success = true, Data = product };
            }
            catch (Exception ex)
            {
                return new RepositoryResponse<Product> { Success = false, Message = ex.Message };
            }
        }

        public async Task<RepositoryResponse> CreateProduct(Product product)
        {
            try
            {
                var serializedProduct = JsonConvert.SerializeObject(product);
                await _database.StringSetAsync(product.Id.ToString(), serializedProduct);
                return new RepositoryResponse { Success = true, Message = "Product created successfully." };
            }
            catch (Exception ex)
            {
                return new RepositoryResponse { Success = false, Message = ex.Message };
            }
        }

        public async Task<RepositoryResponse> UpdateProduct(Guid IdProduct, Product product)
        {
            try
            {
                var key = $"product:{IdProduct}";
                if (!await _database.KeyExistsAsync(key))
                {
                    return new RepositoryResponse { Success = false, Message = "Product not found." };
                }

                var serializedProduct = JsonConvert.SerializeObject(product);
                bool updated = await _database.StringSetAsync(key, serializedProduct);
                return new RepositoryResponse
                {
                    Success = updated,
                    Message = updated ? "Product updated successfully." : "Failed to update product."
                };
            }
            catch (Exception ex)
            {
                return new RepositoryResponse { Success = false, Message = ex.Message };
            }
        }

        public async Task<RepositoryResponse> DeleteProduct(Guid IdProduct)
        {
            try
            {
                bool deleted = await _database.KeyDeleteAsync(IdProduct.ToString());
                return new RepositoryResponse
                {
                    Success = deleted,
                    Message = deleted ? "Product deleted successfully." : "Failed to delete product."
                };
            }
            catch (Exception ex)
            {
                return new RepositoryResponse { Success = false, Message = ex.Message };
            }
        }
    }
}
