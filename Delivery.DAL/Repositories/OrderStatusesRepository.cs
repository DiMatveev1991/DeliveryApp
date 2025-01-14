using Delivery.DAL.Context;
using Delivery.DAL.Interfaces;
using Delivery.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Delivery.DAL.Repositories
{
    internal class OrderStatusesRepository : IOrderStatusesRepository
    {
        private readonly DeliveryDbContext _db;
        private readonly DbSet<OrderStatus> _Set;

        public bool AutoSaveChanges { get; set; } = true;

        public OrderStatusesRepository(DeliveryDbContext db)
        {
            _db = db;
            _Set = db.Set<OrderStatus>();
        }

        public IQueryable<OrderStatus> Items => _Set.AsNoTracking();

        public OrderStatus Get(Guid id) => Items.SingleOrDefault(item => item.Id == id);

        public async Task<OrderStatus> GetAsync(Guid id, CancellationToken cancel = default) => await Items
            .SingleOrDefaultAsync(item => item.Id == id, cancel)
            .ConfigureAwait(false);

        public OrderStatus Add(OrderStatus item)
        {
            if (item is null) throw new ArgumentNullException(nameof(item));
            _db.Entry(item).State = EntityState.Added;
            if (AutoSaveChanges)
                _db.SaveChanges();
            return item;
        }

        public async Task<OrderStatus> AddAsync(OrderStatus item, CancellationToken cancel = default)
        {
            if (item is null) throw new ArgumentNullException(nameof(item));
            _db.Entry(item).State = EntityState.Added;
            if (AutoSaveChanges)
                await _db.SaveChangesAsync(cancel).ConfigureAwait(false);
            return item;
        }

        public void Update(OrderStatus item)
        {
            if (item is null) throw new ArgumentNullException(nameof(item));
            _db.Entry(item).State = EntityState.Modified;
            if (AutoSaveChanges)
                _db.SaveChanges();
        }

        public async Task UpdateAsync(OrderStatus item, CancellationToken cancel = default)
        {
            if (item is null) throw new ArgumentNullException(nameof(item));
            _db.Entry(item).State = EntityState.Modified;
            if (AutoSaveChanges)
                await _db.SaveChangesAsync(cancel).ConfigureAwait(false);
        }

        public void Remove(Guid id)
        {
            var item = Get(id);

            if (item is null)
                throw new ArgumentException($"Нет статуса с id = {id}");

            _db.Remove(item);
            if (AutoSaveChanges)
                _db.SaveChanges();
        }

        public async Task RemoveAsync(Guid id, CancellationToken cancel = default)
        {
            var item = await GetAsync(id, cancel);

            if (item is null)
                throw new ArgumentException($"Нет статуса с id = {id}");

            _db.Remove(item);
            if (AutoSaveChanges)
                await _db.SaveChangesAsync(cancel).ConfigureAwait(false);
        }
    }
}
