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
using Delivery.WPF.Views;
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
                        SortDescriptions =
                        { new SortDescription(nameof(Delivery.DAL.Models.Order.OrderStatus), ListSortDirection.Ascending),
                            new SortDescription(nameof(Delivery.DAL.Models.Order.Client), ListSortDirection.Ascending),
                            new SortDescription(nameof(Delivery.DAL.Models.Order.Courier), ListSortDirection.Ascending),

                            //new SortDescription(nameof(Delivery.DAL.Models.Order.OrderStatus.StatusName), ListSortDirection.Ascending)
                        }
                    };

                    //_ordersViewSource.Filter += OnOrdersFilter;
                    _ordersViewSource.View.Refresh();

                    OnPropertyChanged(nameof(OrdersView));
                }
            }
        }
        #endregion

        #region Orders : ObservableCollection<Order> - Коллекция заказов

        public ObservableCollection<OrderLine> OrdersLines
        {
            get => _orderLines;
            set
            {
                if (Set(ref _orderLines, value))
                {
                    _ordersLinesViewSource = new CollectionViewSource
                    {
                        Source = value,
                        SortDescriptions =
                        {
                            //new SortDescription(nameof(Delivery.DAL.Models.Order.FirstName), ListSortDirection.Ascending),
                            //new SortDescription(nameof(Delivery.DAL.Models.Order.SecondName), ListSortDirection.Ascending),
                            //new SortDescription(nameof(Delivery.DAL.Models.Order.PhoneNumber), ListSortDirection.Ascending),
                            //new SortDescription(nameof(Delivery.DAL.Models.Order.OrderStatus.StatusName), ListSortDirection.Ascending)
                        }
                    };

                    //_ordersViewSource.Filter += OnOrdersFilter;
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
                    return;
                }
                //Если изменения не были сохранены в базе, то сбрасываем на значения из кеша
                if (!_changedCommitted && !_firstSelect)
                {
                    //Set(ref _selectedOrder, _cachedSelectedOrder);
                }
                _selectedOrder = value;
                OrdersLines = value.OrderLines.ToObservableCollection();
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
                var OrderToUpdate = p ?? CachedSelectedOrder;
                if (!_userDialogOrders.Edit(OrderToUpdate))
                    return;
                await _orderService.UpdateOrderAsync(OrderToUpdate);
                await OnLoadDataCommandExecuted();
                SelectedOrder = Orders.Find(x => x.Id == OrderToUpdate.Id);
                _changedCommitted = true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        #endregion

        #region Command RemoveOrderCommand : Order - Удаление указанного курьера
        private ICommand _RemoveOrderCommand;

        public ICommand RemoveOrderCommand => _RemoveOrderCommand
            ??= new LambdaCommandAsync<Order>(OnRemoveOrderCommandExecuted, CanRemoveOrderCommandExecute);

       
        private bool CanRemoveOrderCommandExecute(Order p) => (p != null || SelectedOrder != null) && SelectedOrder.OrderStatus.StatusName != "Готов к выполнению заказа";

        private async Task OnRemoveOrderCommandExecuted(Order? p)
        {
            var OrderToRemove = p ?? SelectedOrder;

            if (!_userDialogOrders.ConfirmWarning($"Вы хотите удалить заказа {OrderToRemove.Id}?", "Удаление заказа"))
                return;
            if (OrderToRemove.OrderStatus.StatusName == "Передано на выполнение") {throw new ArgumentException("Удаление не может быть выполнено, активный заказ");}
           await _orderService.DeleteOrderAsync(OrderToRemove.Id);
            _orders.Remove(OrderToRemove);

            if (ReferenceEquals(SelectedOrder, OrderToRemove))
                SelectedOrder = null;
        }
        #endregion

        #region Command AddNewOrderCommand - Добавление нового курьера
        private ICommand _AddNewOrderCommand;

        public ICommand AddNewOrderCommand => _AddNewOrderCommand
            ??= new LambdaCommandAsync(OnAddNewOrderCommandExecuted, CanAddNewOrderCommandExecute);
        private bool CanAddNewOrderCommandExecute() => true;

        private async Task OnAddNewOrderCommandExecuted()
        {
            var new_Order = new Order();
            if (!_userDialogOrders.Edit(new_Order))
                return;
            new_Order = await _orderService.AddOrderAsync(new_Order.Client, new_Order.FromAddress, new_Order.TargetAddress, new_Order.OrderLines);
            await OnLoadDataCommandExecuted();
            SelectedOrder = new_Order;

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
                var OrderToUpdate = p ?? CachedSelectedOrder;
                var courier = _unitOfWork.CouriersRepository.Items.FirstOrDefault(x => x.CourierStatus.StatusName == "Готов к выполнению заказа");

                await _orderService.TakeInProgressAsync(OrderToUpdate.Id, courier.Id);
                await OnLoadDataCommandExecuted();
                SelectedOrder = Orders.Find(x => x.Id == OrderToUpdate.Id);
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
                var OrderToUpdate = p ?? CachedSelectedOrder;

                if (!_userCancelOrder.Edit(OrderToUpdate)) return;
				await _orderService.CancelOrderAsync(OrderToUpdate.Id, OrderToUpdate.CancelReason);
				await OnLoadDataCommandExecuted();
                SelectedOrder = Orders.Find(x => x.Id == OrderToUpdate.Id);
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
                var OrderToUpdate = p ?? CachedSelectedOrder;
				await _orderService.CompleteOrderAsync(OrderToUpdate.Id);
				await OnLoadDataCommandExecuted();
				SelectedOrder = Orders.Find(x => x.Id == OrderToUpdate.Id);
				
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

        private void OnOrdersFilter(object Sender, FilterEventArgs E)
        {
            if (!(E.Item is Order order) || string.IsNullOrEmpty(OrdersFilter)) return;

            //if (!order.PhoneNumber.Contains(OrdersFilter) &&
            //    !order.FirstName.Contains(OrdersFilter) &&
            //    !order.SecondName.Contains(OrdersFilter) &&
            //    !order.OrderStatus.StatusName.Contains(OrdersFilter)
            //   )
            //    E.Accepted = false;
        }


    }
}
