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
	internal class OrderLinesRepository: IOrderLinesRepository
	{
		private readonly DeliveryDbContext _db;
		private readonly DbSet<OrderLine> _Set;

		public bool AutoSaveChanges { get; set; } = true;

		public OrderLinesRepository(DeliveryDbContext db)
		{
			_db = db;
			_Set = db.Set<OrderLine>();
		}

		public IQueryable<OrderLine> Items => _Set
            .AsNoTracking()
            .Where(x => !x.IsDeleted)
		;

		public OrderLine Get(Guid id) => Items.SingleOrDefault(item => item.Id == id);

		public async Task<OrderLine> GetAsync(Guid id, CancellationToken cancel = default) => await Items
			.SingleOrDefaultAsync(item => item.Id == id, cancel)
			.ConfigureAwait(false);

		public OrderLine Add(OrderLine item)
		{
			if (item is null) throw new ArgumentNullException(nameof(item));
			_db.Entry(item).State = EntityState.Added;
			if (AutoSaveChanges)
				_db.SaveChanges();
			_db.Entry(item).State = EntityState.Detached;
			return item;
		}

		public async Task<OrderLine> AddAsync(OrderLine item, CancellationToken cancel = default)
		{
			if (item is null) throw new ArgumentNullException(nameof(item));
			_db.Entry(item).State = EntityState.Added;
			if (AutoSaveChanges)
				await _db.SaveChangesAsync(cancel).ConfigureAwait(false);
			_db.Entry(item).State = EntityState.Detached;
			return item;
		}

		public void Update(OrderLine item)
		{
			if (item is null) throw new ArgumentNullException(nameof(item));
			_db.Entry(item).State = EntityState.Modified;
			if (AutoSaveChanges)
				_db.SaveChanges();
			_db.Entry(item).State = EntityState.Detached;
		}

		public async Task UpdateAsync(OrderLine item, CancellationToken cancel = default)
		{
			if (item is null) throw new ArgumentNullException(nameof(item));
			_db.Entry(item).State = EntityState.Modified;
			if (AutoSaveChanges)
				await _db.SaveChangesAsync(cancel).ConfigureAwait(false);
			_db.Entry(item).State = EntityState.Detached;
		}

		public void Remove(Guid id)
		{
			var item = _Set.Local.FirstOrDefault(i => i.Id == id) ?? new OrderLine { Id = id };
			_db.Remove(item);
			if (AutoSaveChanges)
				_db.SaveChanges();
			_db.Entry(item).State = EntityState.Detached;
		}

		public async Task RemoveAsync(Guid id, CancellationToken cancel = default)
        {
            var item = await _db.OrderLines.FindAsync(id);
			item.IsDeleted = true;
			item.DeletedOnUtc = DateTime.UtcNow;
            _db.Entry(item).State = EntityState.Modified;
            if (AutoSaveChanges)
				await _db.SaveChangesAsync(cancel).ConfigureAwait(false);
            _db.Entry(item).State = EntityState.Detached;
        }
	}
}
