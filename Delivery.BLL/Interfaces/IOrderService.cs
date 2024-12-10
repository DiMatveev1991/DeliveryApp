using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Delivery.DAL.Models;

namespace Delivery.BLL.Interfaces
{
	internal interface IOrderService
	{
		Task<Order> AddOrder(Client client, Address fromAddress, Address targetAddress, IEnumerable<OrderLine> orderLines);
		// перевести статус заказа - в работе
		Task TakeInProgress(Guid orderId, Guid courierId);
		Task CancelOrder (Guid id, string reason);
		Task CompleteOrder (Guid id);
		Task UpdateOrder (Order order);
		Task DeleteOrderAsync (Guid id);
	}
}
