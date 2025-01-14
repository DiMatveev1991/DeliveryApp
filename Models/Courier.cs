using Delivery.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Delivery.Models.Base;
using System.ComponentModel.DataAnnotations;

namespace Delivery.Models
{
    public class Courier: PersonEntity, ISoftDeletable
	{

		public Guid? CourierStatusId { get; set; }

		public virtual CourierStatus? CourierStatus { get; set; }

		//(несколько заказов могут быть назначены одному курьеру).
		public virtual ICollection<Order>? Orders { get; set; }
		public bool IsDeleted { get; set; }
		public DateTime? DeletedOnUtc { get; set; }
	}
}
