using Delivery.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Delivery.Models.Base;

namespace Delivery.DAL.Models
{
    public class CancelReason : Entity
	{
		[MaxLength(256)]
		public string? ReasonCancel { get; set; }

		public Guid ? OrderId { get; set; }

		public Order? Order { get; set; }
		
	}
}
