using TxAssignmentServices.Models;
using TxAssignmentServices.Services;

namespace TxAssignmentServices.Strategies.Products
{
    public interface IStrategyUpdateProductOperation
    {
        Task<ServiceResponse> ExecuteAsync(string janCode, ModelProduct modelProduct);
    }
}
