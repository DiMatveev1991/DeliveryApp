using System;
using System.Collections.Generic;
using System.Text;
using Delivery.DAL.Modals.Base;

namespace Delivery.DAL.Modals
{
	// реализация в абстрактном классе так как несколько однотипичных таблиц
	public class OrderTypes: TypeEntity
	{
		//(несколько заказов могут иметь один и тот же тип).
		public virtual ICollection<Orders> Orders { get; set; }
	}
}
