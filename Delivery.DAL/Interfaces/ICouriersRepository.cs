using Delivery.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Delivery.DAL.Interfaces
{
    public interface ICouriersRepository
    {
        IQueryable<Courier> Items { get; }
        Courier Get(Guid id);
        Task<Courier> GetAsync(Guid id, CancellationToken cancel = default);

        Courier Add(Courier items);
        Task<Courier> AddAsync(Courier item, CancellationToken cancel = default);

        void Update(Courier item);
        Task<Courier> UpdateAsync(Courier item, CancellationToken cancel = default);

        void Remove(Guid id);
        Task RemoveAsync(Guid id, CancellationToken cancel = default);

        Task<IEnumerable<Order>> GetActiveOrdersAsync(Guid courierId);

        Task<CourierStatus> GetCourierStatusAsync(string statusName);

    }
}
