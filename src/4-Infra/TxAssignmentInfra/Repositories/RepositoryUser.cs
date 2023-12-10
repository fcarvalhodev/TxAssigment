using Newtonsoft.Json;
using StackExchange.Redis;
using TxAssignmentInfra.Entities;
using TxAssignmentInfra.Entities.Enumerators;

namespace TxAssignmentInfra.Repositories
{
    public class RepositoryUser : IRepositoryUser
    {

        private readonly IDatabase _database;

        public RepositoryUser(IDatabase database)
        {
            _database = database;
        }


        public async Task<RepositoryResponse> CreateUser(User user)
        {
            try
            {
                if (string.IsNullOrEmpty(user.Email))
                    return new RepositoryResponse { Success = false, Message = "User email is mandatory" };

                string userKey = $"{RedisDocTypes.USER}{user.Id}";
                bool userExists = await _database.KeyExistsAsync(userKey);
                if (userExists)
                    return new RepositoryResponse { Success = false, Message = "A user with this email already exists." };

                var serializedUser = JsonConvert.SerializeObject(user);
                await _database.StringSetAsync(userKey, serializedUser);
                await _database.SetAddAsync("userKeys", userKey);

                return new RepositoryResponse { Success = true, Message = "User created successfully." };
            }
            catch (Exception ex)
            {
                return new RepositoryResponse { Success = false, Message = ex.Message };
            }
        }

        public async Task<RepositoryResponse<User>> GetUserByEmail(string email)
        {
            try
            {
                var serializedUserKeys = await _database.SetMembersAsync("userKeys");

                if (serializedUserKeys.Length == 0)
                    return new RepositoryResponse<User> { Success = false, Message = "No users found." };

                foreach (var keyVal in serializedUserKeys)
                {
                    string key = keyVal.ToString();
                    var serializedUser = await _database.StringGetAsync(key);
                    if (!serializedUser.IsNullOrEmpty)
                    {
                        var user = JsonConvert.DeserializeObject<User>(serializedUser);
                        if (user.Email == email)
                        {
                            return new RepositoryResponse<User> { Success = true, Data = user };
                        }
                    }
                }
                return new RepositoryResponse<User> { Success = false, Message = "User not found." };
            }
            catch (Exception ex)
            {
                return new RepositoryResponse<User> { Success = false, Message = ex.Message };
            }
        }

        public async Task<RepositoryResponse> DeleteUser(Guid Id)
        {
            try
            {
                var key = $"{RedisDocTypes.USER}{Id}";
                bool deleted = await _database.KeyDeleteAsync(key);
                await _database.SetRemoveAsync("userKeys", key);
                return new RepositoryResponse
                {
                    Success = deleted,
                    Message = deleted ? "User deleted successfully." : "Failed to delete user."
                };
            }
            catch (Exception ex)
            {
                return new RepositoryResponse { Success = false, Message = ex.Message };
            }
        }
    }
}
