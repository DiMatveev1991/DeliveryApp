using Delivery.DAL.Modals.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace Delivery.DAL.Modals
{
	// реализация в абстрактном классе так как несколько однотипичных таблиц
	public class CourierTypes: StatuseEntity
	{
		//(несколько курьеров могут иметь один и тот же тип).
		public virtual ICollection<Couriers> Couriers { get; set; }
	}
}
