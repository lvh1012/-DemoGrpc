using GrpcService.Database;
using GrpcService.Interceptors;
using GrpcService.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Additional configuration is required to successfully run gRPC on macOS.
// For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682

// Add services to the container.
builder.Services.AddGrpc(options =>
{
    options.Interceptors.Add<ServerLoggerInterceptor>();
});
builder.Services.AddDbContext<ApplicationDbContext>(
        options =>
        {
            options.EnableSensitiveDataLogging()
            .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
            .UseSqlServer(builder.Configuration.GetConnectionString("DemoDb"));
        });

var app = builder.Build();

// authen, author
//app.UseRouting();

//app.UseAuthentication();
//app.UseAuthorization();

// Configure the HTTP request pipeline.
app.MapGrpcService<GreeterService>();
app.MapGrpcService<UserService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();