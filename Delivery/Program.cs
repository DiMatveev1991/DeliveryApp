using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace Delivery
{
	
	class Program
	{
		[STAThread]
		static void Main(string[] args)
		{
			var app = new App();
			app.InitializeComponent();
			app.Run();
		}

		public static IHostBuilder CreatHostBuilder(string[] args) => Host
			.CreateDefaultBuilder(args)
			.ConfigureServices(App.ConfigureServices)
		;
		
	}
}
