using Delivery.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Delivery.DAL.Interfaces
{
	public interface ICourierStatusesRepository
	{
		IQueryable<CourierStatus> Items { get; }
		CourierStatus Get(Guid id);
		Task<CourierStatus> GetAsync(Guid id, CancellationToken cancel = default);

		CourierStatus Add(CourierStatus items);
		Task<CourierStatus> AddAsync(CourierStatus item, CancellationToken cancel = default);

		void Update(CourierStatus item);
		Task UpdateAsync(CourierStatus item, CancellationToken cancel = default);

		void Remove(Guid id);
		Task RemoveAsync(Guid id, CancellationToken cancel = default);

		public Task<CourierStatus> GetByCourierStatus(CourierStatus courierStatus);
	}
}
