﻿using TxAssignmentInfra.Entities;

namespace TxAssignmentInfra.Repositories
{
    public interface IRepositoryCabinet
    {
        Task<RepositoryResponse> CreateCabinet(Cabinet cabinet);
        Task<RepositoryResponse> UpadteCabinet(Guid IdCabinet, Cabinet cabinet);
        Task<RepositoryResponse> DeleteCabinet(Guid IdCabinet);
        Task<RepositoryResponse<Cabinet>> GetCabinetById(Guid IdProduct);
        Task<RepositoryResponse<Cabinet>> GetEmptySpaceOnCabinet(Guid IdProduct);   
    }
}
