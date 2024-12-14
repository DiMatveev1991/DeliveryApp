using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Delivery.BLL.Interfaces;
using Delivery.DAL.Interfaces;
using MathCore.WPF.ViewModels;

namespace Delivery.WPF.ViewModels
{
	internal class OrdersViewModel: ViewModel
	{
		private readonly IUnitOfWork _UnitOfWork;
		private readonly IOrderService _OrderService;
		
		public OrdersViewModel(IUnitOfWork unitOfWork, IOrderService orderService)
		{
	      _UnitOfWork = unitOfWork;
		  _OrderService = orderService;
		}
	}
}
