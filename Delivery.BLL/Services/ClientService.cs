using System;
using System.Collections.Generic;
using System.Linq;
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
			try
			{
				var existAddress = await _unitOfWork.AddressRepository.GetByAddressAsync(address);
				existAddress ??= await _unitOfWork.AddressRepository.AddAsync(address);

				var client = await _unitOfWork.ClientsRepository.AddAsync(new Client()
					{ FistName = fistName, SecondName = secondName, PhoneNumber = phoneNumber, Address = existAddress });
				return client;
			}
			catch (Exception ex)
			{
				throw new Exception(ex.Message);
			}
		}

		public async Task DeleteClientAsync(Guid id)
		{
			try
			{
				var order = await _unitOfWork.OrdersRepository.Items.Where(o => o.ClientId == id).FirstOrDefaultAsync();
				var orderStatus = await _unitOfWork.OrderStatusesRepository.GetAsync((Guid)order.OrderStatusId);
				if (orderStatus.StatusName != "Передано на выполнение") { await _unitOfWork.ClientsRepository.RemoveAsync(id); }
			}
			catch (Exception ex)
			{
				throw new Exception(ex.Message);
			}
		}

		public async Task<Client> UpdateClientAsync(Client client)
		{
			try
			{
				var order = await _unitOfWork.OrdersRepository.Items.Where(o => o.Client == client).FirstOrDefaultAsync();
				var orderStatus = await _unitOfWork.OrderStatusesRepository.GetAsync((Guid)order.OrderStatusId);
				if (orderStatus.StatusName != "Передано на выполнение") { await _unitOfWork.ClientsRepository.UpdateAsync(client); }
				return client;
			}
			catch (Exception ex)
			{
				throw new Exception(ex.Message);
			}
		}
	}
}
