using TxAssignmentInfra.Entities;

namespace TxAssignmentInfra.Repositories
{
    public interface IRepositoryUser
    {
        Task<RepositoryResponse> CreateUser(User user);
        Task<RepositoryResponse<User>> GetUserByEmail(string email);
        Task<RepositoryResponse> DeleteUser(Guid Id);
    }
}
