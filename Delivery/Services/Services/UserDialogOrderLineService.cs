using Delivery.WPF.Services.Services.Interfaces;
using Delivery.WPF.ViewModels;
using Delivery.WPF.Views.Windows;
using System.Windows;

namespace Delivery.WPF.Services.Services
{
    internal class UserDialogOrderLineService : IUserDialogOrderLine
    {
        private OrderLineEditorWindow _orderLineEditorWindow;

        public bool Edit(OrderEditorAddViewModel orderEditorViewModel)
        {
            var orderLineEditorViewModel = new OrderLineEditorViewModel(orderEditorViewModel.SelectedOrderLine);

            _orderLineEditorWindow = new OrderLineEditorWindow
            {
                DataContext = orderLineEditorViewModel
            };

            return _orderLineEditorWindow.ShowDialog() == true;

        }
        public bool Edit(OrderEditorRedactorViewModel orderEditorViewModel)
        {
	        var orderLineEditorViewModel = new OrderLineEditorViewModel(orderEditorViewModel.SelectedOrderLine);

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

        public void Close()
        {
            _orderLineEditorWindow.Close();
        }
    }
}
