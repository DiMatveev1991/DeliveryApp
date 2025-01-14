using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Delivery.Models.Base;

namespace Delivery.Models
{
	public class CourierStatus : StatusEntity
	{
		//(несколько курьеров могут иметь один и тот же статус)

		public virtual ICollection<Courier>? Couriers { get; set; }
	}
}