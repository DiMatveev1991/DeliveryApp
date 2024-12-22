using Delivery.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Delivery.DAL.Interfaces
{
	public interface IOrdersRepository
	{
		IQueryable<Order> Items { get; }
		Order Get(Guid id);
		Task<Order> GetAsync(Guid id, CancellationToken cancel = default);

		Task<IEnumerable<Order>> GetFilteredAsync(string filter, CancellationToken cancel = default);

		Order Add(Order items);
		Task<Order> AddAsync(Order item, CancellationToken cancel = default);

		void Update(Order item);
		Task UpdateAsync(Order item, CancellationToken cancel = default);

		void Remove(Guid id);
		Task RemoveAsync(Guid id, CancellationToken cancel = default);

        Task<OrderStatus> GetOrderStatusAsync(string statusName);
	}
}
