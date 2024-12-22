using Delivery.BLL.Interfaces;
using Delivery.BLL.Services;
using Delivery.DAL.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Delivery.BLL.ServicesRegistrator
{
    public static class ServicesRegistrator
    {
        public static IServiceCollection AddServicesRegistrator (this IServiceCollection services) => services
            .AddTransient<IClientService, ClientService>()
            .AddTransient<ICourierService, CourierService>()
            .AddTransient<IOrderService, OrderService>()
                ;
        
    }
}
