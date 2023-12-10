using TxAssignmentServices.Models;
using TxAssignmentServices.Services;

namespace TxAssignmentServices.Strategies.Products
{
    public interface IStrategyCreateProductOperation
    {
        Task<ServiceResponse> ExecuteAsync(ModelProduct modelProduct);
    }
}
