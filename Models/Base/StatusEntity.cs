using System.ComponentModel.DataAnnotations;

namespace Delivery.Models.Base
{
	public abstract class StatusEntity : Entity
	{
		[MaxLength(256)]
		public string? StatusName { get; set; }
	}
}