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

namespace Delivery.Data
{
    public static class DbRegistrator
    {
	    public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration) => services
		    .AddDbContext<DeliveryDbContext>(opt =>
			    {
					var type = configuration["Type"];

					switch (type)
					{
						case null: throw new InvalidOperationException("Не определен тип БД");
						default: throw new InvalidOperationException($"Тип подключения {type} не поддерживается");
						case "MSSQL":
							opt.UseSqlServer(configuration.GetConnectionString(type) ?? throw new InvalidOperationException());

							break;
					}
			    }
			    
			    ).AddRepositoriesInDb()
                 ;
    }
}
