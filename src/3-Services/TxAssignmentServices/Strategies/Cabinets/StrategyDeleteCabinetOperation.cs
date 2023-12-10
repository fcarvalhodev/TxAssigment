using Microsoft.Extensions.Logging;
using TxAssignmentInfra.Repositories;
using TxAssignmentServices.Services;

namespace TxAssignmentServices.Strategies.Cabinets
{
    public class StrategyDeleteCabinetOperation : IStrategyDeleteCabinetOperation
    {
        private readonly IRepositoryCabinet _repositoryCabinet;
        private readonly ILogger _logger;

        public StrategyDeleteCabinetOperation(IRepositoryCabinet repositoryCabinet, ILogger logger)
        {
            _repositoryCabinet = repositoryCabinet;
            _logger = logger;
        }

        public async Task<ServiceResponse> ExecuteAsync(Guid IdCabinet)
        {
            try
            {
                var cabinetEntity = await _repositoryCabinet.GetCabinetById(IdCabinet);

                if (cabinetEntity == null)
                    return new ServiceResponse { Success = false, Message = "Cabinet cannot be null" };

                var result = await _repositoryCabinet.DeleteCabinet(IdCabinet);

                if (result.Success)
                    return new ServiceResponse { Success = true, Message = "Cabinet deleted successfully." };
                else return new ServiceResponse { Success = false, Message = string.Empty };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while validate a strategy to delete the cabinet.");
                return new ServiceResponse { Success = false, Message = ex.Message };
            }
        }
    }
}
