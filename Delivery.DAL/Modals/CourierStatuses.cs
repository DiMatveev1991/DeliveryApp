using Delivery.DAL.Modals.Base;
using System.Collections.Generic;

namespace Delivery.DAL.Modals
{
	public class CourierStatuses : StatuseEntity
	{
		//(несколько курьеров могут иметь один и тот же статус)
		public virtual ICollection<Couriers> Couriers { get; set; }
	}
}