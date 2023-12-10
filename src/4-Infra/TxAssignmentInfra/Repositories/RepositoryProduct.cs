using Newtonsoft.Json;
using StackExchange.Redis;
using System.Numerics;
using TxAssignmentInfra.Entities;
using TxAssignmentInfra.Entities.Enumerators;

namespace TxAssignmentInfra.Repositories
{
    public class RepositoryProduct : IRepositoryProduct
    {

        private readonly IDatabase _database;

        public RepositoryProduct(IDatabase database)
        {
            _database = database;
        }

        public async Task<RepositoryResponse<Product>> GetProductByJanCode(string JanCode)
        {
            try
            {
                string key = $"{RedisDocTypes.SKU}{JanCode}";
                var serializedProduct = await _database.StringGetAsync(key);

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
                if (string.IsNullOrEmpty(product.JanCode))
                    return new RepositoryResponse { Success = false, Message = "The product JanCode is mandatory" };

                string key = $"{RedisDocTypes.SKU}{product.JanCode}";

                var products = await GetAllProducts();
                if (!products.Success)
                    return new RepositoryResponse { Success = false, Message = "The request was not able to fetch the products" };

                if (products.Data.Where(me => me.JanCode.Equals(product.JanCode)).Count() >= 1)
                    return new RepositoryResponse { Success = false, Message = $"The product with JanCode {product.JanCode} already exists in the database." };

                var serializedProduct = JsonConvert.SerializeObject(product);
                await _database.StringSetAsync(key, serializedProduct);
                await _database.SetAddAsync("productKeys", key);
                return new RepositoryResponse { Success = true, Message = "Product created successfully." };
            }
            catch (Exception ex)
            {
                return new RepositoryResponse { Success = false, Message = ex.Message };
            }
        }

        public async Task<RepositoryResponse> UpdateProduct(string janCode, Product product)
        {
            try
            {
                var key = $"{RedisDocTypes.SKU}{janCode}";
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

        public async Task<RepositoryResponse> DeleteProduct(string janCode)
        {
            try
            {
                var key = $"{RedisDocTypes.SKU}{janCode}";
                bool deleted = await _database.KeyDeleteAsync(key);
                await _database.SetRemoveAsync("productKeys", key);
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

        public async Task<RepositoryResponse<List<Product>>> GetAllProducts()
        {
            try
            {
                var productKeys = await _database.SetMembersAsync("productKeys");
                var products = new List<Product>();

                foreach (var keyVal in productKeys)
                {
                    string key = keyVal.ToString();
                    var serializedProduct = await _database.StringGetAsync(key);
                    if (!serializedProduct.IsNullOrEmpty)
                    {
                        var product = JsonConvert.DeserializeObject<Product>(serializedProduct);
                        products.Add(product);
                    }
                }

                return new RepositoryResponse<List<Product>> { Success = true, Data = products };
            }
            catch (Exception ex)
            {
                return new RepositoryResponse<List<Product>> { Success = false, Message = ex.Message };
            }
        }

    }
}
