﻿using Delivery.Models;
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

		bool ConfirmInformation(string information, string caption);
		bool ConfirmWarning(string warning, string caption);
		bool ConfirmError(string error, string caption);
	}
}
