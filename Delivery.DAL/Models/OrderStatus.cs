using Delivery.DAL.Models.Base;
using System.Collections.Generic;

namespace Delivery.DAL.Models
{
	// реализация в абстрактном классе так как несколько однотипичных таблиц
	public class OrderStatus : StatuseEntity
	{
		//(несколько заказов могут иметь один и тот же статус).
		public virtual ICollection<Order>? Orders { get; set; }
	}
}