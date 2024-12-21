using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Delivery.DAL.Interfaces;
using Delivery.DAL.Models;
using Delivery.WPF.ViewModels;
using Delivery.WPF.Views.Windows;

namespace Delivery.WPF.Services.Services.Interfaces
{
	internal class UserDialogOrdersService: IUserDialogOrder
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IUserDialogOrderLine _userDialogOrderLine;

        public UserDialogOrdersService(IUnitOfWork unitOfWork, IUserDialogOrderLine userDialogOrderLine)
        {
            _unitOfWork = unitOfWork;
            _userDialogOrderLine = userDialogOrderLine;
        }

			public bool Edit(Order order)
			{
				var order_editor_model = new OrderEditorViewModel(order, _unitOfWork, _userDialogOrderLine);

				var order_editor_window = new OrderEditorWindow
				{
					DataContext = order_editor_model
				};

				if (order_editor_window.ShowDialog() != true) return true;

				order.Client = order_editor_model.Order.Client;
				order.FromAddress = order_editor_model.Order.FromAddress;
				order.TargetAddress = order_editor_model.Order.TargetAddress;
				order.OrderLines = order_editor_model.Order.OrderLines;

				return true;
			}

			public bool ConfirmInformation(string Information, string Caption) => MessageBox
					.Show(
						Information, Caption,
						MessageBoxButton.YesNo,
						MessageBoxImage.Information)
				== MessageBoxResult.Yes;

			public bool ConfirmWarning(string Warning, string Caption) => MessageBox
				                                                              .Show(
					                                                              Warning, Caption,
					                                                              MessageBoxButton.YesNo,
					                                                              MessageBoxImage.Warning)
			                                                              == MessageBoxResult.Yes;

			public bool ConfirmError(string Error, string Caption) => MessageBox
				                                                          .Show(
					                                                          Error, Caption,
					                                                          MessageBoxButton.YesNo,
					                                                          MessageBoxImage.Error)
			                                                          == MessageBoxResult.Yes;
		}
}

