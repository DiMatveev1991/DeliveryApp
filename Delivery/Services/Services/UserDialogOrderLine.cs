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
    public class UserDialogOrderLine : IUserDialogOrderLine
    {
        public bool Edit(OrderLine orderLine)
        {
            var orderLineEditorViewModel = new OrderLineEditorViewModel(orderLine);

            var orderLineEditorWindow = new OrderLineEditorWindow
            {
                DataContext = orderLineEditorViewModel
            };

            return orderLineEditorWindow.ShowDialog() == true;

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
                                                                          MessageBoxButton.OK,
                                                                          MessageBoxImage.Error)
                                                                  == MessageBoxResult.Yes;
    }
}
