using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using TxAssignmentInfra.Repositories;
using TxAssignmentServices.Models;
using TxAssignmentServices.Services;

namespace TxAssignmentServices.Strategies.User
{
    public class StrategyCreateUserOperation : IStrategyCreateUserOperation
    {
        private readonly IRepositoryUser _repositoryUser;
        private readonly IMapper _mapper;
        private readonly ILogger<StrategyCreateUserOperation> _logger;

        private readonly PasswordHasher<ModelUser> _passwordHasher;


        public StrategyCreateUserOperation(IRepositoryUser repositoryUser, 
            IMapper mapper, 
            ILogger<StrategyCreateUserOperation> logger,
             PasswordHasher<ModelUser> passwordHasher)
        {
            _repositoryUser = repositoryUser;
            _mapper = mapper;
            _logger = logger;
            _passwordHasher = passwordHasher;

        }


        public async Task<ServiceResponse> ExecuteAsync(ModelUser modelUser)
        {
            try
            {
                var isValidModel = HelperUser.ModelIsValid(modelUser);
                if (!isValidModel.isValid)
                    return new ServiceResponse { Success = false, Message = isValidModel.message };

                var user = _mapper.Map<TxAssignmentInfra.Entities.User>(modelUser);
                user.PasswordHash = HashPassword(modelUser.Password);
                var response = await _repositoryUser.CreateUser(user);

                return new ServiceResponse
                {
                    Success = response.Success,
                    Message = response.Message
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while validating the strategy to create a new user.");
                return new ServiceResponse { Success = false, Message = ex.Message };
            }
        }

        internal string HashPassword(string password)
        {
            return _passwordHasher.HashPassword(null, password);
        }
    }
}
