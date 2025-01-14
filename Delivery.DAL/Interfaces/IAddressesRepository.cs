using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Linq;
using Delivery.Models;

namespace Delivery.DAL.Interfaces
{
    public interface IAddressesRepository
	{
		IQueryable<Address> Items { get; }
		Address Get(Guid id);
		Task<Address> GetAsync(Guid id, CancellationToken cancel = default);

		Address Add(Address items);
		Task<Address> AddAsync(Address item, CancellationToken cancel = default);

		void Update(Address item);
		Task UpdateAsync(Address item, CancellationToken cancel = default);

		void Remove(Guid id);
		Task RemoveAsync(Guid id, CancellationToken cancel = default);

		Task<Address> GetByAddressAsync(Address address);

	}
}
