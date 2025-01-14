using Delivery.DAL.Context;
using Delivery.DAL.Interfaces;
using Delivery.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Delivery.DAL.Repositories
{
    internal class OrdersRepository : IOrdersRepository
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
            .AsNoTracking()
            .Include(item => item.OrderLines).AsNoTracking()
            .Include(item => item.Courier)
            .Include(item => item.FromAddress).AsNoTracking()
            .Include(item => item.TargetAddress).AsNoTracking()
            .Include(item => item.Client)
            .Include(item => item.OrderStatus).AsNoTracking()
            .Include(item => item.CancelReason)
        ;

        public Order Get(Guid id) => Items.SingleOrDefault(item => item.Id == id);

        public async Task<Order> GetAsync(Guid id, CancellationToken cancel = default) => await Items
            .SingleOrDefaultAsync(item => item.Id == id, cancel)
            .ConfigureAwait(false);

        // Начало написания фильтра на уровне бд
        public async Task<IEnumerable<Order>> GetFilteredAsync(string filter, CancellationToken cancel = default)
        {
            return await Items
                .Where(x =>
                    x.TargetAddress.Street.Contains(filter)
                    || x.Client.FirstName.Contains(filter))
                .ToListAsync(cancellationToken: cancel);
        }

        public Order Add(Order item)
        {
            if (item is null) throw new ArgumentNullException(nameof(item));
            item.IsDeleted = false;
            _Set.Add(item);
            if (AutoSaveChanges) _db.SaveChanges();
            DetachOrderNavigationProps(item);
            return Items.FirstOrDefault(x => x.Id == item.Id);
        }

        public async Task<Order> AddAsync(Order item, CancellationToken cancel = default)
        {
            if (item is null) throw new ArgumentNullException(nameof(item));
            item.IsDeleted = false;
            await _Set.AddAsync(item, cancel);
            if (AutoSaveChanges)
                await _db.SaveChangesAsync(cancel).ConfigureAwait(false);
            DetachOrderNavigationProps(item);
            return await Items.FirstOrDefaultAsync(x => x.Id == item.Id, cancellationToken: cancel);
        }

        public Order Update(Order item)
        {
            if (item is null) throw new ArgumentNullException(nameof(item));
            _Set.Update(item);
            if (AutoSaveChanges)
                 _db.SaveChanges();
            DetachOrderNavigationProps(item);
            return Items.FirstOrDefault(x => x.Id == item.Id);
        }

        public async Task<Order> UpdateAsync(Order item, CancellationToken cancel = default)
        {
            if (item is null) throw new ArgumentNullException(nameof(item));
            _Set.Update(item);
            if (AutoSaveChanges)
                await _db.SaveChangesAsync(cancel).ConfigureAwait(false);
            DetachOrderNavigationProps(item);
            return await Items.FirstOrDefaultAsync(x => x.Id == item.Id, cancellationToken: cancel);
        }

        public void Remove(Guid id)
        {
            var order = Get(id);
            order.IsDeleted = true;
            order.DeletedOnUtc = DateTime.UtcNow;
            _db.Entry(order).State = EntityState.Modified;
            if (AutoSaveChanges)
                _db.SaveChanges();
            _db.Entry(order).State = EntityState.Detached;
        }

        public async Task RemoveAsync(Guid id, CancellationToken cancel = default)
        {
            var order = await GetAsync(id, cancel);
            order.IsDeleted = true;
            order.DeletedOnUtc = DateTime.UtcNow;
            _db.Entry(order).State = EntityState.Modified;
            if (AutoSaveChanges)
                await _db.SaveChangesAsync(cancel).ConfigureAwait(false);
            _db.Entry(order).State = EntityState.Detached;
        }

        public async Task<OrderStatus> GetOrderStatusAsync(string statusName)
        {
            return await _db.OrderStatuses.FirstOrDefaultAsync(n => n.StatusName == statusName);
        }

        private void DetachOrderNavigationProps(Order item)
        {
            _db.Entry(item).State = EntityState.Detached;

            if (item.FromAddress != null)
            {
                _db.Entry(item.FromAddress).State = EntityState.Detached;
            }

            if (item.TargetAddress != null)
            {
                _db.Entry(item.TargetAddress).State = EntityState.Detached;
            }

            if (item.Courier != null)
            {
                _db.Entry(item.Courier).State = EntityState.Detached;
            }

            if (item.Client != null)
            {
                _db.Entry(item.Client).State = EntityState.Detached;
            }
            if (item.CancelReason != null)
            {
                _db.Entry(item.CancelReason).State = EntityState.Detached;
            }
            if (item.OrderLines == null) return;

            foreach (var orderLine in item.OrderLines)
            {
                _db.Entry(orderLine).State = EntityState.Detached;
            }
        }

    }
}
