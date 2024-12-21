using Delivery.DAL.Models;

namespace Delivery.WPF.Services.Services.Interfaces;

public interface IUserDialogOrder
{
	bool Edit(Order order);

	bool ConfirmInformation(string information, string caption);
	bool ConfirmWarning(string warning, string caption);
	bool ConfirmError(string error, string caption);
}