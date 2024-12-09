using Delivery.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Delivery.DAL.Interfaces
{
	public interface IOrderTypesRepository
	{
		IQueryable<OrderType> Items { get; }
		OrderType Get(Guid id);
		Task<OrderType> GetAsync(Guid id, CancellationToken cancel = default);

		OrderType Add(OrderType items);
		Task<OrderType> AddAsync(OrderType item, CancellationToken cancel = default);

		void Update(OrderType item);
		Task UpdateAsync(OrderType item, CancellationToken cancel = default);

		void Remove(Guid id);
		Task RemoveAsync(Guid id, CancellationToken cancel = default);

	}
}
