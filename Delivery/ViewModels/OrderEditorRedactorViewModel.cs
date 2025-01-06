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
    internal class OrderEditorRedactorViewModel : ViewModel

    {
        private readonly IUserDialogOrderLine _userDialogOrderLine;
        private readonly IOrderService _orderService;
        private readonly IUserDialogRedactorOrder _userDialogRedactorOrder;
        private readonly IUnitOfWork _unitOfWork;
        public bool WasChanged { get; set; }

        public OrderEditorRedactorViewModel(OrderDto order, IUnitOfWork unitOfWork, IUserDialogOrderLine userDialogOrderLine,
            IOrderService orderService, IUserDialogRedactorOrder userDialogRedactorOrder)
        {
            _order = order;
            _unitOfWork = unitOfWork;
            _userDialogOrderLine = userDialogOrderLine;
            _orderService = orderService;
            _userDialogRedactorOrder = userDialogRedactorOrder;

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

        #region -  Строки заказа

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

        public OrderDto Order
        {
            get => _order;
            set
            {
	            Set(ref _order, value);
                OnPropertyChanged(nameof(OrdersLines));
			}
        }

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

        public OrderLineDto CachedSelectedOrderLine
        {
            get => _cachedSelectedOrderLine;
            set => Set(ref _cachedSelectedOrderLine, value);
        }

        #endregion

        #region Command UpdateOrderLineCommand  - команда изменения строки заказа в бд в БД

        private ICommand _updateOrderLineCommand;

        public ICommand UpdateOrderLineCommand => _updateOrderLineCommand
            ??= new LambdaCommandAsync<OrderLineDto>(OnUpdateOrderLineCommandExecuted, CanUpdateOrderLineCommandExecute);

        //Тут бы сделать валидацию вводимых данных, но как нибудь в другой раз
        private bool CanUpdateOrderLineCommandExecute(OrderLineDto p) =>
            p != null || SelectedOrderLine != null;

        private async Task OnUpdateOrderLineCommandExecuted(OrderLineDto? p)
        {
            try
            {
                var orderLineToUpdate = p ?? CachedSelectedOrderLine;
                await _orderService.UpdateOrderLine(orderLineToUpdate);
                SelectedOrderLine.OrderId = Order.Id;
				SelectedOrderLine = orderLineToUpdate;
                _changedCommitted = true;
              
                var orderLines = (await _unitOfWork.OrdersRepository
                    .GetAsync(Order.Id)).OrderLines;
                if (orderLines != null)
                    OrdersLines = new ObservableCollection<OrderLineDto>(orderLines
                        .Select(x => new OrderLineDto(x)).ToList());
                Order.OrderLines = OrdersLines.ToList();
				OnPropertyChanged(nameof(OrdersLinesView));
                WasChanged = true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        #endregion

        #region Command RemoveOrderLineCommand : OrderLineDto - Удаление указанной строки заказа

        private ICommand _removeOrderLineCommand;

        public ICommand RemoveOrderLineCommand => _removeOrderLineCommand
            ??= new LambdaCommandAsync<OrderLineDto>(OnRemoveOrderLineCommandExecuted, CanRemoveOrderLineCommandExecute);

        private bool CanRemoveOrderLineCommandExecute(OrderLineDto p) =>
            p != null || SelectedOrderLine != null;

        private async Task OnRemoveOrderLineCommandExecuted(OrderLineDto? p)
        {
            var orderLineToRemove = p ?? SelectedOrderLine;

            if (!_userDialogOrderLine.ConfirmWarning($"Вы хотите удалить товар {orderLineToRemove.ItemName}?",
                    "Удаление товара"))
                return;
            await _orderService.RemoveOrderLineFromOrder(orderLineToRemove.Id);

            var lineToRemove = Order.OrderLines.FirstOrDefault(x => x.Id == SelectedOrderLine.Id);
            Order.OrderLines.Remove(lineToRemove);
            OrdersLines = Order.OrderLines.ToObservableCollection();

            if (ReferenceEquals(SelectedOrderLine, orderLineToRemove))
                SelectedOrderLine = null;

            WasChanged = true;
        }

        #endregion

        #region Command AddNewOrderLineCommand - Добавление новой строки заказа

        private ICommand _addNewOrderLineCommand;

        public ICommand AddNewOrderLineCommand => _addNewOrderLineCommand
            ??= new LambdaCommandAsync(OnAddNewOrderLineCommandExecuted, CanAddNewOrderLineCommandExecute);

        private bool CanAddNewOrderLineCommandExecute() => true;

        private async Task OnAddNewOrderLineCommandExecuted()
        {
            SelectedOrderLine = new OrderLineDto();
            if (!_userDialogOrderLine.Edit(this))
                return;

            SelectedOrderLine.OrderId = Order.Id;
            SelectedOrderLine = await _orderService.AddOrderLineToOrder(SelectedOrderLine);

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
            OrdersLines = new ObservableCollection<OrderLineDto>((await _unitOfWork.OrdersRepository
                    .GetAsync(Order.Id)).OrderLines
                .Select(x => new OrderLineDto(x))
                .ToList());

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
        private bool CanSaveCommandExecute() => true;
        private async Task OnSaveCommandExecuted()
        {
            _userDialogRedactorOrder.Close();
            WasChanged = true;
        }

        #endregion

    }
}
