using System;
using System.Collections.Generic;
using System.Linq;
using Delivery.BLL.Interfaces;
using Delivery.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Delivery.Models;
using Delivery.DTOs;

namespace Delivery.BLL.Services
{
    public class ClientService : IClientService

	{
		private readonly IUnitOfWork _unitOfWork;

		public ClientService(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}


		public async Task<ClientDto> AddClientAsync(string fistName, string secondName, string phoneNumber, AddressDto address)
		{
			try
			{
				var addressModel = address.ToModel();
				var existAddress = await _unitOfWork.AddressRepository.GetByAddressAsync(addressModel);
				existAddress ??= await _unitOfWork.AddressRepository.AddAsync(addressModel);

				var client = await _unitOfWork.ClientsRepository.AddAsync(new Client()
				{ FirstName = fistName, SecondName = secondName, PhoneNumber = phoneNumber, AddressId = existAddress.Id });
                client.Address = existAddress;
				return new ClientDto(client);
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

		public async Task<ClientDto> UpdateClientAsync(ClientDto client)
        {
            var clientToUpdate = client.ToModel();
			try
			{
				if(clientToUpdate.Address is null)
					throw new ArgumentNullException(nameof(clientToUpdate.Address), "У клиента не указан адрес");

				var order = await _unitOfWork.OrdersRepository.Items.AsNoTracking().FirstOrDefaultAsync(o =>
					o.ClientId == clientToUpdate.Id && o.OrderStatus.StatusName == "Передано на выполнение");
				if (order == null)
				{
					await _unitOfWork.ClientsRepository.UpdateAsync(clientToUpdate);
                    if (clientToUpdate.AddressId.HasValue && clientToUpdate.Address != null)
                    {
                        await _unitOfWork.AddressRepository.UpdateAsync(clientToUpdate.Address);
                    }
					return new ClientDto(clientToUpdate);
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