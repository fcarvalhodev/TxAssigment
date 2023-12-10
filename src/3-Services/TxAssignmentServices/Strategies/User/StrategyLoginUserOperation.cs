using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TxAssignmentInfra.Repositories;
using TxAssignmentServices.Models;
using TxAssignmentServices.Services;

namespace TxAssignmentServices.Strategies.User
{
    public class StrategyLoginUserOperation : IStrategyLoginUserOperation
    {
        private readonly IRepositoryUser _repositoryUser;
        private readonly IMapper _mapper;
        private readonly ILogger<StrategyLoginUserOperation> _logger;

        private readonly PasswordHasher<TxAssignmentInfra.Entities.User> _passwordHasher;
        private readonly IConfiguration _configuration;


        public StrategyLoginUserOperation(IRepositoryUser repositoryUser,
            IMapper mapper,
            ILogger<StrategyLoginUserOperation> logger,
            IConfiguration configuration,
            PasswordHasher<TxAssignmentInfra.Entities.User> passwordHasher)
        {
            _repositoryUser = repositoryUser;
            _mapper = mapper;
            _logger = logger;
            _configuration = configuration;
            _passwordHasher = passwordHasher;
        }


        public async Task<ServiceResponse<ModelUser>> ExecuteAsync(string email, string password)
        {
            try
            {
                var userResponse = await _repositoryUser.GetUserByEmail(email);
                if (!userResponse.Success || userResponse.Data == null)
                {
                    return new ServiceResponse<ModelUser> { Success = false, Message = "Invalid login attempt." };
                }

                var result = _passwordHasher.VerifyHashedPassword(null, userResponse.Data.PasswordHash, password);
                if (result == PasswordVerificationResult.Failed)
                {
                    return new ServiceResponse<ModelUser> { Success = false, Message = "Invalid login attempt." };
                }

                var token = GenerateJwtToken(userResponse.Data);
                return new ServiceResponse<ModelUser>
                {
                    Success = true,
                    Message = "Login successfully",
                    Data = new ModelUser { Email = email, Token = token } 
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while validating the strategy to login the user.");
                return new ServiceResponse<ModelUser> { Success = false, Message = "Invalid login attempt during the strateegy." };
            }
        }

        private string GenerateJwtToken(TxAssignmentInfra.Entities.User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
            new Claim(JwtRegisteredClaimNames.Sub, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"],
                claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}
