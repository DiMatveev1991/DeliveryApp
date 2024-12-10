using Delivery.BLL.Interfaces;
using Delivery.DAL.Interfaces;
using Delivery.DAL.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Delivery.BLL.Services
{
	internal class OrderService : IOrderService
	{

		private readonly IUnitOfWork _unitOfWork;

		public OrderService (IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		public async Task<Order> AddOrderAsync(Client client, Address fromAddress, Address targetAddress, IEnumerable<OrderLine> orderLines)
		{
			var orderStatus = await _unitOfWork.OrdersRepository.GetOrderStatusAsync("Новая");


			var order = await _unitOfWork.OrdersRepository.AddAsync(new Order()
			{
				Client = client, FromAddress = fromAddress, TargetAddress = targetAddress, OrderStatus = orderStatus
			});

			foreach (var orderLine in orderLines)
			{
				orderLine.Order = order;
				await _unitOfWork.OrderLinesRepository.AddAsync(orderLine);
			}
			return order;
		}
        // получить заказ по Id, поставить статус отменен и заполнить поле причина
		public Task CancelOrderAsync(Guid id, string reason)
		{
			throw new NotImplementedException();
		}
		// получить заказ по Id, поставить статус выполнено
		public Task CompleteOrderAsync(Guid id)
		{
			throw new NotImplementedException();
		}
		//если заказ не в работе
		public Task DeleteOrderAsync(Guid id)
		{
			throw new NotImplementedException();
		}
		// получить заказ по Id, поставить статус выполнено
		public Task TakeInProgressAsync(Guid orderId, Guid courierId)
		{
			throw new NotImplementedException();
		}
		// проверить статус заявки если новая изменить
		public Task UpdateOrderAsync(Order order)
		{
			throw new NotImplementedException();
		}
	}
}
