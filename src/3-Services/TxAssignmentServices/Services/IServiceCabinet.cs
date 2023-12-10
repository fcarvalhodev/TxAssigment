using TxAssignmentInfra.Entities;
using TxAssignmentInfra.Repositories;
using TxAssignmentServices.Models;

namespace TxAssignmentServices.Services
{
    public interface IServiceCabinet
    {
        Task<ServiceResponse> CreateCabinet(ModelCabinet cabinet);
        Task<ServiceResponse> UpdateCabinet(Guid IdCabinet, ModelCabinet newCabinet);
        Task<ServiceResponse> DeleteCabinet(Guid IdCabinet);
        Task<ServiceResponse<ModelCabinet>> GetCabinetById(Guid IdCabinet);
        Task<ServiceResponse<List<ModelCabinet>>> GetAllCabinets();
    }
}
