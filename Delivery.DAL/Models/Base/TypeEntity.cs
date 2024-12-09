using System.ComponentModel.DataAnnotations;

namespace Delivery.DAL.Models.Base
{
	public abstract class TypeEntity: Entity
	{
		[MaxLength(256)]
		public string? TypeName { get; set; }
	}

}
