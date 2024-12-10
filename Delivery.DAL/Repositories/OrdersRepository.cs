using Delivery.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Delivery.DAL.Context;
using Delivery.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace Delivery.DAL.Repositories
{
	internal class OrdersRepository: IOrdersRepository 
	{
		private readonly DeliveryDbContext _db;
		private readonly DbSet<Order> _Set;

		public bool AutoSaveChanges { get; set; } = true;

		public OrdersRepository(DeliveryDbContext db)
		{
			_db = db;
			_Set = db.Set<Order>();
		}

		public IQueryable<Order> Items => _Set
			.Include(item => item.OrderType)
			.Include( item => item.OrderLines)
			.Include(item => item.Courier)
			.ThenInclude(item => item.CourierStatus)
			.Include(item => item.Courier)
			.ThenInclude(item => item.CourierType)
			.Include(item =>item.FromAddress)
			.Include(item => item.TargetAddress)
			.Include(item => item.Client)
		;

		public Order Get(Guid id) => Items.SingleOrDefault(item => item.Id == id);

		public async Task<Order> GetAsync(Guid id, CancellationToken cancel = default) => await Items
			.SingleOrDefaultAsync(item => item.Id == id, cancel)
			.ConfigureAwait(false);

		public Order Add(Order item)
		{
			if (item is null) throw new ArgumentNullException(nameof(item));
			_db.Entry(item).State = EntityState.Added;
			if (AutoSaveChanges)
				_db.SaveChanges();
			return item;
		}

		public async Task<Order> AddAsync(Order item, CancellationToken cancel = default)
		{
			if (item is null) throw new ArgumentNullException(nameof(item));
			_db.Entry(item).State = EntityState.Added;
			if (AutoSaveChanges)
				await _db.SaveChangesAsync(cancel).ConfigureAwait(false);
			return item;
		}

		public void Update(Order item)
		{
			if (item is null) throw new ArgumentNullException(nameof(item));
			_db.Entry(item).State = EntityState.Modified;
			if (AutoSaveChanges)
				_db.SaveChanges();
		}

		public async Task UpdateAsync(Order item, CancellationToken cancel = default)
		{
			if (item is null) throw new ArgumentNullException(nameof(item));
			_db.Entry(item).State = EntityState.Modified;
			if (AutoSaveChanges)
				await _db.SaveChangesAsync(cancel).ConfigureAwait(false);
		}

		public void Remove(Guid id)
		{
			var item = _Set.Local.FirstOrDefault(i => i.Id == id) ?? new Order { Id = id };
			_db.Remove(item);
			if (AutoSaveChanges)
				_db.SaveChanges();
		}

		public async Task RemoveAsync(Guid id, CancellationToken cancel = default)
		{
			_db.Remove(new Order { Id = id });
			if (AutoSaveChanges)
				await _db.SaveChangesAsync(cancel).ConfigureAwait(false);
		}

		public async Task <OrderStatus> GetOrderStatusAsync(string statusName)
		{
			return await _db.OrderStatuses.FirstOrDefaultAsync(n=> n.StatusName == statusName);
		}
	}
}
