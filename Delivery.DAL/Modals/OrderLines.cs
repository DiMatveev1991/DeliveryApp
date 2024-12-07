using Delivery.DAL.Modals.Base;

namespace Delivery.DAL.Modals
{
	// реализация в абстрактном классе так как несколько однотипичных таблиц
	public class OrderLines : Entity
	{
		public int OrderId { get; set; }
		public string ItemName { get; set; }
		public double Weight { get; set; }
		public double Length { get; set; }
		public double Width { get; set; }
		public virtual Orders Order { get; set; }
	}
}