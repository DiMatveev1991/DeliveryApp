using Delivery.DAL.Modals.Base;
using System.Net;
using System;
using System.Collections.Generic;

namespace Delivery.DAL.Modals
{
	public class Orders : Entity
	{
		public Guid TargetAddressId { get; set; }
		public Guid FromAddressId { get; set; }
		public DateTime TargetDateTime { get; set; }
		public Guid OrderTypeId { get; set; }
		public Guid StatusId { get; set; }
		public Guid ClientId { get; set; }
		public Guid CourierId { get; set; }
		public virtual Adresses TargetAddress { get; set; }
		public virtual Adresses FromAddress { get; set; }
		public virtual OrderTypes OrderType { get; set; }
		public virtual OrderStatuses OrderStatus { get; set; }
		public virtual Clients Client { get; set; }
		public virtual Couriers Courier { get; set; }

		// (один заказ может содержать несколько строк с товарами).
		public virtual ICollection<OrderLines> OrderLines { get; set; }


	}
}