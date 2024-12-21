using Delivery.DAL.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathCore.WPF.ViewModels;
using System.Net;
using Delivery.WPF.Views;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Data;
using Delivery.WPF.Services.Services.Interfaces;
using MathCore.WPF.Commands;
using System.Windows.Input;
using Delivery.DAL.Interfaces;

namespace Delivery.WPF.ViewModels
{
	internal class OrderEditorViewModel : ViewModel
    {
        private readonly IUserDialogOrderLine _userDialogOrderLine;
        private readonly IUnitOfWork _unitOfWork;

        #region  -  заказ

        private ObservableCollection<OrderLine> _orderLines;
        private CollectionViewSource _ordersLinesViewSource;
        public ICollectionView OrdersLinesView => _ordersLinesViewSource?.View;

        #region Orders : ObservableCollection<Order> - Коллекция строк заказа

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


        private Order _Order;
		public Order Order { get => _Order; set => Set(ref _Order, value); }
        #endregion

        #region SelectedOrderLine : OrderLine - Выбранный курьер
        private OrderLine _SelectedOrderLine;
        public OrderLine SelectedOrderLine
        {
            get => _SelectedOrderLine;
            set
            {
                if (value is null)
                {
                    _SelectedOrderLine = value;
                    return;
                }
                //Если изменения не были сохранены в базе, то сбрасываем на значения из кеша
                if (!_changedCommitted && !_firstSelect)
                {
                    //Set(ref _SelectedOrderLine, _cachedSelectedOrderLine);
                }
                _SelectedOrderLine = value;
                CachedSelectedOrderLine = new OrderLine()
                {
                    Id = value.Id,
                    ItemName = value.ItemName,
                    Length = value.Length,
                    Weight = value.Weight,
                    Width = value.Width,
                    OrderId = value.OrderId,
                };
                Set(ref _SelectedOrderLine, value);
                _changedCommitted = false;
                _firstSelect = false;
            }
        }

        private bool _firstSelect = true;
        private bool _changedCommitted;
        private OrderLine _cachedSelectedOrderLine;
        public OrderLine CachedSelectedOrderLine { get => _cachedSelectedOrderLine; set => Set(ref _cachedSelectedOrderLine, value); }

        #endregion

        public OrderEditorViewModel (Order order, IUnitOfWork unitOfWork, IUserDialogOrderLine userDialogOrderLine)
		{
			_Order = order;
            _unitOfWork = unitOfWork;
            _userDialogOrderLine = userDialogOrderLine;
            OrdersLines = order.OrderLines?.ToObservableCollection() ?? new ObservableCollection<OrderLine>();
        }

        #region Command UpdateOrderLineCommand  - команда измененияданных курьера в БД

        private ICommand _UpdateOrderLineCommand;
        public ICommand UpdateOrderLineCommand => _UpdateOrderLineCommand
            ??= new LambdaCommandAsync<OrderLine>(OnUpdateOrderLineCommandExecuted, CanUpdateOrderLineCommandExecute);
        //Тут бы сделать валидацию вводимых данных, но как нибудь в другой раз
        private bool CanUpdateOrderLineCommandExecute(OrderLine p) => p != null || SelectedOrderLine != null;

        private async Task OnUpdateOrderLineCommandExecuted(OrderLine? p)
        {
            try
            {
                var OrderLineToUpdate = p ?? CachedSelectedOrderLine;
                await _unitOfWork.OrderLinesRepository.UpdateAsync(OrderLineToUpdate);
                SelectedOrderLine = OrdersLines.Find(x => x.Id == OrderLineToUpdate.Id);
                _changedCommitted = true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        #endregion

        #region Command RemoveOrderLineCommand : OrderLine - Удаление указанного курьера
        private ICommand _RemoveOrderLineCommand;

        public ICommand RemoveOrderLineCommand => _RemoveOrderLineCommand
            ??= new LambdaCommandAsync<OrderLine>(OnRemoveOrderLineCommandExecuted, CanRemoveOrderLineCommandExecute);

        private bool CanRemoveOrderLineCommandExecute(OrderLine p) => p != null || SelectedOrderLine != null;

        private async Task OnRemoveOrderLineCommandExecuted(OrderLine? p)
        {
            var OrderLineToRemove = p ?? SelectedOrderLine;

            if (!_userDialogOrderLine.ConfirmWarning($"Вы хотите удалить товар {OrderLineToRemove.ItemName}?", "Удаление товара"))
                return;
            await _unitOfWork.OrderLinesRepository.RemoveAsync(OrderLineToRemove.Id);

            OrdersLines.Remove(OrderLineToRemove);

            if (ReferenceEquals(SelectedOrderLine, OrderLineToRemove))
                SelectedOrderLine = null;
        }
        #endregion

        #region Command AddNewOrderLineCommand - Добавление нового курьера
        private ICommand _AddNewOrderLineCommand;

        public ICommand AddNewOrderLineCommand => _AddNewOrderLineCommand
            ??= new LambdaCommandAsync(OnAddNewOrderLineCommandExecuted, CanAddNewOrderLineCommandExecute);
        private bool CanAddNewOrderLineCommandExecute() => true;

        /// <summary>Логика выполнения - Добавление новой книги</summary>
        private async Task OnAddNewOrderLineCommandExecuted()
        {
            var new_OrderLine = new OrderLine();
            if (!_userDialogOrderLine.Edit(new_OrderLine))
                return;
            new_OrderLine.OrderId = Order.Id;
            new_OrderLine = await _unitOfWork.OrderLinesRepository.AddAsync(new_OrderLine);
            OrdersLines.Add(new_OrderLine);
            SelectedOrderLine = new_OrderLine;

        }

        #endregion
    }
}
