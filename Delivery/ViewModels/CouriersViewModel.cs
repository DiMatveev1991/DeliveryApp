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
	internal class CouriersViewModel: ViewModel
	{
		private readonly ICourierService _CourierService;
		private readonly IUnitOfWork _UnitOfWork;

		public CouriersViewModel(IUnitOfWork unitOfWork, ICourierService courierService)
		{
			_CourierService = courierService;
			_UnitOfWork = unitOfWork;
		}
	}
}
