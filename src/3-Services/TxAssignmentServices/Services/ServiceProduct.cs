using AutoMapper;
using Microsoft.Extensions.Logging;
using TxAssignmentInfra.Repositories;
using TxAssignmentServices.Models;
using TxAssignmentServices.Strategies.Products;

namespace TxAssignmentServices.Services
{
    public class ServiceProduct : IServiceProduct
    {
        private readonly IRepositoryProduct _repositoryProduct;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        private readonly IStrategyCreateProductOperation _strategyCreateProductOperation;
        private readonly IStrategyUpdateProductOperation _strategyUpdateProductOperation;
        private readonly IStrategyDeleteProductOperation _strategyDeleteProductOperation;

        public ServiceProduct(IRepositoryProduct repositoryProduct, IMapper mapper, ILogger<ServiceProduct> logger, 
            IStrategyCreateProductOperation strategyCreateProductOperation,
            IStrategyUpdateProductOperation strategyUpdateProductOperation,
            IStrategyDeleteProductOperation strategyDeleteProductOperation)
        {
            _mapper = mapper;
            _logger = logger;
            _repositoryProduct = repositoryProduct;
            _strategyCreateProductOperation = strategyCreateProductOperation;
            _strategyUpdateProductOperation = strategyUpdateProductOperation;
            _strategyDeleteProductOperation = strategyDeleteProductOperation;
        }

        public async Task<ServiceResponse> CreateProduct(ModelProduct modelProduct)
        {
            try
            {
                return await _strategyCreateProductOperation.ExecuteAsync(modelProduct);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating product.");
                return new ServiceResponse { Success = false, Message = ex.Message };
            }
        }

        public async Task<ServiceResponse> UpdateProduct(string janCode, ModelProduct modelProduct)
        {
            try
            {
                return await _strategyUpdateProductOperation.ExecuteAsync(janCode, modelProduct);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating product.");
                return new ServiceResponse { Success = false, Message = ex.Message };
            }
        }

        public async Task<ServiceResponse> DeleteProduct(string janCode)
        {
            try
            {
                return await _strategyDeleteProductOperation.ExecuteAsync(janCode);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting product.");
                return new ServiceResponse { Success = false, Message = ex.Message };
            }
        }

        public async Task<ServiceResponse<List<ModelProduct>>> GetAllProducts()
        {
            try
            {
                var response = await _repositoryProduct.GetAllProducts();
                if (!response.Success)
                {
                    return new ServiceResponse<List<ModelProduct>> { Success = false, Message = response.Message };
                }

                var modelProducts = _mapper.Map<List<ModelProduct>>(response.Data);
                return new ServiceResponse<List<ModelProduct>>
                {
                    Success = true,
                    Data = modelProducts
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving all products.");
                return new ServiceResponse<List<ModelProduct>> { Success = false, Message = ex.Message };
            }
        }

        public async Task<ServiceResponse<ModelProduct>> GetProductByJanCode(string janCode)
        {
            try
            {
                var response = await _repositoryProduct.GetProductByJanCode(janCode);
                if (!response.Success)
                {
                    return new ServiceResponse<ModelProduct> { Success = false, Message = response.Message };
                }

                var modelProduct = _mapper.Map<ModelProduct>(response.Data);
                return new ServiceResponse<ModelProduct>
                {
                    Success = true,
                    Data = modelProduct
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving product.");
                return new ServiceResponse<ModelProduct> { Success = false, Message = ex.Message };
            }
        }
    }
}
