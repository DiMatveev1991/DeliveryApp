using Delivery.WPF.Services.Services;
using Delivery.WPF.Services.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Delivery.WPF.Services
{
    static class ServicesRegistrator
    {
        public static IServiceCollection AddServicesDialog(this IServiceCollection services) => services
           .AddTransient<IUserDialog, UserDialogService>()
        ;
    }
}
