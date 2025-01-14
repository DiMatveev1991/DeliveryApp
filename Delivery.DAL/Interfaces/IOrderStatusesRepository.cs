using Delivery.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Delivery.Models;

namespace Delivery.DAL.Interfaces
{
	public interface IOrderStatusesRepository 
	{
		IQueryable<OrderStatus> Items { get; }
		OrderStatus Get(Guid id);
		Task<OrderStatus> GetAsync(Guid id, CancellationToken cancel = default);

		OrderStatus Add(OrderStatus items);
		Task<OrderStatus> AddAsync(OrderStatus item, CancellationToken cancel = default);

		void Update(OrderStatus item);
		Task UpdateAsync(OrderStatus item, CancellationToken cancel = default);

		void Remove(Guid id);
		Task RemoveAsync(Guid id, CancellationToken cancel = default);
	}
}
