using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Configuration;
using System.Data;
using System.Windows;
using Delivery.BLL;
using Delivery.Data;
using Delivery.WPF.Data;
using Delivery.BLL.ServicesRegistrator;
using Delivery.WPF.ViewModels;
using Delivery.WPF.Services;

namespace Delivery.WPF
{

    public partial class App : Application
    {

        public App()
        {
            Startup += PreStart;
        }

        public static Window ActiveWindow => Application.Current.Windows
              .OfType<Window>()
              .FirstOrDefault(w => w.IsActive);

        public static Window FocusedWindow => Application.Current.Windows
           .OfType<Window>()
           .FirstOrDefault(w => w.IsFocused);

        public static Window CurrentWindow => FocusedWindow ?? ActiveWindow;

        public static bool IsDesignTime { get; private set; } = true;

        private static IHost _Host;

        public static IHost Host => _Host
            ??= Program.CreateHostBuilder(Environment.GetCommandLineArgs()).Build();

        public static IServiceProvider Services => Host.Services;
        internal static void ConfigureServices(HostBuilderContext host, IServiceCollection services) => services
            .AddDatabase(host.Configuration.GetSection("Database"))
            .AddServicesRegistrator()
            .AddViewModels()
            .AddServicesDialog()

        ;

        private async void PreStart(object sender, StartupEventArgs e)
        {
            //using var scope = Services.CreateScope();
            //var dbInitializer = scope.ServiceProvider.GetRequiredService<DbInitializer>();
            //await dbInitializer.InitializeAsync();
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            IsDesignTime = false;
            var host = Host;
            //using (var scope = Services.CreateAsyncScope())
            //    await scope.ServiceProvider.GetRequiredService<DbInitializer>().InitializeAsync();
            base.OnStartup(e);
            host.Start();
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            using var host = Host;
            base.OnExit(e);
            host.StopAsync().GetAwaiter().GetResult();
        }
    }

}
