using Delivery.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathCore.WPF.ViewModels;

namespace Delivery.WPF.ViewModels
{
	internal class OrderEditorCancelViewModel: ViewModel
	{
		private Order _order;
		public Order Order { get => _order; set => Set(ref _order, value); }

		public OrderEditorCancelViewModel (Order order)
		{
			_order = order;
		}
	}
}
