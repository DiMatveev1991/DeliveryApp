using Delivery.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delivery.WPF.Services.Services.Interfaces
{
	public interface IUserDialogClients
	{
		bool Edit(Client client);

		bool ConfirmInformation(string Information, string Caption);
		bool ConfirmWarning(string Warning, string Caption);
		bool ConfirmError(string Error, string Caption);
	}
}
