using System.ComponentModel.DataAnnotations;

namespace Delivery.DAL.Models.Base
{
	public abstract class PersonEntity : Entity
	{
		[MaxLength(256)]
		public string? FistName { get; set; }
		
		[MaxLength(256)]
		public string? SecondName { get; set;}
		
		[MaxLength(256)]
		public string? PhoneNumber { get; set;}
	}
}