using System.Net;
using System;
using System.Collections.Generic;
using Delivery.DAL.Models;
using Delivery.Models.Base;
using System.ComponentModel.DataAnnotations;

namespace Delivery.Models
{
    public class Order : Entity, ISoftDeletable
    {
        public bool IsDeleted { get; set; }
        public DateTime? DeletedOnUtc { get; set; }

		public Guid? TargetAddressId { get; set; }
	
		public Guid? FromAddressId { get; set; }
        public DateTime TargetDateTime { get; set; }

		public Guid? OrderStatusId { get; set; }

		public Guid? ClientId { get; set; }

		public Guid? CourierId { get; set; }

		public virtual Address? TargetAddress { get; set; }

		public virtual Address? FromAddress { get; set; }

		public virtual OrderStatus? OrderStatus { get; set; }

		public virtual Client? Client { get; set; }

		public virtual Courier? Courier { get; set; }

		// (один заказ может содержать несколько строк с товарами).
		public virtual ICollection<OrderLine>? OrderLines { get; set; }

		public CancelReason? CancelReason { get; set; }

    }
}