using AutoMapper;
using Microsoft.Extensions.Logging;
using TxAssignmentInfra.Entities;
using TxAssignmentInfra.Repositories;
using TxAssignmentServices.Models;
using TxAssignmentServices.Services;

namespace TxAssignmentServices.Strategies.Products
{
    public class StrategyUpdateProductOperation : IStrategyUpdateProductOperation
    {
        private readonly IRepositoryProduct _repositoryProduct;
        private readonly IMapper _mapper;
        private readonly ILogger<StrategyUpdateProductOperation> _logger;

        public StrategyUpdateProductOperation(IRepositoryProduct repositoryProduct, IMapper mapper, ILogger<StrategyUpdateProductOperation> logger)
        {
            _repositoryProduct = repositoryProduct;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ServiceResponse> ExecuteAsync(string janCode, ModelProduct modelProduct)
        {
            try
            {
                var existingProduct = await _repositoryProduct.GetProductByJanCode(janCode);
                if (existingProduct.Data == null || !existingProduct.Success)
                    return new ServiceResponse { Success = false, Message = $"A problem occurred while try to fetch the product with Jan code {janCode}." };

                var isValidModel = HelperProducts.ModelIsValid(modelProduct);
                if (!isValidModel.isValid)
                    return new ServiceResponse { Success = false, Message = isValidModel.message };

                var product = _mapper.Map<Product>(modelProduct);
                var response = await _repositoryProduct.UpdateProduct(janCode, product);

                return new ServiceResponse
                {
                    Success = response.Success,
                    Message = response.Message
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating product.");
                return new ServiceResponse { Success = false, Message = ex.Message };
            }
        }
    }
}
