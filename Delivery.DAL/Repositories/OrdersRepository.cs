using Delivery.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Delivery.DAL.Context;
using Delivery.DAL.Models;
using Delivery.DAL.Models.Base;
using Microsoft.EntityFrameworkCore;

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
            .Include(item => item.OrderLines)
            .Include(item => item.Courier)
            .ThenInclude(item => item.CourierStatus)
            .Include(item => item.FromAddress)
            .Include(item => item.TargetAddress)
            .Include(item => item.Client)
            .Include(item => item.OrderStatus)
        ;

        public Order Get(Guid id) => Items.SingleOrDefault(item => item.Id == id);

        public async Task<Order> GetAsync(Guid id, CancellationToken cancel = default) => await Items
            .SingleOrDefaultAsync(item => item.Id == id, cancel)
            .ConfigureAwait(false);

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
            _db.Entry(item).State = EntityState.Added;
            if (AutoSaveChanges)
                _db.SaveChanges();
            _db.Entry(item).State = EntityState.Detached;
            return item;
        }

        public async Task<Order> AddAsync(Order item, CancellationToken cancel = default)
        {
            if (item is null) throw new ArgumentNullException(nameof(item));
            item.IsDeleted = false;
            _db.Entry(item).State = EntityState.Added;
            if (AutoSaveChanges)
                await _db.SaveChangesAsync(cancel).ConfigureAwait(false);
            _db.Entry(item).State = EntityState.Detached;
            return item;
        }

        public void Update(Order item)
        {
            if (item is null) throw new ArgumentNullException(nameof(item));
            _db.Entry(item).State = EntityState.Modified;
            if (AutoSaveChanges)
                _db.SaveChanges();
            _db.Entry(item).State = EntityState.Detached;
        }

        public async Task UpdateAsync(Order item, CancellationToken cancel = default)
        {
            if (item is null) throw new ArgumentNullException(nameof(item));
            _db.Entry(item).State = EntityState.Modified;
            if (AutoSaveChanges)
                await _db.SaveChangesAsync(cancel).ConfigureAwait(false);
            _db.Entry(item).State = EntityState.Detached;
        }

        public void Remove(Guid id)
        {
            Order order = Get(id) ?? new Order { Id = id }; ;
            order.IsDeleted = true;
            _db.Remove(order);
            if (AutoSaveChanges)
                _db.SaveChanges();
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

    }
}
