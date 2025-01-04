using Delivery.BLL.Interfaces;
using Delivery.DAL.Interfaces;
using Delivery.DAL.Models;
using Delivery.DTOs;
using Delivery.Models;
using System;
using System.Linq;
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

        public async Task<OrderDto> AddOrderAsync(OrderDto order)
        {
            try
            {
                var orderStatus = await _unitOfWork.OrdersRepository.GetOrderStatusAsync("Новая");

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
                throw new Exception(e.Message);
            }
        }

        public async Task CancelOrderAsync(Guid orderId, string reason)
        {
            try
            {
                var order = await _unitOfWork.OrdersRepository.GetAsync(orderId);
                if (order is null) { throw new Exception("Такого заказа нет"); }

                var orderStatus = await _unitOfWork.OrdersRepository.GetOrderStatusAsync("Отменена");
                if (orderStatus is null) { throw new Exception("Статус заказа отсутствует"); }
                var statusCourier = await _unitOfWork.CourierStatusesRepository.GetStatusAsync("Готов к выполнению заказа");
                if (statusCourier is null) { throw new Exception("Статус курьера отсутствует"); }
				if (order.Courier is null) { throw new Exception("курьер отсутствует"); }
				order.Courier.CourierStatusId = statusCourier.Id;
	          
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

                order.Courier.CourierStatus = statusCourier;
                order.OrderStatus = orderStatus;

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

        public async Task<OrderLineDto> AddOrderLineToOrder(OrderLineDto orderLine)
        {
            return new OrderLineDto(await _unitOfWork.OrderLinesRepository.AddAsync(orderLine.ToModel()));
        }

        public async Task<OrderLineDto> UpdateOrderLine(OrderLineDto orderLine)
        {
            return new OrderLineDto(await _unitOfWork.OrderLinesRepository.UpdateAsync(orderLine.ToModel()));
        }

        public async Task RemoveOrderLineFromOrder(Guid id)
        {
            await _unitOfWork.OrderLinesRepository.RemoveAsync(id);
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
                order.CourierId = courier.Id;
                courier.CourierStatus = courierStatus;
                await _unitOfWork.OrdersRepository.UpdateAsync(order);
                await _unitOfWork.CouriersRepository.UpdateAsync(courier);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<OrderDto> UpdateOrderAsync(OrderDto order)
        {
            try
            {
                if (!order.OrderStatusId.HasValue) throw new Exception("Отсутствует статус заказа");
                var orderStatus = await _unitOfWork.OrderStatusesRepository.GetAsync(order.OrderStatusId.Value);

                if (orderStatus.StatusName != "Новая") return null;

                var updated = await _unitOfWork.OrdersRepository.UpdateAsync(order.ToModel());
                return new OrderDto(updated);

            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}
