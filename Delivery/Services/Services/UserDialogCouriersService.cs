using System.Windows;
using Delivery.WPF.Views.Windows;
using Delivery.DAL.Models;
using Delivery.WPF.Services.Services.Interfaces;
using Delivery.WPF.ViewModels;


namespace Delivery.WPF.Services.Services
{
    internal class UserDialogCouriersService : IUserDialogCouriers
    {
        public bool Edit(Courier courier)
        {
            var courierEditorModel = new CourierEditorViewModel(courier);

            var courierEditorWindow = new CourierEditorWindow
            {
                DataContext = courierEditorModel
            };

            if (courierEditorWindow.ShowDialog() != true) return false;

            courier.FirstName = courierEditorModel.FirstName;
            courier.SecondName = courierEditorModel.SecondName;
            courier.PhoneNumber = courierEditorModel.PhoneNumber;
            return true;
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
