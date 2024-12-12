using Delivery.BLL.Interfaces;
using Delivery.BLL.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Delivery.BLL
{
	public static class ServicesRegistrator
	{
		public static IServiceCollection AddServicesRegistrator (this IServiceCollection services) => services
			.AddTransient<ICourierService, CourierService>()
			.AddTransient<IOrderService, OrderService>()
			.AddTransient<IClientService, ClientService>()
			;
	}
}
