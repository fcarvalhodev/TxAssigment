using AutoMapper;
using Microsoft.Extensions.Logging;
using TxAssignmentInfra.Repositories;
using TxAssignmentServices.Services;

namespace TxAssignmentServices.Strategies.Products
{
    public class StrategyDeleteProductOperation : IStrategyDeleteProductOperation
    {
        private readonly IRepositoryProduct _repositoryProduct;
        private readonly ILogger _logger;

        public StrategyDeleteProductOperation(IRepositoryProduct repositoryProduct, ILogger logger)
        {
            _repositoryProduct = repositoryProduct;
            _logger = logger;
        }

        public async Task<ServiceResponse> ExecuteAsync(string janCode)
        {
            try
            {
                var existingProduct = await _repositoryProduct.GetProductByJanCode(janCode);
                if (existingProduct.Data == null || !existingProduct.Success)
                    return new ServiceResponse { Success = false, Message = $"A problem occurred while try to fetch the product with Jan code {janCode}." };

                var response = await _repositoryProduct.DeleteProduct(janCode);
                return new ServiceResponse
                {
                    Success = response.Success,
                    Message = response.Message
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting product.");
                return new ServiceResponse { Success = false, Message = ex.Message };
            }
        }
    }
}
