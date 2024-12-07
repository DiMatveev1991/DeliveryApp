using System;
using System.Collections.Generic;
using System.Text;
using Delivery.DAL.Models;
using Delivery.DAL.Models.Base;

namespace Delivery.DAL.Models
{
	// реализация в абстрактном классе так как несколько однотипичных таблиц
	public class OrderType: TypeEntity
	{
		//(несколько заказов могут иметь один и тот же тип).
		public virtual ICollection<Order>? Orders { get; set; }
	}
}
