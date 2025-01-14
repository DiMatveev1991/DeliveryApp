using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Delivery.BLL.Interfaces;
using Delivery.DAL.Interfaces;
using System.Windows;
using Delivery.WPF.Services.Services.Interfaces;
using Delivery.WPF.ViewModels;
using Delivery.WPF.Views.Windows;

namespace Delivery.WPF.Services.Services
{
	internal class UserDialogRedactorOrderService: IUserDialogRedactorOrder
		{
			private readonly IUnitOfWork _unitOfWork;
			private readonly IUserDialogOrderLine _userDialogOrderLine;
			private readonly IOrderService _orderService;
			private OrderEditorRedactorWindow _window;

            public UserDialogRedactorOrderService (IUnitOfWork unitOfWork, IUserDialogOrderLine userDialogOrderLine, IOrderService orderService)
			{
				_unitOfWork = unitOfWork;
				_userDialogOrderLine = userDialogOrderLine;
				_orderService = orderService;
			}

			public bool Edit(OrdersViewModel orderViewModel, bool addingState = false)
			{
				var orderEditorModel = new OrderEditorRedactorViewModel(orderViewModel.SelectedOrder, _unitOfWork, _userDialogOrderLine, _orderService, this)
				{
					WasChanged = false
				};

                _window = new OrderEditorRedactorWindow
				{
					DataContext = orderEditorModel
				};

				return _window.ShowDialog() != true && orderEditorModel.WasChanged;
			}

			public bool ConfirmInformation(string information, string caption) => MessageBox
					.Show(
						information, caption,
						MessageBoxButton.YesNo,
						MessageBoxImage.Information)
				== MessageBoxResult.Yes;

			public bool ConfirmWarning(string warning, string caption) => MessageBox
																			  .Show(
																				  warning, caption,
																				  MessageBoxButton.YesNo,
																				  MessageBoxImage.Warning)
																		  == MessageBoxResult.Yes;

			public bool ConfirmError(string error, string caption) => MessageBox
																		  .Show(
																			  error, caption,
																			  MessageBoxButton.YesNo,
																			  MessageBoxImage.Error)
																	  == MessageBoxResult.Yes;

            public void Close()
            {
                _window.Close();
            }
        }
}

