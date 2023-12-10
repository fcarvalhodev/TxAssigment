using AutoMapper;
using TxAssigmentApi.Middlewares;
using TxAssignmentInfra.Connectors;
using TxAssignmentServices.Profiles;

namespace TxAssigmentApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Register Redis Database
            builder.Services.AddSingleton(provider =>
            {
                return RedisConnectorHelper.Connection.GetDatabase();
            });

            // Register Services and Repositories
            builder.AddServiceProduct();
            builder.AddServiceCabinet();
            builder.AddServiceUser();

            // Add AutoMapper
            var mapperConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new ProfileCabinet());
                mc.AddProfile(new ProfileProduct());
                mc.AddProfile(new ProfileUser());
            });
            IMapper mapper = mapperConfig.CreateMapper();
            builder.Services.AddSingleton(mapper);

            // Add Controllers, Swagger, etc.
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddLogging();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
    }
}
