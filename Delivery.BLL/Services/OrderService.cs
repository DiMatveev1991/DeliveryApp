using Delivery.BLL.Interfaces;
using Delivery.DAL.Interfaces;
using Delivery.DAL.Models;
using Delivery.DTOs;
using Delivery.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using ConstantProperty;

namespace Delivery.BLL.Services
{
	public class OrderService : IOrderService
	{
		private readonly IUnitOfWork _unitOfWork;
		public CourierStatusesConstant CourierStatusesConstant = new CourierStatusesConstant();
		public OrderStatusesConst OrderStatusesConst = new OrderStatusesConst();
		public OrderService(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		public async Task<OrderDto> AddOrderAsync(OrderDto order)
		{
			try
			{
				var orderStatus = await _unitOfWork.OrdersRepository.GetOrderStatusAsync(OrderStatusesConst.NewOrder);

				var newOrder = await _unitOfWork.OrdersRepository.AddAsync(new Order()
				{
					ClientId = order.ClientId,
					FromAddressId = order.FromAddressId,
					TargetAddressId = order.TargetAddressId,
					OrderStatusId = orderStatus.Id,
					TargetDateTime = order.TargetDateTime,
					OrderLines = order.OrderLines.Select(x => x.ToModel()).ToList(),
				});

				return new OrderDto(newOrder);
			}
			catch (Exception e)
			{
				throw new Exception("Не удалось добавить заказ.", e);
			}
		}

		public async Task CancelOrderAsync(Guid orderId, string reason)
		{
			try
			{
				var order = await _unitOfWork.OrdersRepository.GetAsync(orderId);
				if (order is null) { throw new Exception("Такого заказа нет"); }

				var orderStatus = await _unitOfWork.OrdersRepository.GetOrderStatusAsync(OrderStatusesConst.ChanelOrder);
				if (orderStatus is null) { throw new Exception("Статус заказа отсутствует"); }

				var statusCourier = await _unitOfWork.CourierStatusesRepository.GetStatusAsync(CourierStatusesConstant.ReadyToJobStatus);
				if (statusCourier is null) { throw new Exception("Статус курьера отсутствует"); }

				if (order.Courier != null) order.Courier.CourierStatusId = statusCourier.Id;

				order.CancelReason = new CancelReason()
				{
					OrderId = orderId,
					ReasonCancel = reason
				};
				order.OrderStatus = orderStatus;
				await _unitOfWork.OrdersRepository.UpdateAsync(order);
			}
			catch (Exception e)
			{
				throw new Exception($"Произошла ошибка при отмене заказа - {e.Message}", e);
			}
		}

		public async Task CompleteOrderAsync(Guid id)
		{
			try
			{
				var order = await _unitOfWork.OrdersRepository.GetAsync(id);
				if (order is null) { throw new Exception("Такого заказа нет"); }

				var orderStatus = await _unitOfWork.OrdersRepository.GetOrderStatusAsync(OrderStatusesConst.CompletedOrder);
				if (orderStatus is null) { throw new Exception("Статус заказа отсутствует"); }

				var statusCourier = await _unitOfWork.CourierStatusesRepository.GetStatusAsync(CourierStatusesConstant.ReadyToJobStatus);
				if (statusCourier is null) { throw new Exception("Статус курьера отсутствует"); }

				if (!order.CourierId.HasValue) throw new Exception("Отсутствует курьер");
				var courier = await _unitOfWork.CouriersRepository.GetAsync(order.CourierId.Value);

				order.Courier.CourierStatus = statusCourier;
				order.OrderStatus = orderStatus;

				await _unitOfWork.OrdersRepository.UpdateAsync(order);
			}
			catch (Exception e)
			{
				throw new Exception($"Произошла ошибка при завершении заказа - {e.Message}", e);
			}
		}

		public async Task DeleteOrderAsync(Guid id)
		{
			try
			{
				var order = await _unitOfWork.OrdersRepository.GetAsync(id);

				if (order is null)
				{
					throw new Exception("По данному Id заказ не найден");
				}

				var orderStatusComplete = await _unitOfWork.OrdersRepository.GetOrderStatusAsync(OrderStatusesConst.OrderSubmittedForExecution);
				if (orderStatusComplete is null)
				{
					throw new Exception("Такого статуса нет в БД");
				}

				if (order.OrderStatus.Id != orderStatusComplete.Id)
				{
					await _unitOfWork.OrdersRepository.RemoveAsync(order.Id);
					var orderLines = order.OrderLines;
					foreach (var orderLine in orderLines)
					{
						await _unitOfWork.OrderLinesRepository.RemoveAsync(orderLine.Id);
					}
				}
				else
				{
					throw new Exception("Заказ нельзя удалить, т.к. он в работе");
				}
			}
			catch (Exception e)
			{
				throw new Exception($"Ошибка при удалении заказа- {e.Message}", e);
			}
		}

		public async Task<OrderLineDto> AddOrderLineToOrder(OrderLineDto orderLine)
		{
			try
			{
				return new OrderLineDto(await _unitOfWork.OrderLinesRepository.AddAsync(orderLine.ToModel()));
			}
			catch (Exception e)
			{
				throw new Exception($"Ошибка при добавлении строки заказа- {e.Message}", e);
			}
		}

		public async Task<OrderLineDto> UpdateOrderLine(OrderLineDto orderLine)
		{
			try
			{
				return new OrderLineDto(await _unitOfWork.OrderLinesRepository.UpdateAsync(orderLine.ToModel()));
			}
			catch (Exception e)
			{
				throw new Exception($"Ошибка при обновлении строки заказа- {e.Message}", e);
			}
		}

		public async Task RemoveOrderLineFromOrder(Guid id)
		{
			try
			{
				await _unitOfWork.OrderLinesRepository.RemoveAsync(id);
			}
			catch (Exception e)
			{
				throw new Exception($"Ошибка при удалении строки заказа- {e.Message}", e);
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

				var orderStatus = await _unitOfWork.OrdersRepository.GetOrderStatusAsync(OrderStatusesConst.OrderSubmittedForExecution);
				order.OrderStatus = orderStatus;

				var courierStatus = await _unitOfWork.CouriersRepository.GetCourierStatusAsync(CourierStatusesConstant.OnJobStatus);
				order.Courier = courier;
				order.CourierId = courier.Id;
				courier.CourierStatus = courierStatus;
				await _unitOfWork.OrdersRepository.UpdateAsync(order);
				await _unitOfWork.CouriersRepository.UpdateAsync(courier);
			}
			catch (Exception e)
			{
				throw new Exception($"Ошибка при взятии заказа в работу- {e.Message}", e);
			}
		}

		public async Task<OrderDto> UpdateOrderAsync(OrderDto order)
		{
			try
			{
				if (!order.OrderStatusId.HasValue) throw new Exception("Отсутствует статус заказа");
				var orderStatus = await _unitOfWork.OrderStatusesRepository.GetAsync(order.OrderStatusId.Value);

				if (orderStatus.StatusName != OrderStatusesConst.NewOrder) return null;
				var updated = await _unitOfWork.OrdersRepository.UpdateAsync(order.ToModel());
				return new OrderDto(updated);
			}
			catch (Exception e)
			{
				throw new Exception($"Ошибка при обновлении заказа- {e.Message}", e);
			}
		}
	}
}