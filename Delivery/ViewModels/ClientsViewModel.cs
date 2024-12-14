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
	internal class ClientsViewModel: ViewModel
	{
		private readonly IUnitOfWork _UnitOfWork;
		private readonly IClientService _ClientService;

		public ClientsViewModel(IUnitOfWork unitOfWork, IClientService clientService)
		{
			_UnitOfWork = unitOfWork;
			_ClientService = clientService;
		}
	}
}
