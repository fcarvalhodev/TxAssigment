using Newtonsoft.Json;
using StackExchange.Redis;
using TxAssignmentInfra.Entities;

namespace TxAssignmentInfra.Repositories
{
    public class RepositoryCabinet : IRepositoryCabinet
    {

        private readonly IDatabase _database;

        public RepositoryCabinet(IDatabase database)
        {
            _database = database;
        }

        public async Task<RepositoryResponse> CreateCabinet(Cabinet cabinet)
        {
            try
            {
                var serializedProduct = JsonConvert.SerializeObject(cabinet);
                await _database.StringSetAsync(cabinet.Id.ToString(), serializedProduct);
                return new RepositoryResponse { Success = true, Message = "Cabinet created successfully." };
            }
            catch (Exception ex)
            {
                return new RepositoryResponse { Success = false, Message = ex.Message };
            }
        }

        public async Task<RepositoryResponse> DeleteCabinet(Guid IdCabinet)
        {
            try
            {
                bool deleted = await _database.KeyDeleteAsync(IdCabinet.ToString());
                return new RepositoryResponse
                {
                    Success = deleted,
                    Message = deleted ? "Cabinet deleted successfully." : "Failed to delete cabinet."
                };
            }
            catch (Exception ex)
            {
                return new RepositoryResponse { Success = false, Message = ex.Message };
            }
        }

        public async Task<RepositoryResponse<Cabinet>> GetCabinetById(Guid IdCabinet)
        {
            try
            {
                var serializedCabinet = await _database.StringGetAsync(IdCabinet.ToString());

                if (serializedCabinet.IsNullOrEmpty)
                {
                    return new RepositoryResponse<Cabinet> { Success = false, Message = "Cabinet not found." };

                }

                var cabinet = JsonConvert.DeserializeObject<Cabinet>(serializedCabinet);
                return new RepositoryResponse<Cabinet> { Success = true, Data = cabinet };
            }
            catch (Exception ex)
            {
                return new RepositoryResponse<Cabinet> { Success = false, Message = ex.Message };
            }
        }

        public async Task<RepositoryResponse> UpdateCabinet(Guid IdCabinet, Cabinet cabinet)
        {
            try
            {
                var key = IdCabinet.ToString();
                if (!await _database.KeyExistsAsync(key))
                {
                    return new RepositoryResponse { Success = false, Message = "Cabinet not found." };
                }

                var serializedCabinet = JsonConvert.SerializeObject(cabinet);
                bool updated = await _database.StringSetAsync(key, serializedCabinet);
                return new RepositoryResponse
                {
                    Success = updated,
                    Message = updated ? "Cabinet updated successfully." : "Failed to update cabinet."
                };
            }
            catch (Exception ex)
            {
                return new RepositoryResponse { Success = false, Message = ex.Message };
            }
        }
    }
}
