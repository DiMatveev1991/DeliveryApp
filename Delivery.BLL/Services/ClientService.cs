using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Delivery.BLL.Interfaces;
using Delivery.DAL.Context;
using Delivery.DAL.Interfaces;
using Delivery.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace Delivery.BLL.Services
{
	internal class ClientService : IClientService

	{
		private readonly IUnitOfWork _unitOfWork;

		public ClientService(IUnitOfWork unitOfWork)
		{
			_unitOfWork=unitOfWork;
		}


		public async Task<Client> AddClientAsync(string fistName, string secondName, string phoneNumber, Address address)
		{
			var existAddress = await _unitOfWork.AddressRepository.GetByAddressAsync(address);
			existAddress ??= await _unitOfWork.AddressRepository.AddAsync(address);

			var client = await _unitOfWork.ClientsRepository.AddAsync(new Client() 
				{ FistName = fistName, SecondName = secondName, PhoneNumber = phoneNumber, Address = existAddress });
			return client;
		}

		public Task DeleteClientAsync(Guid id)
		{
			throw new NotImplementedException();
		}

		public Task<Client> UpdateClientAsync(Client client)
		{
			throw new NotImplementedException();
		}
	}
}
