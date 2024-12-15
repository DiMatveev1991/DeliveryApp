using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Delivery.BLL.Interfaces;
using Delivery.DAL.Interfaces;
using Delivery.WPF.ViewModels;
using MathCore.WPF.Commands;
using MathCore.WPF.ViewModels;

namespace Delivery.WPF.ViewModels
{
	internal class MainWindowViewModel: ViewModel
	{

		private readonly IUnitOfWork _UnitOfWork;
		private readonly IClientService _ClientService;
		private readonly ICourierService _CourierService;
		private readonly IOrderService _OrderService;

		#region CurrentModel: ViewModel - текущая дочерняя модель-представления
		private ViewModel _currentModel;
		public ViewModel CurrentModel { get => _currentModel; private set => Set(ref _currentModel, value); }
		#endregion

		#region Command ShowClientsViewCommand - отобразить предтавление клиентов
		private ICommand _ShowClientsViewCommand;
		public ICommand ShowClientsViewCommand => _ShowClientsViewCommand
			??= new LambdaCommand(OnShowClientsCommandExecuted, CanShowClientsCommandExecute);
		public bool CanShowClientsCommandExecute() => true;
		private void OnShowClientsCommandExecuted()
		{
			CurrentModel = new ClientsViewModel(_UnitOfWork, _ClientService);
		}
		#endregion

		#region Command ShowCouriersViewCommand - отобразить предтавление курьеров
		private ICommand _ShowCouriersViewCommand;
		public ICommand ShowCouriersViewCommand => _ShowCouriersViewCommand
			??= new LambdaCommand(OnShowCouriersCommandExecuted, CanShowCouriersCommandExecute);
		public bool CanShowCouriersCommandExecute() => true;
		private void OnShowCouriersCommandExecuted()
		{
			CurrentModel = new CouriersViewModel(_UnitOfWork);
		}
		#endregion


		#region Command ShowOrdersViewCommand  - отобразить предтавление заказов
		private ICommand _ShowOrdersViewCommand;
		public ICommand ShowOrdersViewCommand => _ShowOrdersViewCommand
			??= new LambdaCommand(OnShowOrdersCommandExecuted, CanShowOrdersCommandExecute);
		public bool CanShowOrdersCommandExecute() => true;
		private void OnShowOrdersCommandExecuted()
		{
			CurrentModel = new OrdersViewModel(_UnitOfWork, _OrderService);
		}
		#endregion



		private string _Title ="Главное окно";
		public string Title { get => _Title; set => Set(ref _Title, value);}


		public MainWindowViewModel(IUnitOfWork unitWork, IClientService clientService, ICourierService courierService, IOrderService orderService)
		{
			_UnitOfWork=unitWork;
			_ClientService = clientService;
			_CourierService = courierService;
			_OrderService=orderService;
		}
	}
}
