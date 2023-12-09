using TxAssignmentInfra.Entities;

namespace TxAssignmentInfra.Repositories
{
    public interface IRepositoryCabinet
    {
        Task<RepositoryResponse> CreateCabinet(Cabinet cabinet);
        Task<RepositoryResponse> UpdateCabinet(Guid IdCabinet, Cabinet cabinet);
        Task<RepositoryResponse> DeleteCabinet(Guid IdCabinet);
        Task<RepositoryResponse<Cabinet>> GetCabinetById(Guid IdCabinet);
    }
}
