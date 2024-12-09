using System;
using Delivery.DAL.Models.Base;
using System.ComponentModel.DataAnnotations;

namespace Delivery.DAL.Models
{
	// реализация в абстрактном классе так как несколько однотипичных таблиц
	public class OrderLine : Entity
	{
		public Guid? OrderId { get; set; }
		
		[MaxLength(256)]
		public string? ItemName { get; set; }
		public double Weight { get; set; }
		public double Length { get; set; }
		public double Width { get; set; }
		public virtual Order? Order { get; set; }
	}
}