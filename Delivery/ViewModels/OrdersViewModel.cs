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
using Delivery.WPF.Views;
using MathCore.WPF.Commands;
using MathCore.WPF.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace Delivery.WPF.ViewModels
{
	internal class OrdersViewModel: ViewModel
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IOrderService _orderService;
        private ObservableCollection<Order> _orders;
        private CollectionViewSource _ordersViewSource;
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
                        {
                            //new SortDescription(nameof(Delivery.DAL.Models.Courier.FirstName), ListSortDirection.Ascending),
                            //new SortDescription(nameof(Delivery.DAL.Models.Courier.SecondName), ListSortDirection.Ascending),
                            //new SortDescription(nameof(Delivery.DAL.Models.Courier.PhoneNumber), ListSortDirection.Ascending),
                            //new SortDescription(nameof(Delivery.DAL.Models.Courier.CourierStatus.StatusName), ListSortDirection.Ascending)
                        }
                    };

                    //_ordersViewSource.Filter += OnOrdersFilter;
                    _ordersViewSource.View.Refresh();

                    OnPropertyChanged(nameof(OrdersView));
                }
            }
        }
        #endregion

        #region Orders : ObservableCollection<Courier> - Коллекция курьеров

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
                            //new SortDescription(nameof(Delivery.DAL.Models.Courier.FirstName), ListSortDirection.Ascending),
                            //new SortDescription(nameof(Delivery.DAL.Models.Courier.SecondName), ListSortDirection.Ascending),
                            //new SortDescription(nameof(Delivery.DAL.Models.Courier.PhoneNumber), ListSortDirection.Ascending),
                            //new SortDescription(nameof(Delivery.DAL.Models.Courier.CourierStatus.StatusName), ListSortDirection.Ascending)
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

        #region SelectedOrder : Courier - Выбранный курьер
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
                    //Set(ref _selectedOrder, _cachedSelectedCourier);
                }
                _selectedOrder = value;
                OrdersLines = value.OrderLines.ToObservableCollection();
                CachedSelectedCourier = new Order()
                {
                    Id = value.Id,
                    //FirstName = value.FirstName,
                    //SecondName = value.SecondName,
                    //PhoneNumber = value.PhoneNumber,
                    //CourierStatus = value.CourierStatus,
                    //CourierStatusId = value.CourierStatusId,
                };
                Set(ref _selectedOrder, value);
                _changedCommitted = false;
                _firstSelect = false;
            }
        }

        private bool _firstSelect = true;
        private bool _changedCommitted;
        private Order _cachedSelectedCourier;
        public Order CachedSelectedCourier { get => _cachedSelectedCourier; set => Set(ref _cachedSelectedCourier, value); }

        #endregion

        public OrdersViewModel(IUnitOfWork unitOfWork)
		{
	      _unitOfWork = unitOfWork;
          _orderService = new OrderService(_unitOfWork);

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
            //    !order.CourierStatus.StatusName.Contains(OrdersFilter)
            //   )
            //    E.Accepted = false;
        }


    }
}
