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
	internal class CourierTypesRepository: ICourierTypesRepository
	{
		private readonly DeliveryDbContext _db;
		private readonly DbSet<CourierType> _Set;

		public bool AutoSaveChanges { get; set; } = true;

		public CourierTypesRepository(DeliveryDbContext db)
		{
			_db = db;
			_Set = db.Set<CourierType>();
		}

		public IQueryable<CourierType> Items => _Set;

		public CourierType Get(Guid id) => Items.SingleOrDefault(item => item.Id == id);

		public async Task<CourierType> GetAsync(Guid id, CancellationToken cancel = default) => await Items
			.SingleOrDefaultAsync(item => item.Id == id, cancel)
			.ConfigureAwait(false);

		public CourierType Add(CourierType item)
		{
			if (item is null) throw new ArgumentNullException(nameof(item));
			_db.Entry(item).State = EntityState.Added;
			if (AutoSaveChanges)
				_db.SaveChanges();
			return item;
		}

		public async Task<CourierType> AddAsync(CourierType item, CancellationToken cancel = default)
		{
			if (item is null) throw new ArgumentNullException(nameof(item));
			_db.Entry(item).State = EntityState.Added;
			if (AutoSaveChanges)
				await _db.SaveChangesAsync(cancel).ConfigureAwait(false);
			return item;
		}

		public void Update(CourierType item)
		{
			if (item is null) throw new ArgumentNullException(nameof(item));
			_db.Entry(item).State = EntityState.Modified;
			if (AutoSaveChanges)
				_db.SaveChanges();
		}

		public async Task UpdateAsync(CourierType item, CancellationToken cancel = default)
		{
			if (item is null) throw new ArgumentNullException(nameof(item));
			_db.Entry(item).State = EntityState.Modified;
			if (AutoSaveChanges)
				await _db.SaveChangesAsync(cancel).ConfigureAwait(false);
		}

		public void Remove(Guid id)
		{
			var item = _Set.Local.FirstOrDefault(i => i.Id == id) ?? new CourierType { Id = id };
			_db.Remove(item);
			if (AutoSaveChanges)
				_db.SaveChanges();
		}

		public async Task RemoveAsync(Guid id, CancellationToken cancel = default)
		{
			_db.Remove(new CourierType { Id = id });
			if (AutoSaveChanges)
				await _db.SaveChangesAsync(cancel).ConfigureAwait(false);
		}

		public async Task<CourierType> GetByCourierType (CourierType courierType)
		{
			return await _db.CourierTypes.FirstOrDefaultAsync(n =>  n.StatusName == courierType.StatusName);
		}
	}
}
