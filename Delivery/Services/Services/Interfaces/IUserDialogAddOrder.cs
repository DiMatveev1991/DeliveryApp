using Delivery.WPF.ViewModels;

namespace Delivery.WPF.Services.Services.Interfaces;

public interface IUserDialogAddOrder
{
    bool Edit(OrdersViewModel orderViewModel);

    bool ConfirmInformation(string information, string caption);
    bool ConfirmWarning(string warning, string caption);
    bool ConfirmError(string error, string caption);
    void Close();
}