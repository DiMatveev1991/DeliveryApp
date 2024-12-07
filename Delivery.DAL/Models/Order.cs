using Delivery.DAL.Models.Base;
using System.Net;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Delivery.DAL.Models;

namespace Delivery.DAL.Models
{
	public class Order : Entity
	{
		public Guid TargetAddressId { get; set; }
		public Guid FromAddressId { get; set; }
		public DateTime TargetDateTime { get; set; }
		public Guid OrderTypeId { get; set; }
		public Guid StatusId { get; set; }
		public Guid ClientId { get; set; }
		public Guid CourierId { get; set; }
		public virtual Address? TargetAddress { get; set; }
		public virtual Address? FromAddress { get; set; }
		public virtual OrderType? OrderType { get; set; }
		public virtual OrderStatus? OrderStatus { get; set; }
		public virtual Client? Client { get; set; }
		public virtual Courier? Courier { get; set; }

		// (один заказ может содержать несколько строк с товарами).
		public virtual ICollection<OrderLine>? OrderLines { get; set; }

		[MaxLength(256)]
		public string? CancelReason { get; set; }

	}
}