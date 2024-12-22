using Delivery.WPF.ViewModels;

namespace Delivery.WPF.Services.Services.Interfaces
{
    public interface IUserDialogCancelOrder
    {
        public bool Edit(OrdersViewModel orderViewModel);
        bool ConfirmInformation(string information, string caption);
        bool ConfirmWarning(string warning, string caption);
        bool ConfirmError(string error, string caption);
    }
}
