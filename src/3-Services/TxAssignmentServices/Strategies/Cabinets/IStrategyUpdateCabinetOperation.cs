using TxAssignmentServices.Models;
using TxAssignmentServices.Services;

namespace TxAssignmentServices.Strategies.Cabinets
{
    public interface IStrategyUpdateCabinetOperation
    {
        Task<ServiceResponse> ExecuteAsync(Guid IdCabinet, ModelCabinet newCabinet);
    }
}
