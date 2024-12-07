using Delivery.DAL.Models.Base;
using System.Collections.Generic;

namespace Delivery.DAL.Models
{
	public class CourierStatus : StatuseEntity
	{
		//(несколько курьеров могут иметь один и тот же статус)
		public virtual ICollection<Courier>? Couriers { get; set; }
	}
}