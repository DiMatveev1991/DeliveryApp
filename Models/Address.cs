using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using Delivery.Models.Base;

namespace Delivery.Models
{
    public class Address : Entity
    {
        [MaxLength(256)]
        public string? City { get; set; }

        [MaxLength(256)]
        public string? Region { get; set; }

        [MaxLength(256)]
        public string? PostalCode { get; set; }

        [MaxLength(256)]
        public string? Street { get; set; }

        [MaxLength(256)]
        public string? HomeNumber { get; set; }

        [MaxLength(256)]
        public string? Corpus { get; set; }

        [MaxLength(256)]
        public string? ApartmentNumber { get; set; }

		//(один адрес может быть связан с несколькими клиентами)

		public virtual ICollection<Client>? Clients { get; set; }

		//(один адрес может быть связан с несколькими заказами)
		public virtual ICollection<Order>? FromOrders { get; set; }

		public virtual ICollection<Order>? TargetOrders { get; set; }
    }
}