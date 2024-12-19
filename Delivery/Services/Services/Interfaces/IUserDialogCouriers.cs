using Delivery.DAL.Models;

namespace Delivery.WPF.Services.Services.Interfaces
{
    internal interface IUserDialogCouriers
    {
        bool Edit(Courier courier);

        bool ConfirmInformation(string Information, string Caption);
        bool ConfirmWarning(string Warning, string Caption);
        bool ConfirmError(string Error, string Caption);
    }
}
