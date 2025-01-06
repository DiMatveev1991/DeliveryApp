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
using Delivery.DTOs;
using Delivery.WPF.Services.Services.Interfaces;
using MathCore.WPF.Commands;
using MathCore.WPF.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace Delivery.WPF.ViewModels
{
	public class OrdersViewModel : ViewModel
    {
        private readonly IUserDialogAddOrder _userDialogAddOrders;
        private readonly IUserDialogRedactorOrder _userDialogRedactorOrder;
		private readonly IUnitOfWork _unitOfWork;
        private readonly IOrderService _orderService;
        private ObservableCollection<OrderDto> _orders;
        private CollectionViewSource _ordersViewSource;
        private readonly IUserDialogCancelOrder _userCancelOrder;
        public ICollectionView OrdersView => _ordersViewSource?.View;

        private ObservableCollection<OrderLineDto> _orderLines;
        private CollectionViewSource _ordersLinesViewSource;
        public ICollectionView OrdersLinesView => _ordersLinesViewSource?.View;

        #region Orders : ObservableCollection<OrderDto> - Коллекция заказов

        public ObservableCollection<OrderDto> Orders
        {
            get => _orders;
            set
            {
                if (!Set(ref _orders, value)) return;

                _ordersViewSource = new CollectionViewSource
                {
                    Source = value,
                };

                _ordersViewSource.Filter += OnOrdersFilter;
                _ordersViewSource.View.Refresh();

                OnPropertyChanged(nameof(OrdersView));
            }
        }
        #endregion

        #region Orders : ObservableCollection<OrdersLine> - Коллекция строк заказа

        public ObservableCollection<OrderLineDto> OrdersLines
        {
            get => _orderLines;
            set
            {
                if (!Set(ref _orderLines, value)) return;

                _ordersLinesViewSource = new CollectionViewSource
                {
                    //Настройка фильтра
                    Source = value,
                };

                _ordersLinesViewSource.View.Refresh();

                OnPropertyChanged(nameof(OrdersLinesView));
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

        #region SelectedOrder : OrderDto - Выбранный заказ
        private OrderDto _selectedOrder;
        public OrderDto SelectedOrder
        {
            get => _selectedOrder;
            set
            {
                if (value is null)
                {
                    _selectedOrder = value;
                    OrdersLines = new ObservableCollection<OrderLineDto>();
                    CachedSelectedOrder = null;
                    return;
                }
                _selectedOrder = value;
                OrdersLines = value.OrderLines?.ToObservableCollection() ?? new ObservableCollection<OrderLineDto>();
	                CachedSelectedOrder = new OrderDto()
	                {
		                Id = value.Id,
		                ClientName = value.ClientName,
		                ClientPhone = value.ClientPhone,
		                ClientId = value.ClientId,
		                CourierName = value.CourierName,
		                CourierId = value.CourierId,
		                FromAddress = value.FromAddress,
		                TargetAddress = value.TargetAddress,
		                CancelReason = value.CancelReason,
		                OrderLines = value.OrderLines,
		                OrderStatusName = value.OrderStatusName,
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
        private OrderDto _cachedSelectedOrder;
        public OrderDto CachedSelectedOrder { get => _cachedSelectedOrder; set => Set(ref _cachedSelectedOrder, value); }

        #endregion

        #region Command UpdateOrderCommand  - команда изменения данных в БД

        private ICommand _updateOrderCommand;
        public ICommand UpdateOrderCommand => _updateOrderCommand
            ??= new LambdaCommandAsync<OrderDto>(OnUpdateOrderCommandExecuted, CanUpdateOrderCommandExecute);
        //Тут бы сделать валидацию вводимых данных, но как нибудь в другой раз
        private bool CanUpdateOrderCommandExecute(OrderDto p) => (p != null || SelectedOrder != null) && SelectedOrder.OrderStatusName == "Новая";

        private async Task OnUpdateOrderCommandExecuted(OrderDto? p)
        {
            try
            {
                var orderToUpdate = p ?? CachedSelectedOrder;
                if (!_userDialogRedactorOrder.Edit(this))
                    return;
                await _unitOfWork.OrdersRepository.UpdateAsync(SelectedOrder.ToModel());
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

        #region Command RemoveOrderCommand : OrderDto - Удаление указанного заказа
        private ICommand _removeOrderCommand;

        public ICommand RemoveOrderCommand => _removeOrderCommand
            ??= new LambdaCommandAsync<OrderDto>(OnRemoveOrderCommandExecuted, CanRemoveOrderCommandExecute);

        //можно раскоментировать строку ниже для избежания попадания в эксепшен или вывести информацию об ошибке, сделать бы
        private bool CanRemoveOrderCommandExecute(OrderDto p) => (p != null || SelectedOrder != null) && SelectedOrder.OrderStatusName != "Передано на выполнение"
															  //&& SelectedOrder.OrderStatus.StatusName != "Передано на выполнение"
															  ;

        private async Task OnRemoveOrderCommandExecuted(OrderDto? p)
        {
            var orderToRemove = p ?? SelectedOrder;

            if (!_userDialogRedactorOrder.ConfirmWarning($"Вы хотите удалить заказа {orderToRemove.Id}?", "Удаление заказа"))
                return;
            if (orderToRemove.OrderStatusName == "Передано на выполнение")
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
        private ICommand _addNewOrderCommand;

        public ICommand AddNewOrderCommand => _addNewOrderCommand
            ??= new LambdaCommandAsync(OnAddNewOrderCommandExecuted, CanAddNewOrderCommandExecute);
        private bool CanAddNewOrderCommandExecute() => true;

        private async Task OnAddNewOrderCommandExecuted()
        {
            SelectedOrder = new OrderDto();
            if (!_userDialogAddOrders.Edit(this))
                return;
            SelectedOrder = await _orderService.AddOrderAsync(SelectedOrder);
            await OnLoadDataCommandExecuted();
            SelectedOrder = Orders.Find(x => x.Id == SelectedOrder.Id);

        }

        #endregion

        #region Command TakeToJobOrderCommand  - команда передачи заказа в работу

        private ICommand _takeToJobOrderCommand;
        public ICommand TakeToJobOrderCommand => _takeToJobOrderCommand
            ??= new LambdaCommandAsync<OrderDto>(OnTakeToJobOrderCommandExecuted, CanTakeToJobOrderCommandExecute);
        //Тут бы сделать валидацию вводимых данных, но как нибудь в другой раз
        private bool CanTakeToJobOrderCommandExecute(OrderDto p) => (p != null || SelectedOrder != null) && SelectedOrder.OrderStatusName == "Новая";

        private async Task OnTakeToJobOrderCommandExecuted(OrderDto? p)
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

        private ICommand _cancelOrderCommand;
        public ICommand CancelOrderCommand => _cancelOrderCommand
            ??= new LambdaCommandAsync<OrderDto>(OnCancelOrderOrderCommandExecuted, CanCancelOrderCommandExecute);
        //Тут бы сделать валидацию вводимых данных, но как нибудь в другой раз
        private bool CanCancelOrderCommandExecute(OrderDto p) => (p != null || SelectedOrder != null) && (SelectedOrder.OrderStatusName == "Новая" || SelectedOrder.OrderStatusName == "Передано на выполнение");

        private async Task OnCancelOrderOrderCommandExecuted(OrderDto? p)
        {
            try
            {
                var orderToUpdate = p ?? SelectedOrder;

                if (!_userCancelOrder.Edit(this)) return;
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

        private ICommand _completeOrderCommand;
        public ICommand CompleteOrderCommand => _completeOrderCommand
            ??= new LambdaCommandAsync<OrderDto>(OnCompleteOrderOrderCommandExecuted, CanCompleteOrderOrderCommandExecute);
        //Тут бы сделать валидацию вводимых данных, но как нибудь в другой раз
        private bool CanCompleteOrderOrderCommandExecute(OrderDto p) => (p != null || SelectedOrder != null) && SelectedOrder.OrderStatusName == "Передано на выполнение";

        private async Task OnCompleteOrderOrderCommandExecuted(OrderDto? p)
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

        public OrdersViewModel(IUnitOfWork unitOfWork, IUserDialogAddOrder userDialogAddOrder, IUserDialogRedactorOrder userDialogRedactorOrder, IUserDialogCancelOrder userCancelOrder)
        {
            _unitOfWork = unitOfWork;
            _orderService = new OrderService(_unitOfWork);
            _userDialogAddOrders = userDialogAddOrder;
            _userCancelOrder = userCancelOrder;
            _userDialogRedactorOrder = userDialogRedactorOrder;


        }

        #region Command LoadDataCommand - Команда загрузки данных из репозитория

        private ICommand _loadDataCommand;
        public ICommand LoadDataCommand => _loadDataCommand
            ??= new LambdaCommandAsync(OnLoadDataCommandExecuted, CanLoadDataCommandExecute);
        private bool CanLoadDataCommandExecute() => true;

        private async Task OnLoadDataCommandExecuted()
        {
            Orders = new ObservableCollection<OrderDto>(await _unitOfWork.OrdersRepository.Items
                .Select(x => new OrderDto(x))
                .ToArrayAsync());
            OnPropertyChanged(nameof(Orders));
        }

        #endregion

        private void OnOrdersFilter(object sender, FilterEventArgs E)
        {
            if (!(E.Item is OrderDto OrderDto) || string.IsNullOrEmpty(OrdersFilter)) return;

            //подключить все необходимые поля фильтрации
            if (!OrderDto.ClientName.ToLower().Contains(OrdersFilter.ToLower()) &&
                !OrderDto.ClientPhone.ToLower().Contains(OrdersFilter.ToLower()) &&
                !OrderDto.FromAddress.City.ToLower().Contains(OrdersFilter.ToLower()) &&
                !OrderDto.FromAddress.Street.ToLower().Contains(OrdersFilter.ToLower()) &&
                !OrderDto.FromAddress.HomeNumber.ToLower().Contains(OrdersFilter.ToLower()) &&
                !OrderDto.FromAddress.ApartmentNumber.ToLower().Contains(OrdersFilter.ToLower()) &&
                !OrderDto.TargetAddress.City.ToLower().Contains(OrdersFilter.ToLower()) &&
                !OrderDto.TargetAddress.Street.ToLower().Contains(OrdersFilter.ToLower()) &&
                !OrderDto.TargetAddress.HomeNumber.ToLower().Contains(OrdersFilter.ToLower()) &&
                !OrderDto.TargetAddress.ApartmentNumber.ToLower().Contains(OrdersFilter.ToLower()) &&
                !OrderDto.TargetDateTime.ToString().ToLower().Contains(OrdersFilter.ToLower()) &&
                !OrderDto.OrderStatusName.ToLower().Contains(OrdersFilter.ToLower())
               )
                E.Accepted = false;
        }


    }
}
