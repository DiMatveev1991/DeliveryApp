using System.Windows;
using Delivery.WPF.Views.Windows;
using Delivery.DAL.Models;
using Delivery.WPF.Services.Services.Interfaces;
using Delivery.WPF.ViewModels;


namespace Delivery.WPF.Services.Services
{
    internal class UserDialogService : IUserDialog
    {
        public bool Edit(Courier courier)
        {
            var courier_editor_model = new CourierEditorViewModel(courier);

            var courier_editor_window = new CourierEditorWindow
            {
                DataContext = courier_editor_model
            };

            if (courier_editor_window.ShowDialog() != true) return false;

            courier.FirstName = courier_editor_model.FirstName;
            courier.SecondName = courier_editor_model.SecondName;
            courier.PhoneNumber = courier_editor_model.PhoneNumber;
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
