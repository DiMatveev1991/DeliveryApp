using Delivery.DAL.Models.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace Delivery.DAL.Models
{
	public class Courier: PersonEntity
	{
		public Guid TypeId { get; set; }
		public Guid StatusId { get; set; }
		public virtual CourierType? CourierType { get; set; }
		public virtual CourierStatus? CourierStatus { get; set; }

		//(несколько заказов могут быть назначены одному курьеру).
		public virtual ICollection<Order>? Orders { get; set; }
	}
}
