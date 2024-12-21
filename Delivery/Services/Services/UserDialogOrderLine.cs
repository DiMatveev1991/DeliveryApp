﻿using System;
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
            //orderLine.Id = orderLineEditorViewModel.OrderLine.Id;
            //orderLine.Id = orderLineEditorViewModel.OrderLine.Id
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
