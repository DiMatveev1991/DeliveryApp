using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Delivery.DAL.Models;
using System.Threading.Tasks;
using System.Threading;
using Delivery.DAL.Models.Base;

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
		Task UpdateAsync(Courier item, CancellationToken cancel = default);

		void Remove(Guid id);
		Task RemoveAsync(Guid id, CancellationToken cancel = default);
	}
}
