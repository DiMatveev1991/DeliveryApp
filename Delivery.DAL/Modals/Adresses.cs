using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Delivery.DAL.Modals.Base;

namespace Delivery.DAL.Modals
{
	public class Adresses : Entity
	{
		public string Country { get; set; }
		public string City { get; set; }
		public string Region { get; set; }
		public string PostalCode { get; set; }
		public string Street { get; set; }
		public string HomeNumber { get; set; }
		public string? Corpus { get; set; }
		public string ApartmentNumber { get; set; }
		
		//(один адрес может быть связан с несколькими клиентами)
		public virtual ICollection<Clients> Clients { get; set; }
		
		//(один адрес может быть связан с несколькими заказами)
		public virtual ICollection<Orders> Orders { get; set; }
	}
}