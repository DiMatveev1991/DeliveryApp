using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Delivery.BLL;
using Delivery.BLL.ServicesRegistrator;
using Delivery.DAL.Context;
using Delivery.DAL.Registrator;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Delivery.WPF.Data;
using Delivery.WPF.Services;

namespace Delivery.Data
{
    public static class DbRegistrator
    {
	    public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration Configuration) => services
		    .AddDbContext<DeliveryDbContext>(opt =>
			    {
					var type = Configuration["Type"];

					switch (type)
					{
						case null: throw new InvalidOperationException("Не определен тип БД");
						default: throw new InvalidOperationException($"Тип подключения {type} не поддерживается");
						case "MSSQL":
							opt.UseSqlServer(Configuration.GetConnectionString(type));
							break;
					}
			    }
			    
			    ).AddTransient<DbInitializer> ()
		         .AddRepositoriesInDb()
		        
        ;
    }
}
