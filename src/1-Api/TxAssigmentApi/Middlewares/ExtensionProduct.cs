using TxAssignmentInfra.Repositories;
using TxAssignmentServices.Services;
using TxAssignmentServices.Strategies.Products;

namespace TxAssigmentApi.Middlewares
{
    public static class ExtensionProduct
    {
        public static void AddServiceProduct(this WebApplicationBuilder builder)
        {
            builder.Services.AddScoped<IServiceProduct, ServiceProduct>();
            builder.Services.AddScoped<IRepositoryProduct, RepositoryProduct>();

            builder.Services.AddScoped<IStrategyCreateProductOperation, StrategyCreateProductOperation>();
            builder.Services.AddScoped<IStrategyUpdateProductOperation, StrategyUpdateProductOperation>();
            builder.Services.AddScoped<IStrategyDeleteProductOperation, StrategyDeleteProductOperation>();
        }
    }
}
