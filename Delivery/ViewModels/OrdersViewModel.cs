using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using Delivery.BLL.Interfaces;
using Delivery.BLL.Services;
using Delivery.DAL.Interfaces;
using Delivery.DAL.Models;
using Delivery.WPF.Services.Services.Interfaces;
using MathCore.WPF.Commands;
using MathCore.WPF.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace Delivery.WPF.ViewModels
{
    internal class OrdersViewModel : ViewModel
    {
        private readonly IUserDialogOrder _userDialogOrders;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOrderService _orderService;
        private ObservableCollection<Order> _orders;
        private CollectionViewSource _ordersViewSource;
        private readonly IUserDialogCancelOrder _userCancelOrder;

        public ICollectionView OrdersView => _ordersViewSource?.View;

        private ObservableCollection<OrderLine> _orderLines;
        private CollectionViewSource _ordersLinesViewSource;
        public ICollectionView OrdersLinesView => _ordersLinesViewSource?.View;

        #region Orders : ObservableCollection<Order> - Коллекция заказов

        public ObservableCollection<Order> Orders
        {
            get => _orders;
            set
            {
                if (Set(ref _orders, value))
                {
                    _ordersViewSource = new CollectionViewSource
                    {
                        Source = value,
                    };

                    _ordersViewSource.Filter += OnOrdersFilter;
                    _ordersViewSource.View.Refresh();

                    OnPropertyChanged(nameof(OrdersView));
                }
            }
        }
        #endregion

        #region Orders : ObservableCollection<OrdersLine> - Коллекция строк заказа

        public ObservableCollection<OrderLine> OrdersLines
        {
            get => _orderLines;
            set
            {
                if (Set(ref _orderLines, value))
                {
                    _ordersLinesViewSource = new CollectionViewSource
                    {
                       //Настройка фильтра
	                    Source = value,
                    };

                    _ordersLinesViewSource.View.Refresh();

                    OnPropertyChanged(nameof(OrdersLinesView));
                }
            }
        }
        #endregion

        #region OrdersFilter : string - Искомое слово

        private string _ordersFilter;
        public string OrdersFilter
        {
            get => _ordersFilter;
            set
            {
                if (Set(ref _ordersFilter, value))
                    _ordersViewSource.View.Refresh();

            }
        }
        #endregion

        #region SelectedOrder : Order - Выбранный заказ
        private Order _selectedOrder;
        public Order SelectedOrder
        {
            get => _selectedOrder;
            set
            {
                if (value is null)
                {
                    _selectedOrder = value;
                    OrdersLines = new ObservableCollection<OrderLine>();
                    CachedSelectedOrder = null;
                    return;
                }
                _selectedOrder = value;
                OrdersLines = value.OrderLines?.ToObservableCollection();
                CachedSelectedOrder = new Order()
                {
                    Id = value.Id,
                    Client = value.Client,
                    ClientId = value.ClientId,
                    Courier = value.Courier,
                    CourierId = value.CourierId,
                    FromAddress = value.FromAddress,
                    FromAddressId = value.FromAddressId,
                    TargetAddress = value.TargetAddress,
                    TargetAddressId = value.TargetAddressId,
                    CancelReason = value.CancelReason,
                    OrderLines = value.OrderLines,
                    OrderStatus = value.OrderStatus,
                    OrderStatusId = value.OrderStatusId,
                    TargetDateTime = value.TargetDateTime
                };
                Set(ref _selectedOrder, value);
                _changedCommitted = false;
                _firstSelect = false;
            }
        }

        private bool _firstSelect = true;
        private bool _changedCommitted;
        private Order _cachedSelectedOrder;
        public Order CachedSelectedOrder { get => _cachedSelectedOrder; set => Set(ref _cachedSelectedOrder, value); }

        #endregion

        #region Command UpdateOrderCommand  - команда измененияданных курьера в БД

        private ICommand _UpdateOrderCommand;
        public ICommand UpdateOrderCommand => _UpdateOrderCommand
            ??= new LambdaCommandAsync<Order>(OnUpdateOrderCommandExecuted, CanUpdateOrderCommandExecute);
        //Тут бы сделать валидацию вводимых данных, но как нибудь в другой раз
        private bool CanUpdateOrderCommandExecute(Order p) => (p != null || SelectedOrder != null) && SelectedOrder.OrderStatus.StatusName == "Новая";

        private async Task OnUpdateOrderCommandExecuted(Order? p)
        {
            try
            {
                var orderToUpdate = p ?? CachedSelectedOrder;
                if (!_userDialogOrders.Edit(orderToUpdate))
                    return;
                await _orderService.UpdateOrderAsync(orderToUpdate);
                await OnLoadDataCommandExecuted();
                SelectedOrder = Orders.Find(x => x.Id == orderToUpdate.Id);
                _changedCommitted = true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        #endregion

        #region Command RemoveOrderCommand : Order - Удаление указанного заказа
        private ICommand _RemoveOrderCommand;

        public ICommand RemoveOrderCommand => _RemoveOrderCommand
            ??= new LambdaCommandAsync<Order>(OnRemoveOrderCommandExecuted, CanRemoveOrderCommandExecute);

        //можно раскоментировать строку ниже для избежания попадания в эксепшен или вывести информацию об ошибке, сделать бы
        private bool CanRemoveOrderCommandExecute(Order p) => (p != null || SelectedOrder != null)
                                                              && SelectedOrder.OrderStatus.StatusName != "Готов к выполнению заказа"
                                                              //&& SelectedOrder.OrderStatus.StatusName != "Передано на выполнение"
                                                              ;

        private async Task OnRemoveOrderCommandExecuted(Order? p)
        {
            var orderToRemove = p ?? SelectedOrder;

            if (!_userDialogOrders.ConfirmWarning($"Вы хотите удалить заказа {orderToRemove.Id}?", "Удаление заказа"))
                return;
            if (orderToRemove.OrderStatus.StatusName == "Передано на выполнение")
            {
	            
	            //if (!_userDialogOrders.ConfirmInformation($"Удаление не может быть выполнено, активный заказ", "Нельзя удалить."))
                //    return;
                throw new ArgumentException("Удаление не может быть выполнено, активный заказ");
            }
            await _orderService.DeleteOrderAsync(orderToRemove.Id);
            _orders.Remove(orderToRemove);

            if (ReferenceEquals(SelectedOrder, orderToRemove))
                SelectedOrder = null;
        }
        #endregion

        #region Command AddNewOrderCommand - Добавление нового заказа
        private ICommand _AddNewOrderCommand;

        public ICommand AddNewOrderCommand => _AddNewOrderCommand
            ??= new LambdaCommandAsync(OnAddNewOrderCommandExecuted, CanAddNewOrderCommandExecute);
        private bool CanAddNewOrderCommandExecute() => true;

        private async Task OnAddNewOrderCommandExecuted()
        {
            var newOrder = new Order();
            if (!_userDialogOrders.Edit(newOrder, true))
                return;
            newOrder = await _orderService.AddOrderAsync(newOrder.Client, newOrder.Client.Address, newOrder.TargetAddress, newOrder.OrderLines, newOrder.TargetDateTime);
            await OnLoadDataCommandExecuted();
            SelectedOrder = Orders.Find(x => x.Id == newOrder.Id);

        }

        #endregion

        #region Command TakeToJobOrderCommand  - команда передачи заказа в работу

        private ICommand _TakeToJobOrderCommand;
        public ICommand TakeToJobOrderCommand => _TakeToJobOrderCommand
            ??= new LambdaCommandAsync<Order>(OnTakeToJobOrderCommandExecuted, CanTakeToJobOrderCommandExecute);
        //Тут бы сделать валидацию вводимых данных, но как нибудь в другой раз
        private bool CanTakeToJobOrderCommandExecute(Order p) => (p != null || SelectedOrder != null) && SelectedOrder.OrderStatus.StatusName == "Новая";

        private async Task OnTakeToJobOrderCommandExecuted(Order? p)
        {
            try
            {
                var orderToUpdate = p ?? CachedSelectedOrder;
                var courier = _unitOfWork.CouriersRepository.Items.FirstOrDefault(x => x.CourierStatus.StatusName == "Готов к выполнению заказа");

                await _orderService.TakeInProgressAsync(orderToUpdate.Id, courier.Id);
                await OnLoadDataCommandExecuted();
                SelectedOrder = Orders.Find(x => x.Id == orderToUpdate.Id);
                _changedCommitted = true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        #endregion

        #region Command CancelOrderCommand  - команда отмены заказа

        private ICommand _CancelOrderCommand;
        public ICommand CancelOrderCommand => _CancelOrderCommand
            ??= new LambdaCommandAsync<Order>(OnCancelOrderOrderCommandExecuted, CanCancelOrderCommandExecute);
        //Тут бы сделать валидацию вводимых данных, но как нибудь в другой раз
        private bool CanCancelOrderCommandExecute(Order p) => (p != null || SelectedOrder != null) && SelectedOrder.OrderStatus.StatusName == "Новая";

        private async Task OnCancelOrderOrderCommandExecuted(Order? p)
        {
            try
            {
                var orderToUpdate = p ?? CachedSelectedOrder;

                if (!_userCancelOrder.Edit(orderToUpdate)) return;
                await _orderService.CancelOrderAsync(orderToUpdate.Id, orderToUpdate.CancelReason);
                await OnLoadDataCommandExecuted();
                SelectedOrder = Orders.Find(x => x.Id == orderToUpdate.Id);
                _changedCommitted = true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        #endregion

        #region Command CompleteOrderCommand  - команда смены статуса на заказ выполнен

        private ICommand _CompleteOrderCommand;
        public ICommand CompleteOrderCommand => _CompleteOrderCommand
            ??= new LambdaCommandAsync<Order>(OnCompleteOrderOrderCommandExecuted, CanCompleteOrderOrderCommandExecute);
        //Тут бы сделать валидацию вводимых данных, но как нибудь в другой раз
        private bool CanCompleteOrderOrderCommandExecute(Order p) => (p != null || SelectedOrder != null) && SelectedOrder.OrderStatus.StatusName == "Передано на выполнение";

        private async Task OnCompleteOrderOrderCommandExecuted(Order? p)
        {
            try
            {
                var orderToUpdate = p ?? CachedSelectedOrder;
                await _orderService.CompleteOrderAsync(orderToUpdate.Id);
                await OnLoadDataCommandExecuted();
                SelectedOrder = Orders.Find(x => x.Id == orderToUpdate.Id);

                _changedCommitted = true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        #endregion

        public OrdersViewModel(IUnitOfWork unitOfWork, IUserDialogOrder userDialogOrder, IUserDialogCancelOrder userCancelOrder)
        {
            _unitOfWork = unitOfWork;
            _orderService = new OrderService(_unitOfWork);
            _userDialogOrders = userDialogOrder;
            _userCancelOrder = userCancelOrder;


        }

        #region Command LoadDataCommand - Команда загрузки данных из репозитория

        private ICommand _LoadDataCommand;
        public ICommand LoadDataCommand => _LoadDataCommand
            ??= new LambdaCommandAsync(OnLoadDataCommandExecuted, CanLoadDataCommandExecute);
        private bool CanLoadDataCommandExecute() => true;

        private async Task OnLoadDataCommandExecuted()
        {
	        Orders = new ObservableCollection<Order>(await _unitOfWork.OrdersRepository.Items.ToArrayAsync());
	        OnPropertyChanged(nameof(Orders));
        }

        #endregion

        private void OnOrdersFilter(object sender, FilterEventArgs E)
        {
            if (!(E.Item is Order order) || string.IsNullOrEmpty(OrdersFilter)) return;

            //подключить все необходимые поля фильтрации
            if (!order.Client.FirstName.Contains(OrdersFilter) &&
                !order.Client.PhoneNumber.Contains(OrdersFilter) && 
				 !order.FromAddress.City.Contains(OrdersFilter) &&
                 !order.FromAddress.Street.Contains(OrdersFilter) &&
                !order.FromAddress.HomeNumber.ToString().Contains(OrdersFilter) &&
                !order.FromAddress.ApartmentNumber.ToString().Contains(OrdersFilter) &&
				 !order.TargetAddress.City.Contains(OrdersFilter) &&
                 !order.TargetAddress.Street.Contains(OrdersFilter) &&
                !order.TargetAddress.HomeNumber.Contains(OrdersFilter) &&
                !order.TargetAddress.ApartmentNumber.Contains(OrdersFilter) && 
                !order.TargetDateTime.ToString().Contains(OrdersFilter) &&
				 !order.OrderStatus.StatusName.Contains(OrdersFilter)
               )
                E.Accepted = false;
        }


    }
}
