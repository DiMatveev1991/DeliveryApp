using Delivery.BLL.Interfaces;
using Delivery.DAL.Interfaces;
using Delivery.WPF.Services.Services.Interfaces;
using MathCore.WPF.Commands;
using MathCore.WPF.ViewModels;
using System.Windows.Input;

namespace Delivery.WPF.ViewModels
{
    internal class MainWindowViewModel : ViewModel
    {
        private readonly IUserDialogCouriers _userDialogCouriers;
        private readonly IUserDialogCancelOrder _userDialogCancelOrder;
        private readonly IUserDialogClients _userDialogClients;
        private readonly IUserDialogAddOrder _userDialogAddOrder;
        private readonly IUserDialogRedactorOrder _userDialogRedactorOrder;
		private readonly IUserDialogOrderLine _dialogOrderLine;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IClientService _clientService;
        private readonly ICourierService _courierService;
        private readonly IOrderService _orderService;

        private string openedTabName = "";

        #region CurrentModel: ViewModel - текущая дочерняя модель-представления
        private ViewModel _currentModel;
        public ViewModel CurrentModel { get => _currentModel; private set => Set(ref _currentModel, value); }
        #endregion

        #region Command ShowClientsViewCommand - отобразить предтавление клиентов
        private ICommand _showClientsViewCommand;
        public ICommand ShowClientsViewCommand => _showClientsViewCommand
            ??= new LambdaCommand(OnShowClientsCommandExecuted, CanShowClientsCommandExecute);
        public bool CanShowClientsCommandExecute() => true;
        private void OnShowClientsCommandExecuted()
        {
	        if (openedTabName == "Clients") return;
			CurrentModel = new ClientsViewModel(_unitOfWork, _userDialogClients);
			openedTabName = "Clients";
		}
        #endregion

        #region Command ShowCouriersViewCommand - отобразить предтавление курьеров
        private ICommand _showCouriersViewCommand;
        public ICommand ShowCouriersViewCommand => _showCouriersViewCommand
            ??= new LambdaCommand(OnShowCouriersCommandExecuted, CanShowCouriersCommandExecute);
        public bool CanShowCouriersCommandExecute() => true;
        private void OnShowCouriersCommandExecuted()
        {
            if (openedTabName == "Couriers") return;
            CurrentModel = new CouriersViewModel(_unitOfWork, _userDialogCouriers);
            openedTabName = "Couriers";
        }
        #endregion


        #region Command ShowOrdersViewCommand  - отобразить предтавление заказов
        private ICommand _showOrdersViewCommand;
        public ICommand ShowOrdersViewCommand => _showOrdersViewCommand
            ??= new LambdaCommand(OnShowOrdersCommandExecuted, CanShowOrdersCommandExecute);
        public bool CanShowOrdersCommandExecute() => true;
        private void OnShowOrdersCommandExecuted()
        {
            if (openedTabName == "Orders") return;
            CurrentModel = new OrdersViewModel(_unitOfWork, _userDialogAddOrder, _userDialogRedactorOrder, _userDialogCancelOrder);
            openedTabName = "Orders";
        }
        #endregion

        private string _title = "Главное окно";
        public string Title { get => _title; set => Set(ref _title, value); }


        public MainWindowViewModel(IUnitOfWork unitWork,
                                   IClientService clientService,
                                   ICourierService courierService,
                                   IOrderService orderService,
                                   IUserDialogCouriers userDialogCouriers,
                                   IUserDialogClients userDialogClients,
                                   IUserDialogAddOrder userDialogAddOrder,
                                   IUserDialogRedactorOrder userDialogRedactorOrder,
								   IUserDialogOrderLine dialogOrderLine,
                                   IUserDialogCancelOrder userDialogCancelOrder)
        {
            _unitOfWork = unitWork;
            _clientService = clientService;
            _courierService = courierService;
            _orderService = orderService;
            _userDialogCouriers = userDialogCouriers;
            _userDialogClients = userDialogClients;
            _userDialogAddOrder = userDialogAddOrder;
            _userDialogRedactorOrder = userDialogRedactorOrder;
            _dialogOrderLine = dialogOrderLine;
            _userDialogCancelOrder = userDialogCancelOrder;

        }
    }
}
