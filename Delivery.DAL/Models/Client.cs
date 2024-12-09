using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Delivery.DAL.Models.Base;

namespace Delivery.DAL.Models
{
	public class Client : PersonEntity
	{
		public Guid? AddressId { get; set; }
		public virtual Address? Address { get; set; }
		
		//(один клиент может сделать несколько заказов)
		public virtual ICollection<Order>? Orders { get; set; }

	}
}
