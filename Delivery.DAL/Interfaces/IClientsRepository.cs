using Delivery.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Delivery.DAL.Interfaces
{
	public interface IClientsRepository 
	{
		IQueryable<Client> Items { get; }
		Client Get(Guid id);
		Task<Client> GetAsync(Guid id, CancellationToken cancel = default);

		Client Add(Client items);
		Task<Client> AddAsync(Client item, CancellationToken cancel = default);

		void Update(Client item);
		Task UpdateAsync(Client item, CancellationToken cancel = default);

		void Remove(Guid id);
		Task RemoveAsync(Guid id, CancellationToken cancel = default);
	}
}
