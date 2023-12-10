using AutoMapper;
using Microsoft.Extensions.Logging;
using TxAssignmentInfra.Entities;
using TxAssignmentInfra.Repositories;
using TxAssignmentServices.Models;
using TxAssignmentServices.Services;

namespace TxAssignmentServices.Strategies.Cabinets
{
    public class StrategyUpdateCabinetOperation : IStrategyUpdateCabinetOperation
    {
        private readonly IRepositoryCabinet _repositoryCabinet;
        private readonly IRepositoryProduct _repositoryProduct;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;


        public StrategyUpdateCabinetOperation(IRepositoryCabinet repositoryCabinet, IRepositoryProduct repositoryProduct, IMapper mapper, ILogger logger)
        {
            _repositoryCabinet = repositoryCabinet;
            _repositoryProduct = repositoryProduct;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ServiceResponse> ExecuteAsync(Guid IdCabinet, ModelCabinet newCabinet)
        {
            try
            {
                var cabinetEntity = await _repositoryCabinet.GetCabinetById(IdCabinet);

                if (cabinetEntity == null)
                    return new ServiceResponse { Success = false, Message = "Cabinet cannot be null" };


                var isValidModel = HelpersCabinets.ModelIsValid(newCabinet);
                if (!isValidModel.isValid)
                    return new ServiceResponse { Success = false, Message = isValidModel.message };


                if (HelpersCabinets.ValidateRowsForCabinet(newCabinet.Rows, newCabinet.Size.Height))
                    return new ServiceResponse { Success = false, Message = "The total height of the rows is larger than the cabinet's height." };

                //Validations
                foreach (var row in newCabinet.Rows)
                {
                    var validateLane = HelpersCabinets.ValidateLaneWidth(row.Lanes, newCabinet.Size.Width);
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

                var result = await _repositoryCabinet.UpdateCabinet(IdCabinet, _mapper.Map<Cabinet>(newCabinet));
                return result.Success ?
                    new ServiceResponse { Success = true, Message = "Cabinet updated successfully." } :
                    new ServiceResponse { Success = false, Message = result.Message };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred on the strategy while update a cabinet.");
                return new ServiceResponse { Success = false, Message = ex.Message };
            }
        }
    }
}
