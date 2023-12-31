﻿using TxAssignmentServices.Models;

namespace TxAssignmentServices.Services
{
    public interface IServiceUser
    {
        Task<ServiceResponse> CreateUserAsync(ModelUser user);
        Task<ServiceResponse<ModelUser>> GetUserByEmailAsync(string email);

        Task<ServiceResponse<ModelUser>> LoginAsync(string email, string password);
    }
}
