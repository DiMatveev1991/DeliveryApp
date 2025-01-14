using System;
using Delivery.Models;
using MathCore.WPF.ViewModels;


namespace Delivery.WPF.ViewModels
{
    internal class ClientEditorViewModel : ViewModel
	{
		#region Name : string - Название клиента

		private string _firstName;

		public string FirstName
		{
			get => _firstName;
			set => Set(ref _firstName, value);
		}

		private string _secondName;

		public string SecondName
		{
			get => _secondName; 
			set => Set(ref _secondName, value);
		}

		private string _phoneNumber;

		public string PhoneNumber
		{
			get => _phoneNumber;
			set => Set(ref _phoneNumber, value);
		}

		private Address _address;

		public Address ClientAddress
		{
			get => _address;
			set => Set(ref _address, value);
		}
		
		private Client _сlient;

		public Client Client
		{
			get => _сlient;
			set => Set(ref _сlient, value);
		}

		#endregion

		public ClientEditorViewModel(Client client)
		{
			_сlient = client;

            client.Address ??= new Address();
            _address = client.Address;

            _firstName = Client.FirstName;
			_secondName = Client.SecondName;
			_phoneNumber = Client.PhoneNumber;
		}
	}
}
