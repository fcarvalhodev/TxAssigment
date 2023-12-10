using Microsoft.AspNetCore.Identity;
using TxAssignmentInfra.Entities;
using TxAssignmentInfra.Repositories;
using TxAssignmentServices.Models;
using TxAssignmentServices.Services;
using TxAssignmentServices.Strategies.User;

namespace TxAssigmentApi.Middlewares
{
    public static class ExtensionUser
    {
        public static void AddServiceUser(this WebApplicationBuilder builder)
        {
            builder.Services.AddScoped<IServiceUser, ServiceUser>();
            builder.Services.AddScoped<IRepositoryUser, RepositoryUser>();

            builder.Services.AddScoped<IStrategyLoginUserOperation, StrategyLoginUserOperation>();
            builder.Services.AddScoped<IStrategyCreateUserOperation, StrategyCreateUserOperation>();

            builder.Services.AddScoped<PasswordHasher<ModelUser>>();
            builder.Services.AddScoped<PasswordHasher<User>>();
        }
    }
}
