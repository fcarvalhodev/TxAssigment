using TxAssignmentInfra.Repositories;
using TxAssignmentServices.Services;

namespace TxAssigmentApi.Middlewares
{
    public static class ExtensionProduct
    {
        public static void AddServiceProduct(this WebApplicationBuilder builder)
        {
            builder.Services.AddScoped<IServiceProduct, ServiceProduct>();
            builder.Services.AddScoped<IRepositoryProduct, RepositoryProduct>();
        }
    }
}
