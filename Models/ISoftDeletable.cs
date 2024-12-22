using System;

namespace Delivery.Models
{
	public interface ISoftDeletable
	{
		bool IsDeleted { get; set; }

		DateTime? DeletedOnUtc { get; set; }
	}
}
