using Grpc.Core;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using TxAssignmentWeb.Data;
using Grpc.Net.Client;
using Grpc.Net.Client.Web;


namespace TxAssignmentWeb;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddRazorPages();
        builder.Services.AddServerSideBlazor();
        builder.Services.AddSingleton<WeatherForecastService>();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();

        app.UseStaticFiles();

        app.UseRouting();

        app.MapBlazorHub();
        app.MapFallbackToPage("/_Host");

        app.Run();
    }
    
    // private static void AddGrpcService<TClient>(IServiceCollection services, string baseAddress) where TClient : ClientBase<TClient>
    // {
    //     services.AddScoped<TClient>(provider =>
    //     {
    //         var httpClient = new HttpClient(new GrpcWebHandler(GrpcWebMode.GrpcWeb, new HttpClientHandler()));
    //         var channel = GrpcChannel.ForAddress(baseAddress, new GrpcChannelOptions { HttpClient = httpClient });
    //         return (TClient)Activator.CreateInstance(typeof(TClient), channel);
    //     });
    // }
}