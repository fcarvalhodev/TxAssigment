using AutoMapper;
using Microsoft.Extensions.Logging;
using TxAssignmentInfra.Entities;
using TxAssignmentInfra.Repositories;
using TxAssignmentServices.Models;
using TxAssignmentServices.Services;
using TxAssignmentServices.Strategies.Cabinets;

namespace TxAssignmentServices.Strategies.Products
{
    public class StrategyCreateProductOperation : IStrategyCreateProductOperation
    {
        private readonly IRepositoryProduct _repositoryProduct;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public StrategyCreateProductOperation(IRepositoryProduct repositoryProduct, IMapper mapper, ILogger logger)
        {
            _repositoryProduct = repositoryProduct;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ServiceResponse> ExecuteAsync(ModelProduct modelProduct)
        {
            try
            {
                var isValidModel = HelperProducts.ModelIsValid(modelProduct);
                if (!isValidModel.isValid)
                    return new ServiceResponse { Success = false, Message = isValidModel.message };

                var product = _mapper.Map<Product>(modelProduct);
                var response = await _repositoryProduct.CreateProduct(product);

                return new ServiceResponse
                {
                    Success = response.Success,
                    Message = response.Message
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while validating the strategy to create a new product.");
                return new ServiceResponse { Success = false, Message = ex.Message };
            }
        }
    }
}
