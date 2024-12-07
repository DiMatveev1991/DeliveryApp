using Delivery.DAL.Modals.Base;
using System.Collections.Generic;

namespace Delivery.DAL.Modals
{
	// реализация в абстрактном классе так как несколько однотипичных таблиц
	public class OrderStatuses : StatuseEntity
	{
		//(несколько заказов могут иметь один и тот же статус).
		public virtual ICollection<Orders> Orders { get; set; }
	}
}