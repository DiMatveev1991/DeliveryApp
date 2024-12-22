using Delivery.DAL.Models;
using Delivery.Models;

namespace Delivery.WPF.Services.Services.Interfaces
{
    internal interface IUserDialogCouriers
    {
        bool Edit(Courier courier);

        bool ConfirmInformation(string information, string caption);
        bool ConfirmWarning(string warning, string caption);
        bool ConfirmError(string error, string caption);
    }
}
