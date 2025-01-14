using System.Windows;
using Delivery.BLL.Interfaces;
using Delivery.DAL.Interfaces;
using Delivery.WPF.Services.Services.Interfaces;
using Delivery.WPF.ViewModels;
using Delivery.WPF.Views.Windows;

namespace Delivery.WPF.Services.Services
{
    internal class UserDialogAddOrdersService : IUserDialogAddOrder
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserDialogOrderLine _userDialogOrderLine;
        private readonly IOrderService _orderService;
        private OrderEditorAddWindow _orderEditorAddWindow;

        public UserDialogAddOrdersService(IUnitOfWork unitOfWork, IUserDialogOrderLine userDialogOrderLine, IOrderService orderService)
        {
            _unitOfWork = unitOfWork;
            _userDialogOrderLine = userDialogOrderLine;
            _orderService = orderService;
        }

        public bool Edit(OrdersViewModel orderViewModel)
        {
            var orderEditorModel = new OrderEditorAddViewModel(orderViewModel.SelectedOrder, _unitOfWork, _userDialogOrderLine, _orderService, this)
            {
                WasChanged = false
            };

            _orderEditorAddWindow = new OrderEditorAddWindow
            {
                DataContext = orderEditorModel
            };
            return _orderEditorAddWindow.ShowDialog() != true && orderEditorModel.WasChanged;
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
            _orderEditorAddWindow.Close();
        }
    }
}

