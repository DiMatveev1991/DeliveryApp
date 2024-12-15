using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Delivery.BLL.Interfaces;
using Delivery.DAL.Context;
using Delivery.DAL.Interfaces;
using Delivery.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Delivery.BLL.Services;

namespace Delivery.WPF.Data
{
	internal class DbInitializer
	{
		private readonly DeliveryDbContext _db;
		private readonly CourierService _courierService;
		public DbInitializer(DeliveryDbContext db, IUnitOfWork unitOfWork)
		{
			_db = db;
			_courierService = new CourierService(unitOfWork);
		}
		public async Task InitializeAsync()
		{ 
	
			await _db.Database.MigrateAsync();
			if (await _db.OrderStatuses.AnyAsync()) return;
			{
				await InitializeStatusCourier();
				await InitializerCourier();
				await InitializeStatusOrder();
				await _db.DisposeAsync();
			}
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

		public async Task InitializerCourier()
		{
			Courier courier1 = await _courierService.AddCourierAsync("Dmitry", "Pupkin", "89295501278");
		    Courier courier2 = await _courierService.AddCourierAsync("Dmitry2", "Pupkin2", "89295501279");
			Courier courier3= await _courierService.AddCourierAsync("Dmitry3", "Pupkin3", "89295501289");
			Courier courier4 = await _courierService.AddCourierAsync("Dmitry4", "Pupkin4", "89295511289");
			Courier courier5 = await _courierService.AddCourierAsync("Dmitry5", "Pupkin", "89295501278");
			Courier courier6 = await _courierService.AddCourierAsync("Dmitry6", "Pupkin2", "89295501279");
			Courier courier7 = await _courierService.AddCourierAsync("Dmitry7", "Pupkin3", "89295501276");
			Courier courier8 = await _courierService.AddCourierAsync("Dmitry8", "Pupkin4", "89295511289");
			Courier courier9 = await _courierService.AddCourierAsync("Dmitry4", "Pupkin4", "89295511289");
			Courier courier10 = await _courierService.AddCourierAsync("Dmitry5", "Pupkin", "89295501278");
			Courier courier11 = await _courierService.AddCourierAsync("Dmitry6", "Pupkin2", "89295501279");
			Courier courier12 = await _courierService.AddCourierAsync("Dmitry7", "Pupkin3", "89295501276");
			Courier courier13 = await _courierService.AddCourierAsync("Dmitry8", "Pupkin4", "89295511289");
			Courier courier14 = await _courierService.AddCourierAsync("Dmitry", "Pupkin", "89295501278");
			Courier courier15 = await _courierService.AddCourierAsync("Dmitry2", "Pupkin2", "89295501279");
			Courier courier16 = await _courierService.AddCourierAsync("Dmitry3", "Pupkin3", "89295501289");
			Courier courier17 = await _courierService.AddCourierAsync("Dmitry4", "Pupkin4", "89295511289");
			Courier courier18 = await _courierService.AddCourierAsync("Dmitry5", "Pupkin", "89295501278");
			Courier courier19 = await _courierService.AddCourierAsync("Dmitry6", "Pupkin2", "89295501279");
			Courier courier20 = await _courierService.AddCourierAsync("Dmitry7", "Pupkin3", "89295501276");
			Courier courier21 = await _courierService.AddCourierAsync("Dmitry8", "Pupkin4", "89295511289");
			Courier courier22 = await _courierService.AddCourierAsync("Dmitry4", "Pupkin4", "89295511289");
			Courier courier23 = await _courierService.AddCourierAsync("Dmitry5", "Pupkin", "89295501278");
			Courier courier24 = await _courierService.AddCourierAsync("Dmitry6", "Pupkin2", "89295501279");
			Courier courier25 = await _courierService.AddCourierAsync("Dmitry7", "Pupkin3", "89295501276");
			Courier courier26 = await _courierService.AddCourierAsync("Dmitry8", "Pupkin4", "89295511289");
		}


	}
}
