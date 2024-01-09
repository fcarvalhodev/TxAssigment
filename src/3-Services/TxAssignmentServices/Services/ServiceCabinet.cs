using AutoMapper;
using Microsoft.Extensions.Logging;
using TxAssignmentInfra.Repositories;
using TxAssignmentServices.Models;
using TxAssignmentServices.Strategies.Cabinets;

namespace TxAssignmentServices.Services
{
    public class ServiceCabinet : IServiceCabinet
    {
        private readonly IRepositoryCabinet _repositoryCabinet;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        private readonly IStrategyCreateCabinetOperation _strategyCreateCabinetOperation;
        private readonly IStrategyUpdateCabinetOperation _strategyUpdateCabinetOperation;
        private readonly IStrategyDeleteCabinetOperation _strategyDeleteCabinetOperation;

        public ServiceCabinet(IRepositoryCabinet repositoryCabinet, IMapper mapper, ILogger<ServiceCabinet> logger,
            IStrategyCreateCabinetOperation strategyCreateCabinetOperation,
            IStrategyUpdateCabinetOperation strategyUpdateCabinetOperation,
            IStrategyDeleteCabinetOperation strategyDeleteCabinetOperation)
        {
            _mapper = mapper;
            _repositoryCabinet = repositoryCabinet;
            _logger = logger;

            _strategyCreateCabinetOperation = strategyCreateCabinetOperation;
            _strategyUpdateCabinetOperation = strategyUpdateCabinetOperation;
            _strategyDeleteCabinetOperation = strategyDeleteCabinetOperation;
        }

        public async Task<ServiceResponse> CreateCabinet(ModelCabinet cabinet)
        {
            try
            {
                return await _strategyCreateCabinetOperation.ExecuteAsync(cabinet);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating a cabinet.");
                return new ServiceResponse { Success = false, Message = ex.Message };
            }
        }

        public async Task<ServiceResponse> UpdateCabinet(Guid IdCabinet, ModelCabinet newCabinet)
        {
            try
            {
                return await _strategyUpdateCabinetOperation.ExecuteAsync(IdCabinet, newCabinet);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while update a cabinet.");
                return new ServiceResponse { Success = false, Message = ex.Message };
            }
        }

        public async Task<ServiceResponse> DeleteCabinet(Guid IdCabinet)
        {
            try
            {
                return await _strategyDeleteCabinetOperation.ExecuteAsync(IdCabinet);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while delete the cabinet.");
                return new ServiceResponse { Success = false, Message = ex.Message };
            }
        }

        public async Task<ServiceResponse<ModelCabinet>> GetCabinetById(Guid IdCabinet)
        {
            try
            {
                var result = await _repositoryCabinet.GetCabinetById(IdCabinet);

                if (result.Success)
                    return new ServiceResponse<ModelCabinet>
                    {
                        Success = true, Data = _mapper.Map<ModelCabinet>(result.Data),
                        Message = "Cabinet retrieved successfully."
                    };
                else
                    return new ServiceResponse<ModelCabinet> { Success = false, Message = "Cabinet not found." };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred trying to get the cabinet.");
                return new ServiceResponse<ModelCabinet> { Success = false, Message = ex.Message };
            }
        }

        public async Task<ServiceResponse<List<ModelCabinet>>> GetAllCabinets()
        {
            try
            {
                var response = await _repositoryCabinet.GetAllCabinets();

                if (!response.Success)
                {
                    return new ServiceResponse<List<ModelCabinet>> { Success = false, Message = response.Message };
                }

                var modelCabinets = _mapper.Map<List<ModelCabinet>>(response.Data);
                return new ServiceResponse<List<ModelCabinet>>
                {
                    Success = true,
                    Data = modelCabinets
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving all cabinets.");
                return new ServiceResponse<List<ModelCabinet>> { Success = false, Message = ex.Message };
            }
        }
    }
}