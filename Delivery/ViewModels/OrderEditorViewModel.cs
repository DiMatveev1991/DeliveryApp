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
using Microsoft.EntityFrameworkCore;

namespace Delivery.WPF.ViewModels
{
	internal class OrderEditorViewModel : ViewModel
    {
        private readonly IUserDialogOrderLine _userDialogOrderLine;
        private readonly IUnitOfWork _unitOfWork;
      //переключение представления
        private readonly bool _addingState;
        public bool WasChanged { get; set; }

        public OrderEditorViewModel(Order order, IUnitOfWork unitOfWork, IUserDialogOrderLine userDialogOrderLine, bool addingState = false)
        {
            _order = order;
            _unitOfWork = unitOfWork;
            _userDialogOrderLine = userDialogOrderLine;
            _addingState = addingState;
            if (addingState)
            {
                var currentDate = DateTime.UtcNow;
                Order.TargetDateTime = new DateTime(currentDate.Year, currentDate.Month, currentDate.Day);
            }
            Order.OrderLines ??= new List<OrderLine>();
            OrdersLines = Order.OrderLines.ToObservableCollection();
        }


        #region Orders : ObservableCollection<Client> - Коллекция клиентов

        private ObservableCollection<Client> _clients;
        private CollectionViewSource _clientsViewSource;

        public ObservableCollection<Client> Clients
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

        #region Addresses : ObservableCollection<Address> - Коллекция адресов

        private ObservableCollection<Address> _addresses;
        private CollectionViewSource _addressesViewSource;

        public ObservableCollection<Address> Addresses
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
        private ObservableCollection<OrderLine> _orderLines;
        private CollectionViewSource _ordersLinesViewSource;
        public ICollectionView OrdersLinesView => _ordersLinesViewSource?.View;

        #region Orders : ObservableCollection<OrderLine> - Коллекция строк заказа

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
                    };
                    _ordersLinesViewSource.View.Refresh();

                    OnPropertyChanged(nameof(OrdersLinesView));
                }
            }
        }
        #endregion


        private Order _order;
		public Order Order { get => _order; set => Set(ref _order, value); }
        #endregion

        #region SelectedOrderLine : OrderLine - Выбранная строка заказа
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

        #region Command UpdateOrderLineCommand  - команда изменения строки заказа в бд в БД

        private ICommand _UpdateOrderLineCommand;
        public ICommand UpdateOrderLineCommand => _UpdateOrderLineCommand
            ??= new LambdaCommandAsync<OrderLine>(OnUpdateOrderLineCommandExecuted, CanUpdateOrderLineCommandExecute);
        //Тут бы сделать валидацию вводимых данных, но как нибудь в другой раз
        private bool CanUpdateOrderLineCommandExecute(OrderLine p) => p != null || SelectedOrderLine != null && !_addingState;

        private async Task OnUpdateOrderLineCommandExecuted(OrderLine? p)
        {
            try
            {
                var OrderLineToUpdate = p ?? CachedSelectedOrderLine;
                await _unitOfWork.OrderLinesRepository.UpdateAsync(OrderLineToUpdate);
                SelectedOrderLine = null;
                SelectedOrderLine = OrderLineToUpdate;
                _changedCommitted = true;
                if (!_addingState)
                {
                    OrdersLines = new ObservableCollection<OrderLine>((await _unitOfWork.OrdersRepository.GetAsync(Order.Id)).OrderLines);
                }

                WasChanged = true;
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

        private bool CanRemoveOrderLineCommandExecute(OrderLine p) => p != null || SelectedOrderLine != null && !_addingState;

        private async Task OnRemoveOrderLineCommandExecuted(OrderLine? p)
        {
            var OrderLineToRemove = p ?? SelectedOrderLine;

            if (!_userDialogOrderLine.ConfirmWarning($"Вы хотите удалить товар {OrderLineToRemove.ItemName}?", "Удаление товара"))
                return;
            await _unitOfWork.OrderLinesRepository.RemoveAsync(OrderLineToRemove.Id);

            OrdersLines.Remove(OrderLineToRemove);

            if (ReferenceEquals(SelectedOrderLine, OrderLineToRemove))
                SelectedOrderLine = null;

            WasChanged = true;
        }
        #endregion

        #region Command AddNewOrderLineCommand - Добавление нового курьера
        private ICommand _AddNewOrderLineCommand;

        public ICommand AddNewOrderLineCommand => _AddNewOrderLineCommand
            ??= new LambdaCommandAsync(OnAddNewOrderLineCommandExecuted, CanAddNewOrderLineCommandExecute);
        private bool CanAddNewOrderLineCommandExecute() => true;
        private async Task OnAddNewOrderLineCommandExecuted()
        {
            var new_orderLine = new OrderLine();
            if (!_userDialogOrderLine.Edit(new_orderLine))
                return;
            if (!_addingState)
            {
                new_orderLine.OrderId = Order.Id;
                new_orderLine = await _unitOfWork.OrderLinesRepository.AddAsync(new_orderLine);
            }
            Order.OrderLines.Add(new_orderLine);
            OrdersLines = Order.OrderLines.ToObservableCollection();
            SelectedOrderLine = new_orderLine;
            _ordersLinesViewSource.View.Refresh();
            OnPropertyChanged(nameof(OrdersLinesView));
            WasChanged = true;
        }

		#endregion

		#region Command LoadDataCommand - Команда загрузки данных из репозитория

		private ICommand _LoadDataCommand;
		public ICommand LoadDataCommand => _LoadDataCommand
			??= new LambdaCommandAsync(OnLoadDataCommandExecuted, CanLoadDataCommandExecute);
		private bool CanLoadDataCommandExecute() => true;
		private async Task OnLoadDataCommandExecuted()
		{
            if (!_addingState)
            {
                OrdersLines = new ObservableCollection<OrderLine>((await _unitOfWork.OrdersRepository.GetAsync(Order.Id)).OrderLines);
            }
            Clients = new ObservableCollection<Client>(await _unitOfWork.ClientsRepository.Items.ToArrayAsync());
            Addresses = new ObservableCollection<Address>(await _unitOfWork.AddressRepository.Items.ToArrayAsync());
			OnPropertyChanged(nameof(OrdersLines));
		}
		#endregion
	}
}
