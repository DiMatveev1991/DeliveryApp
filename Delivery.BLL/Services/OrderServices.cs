using Delivery.BLL.Interfaces;
using Delivery.DAL.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Delivery.BLL.Services
{
	internal class OrderServicesI : IOrderService
	{
		public Task<Order> AddOrder(Client client, Address fromAddress, Address targetAddress, IEnumerable<OrderLine> orderLines)
		{
			throw new NotImplementedException();
		}

		public Task CancelOrder(Guid id, string reason)
		{
			throw new NotImplementedException();
		}

		public Task CompleteOrder(Guid id)
		{
			throw new NotImplementedException();
		}

		public Task DeleteOrder(Guid id)
		{
			throw new NotImplementedException();
		}

		public Task TakeInProgress(Guid orderId, Guid courierId)
		{
			throw new NotImplementedException();
		}

		public Task UpdateOrder(Order order)
		{
			throw new NotImplementedException();
		}
	}
}
