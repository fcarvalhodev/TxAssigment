using TxAssignmentServices.Models;
using TxAssignmentServices.Services;

namespace TxAssignmentServices.Strategies.User
{
    public interface IStrategyLoginUserOperation
    {
        Task<ServiceResponse<ModelUser>> ExecuteAsync(string email, string password);
    }
}
