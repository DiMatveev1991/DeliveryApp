﻿using Delivery.DAL.Context;
using Delivery.DAL.Interfaces;
using Delivery.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Delivery.DAL.Repositories
{
    internal class ClientsRepository : IClientsRepository
    {
        private readonly DeliveryDbContext _db;
        private readonly DbSet<Client> _Set;

        public bool AutoSaveChanges { get; set; } = true;

        public ClientsRepository(DeliveryDbContext db)
        {
            _db = db;
            _Set = db.Set<Client>();
        }

        public IQueryable<Client> Items => _Set
            .AsNoTracking()
            .Include(item => item.Address)
            ;

        public Client Get(Guid id) => Items.SingleOrDefault(item => item.Id == id);

        public async Task<Client> GetAsync(Guid id, CancellationToken cancel = default) => await Items
            .SingleOrDefaultAsync(item => item.Id == id, cancel)
            .ConfigureAwait(false);

        public Client Add(Client item)
        {
            if (item is null) throw new ArgumentNullException(nameof(item));
            item.IsDeleted = false;
			_db.Entry(item).State = EntityState.Added;
            if (AutoSaveChanges)
                _db.SaveChanges();
            _db.Entry(item).State = EntityState.Detached;
            return item;
        }

        public async Task<Client> AddAsync(Client item, CancellationToken cancel = default)
        {
            if (item is null) throw new ArgumentNullException(nameof(item));
            item.IsDeleted = false;
            await _Set.AddAsync(item, cancel);
            if (AutoSaveChanges)
                await _db.SaveChangesAsync(cancel).ConfigureAwait(false);
            _db.Entry(item).State = EntityState.Detached;
            return item;
        }

        public void Update(Client item)
        {
            if (item is null) throw new ArgumentNullException(nameof(item));
            _db.Entry(item).State = EntityState.Modified;
            if (AutoSaveChanges)
                _db.SaveChanges();
            _db.Entry(item).State = EntityState.Detached;
        }

        public async Task UpdateAsync(Client item, CancellationToken cancel = default)
        {
            if (item is null) throw new ArgumentNullException(nameof(item));
            _db.Entry(item).State = EntityState.Modified;
            if (AutoSaveChanges)
                await _db.SaveChangesAsync(cancel).ConfigureAwait(false);
            _db.Entry(item).State = EntityState.Detached;
        }

        public void Remove(Guid id)
        {
            var item = _Set.Local.FirstOrDefault(i => i.Id == id) ?? new Client { Id = id };
            item.IsDeleted = true;
			_db.Remove(item);
            if (AutoSaveChanges)
                _db.SaveChanges();
            _db.Entry(item).State = EntityState.Detached;
        }

        public async Task RemoveAsync(Guid id, CancellationToken cancel = default)
        {
			var client = await GetAsync(id, cancel);
			client.IsDeleted = true;
			client.DeletedOnUtc = DateTime.UtcNow;
			_db.Entry(client).State = EntityState.Modified;
			if (AutoSaveChanges)
				await _db.SaveChangesAsync(cancel).ConfigureAwait(false);
			_db.Entry(client).State = EntityState.Detached;
		}
    }
}
