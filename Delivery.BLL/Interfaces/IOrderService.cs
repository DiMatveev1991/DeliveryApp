using Delivery.DTOs;
using System;
using System.Threading.Tasks;

namespace Delivery.BLL.Interfaces
{
    public interface IOrderService
    {
        Task<OrderDto> AddOrderAsync(OrderDto order);
        Task TakeInProgressAsync(Guid orderId, Guid courierId);
        Task CancelOrderAsync(Guid orderId, string reason);
        Task CompleteOrderAsync(Guid id);
        Task<OrderDto> UpdateOrderAsync(OrderDto order);
        Task DeleteOrderAsync(Guid id);
        Task<OrderLineDto> AddOrderLineToOrder(OrderLineDto orderLine);
        Task<OrderLineDto> UpdateOrderLine(OrderLineDto orderLine);
        Task RemoveOrderLineFromOrder(Guid id);
    }
}
