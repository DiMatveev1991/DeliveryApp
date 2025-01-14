using Delivery.BLL.Interfaces;
using Delivery.DAL.Interfaces;
using Delivery.DTOs;
using Delivery.WPF.Services.Services.Interfaces;
using MathCore.WPF.Commands;
using MathCore.WPF.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Input;

namespace Delivery.WPF.ViewModels
{
    public class OrderEditorAddViewModel : ViewModel
    {
        private readonly IUserDialogOrderLine _userDialogOrderLine;
        private readonly IOrderService _orderService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserDialogAddOrder _userDialogAddOrder;

        public bool WasChanged { get; set; }

        public OrderEditorAddViewModel(OrderDto order, IUnitOfWork unitOfWork, IUserDialogOrderLine userDialogOrderLine, IOrderService orderService, IUserDialogAddOrder userDialogAddOrder)
        {
            _order = order;
            _unitOfWork = unitOfWork;
            _userDialogOrderLine = userDialogOrderLine;
            _orderService = orderService;
            _userDialogAddOrder = userDialogAddOrder;

            var currentDate = DateTime.UtcNow;
            Order.TargetDateTime = new DateTime(currentDate.Year, currentDate.Month, currentDate.Day);
            Order.OrderLines ??= new List<OrderLineDto>();
            OrdersLines = Order.OrderLines.ToObservableCollection();
        }


        #region Orders : ObservableCollection<ClientDto> - Коллекция клиентов

        private ObservableCollection<ClientDto> _clients;
        private CollectionViewSource _clientsViewSource;

        public ObservableCollection<ClientDto> Clients
        {
            get => _clients;
            set
            {
                if (Set(ref _clients, value))
                {
                    _clientsViewSource = new CollectionViewSource
                    {
                        Source = value,
                    };

                    _clientsViewSource.View.Refresh();
                }
            }
        }
        #endregion

        #region Addresses : ObservableCollection<AddressDto> - Коллекция адресов

        private ObservableCollection<AddressDto> _addresses;
        private CollectionViewSource _addressesViewSource;

        public ObservableCollection<AddressDto> Addresses
        {
            get => _addresses;
            set
            {
                if (Set(ref _addresses, value))
                {
                    _addressesViewSource = new CollectionViewSource
                    {
                        Source = value,
                    };

                    _addressesViewSource.View.Refresh();
                }
            }
        }
        #endregion

        #region  -  Строки заказа
        private ObservableCollection<OrderLineDto> _orderLines;
        private CollectionViewSource _ordersLinesViewSource;
        public ICollectionView OrdersLinesView => _ordersLinesViewSource?.View;

        #region Orders : ObservableCollection<OrderLineDto> - Коллекция строк заказа

        public ObservableCollection<OrderLineDto> OrdersLines
        {
            get => _orderLines;
            set
            {
                if (Set(ref _orderLines, value))
                {
                    _ordersLinesViewSource = new CollectionViewSource
                    {
                        Source = value,
                    };
                    _ordersLinesViewSource.View.Refresh();

                    OnPropertyChanged(nameof(OrdersLinesView));
                }
            }
        }
        #endregion


        private OrderDto _order;
        public OrderDto Order { get => _order; set => Set(ref _order, value); }
        #endregion

        private ClientDto _client;
        public ClientDto Client
        {
            get => _client;
            set
            {
                Set(ref _client, value);
                Order.ClientName = value.FirstName + " " + value.SecondName;
                Order.ClientPhone = value.PhoneNumber;
                Order.ClientId = value.Id;
                Order.FromAddress = value.Address;
                OnPropertyChanged(nameof(Order));
            }
        }

        #region SelectedOrderLine : OrderLineDto - Выбранная строка заказа
        private OrderLineDto _selectedOrderLine;
        public OrderLineDto SelectedOrderLine
        {
            get => _selectedOrderLine;
            set
            {
                if (value is null)
                {
                    _selectedOrderLine = value;
                    return;
                }
                _selectedOrderLine = value;
                CachedSelectedOrderLine = new OrderLineDto()
                {
                    Id = value.Id,
                    ItemName = value.ItemName,
                    Length = value.Length,
                    Weight = value.Weight,
                    Width = value.Width,
                    OrderId = value.OrderId,
                };
                Set(ref _selectedOrderLine, value);
                _changedCommitted = false;
                _firstSelect = false;
            }
        }

        private bool _firstSelect = true;
        private bool _changedCommitted;
        private OrderLineDto _cachedSelectedOrderLine;
        public OrderLineDto CachedSelectedOrderLine { get => _cachedSelectedOrderLine; set => Set(ref _cachedSelectedOrderLine, value); }

        #endregion

        #region Command AddNewOrderLineCommand - Добавление нового заказа
        private ICommand _addNewOrderLineCommand;

        public ICommand AddNewOrderLineCommand => _addNewOrderLineCommand
            ??= new LambdaCommandAsync(OnAddNewOrderLineCommandExecuted, CanAddNewOrderLineCommandExecute);
        private bool CanAddNewOrderLineCommandExecute() => true;
        private async Task OnAddNewOrderLineCommandExecuted()
        {
            SelectedOrderLine = new OrderLineDto();
            if (!_userDialogOrderLine.Edit(this))
                return;

            Order.OrderLines.Add(SelectedOrderLine);
            OrdersLines = Order.OrderLines.ToObservableCollection();
            _ordersLinesViewSource.View.Refresh();

            OnPropertyChanged(nameof(OrdersLinesView));
            WasChanged = true;
        }

        #endregion

        #region Command LoadDataCommand - Команда загрузки данных из репозитория

        private ICommand _loadDataCommand;
        public ICommand LoadDataCommand => _loadDataCommand
            ??= new LambdaCommandAsync(OnLoadDataCommandExecuted, CanLoadDataCommandExecute);
        private bool CanLoadDataCommandExecute() => true;
        private async Task OnLoadDataCommandExecuted()
        {

            Clients = new ObservableCollection<ClientDto>(await _unitOfWork.ClientsRepository.Items
                .Select(x => new ClientDto(x))
                .ToArrayAsync());
            Addresses = new ObservableCollection<AddressDto>(await _unitOfWork.AddressRepository.Items
                .Select(x => new AddressDto(x))
                .ToArrayAsync());
            OnPropertyChanged(nameof(OrdersLines));
        }
        #endregion

        #region Command SaveCommand - Добавление нового заказа
        private ICommand _saveCommand;

        public ICommand SaveCommand => _saveCommand
            ??= new LambdaCommandAsync(OnSaveCommandExecuted, CanSaveCommandExecute);
        private bool CanSaveCommandExecute() => SelectedOrderLine != null && Client != null && Order.TargetAddress != null;
        private async Task OnSaveCommandExecuted()
        {
            _userDialogAddOrder.Close();
        }

        #endregion
    }
}