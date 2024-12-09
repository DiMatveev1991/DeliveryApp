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
					.AddTransient<ICouriersRepository<Courier>, CouriersRepository>()
					.AddTransient<ICourierStatusesRepository<CourierStatus>, CourierStatusesRepository>()
					.AddTransient<ICourierTypesRepository<CourierType>, CourierTypesRepository>()
					.AddTransient<IOrdersRepository<Order>, OrdersRepository>()
					.AddTransient<IOrderTypesRepository<OrderType>, OrderTypesRepository>()
					.AddTransient<IOrderStatusesRepository<OrderStatus>, OrderStatusesRepository>()
					.AddTransient<IOrderLinesRepository<OrderLine>, OrderLinesRepository>()
					.AddTransient<IClientsRepository<Client>, ClientsRepository>()
					.AddTransient<IAddressesRepository<Address>, AddressesRepository>()
				;
		}
		}

}

	

