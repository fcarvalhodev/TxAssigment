using TxAssignmentInfra.Repositories;
using TxAssignmentServices.Services;

namespace TxAssigmentApi.Middlewares
{
    public static class ExtensionCabinet
    {
        public static void AddServiceCabinet(this WebApplicationBuilder builder)
        {
            builder.Services.AddScoped<IServiceCabinet, ServiceCabinet>();
            builder.Services.AddScoped<IRepositoryCabinet, RepositoryCabinet>();
        }
    }
}
