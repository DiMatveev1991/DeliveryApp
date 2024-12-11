using Delivery.BLL.Interfaces;
using Delivery.DAL.Interfaces;
using Delivery.DAL.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
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
			try
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
			catch (Exception ex)
			{
				throw new Exception("В БД нет  статуса заказа Новая");
			}
		}


        // получить заказ по Id, поставить статус отменен и заполнить поле причина
		public Task CancelOrderAsync(Guid id, string reason)
		{
			throw new NotImplementedException();
		}
		
		public async Task CompleteOrderAsync(Guid id)
		{
			try
			{
				var order = await _unitOfWork.OrdersRepository.GetAsync(id);
				var orderStatus = await _unitOfWork.OrdersRepository.GetOrderStatusAsync("Выполнено");
				order.OrderStatus=orderStatus;

				await _unitOfWork.OrdersRepository.UpdateAsync(order);
			}
			catch (Exception e)
			{
				throw new Exception("По данному Id заказ не найден");
			}
		}

		public async Task DeleteOrderAsync(Guid id)
		{
			try
			{
				var order = await _unitOfWork.OrdersRepository.GetAsync(id);
				var orderStatusComplete = await _unitOfWork.OrdersRepository.GetOrderStatusAsync("Передано на выполнение");
				if (order.OrderStatus != orderStatusComplete) { await _unitOfWork.OrdersRepository.RemoveAsync(order.Id); }
			}
			catch (Exception e)
			{
				throw new Exception("По данному Id заказ не найден");
			}
        }

		public async Task TakeInProgressAsync(Guid orderId, Guid courierId)
		{
			try
			{
				var order = await _unitOfWork.OrdersRepository.GetAsync(orderId);
				var orderStatus = await _unitOfWork.OrdersRepository.GetOrderStatusAsync("Передано на выполнение");
				order.OrderStatus = orderStatus;
				var courier = await _unitOfWork.CouriersRepository.GetAsync(courierId);
				var courierStatus = await _unitOfWork.CouriersRepository.GetCourierStatusAsync("Выполняет заказ");
				order.Courier = courier;
				courier.CourierStatus = courierStatus;
				await _unitOfWork.OrdersRepository.UpdateAsync(order);
				await _unitOfWork.CouriersRepository.UpdateAsync(courier);
			}
			catch (Exception e)
			{
				throw new Exception(e.Message);
			}
		}


		// проверить статус заявки если новая изменить
		public Task UpdateOrderAsync(Order order)
		{
			throw new NotImplementedException();
		}
	}
}
