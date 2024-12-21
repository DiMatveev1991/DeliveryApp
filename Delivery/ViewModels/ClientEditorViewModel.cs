using System;
using Delivery.DAL.Models;
using MathCore.WPF.ViewModels;


namespace Delivery.WPF.ViewModels
{
	internal class ClientEditorViewModel : ViewModel
	{
		#region Name : string - Название клиента

		private string _FirstName;
		public string FirstName { get => _FirstName; set => Set(ref _FirstName, value); }

		private string _SecondName;
		public string SecondName { get => _SecondName; set => Set(ref _SecondName, value); }

		private string _PhoneNumber;
		public string PhoneNumber { get => _PhoneNumber; set => Set(ref _PhoneNumber, value); }

		private Address _Address;
		public Address ClientAddress { get => _Address; set => Set(ref _Address, value); }
		
		private Client _Client;
		public Client Client { get => _Client; set => Set(ref _Client, value); }

		#endregion

		public ClientEditorViewModel(Client client)
		{
			Client = client;

            client.Address ??= new Address();
            ClientAddress = client.Address;

            _FirstName = Client.FirstName;
			_SecondName = Client.SecondName;
			_PhoneNumber = Client.PhoneNumber;
		}
	}
}
