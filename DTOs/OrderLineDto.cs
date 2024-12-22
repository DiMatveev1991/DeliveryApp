using Delivery.Models;
using System;
using System.IO;

namespace Delivery.DTOs
{
    public class OrderLineDto
    {
        public OrderLineDto() { }
		public OrderLineDto(OrderLine model)
		{
			Id = model.Id;
            OrderId = model.OrderId;
            ItemName = model.ItemName;
            Weight = model.Weight;
            Length = model.Length;
            Width = model.Width;
		}
		public Guid Id { get; set; }
        public Guid? OrderId { get; set; }
        public string? ItemName { get; set; }
        public double Weight { get; set; }
        public double Length { get; set; }
        public double Width { get; set; }

        public OrderLine ToModel()
        {
	        return new OrderLine()
	        {
		        Id = Id,
                OrderId = OrderId,
                ItemName = ItemName,
                Weight = Weight,
                Length = Length,
                Width = Width
	        };
        }
	}
}
