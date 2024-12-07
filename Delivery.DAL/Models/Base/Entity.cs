using System;
using System.Collections.Generic;
using System.Text;

namespace Delivery.DAL.Models.Base
{
	public abstract class Entity
	{
		public Guid Id { get; set; }
	}
}
