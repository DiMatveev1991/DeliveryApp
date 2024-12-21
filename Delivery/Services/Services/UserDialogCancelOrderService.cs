using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Delivery.DAL.Models;
using Delivery.WPF.Services.Services.Interfaces;
using Delivery.WPF.ViewModels;
using Delivery.WPF.Views.Windows;

namespace Delivery.WPF.Services.Services
{
	internal class UserDialogCancelOrderService: IUserDialogCancelOrder
	{
		public bool Edit(Order order)
		{
			var order_editor_model = new OrderEditorCancelViewModel(order);

			var order_editor_window = new OrderCancelEditorWindow
			{
				DataContext = order_editor_model
			};

			if (order_editor_window.ShowDialog() != true) return false;

		//	order.OrderStatus= order_editor_model.OrderStatus;
			//order.CancelReason = order_editor_model.CancelReason;
		//	order.Client = order_editor_model.Client;
		//	order.FromAddress = order_editor_model.FromAddress;
		//	order.TargetAddress = order_editor_model.TargetAddress;
		//	order.OrderLines = order_editor_model.OrderLines;
		//	order.TargetDateTime = order_editor_model.TargetDateTime;
		//	order.Courier = order_editor_model.Courier;
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

