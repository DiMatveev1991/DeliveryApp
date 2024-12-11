using System;
using System.Collections.Generic;
using System.Text;
using Delivery.DAL.Interfaces;
using Delivery.DAL.Models;
using Delivery.DAL.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Delivery.DAL.Registrator
{
	
		public static class RepositoryRegistrator
		{
		public static IServiceCollection AddRepositoriesInDb(this IServiceCollection services)
		{
			return services
					.AddTransient<ICouriersRepository, CouriersRepository>()
					.AddTransient<ICourierStatusesRepository, CourierStatusesRepository>()
					.AddTransient<IOrdersRepository, OrdersRepository>()
					.AddTransient<IOrderStatusesRepository, OrderStatusesRepository>()
					.AddTransient<IOrderLinesRepository, OrderLinesRepository>()
					.AddTransient<IClientsRepository, ClientsRepository>()
					.AddTransient<IAddressesRepository, AddressesRepository>()
				;
		}
		}

}

	

