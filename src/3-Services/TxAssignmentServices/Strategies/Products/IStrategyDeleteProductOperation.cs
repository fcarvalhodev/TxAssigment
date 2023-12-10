using TxAssignmentServices.Services;

namespace TxAssignmentServices.Strategies.Products
{
    public interface IStrategyDeleteProductOperation
    {
        Task<ServiceResponse> ExecuteAsync(string janCode);
    }
}
