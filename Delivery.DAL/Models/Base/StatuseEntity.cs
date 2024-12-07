using System.ComponentModel.DataAnnotations;

namespace Delivery.DAL.Models.Base
{
	public abstract class StatuseEntity : Entity
	{
		[MaxLength(256)]
		public string? StatusName { get; set; }
	}
}