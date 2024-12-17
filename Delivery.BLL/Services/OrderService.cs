using Delivery.BLL.Interfaces;
using Delivery.DAL.Interfaces;
using Delivery.DAL.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Delivery.BLL.Services
{
	public class OrderService : IOrderService
	{

		private readonly IUnitOfWork _unitOfWork;

		public OrderService(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		public async Task<Order> AddOrderAsync(Client client, Address fromAddress, Address targetAddress,
			IEnumerable<OrderLine> orderLines)
		{
			try
			{
				var orderStatus = await _unitOfWork.OrdersRepository.GetOrderStatusAsync("Новая");

				var order = await _unitOfWork.OrdersRepository.AddAsync(new Order()
				{
					Client = client,
					FromAddress = fromAddress,
					TargetAddress = targetAddress,
					OrderStatus = orderStatus
				});

				foreach (var orderLine in orderLines)
				{
					orderLine.Order = order;
					await _unitOfWork.OrderLinesRepository.AddAsync(orderLine);
				}

				return order;
			}
			catch (Exception)
			{
				throw new Exception("В БД нет статуса заказа Новая");
			}
		}

		public async Task CancelOrderAsync(Guid id, string reason)
		{
			try
			{
				var order = await _unitOfWork.OrdersRepository.GetAsync(id);
				if (order is null) { throw new Exception("Такого заказа нет");}

				var orderStatus = await _unitOfWork.OrdersRepository.GetOrderStatusAsync("Отменена");
				if (orderStatus is null) { throw new Exception("Статус заказа отсутствует"); }

				if (order.CourierId.HasValue)
				{
					var statusCourier = await _unitOfWork.CourierStatusesRepository.GetStatusAsync("Готов к выполнению заказа");
					if (statusCourier is null) { throw new Exception("Статус курьера отсутствует"); }
					var courier = await _unitOfWork.CouriersRepository.GetAsync(order.CourierId.Value);
					if (courier is null) { throw new Exception("В бд отсутствуют курьеры или нет свободных курьеров"); }
					courier.CourierStatus = statusCourier;
					await _unitOfWork.CouriersRepository.UpdateAsync(courier);
				}

				order.CancelReason = reason;
				order.OrderStatus = orderStatus;
				await _unitOfWork.OrdersRepository.UpdateAsync(order);
				

			}
			catch (Exception e)
			{
				throw new Exception(e.Message);
			}

		}

		public async Task CompleteOrderAsync(Guid id)
		{
			try
			{
				var order = await _unitOfWork.OrdersRepository.GetAsync(id);
				if (order is null) { throw new Exception("Такого заказа нет"); }


				var orderStatus = await _unitOfWork.OrdersRepository.GetOrderStatusAsync("Выполнено");
				if (orderStatus is null) { throw new Exception("Статус заказа отсутствует"); }

				var statusCourier = await _unitOfWork.CourierStatusesRepository.GetStatusAsync("Готов к выполнению заказа");
				if (statusCourier is null) { throw new Exception("Статус курьера отсутствует"); }

				if (!order.CourierId.HasValue) throw new Exception("Отсутствует курьер");
				var courier = await _unitOfWork.CouriersRepository.GetAsync(order.CourierId.Value);

				courier.CourierStatus = statusCourier;
				order.OrderStatus = orderStatus;
				
				await _unitOfWork.CouriersRepository.UpdateAsync(courier);
				await _unitOfWork.OrdersRepository.UpdateAsync(order);
			}
			catch (Exception)
			{
				throw new Exception("По данному Id заказ не найден");
			}
		}

		public async Task DeleteOrderAsync(Guid id)
		{
			var order = await _unitOfWork.OrdersRepository.GetAsync(id);

			if (order is null)
			{
				throw new Exception("По данному Id заказ не найден");
			}

			var orderStatusComplete = await _unitOfWork.OrdersRepository.GetOrderStatusAsync("Передано на выполнение");
			if (orderStatusComplete is null)
			{
				throw new Exception("Такого статуса нет в БД");
			}

			if (order.OrderStatus.Id != orderStatusComplete.Id)
			{
				await _unitOfWork.OrdersRepository.RemoveAsync(order.Id);
			}
			else
			{
				throw new Exception("Заказ нельзя удалить, т.к. он в работе");
			}
		}

		public async Task TakeInProgressAsync(Guid orderId, Guid courierId)
		{
			try
			{
				var order = await _unitOfWork.OrdersRepository.GetAsync(orderId);

				if (order is null)
				{
					throw new Exception("По данному Id заказ не найден");
				}

				var courier = await _unitOfWork.CouriersRepository.GetAsync(courierId);

				if (courier is null)
				{
					throw new Exception("Такого курьера не существует");
				}

				var orderStatus = await _unitOfWork.OrdersRepository.GetOrderStatusAsync("Передано на выполнение");
				order.OrderStatus = orderStatus;

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

		public async Task<Order> UpdateOrderAsync(Order order)
		{
			try
			{
				if (!order.OrderStatusId.HasValue) throw new Exception("Отсутствует статус заказа");
				var orderStatus = await _unitOfWork.OrderStatusesRepository.GetAsync(order.OrderStatusId.Value);
				if (orderStatus.StatusName == "Новая")
				{
					await _unitOfWork.OrdersRepository.UpdateAsync(order);
				}

				return order;

			}
			catch (Exception e)
			{
				throw new Exception(e.Message);
			}
		}
	}
}
