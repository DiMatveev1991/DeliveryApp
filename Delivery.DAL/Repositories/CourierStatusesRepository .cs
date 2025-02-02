﻿using Delivery.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Delivery.DAL.Context;
using Delivery.DAL.Models;
using Delivery.Models;
using Microsoft.EntityFrameworkCore;

namespace Delivery.DAL.Repositories
{
	internal class CourierStatusesRepository: ICourierStatusesRepository
	{
		private readonly DeliveryDbContext _db;
		private readonly DbSet<CourierStatus> _Set;

		public bool AutoSaveChanges { get; set; } = true;

		public CourierStatusesRepository(DeliveryDbContext db)
		{
			_db = db;
			_Set = db.Set<CourierStatus>();
		}

		public IQueryable<CourierStatus> Items => _Set
			.AsNoTracking()
			.Include(Items => Items.Couriers)
        ;

		public CourierStatus Get(Guid id) => Items.SingleOrDefault(item => item.Id == id);

		public async Task<CourierStatus> GetAsync(Guid id, CancellationToken cancel = default) => await Items
			.SingleOrDefaultAsync(item => item.Id == id, cancel)
			.ConfigureAwait(false);

		public CourierStatus Add(CourierStatus item)
		{
			if (item is null) throw new ArgumentNullException(nameof(item));
			_db.Entry(item).State = EntityState.Added;
			if (AutoSaveChanges)
				_db.SaveChanges();
			_db.Entry(item).State = EntityState.Detached;
			return item;
		}

		public async Task<CourierStatus> AddAsync(CourierStatus item, CancellationToken cancel = default)
		{
			if (item is null) throw new ArgumentNullException(nameof(item));
			_db.Entry(item).State = EntityState.Added;
			if (AutoSaveChanges)
				await _db.SaveChangesAsync(cancel).ConfigureAwait(false);
			_db.Entry(item).State = EntityState.Detached;
			return item;
		}

		public void Update(CourierStatus item)
		{
			if (item is null) throw new ArgumentNullException(nameof(item));
			_db.Entry(item).State = EntityState.Modified;
			if (AutoSaveChanges)
				_db.SaveChanges();
			_db.Entry(item).State = EntityState.Detached;
		}

		public async Task UpdateAsync(CourierStatus item, CancellationToken cancel = default)
		{
			if (item is null) throw new ArgumentNullException(nameof(item));
			_db.Entry(item).State = EntityState.Modified;
			if (AutoSaveChanges)
				await _db.SaveChangesAsync(cancel).ConfigureAwait(false);
			_db.Entry(item).State = EntityState.Detached;
		}

		public void Remove(Guid id)
		{
            var item = Get(id);

            if (item is null)
                throw new ArgumentException($"Нет статуса с id = {id}");

            _Set.Remove(item);
            if (AutoSaveChanges)
                _db.SaveChanges();
            _db.Entry(item).State = EntityState.Detached;
}

		public async Task RemoveAsync(Guid id, CancellationToken cancel = default)
		{
            var item = await GetAsync(id, cancel);

            if (item is null)
                throw new ArgumentException($"Нет статуса с id = {id}");

            _Set.Remove(item);
            if (AutoSaveChanges)
                await _db.SaveChangesAsync(cancel).ConfigureAwait(false);
}

		public async Task<CourierStatus> GetStatusAsync(CourierStatus courierStatus)
		{
			return await _db.CourierStatuses.FirstOrDefaultAsync(n => n.StatusName == courierStatus.StatusName);
		}
		public async Task<CourierStatus> GetStatusAsync (string statusName)
		{
			return await _db.CourierStatuses.FirstOrDefaultAsync (n => n.StatusName == statusName);
		}
	}
}
