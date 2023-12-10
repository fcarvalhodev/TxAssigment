using TxAssignmentServices.Services;

namespace TxAssignmentServices.Strategies.Cabinets
{
    public interface IStrategyDeleteCabinetOperation
    {
        Task<ServiceResponse> ExecuteAsync(Guid IdCabinet);
    }
}
