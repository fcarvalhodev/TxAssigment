using TxAssignmentServices.Models;
using TxAssignmentServices.Services;

namespace TxAssignmentServices.Strategies.User
{
    public interface IStrategyCreateUserOperation
    {
        Task<ServiceResponse> ExecuteAsync(ModelUser user);
    }
}
