using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Delivery.BLL.Interfaces;
using Delivery.DAL.Interfaces;
using Delivery.WPF.Services.Services;
using Delivery.WPF.Services.Services.Interfaces;
using Delivery.WPF.ViewModels;
using MathCore.WPF;
using MathCore.WPF.Commands;
using MathCore.WPF.ViewModels;

namespace Delivery.WPF.ViewModels
{
	internal class MainWindowViewModel: ViewModel
	{
        private readonly IUserDialogCouriers _userDialogCouriers;
        private readonly IUserDialogCancelOrder _userDialogCancelOrder;
		private readonly IUserDialogClients _userDialogClients;
        private readonly IUserDialogOrder _userDialogOrder;
        private readonly IUserDialogOrderLine _dialogOrderLine;
		private readonly IUnitOfWork _unitOfWork;
		private readonly IClientService _clientService;
		private readonly ICourierService _courierService;
		private readonly IOrderService _orderService;

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
			CurrentModel = new ClientsViewModel(_unitOfWork, _userDialogClients);
		}
		#endregion

		#region Command ShowCouriersViewCommand - отобразить предтавление курьеров
		private ICommand _ShowCouriersViewCommand;
		public ICommand ShowCouriersViewCommand => _ShowCouriersViewCommand
			??= new LambdaCommand(OnShowCouriersCommandExecuted, CanShowCouriersCommandExecute);
		public bool CanShowCouriersCommandExecute() => true;
		private void OnShowCouriersCommandExecuted()
		{
			CurrentModel = new CouriersViewModel(_unitOfWork, _userDialogCouriers);
		}
		#endregion


		#region Command ShowOrdersViewCommand  - отобразить предтавление заказов
		private ICommand _ShowOrdersViewCommand;
		public ICommand ShowOrdersViewCommand => _ShowOrdersViewCommand
			??= new LambdaCommand(OnShowOrdersCommandExecuted, CanShowOrdersCommandExecute);
		public bool CanShowOrdersCommandExecute() => true;
		private void OnShowOrdersCommandExecuted()
		{
			CurrentModel = new OrdersViewModel(_unitOfWork, _userDialogOrder, _userDialogCancelOrder);
		}
		#endregion

		private string _Title ="Главное окно";
		public string Title { get => _Title; set => Set(ref _Title, value);}


		public MainWindowViewModel(IUnitOfWork unitWork,
			                       IClientService clientService, 
			                       ICourierService courierService, 
			                       IOrderService orderService, 
			                       IUserDialogCouriers userDialogCouriers, 
			                       IUserDialogClients userDialogClients,
                                   IUserDialogOrder userDialogOrder,
                                   IUserDialogOrderLine dialogOrderLine,
			                       IUserDialogCancelOrder userDialogCancelOrder)
		{
			_unitOfWork=unitWork;
			_clientService = clientService;
			_courierService = courierService;
			_orderService=orderService;
            _userDialogCouriers = userDialogCouriers;
            _userDialogClients = userDialogClients;
            _userDialogOrder = userDialogOrder;
            _dialogOrderLine = dialogOrderLine;
			_userDialogCancelOrder = userDialogCancelOrder;

		}
	}
}
