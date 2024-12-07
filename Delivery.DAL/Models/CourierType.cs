using Delivery.DAL.Models.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace Delivery.DAL.Models
{
	// реализация в абстрактном классе так как несколько однотипичных таблиц
	public class CourierType: StatuseEntity
	{
		//(несколько курьеров могут иметь один и тот же тип).
		public virtual ICollection<Courier>? Couriers { get; set; }
	}
}
