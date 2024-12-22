using System;
using Delivery.DAL.Models;
using MathCore.WPF.ViewModels;


namespace Delivery.WPF.ViewModels
{
	internal class ClientEditorViewModel : ViewModel
	{
		#region Name : string - Название клиента

		private string _firstName;
		public string FirstName { get => _firstName; set => Set(ref _firstName, value); }

		private string _secondName;
		public string SecondName { get => _secondName; set => Set(ref _secondName, value); }

		private string _phoneNumber;
		public string PhoneNumber { get => _phoneNumber; set => Set(ref _phoneNumber, value); }

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

            _firstName = Client.FirstName;
			_secondName = Client.SecondName;
			_phoneNumber = Client.PhoneNumber;
		}
	}
}
