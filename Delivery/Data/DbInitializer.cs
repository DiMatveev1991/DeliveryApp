using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Delivery.DAL.Context;
using Delivery.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace Delivery.WPF.Data
{
	internal class DbInitializer
	{
		private readonly DeliveryDbContext _db;
		public DbInitializer(DeliveryDbContext db)
		{
			_db = db;
		}
		public async Task InitializeAsync()
		{
			await _db.Database.MigrateAsync();
			if (await _db.OrderStatuses.AnyAsync()) return;
			await InitializeStatusCourier();
			await InitializeStatusOrder();
			await _db.DisposeAsync();
		}
		
		OrderStatus [] orderStatuses = new OrderStatus [4];
		CourierStatus[] courierStatuses = new CourierStatus [2];
			// 
		public async Task InitializeStatusOrder()
		{
			orderStatuses[0] = new OrderStatus() { StatusName = "Новая" };
			orderStatuses[1] = new OrderStatus() { StatusName = "Передано на выполнение" };
			orderStatuses[2] = new OrderStatus() { StatusName = "Выполнено" };
			orderStatuses[3] = new OrderStatus() { StatusName = "Отменена" };
			await _db.AddRangeAsync(orderStatuses);
			await _db.SaveChangesAsync();
		}

		public async Task InitializeStatusCourier()
		{
			courierStatuses[0] = new CourierStatus() { StatusName = "Выполняет заказ" };
			courierStatuses[1] = new CourierStatus(){ StatusName = "Готов к выполнению заказа" };
			await _db.AddRangeAsync(courierStatuses);
			await _db.SaveChangesAsync();
		}



	}
}
