using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Delivery.DAL.Modals.Base;

namespace Delivery.DAL.Modals
{
	public class Clients : PersonEntity
	{
		public Guid AddressId { get; set; }
		public virtual Adresses Address { get; set; }
		
		//(один клиент может сделать несколько заказов)
		public virtual ICollection<Orders> Orders { get; set; }

	}
}
