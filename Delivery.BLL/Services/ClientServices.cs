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
	internal class ClientServices : IClientService

	{
		private readonly IUnitOfWork _unitOfWork;

		public ClientServices(IUnitOfWork unitOfWork)
		{
			_unitOfWork=unitOfWork;
		}


		public async Task<Client> AddClient(string fistName, string secondName, string phoneNumber, Address address)
		{
			var existAddress = await _unitOfWork.AddressRepository.GetByAddress(address);
			existAddress ??= await _unitOfWork.AddressRepository.AddAsync(address);
			var client = await _unitOfWork.ClientsRepository.AddAsync(new Client() 
				{ FistName = fistName, SecondName = secondName, PhoneNumber = phoneNumber, Address = existAddress });
			return client;
		}

		public Task<Client> DeleteClient(Guid id)
		{
			throw new NotImplementedException();
		}

		public Task<Client> UpdateClient(Client client)
		{
			throw new NotImplementedException();
		}
	}
}
