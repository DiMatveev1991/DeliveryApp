using Delivery.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Delivery.DAL.Interfaces
{
	public interface ICancelReasonRepository
	{
		IQueryable<CancelReason> Items { get; }
		CancelReason Get(Guid id);
		Task<CancelReason> GetAsync(Guid id, CancellationToken cancel = default);

		CancelReason Add(CancelReason items);
		Task<CancelReason> AddAsync(CancelReason item, CancellationToken cancel = default);

		void Update(CancelReason item);
		Task UpdateAsync(CancelReason item, CancellationToken cancel = default);

		void Remove(Guid id);
		Task RemoveAsync(Guid id, CancellationToken cancel = default);
	}
}
