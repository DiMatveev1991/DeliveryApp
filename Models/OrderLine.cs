﻿using System;
using System.ComponentModel.DataAnnotations;
using Delivery.Models.Base;

namespace Delivery.Models
{
    // реализация в абстрактном классе так как несколько однотипичных таблиц
    public class OrderLine : Entity, ISoftDeletable
	{
		public Guid? OrderId { get; set; }
		
		[MaxLength(256)]
		public string? ItemName { get; set; }
		public double Weight { get; set; }
		public double Length { get; set; }
		public double Width { get; set; }
		public virtual Order? Order { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedOnUtc { get; set; }
    }
}