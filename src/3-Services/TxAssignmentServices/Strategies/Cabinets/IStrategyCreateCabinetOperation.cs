using TxAssignmentServices.Models;
using TxAssignmentServices.Services;

namespace TxAssignmentServices.Strategies.Cabinets
{
    public interface IStrategyCreateCabinetOperation
    {
        Task<ServiceResponse> ExecuteAsync(ModelCabinet cabinet);
    }
}
