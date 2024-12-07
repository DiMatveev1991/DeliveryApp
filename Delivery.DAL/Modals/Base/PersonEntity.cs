namespace Delivery.DAL.Modals.Base
{
	public abstract class PersonEntity : Entity
	{
		public string FistName { get; set; }
		public string SecondName { get; set;}
		public string PhoneNumber { get; set;}
	}
}