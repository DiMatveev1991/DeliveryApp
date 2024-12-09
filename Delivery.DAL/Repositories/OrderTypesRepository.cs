using Delivery.DAL.Context;
using Delivery.DAL.Interfaces;
using Delivery.DAL.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Delivery.DAL.Repositories
{
	internal class OrderTypesRepository: IOrderTypesRepository<OrderType>
	{
		private readonly DeliveryDbContext _db;
		private readonly DbSet<OrderType> _Set;

		public bool AutoSaveChanges { get; set; } = true;

		public OrderTypesRepository (DeliveryDbContext db)
		{
			_db = db;
			_Set = db.Set<OrderType>();
		}

		public IQueryable<OrderType> Items => _Set;

		public OrderType Get(Guid id) => Items.SingleOrDefault(item => item.Id == id);

		public async Task<OrderType> GetAsync(Guid id, CancellationToken cancel = default) => await Items
		   .SingleOrDefaultAsync(item => item.Id == id, cancel)
		   .ConfigureAwait(false);

		public OrderType Add(OrderType item)
		{
			if (item is null) throw new ArgumentNullException(nameof(item));
			_db.Entry(item).State = EntityState.Added;
			if (AutoSaveChanges)
				_db.SaveChanges();
			return item;
		}

		public async Task<OrderType> AddAsync(OrderType item, CancellationToken cancel = default)
		{
			if (item is null) throw new ArgumentNullException(nameof(item));
			_db.Entry(item).State = EntityState.Added;
			if (AutoSaveChanges)
				await _db.SaveChangesAsync(cancel).ConfigureAwait(false);
			return item;
		}

		public void Update(OrderType item)
		{
			if (item is null) throw new ArgumentNullException(nameof(item));
			_db.Entry(item).State = EntityState.Modified;
			if (AutoSaveChanges)
				_db.SaveChanges();
		}

		public async Task UpdateAsync(OrderType item, CancellationToken cancel = default)
		{
			if (item is null) throw new ArgumentNullException(nameof(item));
			_db.Entry(item).State = EntityState.Modified;
			if (AutoSaveChanges)
				await _db.SaveChangesAsync(cancel).ConfigureAwait(false);
		}

		public void Remove(Guid id)
		{
			var item = _Set.Local.FirstOrDefault(i => i.Id == id) ?? new OrderType { Id = id };
			_db.Remove(item);
			if (AutoSaveChanges)
				_db.SaveChanges();
		}

		public async Task RemoveAsync(Guid id, CancellationToken cancel = default)
		{
			_db.Remove(new OrderType { Id = id });
			if (AutoSaveChanges)
				await _db.SaveChangesAsync(cancel).ConfigureAwait(false);
		}
	}
}

