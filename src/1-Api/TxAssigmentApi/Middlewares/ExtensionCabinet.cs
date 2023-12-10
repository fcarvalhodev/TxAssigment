using TxAssignmentInfra.Repositories;
using TxAssignmentServices.Services;
using TxAssignmentServices.Strategies.Cabinets;

namespace TxAssigmentApi.Middlewares
{
    public static class ExtensionCabinet
    {
        public static void AddServiceCabinet(this WebApplicationBuilder builder)
        {
            builder.Services.AddScoped<IServiceCabinet, ServiceCabinet>();
            builder.Services.AddScoped<IRepositoryCabinet, RepositoryCabinet>();

            builder.Services.AddScoped<IStrategyCreateCabinetOperation, StrategyCreateCabinet>();
            builder.Services.AddScoped<IStrategyUpdateCabinetOperation, StrategyUpdateCabinetOperation>();
            builder.Services.AddScoped<IStrategyDeleteCabinetOperation, StrategyDeleteCabinetOperation>();
        }
    }
}
