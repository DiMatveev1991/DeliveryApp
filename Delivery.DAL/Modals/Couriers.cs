using Delivery.DAL.Modals.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace Delivery.DAL.Modals
{
	public class Couriers: PersonEntity
	{
		public Guid TypeId { get; set; }
		public Guid StatusId { get; set; }
		public virtual CourierTypes CourierType { get; set; }
		public virtual CourierStatuses CourierStatus { get; set; }

		//(несколько заказов могут быть назначены одному курьеру).
		public virtual ICollection<Orders> Orders { get; set; }
	}
}
