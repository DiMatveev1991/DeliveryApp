// See https://aka.ms/new-console-template for more information

using Delivery.BLL.ServicesRegistrator;
using Delivery.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Delivery.WPF.Data;

static void ConfigureServices(HostBuilderContext host, IServiceCollection services) => services
    .AddDatabase(host.Configuration.GetSection("Database"))
    .AddServicesRegistrator()

;

static IHostBuilder CreateHostBuilder(string[] args) => Host
    .CreateDefaultBuilder(args)
    .ConfigureServices(ConfigureServices)
;

try
{
    var host = CreateHostBuilder(Environment.GetCommandLineArgs()).Build();

    var Services = host.Services;


    Console.WriteLine("Initialize Started");
    using var scope = Services.CreateScope();
    var dbInitializer = scope.ServiceProvider.GetRequiredService<DbInitializer>();
    await dbInitializer.InitializeAsync();

    Console.WriteLine("Initialize Finished");
}
catch (Exception e)
{
    Console.WriteLine(e);
}