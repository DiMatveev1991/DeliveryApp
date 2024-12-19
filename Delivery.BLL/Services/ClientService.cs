using System;
using System.Collections.Generic;
using System.Linq;
using Delivery.BLL.Interfaces;
using Delivery.DAL.Interfaces;
using Delivery.DAL.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Delivery.BLL.Services
{
	public class ClientService : IClientService

	{
		private readonly IUnitOfWork _unitOfWork;

		public ClientService(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}


		public async Task<Client> AddClientAsync(string fistName, string secondName, string phoneNumber, Address address)
		{
			try
			{
				var existAddress = await _unitOfWork.AddressRepository.GetByAddressAsync(address);
				existAddress ??= await _unitOfWork.AddressRepository.AddAsync(address);

				var client = await _unitOfWork.ClientsRepository.AddAsync(new Client()
				{ FirstName = fistName, SecondName = secondName, PhoneNumber = phoneNumber, Address = existAddress });
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
				var order = await _unitOfWork.OrdersRepository.Items.FirstOrDefaultAsync(o => o.ClientId == id && o.OrderStatus.StatusName == "Передано на выполнение");
				if (order == null) { await _unitOfWork.ClientsRepository.RemoveAsync(id); }
				else
				{
					throw new Exception("Удаление не выполнен, у клиента есть активные заказы");
				}
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
				var order = await _unitOfWork.OrdersRepository.Items.FirstOrDefaultAsync(o =>
					o.ClientId == client.Id && o.OrderStatus.StatusName == "Передано на выполнение");
				if (order == null)
				{
					await _unitOfWork.ClientsRepository.UpdateAsync(client);
					return client;
				}
				else
				{
					throw new Exception("Удаление не выполнен, у клиента есть активные заказы");
				}
			}
			catch (Exception ex)
			{
				throw new Exception(ex.Message);
			}
		}

	}
}