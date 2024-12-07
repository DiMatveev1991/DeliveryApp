using Delivery.DAL.Modals;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Delivery.DAL.Context
{
	public class DeliveryDbContext: DbContext
	{
		public DbSet<Adresses> Addresses { get; set; }
		public DbSet<Clients> Clients { get; set; }
		public DbSet<Couriers> Couriers { get; set; }
		public DbSet<CourierTypes> CourierTypes { get; set; }
		public DbSet<CourierStatuses> CourierStatuses { get; set; }
		public DbSet<Orders> Orders { get; set; }
		public DbSet<OrderTypes> OrderTypes { get; set; }
		public DbSet<OrderStatuses> OrderStatuses { get; set; }
		public DbSet<OrderLines> OrderLines { get; set; }

		public DeliveryDbContext(DbContextOptions <DeliveryDbContext> options) : base(options)
		{

		}
	}
}
