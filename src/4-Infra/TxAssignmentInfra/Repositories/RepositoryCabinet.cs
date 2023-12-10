using Newtonsoft.Json;
using StackExchange.Redis;
using TxAssignmentInfra.Entities;
using TxAssignmentInfra.Entities.Enumerators;

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
                string key = $"{RedisDocTypes.CAB}{cabinet.Id}";
                var existingProduct = await GetCabinetById(cabinet.Id);
                if (existingProduct.Data != null)
                    return new RepositoryResponse { Success = false, Message = "The cabinet already exists on the database" };


                var serializedProduct = JsonConvert.SerializeObject(cabinet);
                await _database.StringSetAsync(key, serializedProduct);
                await _database.SetAddAsync("cabinetKeys", key);
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
                string key = $"{RedisDocTypes.CAB}{IdCabinet}";
                bool deleted = await _database.KeyDeleteAsync(key);
                await _database.SetRemoveAsync("cabinetKeys", key);
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
                string key = $"{RedisDocTypes.CAB}{IdCabinet}";
                var serializedCabinet = await _database.StringGetAsync(key);

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
                string key = $"{RedisDocTypes.CAB}{IdCabinet}";
                if (!await _database.KeyExistsAsync(key))
                {
                    return new RepositoryResponse { Success = false, Message = "Cabinet not found." };
                }

                cabinet.Id = IdCabinet;
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

        public async Task<RepositoryResponse<List<Cabinet>>> GetAllCabinets()
        {
            try
            {
                var cabinetKeys = await _database.SetMembersAsync("cabinetKeys");
                var cabinets = new List<Cabinet>();

                foreach (var keyVal in cabinetKeys)
                {
                    string key = keyVal.ToString();
                    var serializedCabinet = await _database.StringGetAsync(key);
                    if (!serializedCabinet.IsNullOrEmpty)
                    {
                        var cabinet = JsonConvert.DeserializeObject<Cabinet>(serializedCabinet);
                        cabinets.Add(cabinet);
                    }
                }

                return new RepositoryResponse<List<Cabinet>> { Success = true, Data = cabinets };
            }
            catch (Exception ex)
            {
                return new RepositoryResponse<List<Cabinet>> { Success = false, Message = ex.Message };
            }
        }
    }
}
