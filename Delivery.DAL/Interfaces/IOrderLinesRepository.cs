using Delivery.Models;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Delivery.DAL.Interfaces
{
    public interface IOrderLinesRepository
    {
        IQueryable<OrderLine> Items { get; }
        OrderLine Get(Guid id);
        Task<OrderLine> GetAsync(Guid id, CancellationToken cancel = default);

        OrderLine Add(OrderLine items);
        Task<OrderLine> AddAsync(OrderLine item, CancellationToken cancel = default);

        void Update(OrderLine item);
        Task<OrderLine> UpdateAsync(OrderLine item, CancellationToken cancel = default);

        void Remove(Guid id);
        Task RemoveAsync(Guid id, CancellationToken cancel = default);
    }
}
