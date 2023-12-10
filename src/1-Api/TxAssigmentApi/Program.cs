using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
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
            var configuration = builder.Configuration;

            // Register Redis Database
            builder.Services.AddSingleton(provider =>
            {
                return RedisConnectorHelper.Connection.GetDatabase();
            });

            // Register Services and Repositories
            builder.AddServiceProduct();
            builder.AddServiceCabinet();
            builder.AddServiceUser();
            builder.AddServiceSwagger();

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

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                               .AddJwtBearer(options =>
                               {
                                   options.TokenValidationParameters = new TokenValidationParameters
                                   {
                                       ValidateIssuer = true,
                                       ValidateAudience = true,
                                       ValidateLifetime = true,
                                       ValidateIssuerSigningKey = true,
                                       ValidIssuer = configuration["Jwt:Issuer"],
                                       ValidAudience = configuration["Jwt:Audience"],
                                       IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]))
                                   };
                               });



            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Your API V1");
                c.RoutePrefix = string.Empty;
            });

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();
            app.Run();
        }
    }
}
