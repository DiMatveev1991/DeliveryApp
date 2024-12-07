using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Configuration;
using System.Data;
using System.Windows;
using Delivery.Data;
using Delivery.Services;
using Delivery.ViewModels;

namespace Delivery
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		private static IHost _Host;

		public static IHost Host => _Host 
			??= Program.CreatHostBuilder(Environment.GetCommandLineArgs()).Build();

		public static IServiceProvider Services => Host.Services;
		internal static void ConfigureServices(HostBuilderContext host, IServiceCollection services) => services
			.AddDatabase(host.Configuration.GetSection("Database"))
			.AddServices()
			.AddViewModels()
		;
		

		protected override async void OnStartup(StartupEventArgs e)
		{
			var host =Host;
			base.OnStartup(e);
			await host.StartAsync();
		}

		protected override async void OnExit(ExitEventArgs e)
		{
			using var host =Host;
			base.OnExit(e);
			await host.StartAsync();
		}
	}

}
