using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Delivery.Models;
using Delivery.WPF.Services.Services.Interfaces;
using Delivery.WPF.ViewModels;
using Delivery.WPF.Views.Windows;

namespace Delivery.WPF.Services.Services
{

    internal class UserDialogClientService : IUserDialogClients
		{
			public bool Edit(Client client)
			{
				var clientEditorModel = new ClientEditorViewModel(client);

				var clientEditorWindow = new ClientEditorWindow
				{
					DataContext = clientEditorModel
				};

				if (clientEditorWindow.ShowDialog() != true) return false;

				client.FirstName = clientEditorModel.FirstName;
				client.SecondName = clientEditorModel.SecondName;
				client.PhoneNumber = clientEditorModel.PhoneNumber;
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

