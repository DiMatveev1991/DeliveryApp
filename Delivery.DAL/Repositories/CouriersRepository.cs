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
	internal class CouriersRepository: ICouriersRepository
	{
		private readonly DeliveryDbContext _db;
		private readonly DbSet<Courier> _Set;

		public bool AutoSaveChanges { get; set; } = true;

		public CouriersRepository(DeliveryDbContext db)
		{
			_db = db;
			_Set = db.Set<Courier>();
		}

		public IQueryable<Courier> Items => _Set
			.Include(item => item.CourierStatus)
		    ;

		public Courier Get(Guid id) => Items.SingleOrDefault(item => item.Id == id);

		public async Task<Courier> GetAsync(Guid id, CancellationToken cancel = default) => await Items
			.SingleOrDefaultAsync(item => item.Id == id, cancel)
			.ConfigureAwait(false);

		public Courier Add(Courier item)
		{
			if (item is null) throw new ArgumentNullException(nameof(item));
			_db.Entry(item).State = EntityState.Added;
			if (AutoSaveChanges)
				_db.SaveChanges();
			return item;
		}

		public async Task<Courier> AddAsync(Courier item, CancellationToken cancel = default)
		{
			if (item is null) throw new ArgumentNullException(nameof(item));
			_db.Entry(item).State = EntityState.Added;
			if (AutoSaveChanges)
				await _db.SaveChangesAsync(cancel).ConfigureAwait(false);
			return item;
		}

		public void Update(Courier item)
		{
			if (item is null) throw new ArgumentNullException(nameof(item));
			_db.Entry(item).State = EntityState.Modified;
			if (AutoSaveChanges)
				_db.SaveChanges();
		}

		public async Task UpdateAsync(Courier item, CancellationToken cancel = default)
		{
			if (item is null) throw new ArgumentNullException(nameof(item));
			_db.Entry(item).State = EntityState.Modified;
			if (AutoSaveChanges)
				await _db.SaveChangesAsync(cancel).ConfigureAwait(false);
		}

		public void Remove(Guid id)
		{
			var item = _Set.Local.FirstOrDefault(i => i.Id == id) ?? new Courier { Id = id };
			_db.Remove(item);
			if (AutoSaveChanges)
				_db.SaveChanges();
		}

		public async Task RemoveAsync(Guid id, CancellationToken cancel = default)
		{
			_db.Remove(new Courier { Id = id });
			if (AutoSaveChanges)
				await _db.SaveChangesAsync(cancel).ConfigureAwait(false);
		}

		public async Task<IEnumerable<Order>> GetActiveOrdersAsync(Guid courierId)
		{
			return await _db.Orders.Where(n => n.OrderStatus.StatusName == "Передано на выполнение" && n.CourierId == courierId).ToListAsync();
		}
        public async Task<CourierStatus> GetCourierStatusAsync(string statusName)
        {
            return await _db.CourierStatuses.FirstOrDefaultAsync(n => n.StatusName == statusName);
        }
    }
}
