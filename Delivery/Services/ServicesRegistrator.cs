using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Delivery.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace Delivery.Services
{
	static class ServicesRegistrator
	{
		public static IServiceCollection AddServices(this IServiceCollection services) => services
			.AddSingleton<MainWindowViewModel>()
		;
	}
}
