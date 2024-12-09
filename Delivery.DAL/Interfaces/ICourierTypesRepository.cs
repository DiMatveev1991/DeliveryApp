using Delivery.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Delivery.DAL.Interfaces
{
	public interface ICourierTypesRepository
	{
		IQueryable<CourierType> Items { get; }
		CourierType Get(Guid id);
		Task<CourierType> GetAsync(Guid id, CancellationToken cancel = default);

		CourierType Add(CourierType items);
		Task<CourierType> AddAsync(CourierType item, CancellationToken cancel = default);

		void Update(CourierType item);
		Task UpdateAsync(CourierType item, CancellationToken cancel = default);

		void Remove(Guid id);
		Task RemoveAsync(Guid id, CancellationToken cancel = default);
		public Task<CourierType> GetByCourierType(CourierType courierType)
	}
}
