using Delivery.WPF.ViewModels;

namespace Delivery.WPF.Services.Services.Interfaces
{
    public interface IUserDialogOrderLine
    {
        bool Edit(OrderEditorAddViewModel orderEditorViewModel);
        bool Edit(OrderEditorRedactorViewModel orderEditorViewModel);
		bool ConfirmInformation(string information, string caption);
        bool ConfirmWarning(string warning, string caption);
        bool ConfirmError(string error, string caption);
        void Close();
    }
}
