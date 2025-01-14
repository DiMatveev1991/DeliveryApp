using Microsoft.Extensions.DependencyInjection;

namespace Delivery.WPF.ViewModels
{
    static class ViewModelRegistrator
    {
        public static IServiceCollection AddViewModels(this IServiceCollection services) => services
            .AddSingleton<MainWindowViewModel>()
        ;
    }
}
