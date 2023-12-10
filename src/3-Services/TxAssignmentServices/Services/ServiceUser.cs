using AutoMapper;
using Microsoft.Extensions.Logging;
using TxAssignmentInfra.Repositories;
using TxAssignmentServices.Models;
using TxAssignmentServices.Strategies.User;

namespace TxAssignmentServices.Services
{
    public class ServiceUser : IServiceUser
    {
        private readonly IRepositoryUser _repositoryUser;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        private readonly IStrategyCreateUserOperation _strategyCreateUserOperation;
        private readonly IStrategyLoginUserOperation _strategyLoginUserOperation;

        public ServiceUser(IRepositoryUser repositoryUser, IMapper mapper, ILogger<ServiceUser> logger, IStrategyCreateUserOperation strategyCreateUserOperation, IStrategyLoginUserOperation strategyLoginUserOperation)
        {
            _repositoryUser = repositoryUser;
            _mapper = mapper;
            _logger = logger;
            _strategyCreateUserOperation = strategyCreateUserOperation;
            _strategyLoginUserOperation = strategyLoginUserOperation;

        }

        public async Task<ServiceResponse> CreateUserAsync(ModelUser user)
        {
            try
            {
                return await _strategyCreateUserOperation.ExecuteAsync(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating the user.");
                return new ServiceResponse { Success = false, Message = ex.Message };
            }
        }

        public async Task<ServiceResponse<ModelUser>> GetUserByEmailAsync(string email)
        {
            try
            {
                var response = await _repositoryUser.GetUserByEmail(email);
                if (!response.Success)
                {
                    return new ServiceResponse<ModelUser> { Success = false, Message = response.Message };
                }

                var modelUser = _mapper.Map<ModelUser>(response.Data);
                return new ServiceResponse<ModelUser>
                {
                    Success = true,
                    Data = modelUser
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving the user.");
                return new ServiceResponse<ModelUser> { Success = false, Message = ex.Message };
            }
        }

        public async Task<ServiceResponse<ModelUser>> LoginAsync(string email, string password)
        {
            try
            {
                try
                {
                    return await _strategyLoginUserOperation.ExecuteAsync(email, password);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Login successfully.");
                    return new ServiceResponse<ModelUser> { Success = false, Message = ex.Message };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during login.");
                return new ServiceResponse<ModelUser> { Success = false, Message = ex.Message };
            }
        }
    }
}
