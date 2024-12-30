using Delivery.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Delivery.DAL.Models;

namespace Delivery.DTOs
{
    public class OrderDto
    {
        public OrderDto()
        {
        }

        public OrderDto(Order model)
        {
            Id = model.Id;
            TargetDateTime = model.TargetDateTime;
            OrderStatusId = model.OrderStatusId;
            ClientId = model.ClientId;
            CourierId = model.CourierId;
            if (model.FromAddress != null) FromAddress = new AddressDto(model.FromAddress);
            if (model.TargetAddress != null) TargetAddress = new AddressDto(model.TargetAddress);
            OrderStatusName = model.OrderStatus?.StatusName ?? string.Empty;
            ClientName = model.Client?.FirstName ?? string.Empty;
            ClientPhone = model.Client?.PhoneNumber ?? string.Empty;
            CourierName = model.Courier?.FirstName ?? string.Empty;
            CourierPhone = model.Courier?.PhoneNumber ?? string.Empty;
            OrderLines = model.OrderLines?.Select(x => new OrderLineDto(x)).ToList() ?? new List<OrderLineDto>();
            if (model.CancelReason != null) CancelReason = model.CancelReason.ReasonCancel;
        }

        public Guid Id { get; set; }
        public Guid? TargetAddressId => TargetAddress?.Id;
        public Guid? FromAddressId => FromAddress?.Id;
        public DateTime TargetDateTime { get; set; }
        public Guid? OrderStatusId { get; set; }
        public Guid? ClientId { get; set; }
        public Guid? CourierId { get; set; }
        public AddressDto FromAddress { get; set; }
        public AddressDto TargetAddress { get; set; }

        //TODO на подумать
        public string FullFromAddress => FromAddress?.City + " " + FromAddress?.Street + " " + FromAddress?.HomeNumber;
        public string FullTargetAddress => TargetAddress?.City + " " + TargetAddress?.Street;

        public string OrderStatusName { get; set; }
        public string ClientName { get; set; }
        public string ClientPhone { get; set; }
        public string CourierName { get; set; }
        public string CourierPhone { get; set; }
        public Guid? CancelReasonId { get; set; }
        public string CancelReason { get; set; }

        public List<OrderLineDto> OrderLines { get; set; }

        public Order ToModel()
        {
	        CancelReason Reason;

	        if (CancelReason != null)
	        {
		        Reason = new CancelReason { ReasonCancel = this.CancelReason };
	        }
	        else
	        {
		        Reason = null;
	        }
        
        return new Order
            {
                Id = Id,
                TargetAddressId = TargetAddressId,
                FromAddressId = FromAddressId,
                OrderStatusId = OrderStatusId,
                ClientId = ClientId,
                CourierId = CourierId,
                FromAddress = FromAddress.ToModel(),
                TargetAddress = TargetAddress.ToModel(),
                OrderLines = OrderLines.Select(x => x.ToModel()).ToList(),
                CancelReason = Reason
            };
        }
    }
}
