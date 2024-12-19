using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Delivery.DAL.Models;
using Delivery.WPF.Services.Services.Interfaces;
using Delivery.WPF.ViewModels;
using Delivery.WPF.Views.Windows;

namespace Delivery.WPF.Services.Services
{

		internal class UserDialogClientService : IUserDialogClients
		{
			public bool Edit(Client client)
			{
				var client_editor_model = new ClientEditorViewModel(client);

				var client_editor_window = new ClientEditorWindow
				{
					DataContext = client_editor_model
				};

				if (client_editor_window.ShowDialog() != true) return false;

				client.FirstName = client_editor_model.FirstName;
				client.SecondName = client_editor_model.SecondName;
				client.PhoneNumber = client_editor_model.PhoneNumber;
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

