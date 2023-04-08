using DemoGrpc.Interceptors;
using Grpc.Core;
using Grpc.Net.Client.Configuration;
using GrpcService;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddGrpcClient<UserContract.UserContractClient>(o =>
{
    o.Address = new Uri(builder.Configuration["GrpcServer"]);
})
    .ConfigureChannel(o =>
    {
        var defaultMethodConfig = new MethodConfig
        {
            Names = { MethodName.Default },
            RetryPolicy = new RetryPolicy
            {
                MaxAttempts = 5, // số lần retry tối đa
                InitialBackoff = TimeSpan.FromSeconds(1), // độ trễ khởi tạo giữa các lần retry
                MaxBackoff = TimeSpan.FromSeconds(5), // độ trễ tối đa
                BackoffMultiplier = 1.5, //độ trễ tăng sau mỗi lần retry
                RetryableStatusCodes = { StatusCode.Unavailable } // status code cần retry
            }
        };

        // gửi gửi liên tục các bản sao của rpc
        // lấy response đầu tiên
        //var defaultMethodConfig = new MethodConfig
        //{
        //    Names = { MethodName.Default },
        //    HedgingPolicy = new HedgingPolicy
        //    {
        //        MaxAttempts = 5,
        //        NonFatalStatusCodes = { StatusCode.Unavailable }
        //    }
        //};

        o.ServiceConfig = new ServiceConfig { MethodConfigs = { defaultMethodConfig } };
    })
        //.AddCallCredentials((context, metadata) =>
        //{
        //    // bearer token
        //    if (!string.IsNullOrEmpty(_token))
        //    {
        //        metadata.Add("Authorization", $"Bearer {_token}");
        //    }
        //    return Task.CompletedTask;
        //})
        .AddInterceptor<ClientLoggingInterceptor>();

builder.Services.AddSingleton<ClientLoggingInterceptor>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=User}/{action=Index}/{id?}");

app.Run();