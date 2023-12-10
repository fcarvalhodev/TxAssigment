using AutoMapper;
using Microsoft.Extensions.Logging;
using TxAssignmentInfra.Entities;
using TxAssignmentInfra.Repositories;
using TxAssignmentServices.Models;
using TxAssignmentServices.Services;

namespace TxAssignmentServices.Strategies.Cabinets
{
    public class StrategyCreateCabinet : IStrategyCreateCabinetOperation
    {
        private readonly IRepositoryCabinet _repositoryCabinet;
        private readonly IRepositoryProduct _repositoryProduct;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public StrategyCreateCabinet(IRepositoryCabinet repositoryCabinet, IRepositoryProduct repositoryProduct, IMapper mapper, ILogger logger)
        {
            _repositoryCabinet = repositoryCabinet;
            _repositoryProduct = repositoryProduct;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ServiceResponse> ExecuteAsync(ModelCabinet cabinet)
        {
            try
            {
                var isValidModel = HelpersCabinets.ModelIsValid(cabinet);
                if (!isValidModel.isValid)
                    return new ServiceResponse { Success = false, Message = isValidModel.message };

                if (HelpersCabinets.ValidateRowsForCabinet(cabinet.Rows, cabinet.Size.Height))
                    return new ServiceResponse { Success = false, Message = "The total height of the rows is larger than the cabinet's height." };

                //Validations
                foreach (var row in cabinet.Rows)
                {
                    var validateLane = HelpersCabinets.ValidateLaneWidth(row.Lanes, cabinet.Size.Width);
                    if (!validateLane.isValid)
                        return new ServiceResponse { Success = false, Message = validateLane.errorMessage };

                    //Validate Products
                    var productResponse = await _repositoryProduct.GetAllProducts();
                    var validationResult = HelpersCabinets.ValidateLaneProducts(row.Lanes, productResponse);
                    if (!validationResult.Success)
                    {
                        return validationResult;
                    }
                }

                var result = await _repositoryCabinet.CreateCabinet(_mapper.Map<Cabinet>(cabinet));

                if (result.Success)
                    return new ServiceResponse { Success = true, Message = "Cabinet created successfully." };
                else return new ServiceResponse { Success = false, Message = string.Empty };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while validate a strategy for the cabinet.");
                return new ServiceResponse { Success = false, Message = ex.Message };
            }
        }
    }
}
