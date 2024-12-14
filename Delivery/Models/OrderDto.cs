using Delivery.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delivery.WPF.Models
{
	internal class OrderDto
	{
		public OrderDto() { }

		public OrderDto(Guid orderId, string targetAddress, string fromAddress, OrderStatus orderStatus, CourierDto courier, DateTime targetDataTime, ClientDto client) { }
	
	}
}
