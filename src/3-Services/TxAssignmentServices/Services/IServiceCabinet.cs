using TxAssignmentServices.Models;

namespace TxAssignmentServices.Services
{
    public interface IServiceCabinet
    {
        Task<ServiceResponse> CreateCabinet(ModelCabinet cabinet);
        Task<ServiceResponse> UpadteCabinet(Guid IdCabinet, ModelCabinet cabinet);
        Task<ServiceResponse> DeleteCabinet(Guid IdCabinet);
        Task<ServiceResponse<ModelCabinet>> GetCabinetById(Guid IdCabinet);
        Task<ServiceResponse<ModelCabinet>> GetEmptySpaceOnCabinet(Guid cabinetId, ModelProduct newProduct);
    }
}
