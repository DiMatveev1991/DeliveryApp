using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Delivery.Models.Base;

namespace Delivery.Models
{
    // реализация в абстрактном классе так как несколько однотипичных таблиц
    public class OrderStatus : StatusEntity
	{
		//(несколько заказов могут иметь один и тот же статус).

		public virtual ICollection<Order>? Orders { get; set; }
	}
}