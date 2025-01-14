using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text;
using Delivery.Models.Base;

namespace Delivery.Models
{
    public class Client : PersonEntity, ISoftDeletable
	{

		public Guid? AddressId { get; set; }

		public virtual Address? Address { get; set; }

		//(один клиент может сделать несколько заказов)
		public virtual ICollection<Order>? Orders { get; set; }

		public bool IsDeleted { get; set; }
		public DateTime? DeletedOnUtc { get; set; }
	}
}
