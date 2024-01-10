using TxAssignmentGRPC.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddGrpc();

builder.WebHost.ConfigureKestrel(options => 
{
    options.ListenAnyIP(80);
    options.ListenAnyIP(443);
});

var app = builder.Build();
app.MapGrpcService<CabinetService>();

app.MapGet("/",
    () =>
        "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();