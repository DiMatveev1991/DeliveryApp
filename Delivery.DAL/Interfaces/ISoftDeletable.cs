using System;
using System.Collections.Generic;
using System.Text;

namespace Delivery.DAL.Interfaces
{
	public interface ISoftDeletable
	{
		bool IsDeleted { get; set; }

		DateTime? DeletedOnUtc { get; set; }
	}
}
