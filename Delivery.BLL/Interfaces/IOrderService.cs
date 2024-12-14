using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Delivery.DAL.Models;

namespace Delivery.BLL.Interfaces
{
	public interface IOrderService
	{
		Task<Order> AddOrderAsync(Client client, Address fromAddress, Address targetAddress, IEnumerable<OrderLine> orderLines);
		// перевести статус заказа - в работе
		Task TakeInProgressAsync(Guid orderId, Guid courierId);
		Task CancelOrderAsync (Guid id, string reason);
		Task CompleteOrderAsync (Guid id);
		Task <Order> UpdateOrderAsync (Order order);
		Task DeleteOrderAsync (Guid id);
	}
}
