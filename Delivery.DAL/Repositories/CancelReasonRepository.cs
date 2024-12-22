using Delivery.DAL.Context;
using Delivery.DAL.Interfaces;
using Delivery.DAL.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Delivery.DAL.Repositories
{
    internal class CancelReasonRepository : ICancelReasonRepository

    {
        private readonly DeliveryDbContext _db;
        private readonly DbSet<CancelReason> _Set;

        public bool AutoSaveChanges { get; set; } = true;

        public CancelReasonRepository(DeliveryDbContext db)
        {
            _db = db;
            _Set = db.Set<CancelReason>();
        }

        public IQueryable<CancelReason> Items => _Set
            .AsNoTracking()
            ;

        public CancelReason Get(Guid id) => Items.SingleOrDefault(item => item.Id == id);

        public async Task<CancelReason> GetAsync(Guid id, CancellationToken cancel = default) => await Items
            .SingleOrDefaultAsync(item => item.Id == id, cancel)
            .ConfigureAwait(false);

        public CancelReason Add(CancelReason item)
        {
            if (item is null) throw new ArgumentNullException(nameof(item));
            _db.Entry(item).State = EntityState.Added;
            if (AutoSaveChanges)
                _db.SaveChanges();
            _db.Entry(item).State = EntityState.Detached;
            return item;
        }

        public async Task<CancelReason> AddAsync(CancelReason item, CancellationToken cancel = default)
        {
            if (item is null) throw new ArgumentNullException(nameof(item));
            _db.Entry(item).State = EntityState.Added;
            if (AutoSaveChanges)
                await _db.SaveChangesAsync(cancel).ConfigureAwait(false);
            _db.Entry(item).State = EntityState.Detached;
            return item;
        }

        public void Update(CancelReason item)
        {
            if (item is null) throw new ArgumentNullException(nameof(item));
            _db.Entry(item).State = EntityState.Modified;
            if (AutoSaveChanges)
                _db.SaveChanges();
            _db.Entry(item).State = EntityState.Detached;
        }

        public async Task UpdateAsync(CancelReason item, CancellationToken cancel = default)
        {
            if (item is null) throw new ArgumentNullException(nameof(item));
            _db.Entry(item).State = EntityState.Modified;
            if (AutoSaveChanges)
                await _db.SaveChangesAsync(cancel).ConfigureAwait(false);
            _db.Entry(item).State = EntityState.Detached;
        }

        public void Remove(Guid id)
        {
            var item = _Set.Local.FirstOrDefault(i => i.Id == id) ?? new CancelReason { Id = id };
            _db.Remove(item);
            if (AutoSaveChanges)
                _db.SaveChanges();
            _db.Entry(item).State = EntityState.Detached;
        }

        public async Task RemoveAsync(Guid id, CancellationToken cancel = default)
        {
            _db.Remove(new CancelReason { Id = id });
            if (AutoSaveChanges)
                await _db.SaveChangesAsync(cancel).ConfigureAwait(false);
        }
    }
}

