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
    internal class UserDialogOrdersService : IUserDialogOrder
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserDialogOrderLine _userDialogOrderLine;

        public UserDialogOrdersService(IUnitOfWork unitOfWork, IUserDialogOrderLine userDialogOrderLine)
        {
            _unitOfWork = unitOfWork;
            _userDialogOrderLine = userDialogOrderLine;
        }

        public bool Edit(Order order, bool addingState = false)
        {
            var orderEditorModel = new OrderEditorViewModel(order, _unitOfWork, _userDialogOrderLine, addingState)
            {
                WasChanged = false
            };

            var orderEditorWindow = new OrderEditorWindow
            {
                DataContext = orderEditorModel
            };

            return orderEditorWindow.ShowDialog() != true && orderEditorModel.WasChanged;
            //списал, не очень понял для чего это
            //order.Client = order_editor_model.Order.Client;
            //order.FromAddress = order_editor_model.Order.FromAddress;
            //order.TargetAddress = order_editor_model.Order.TargetAddress;
            //order.OrderLines = order_editor_model.Order.OrderLines;

            //return true;
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
    }
}

