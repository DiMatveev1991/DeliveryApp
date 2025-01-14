using ConstantProperty;
using Delivery.BLL.Interfaces;
using Delivery.DAL.Interfaces;
using Delivery.DTOs;
using Delivery.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace Delivery.BLL.Services
{
    public class ClientService : IClientService
    {
        private readonly IUnitOfWork _unitOfWork;
        public OrderStatusesConst OrderStatusesConst = new OrderStatusesConst();

        public ClientService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ClientDto> AddClientAsync(string firstName, string secondName, string phoneNumber, AddressDto address)
        {
            try
            {
                var addressModel = address.ToModel();
                var existAddress = await _unitOfWork.AddressRepository.GetByAddressAsync(addressModel);
                existAddress ??= await _unitOfWork.AddressRepository.AddAsync(addressModel);

                var client = await _unitOfWork.ClientsRepository.AddAsync(new Client()
                { FirstName = firstName, SecondName = secondName, PhoneNumber = phoneNumber, AddressId = existAddress.Id });
                client.Address = existAddress;
                return new ClientDto(client);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при добавлении клиента: {ex.Message}");
                throw new Exception(ex.Message);
            }
        }

        public async Task DeleteClientAsync(Guid id)
        {
            try
            {
                var order = await _unitOfWork.OrdersRepository.Items.FirstOrDefaultAsync(o => o.ClientId == id && o.OrderStatus.StatusName == OrderStatusesConst.OrderSubmittedForExecution);
                if (order == null)
                {
                    await _unitOfWork.ClientsRepository.RemoveAsync(id);
                }
                else
                {
                    throw new Exception("Удаление не выполнено, у клиента есть активные заказы");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при удалении клиента: {ex.Message}");
                throw new Exception(ex.Message);
            }
        }

        public async Task<ClientDto> UpdateClientAsync(ClientDto client)
        {
            try
            {
                var clientToUpdate = client.ToModel();
                if (clientToUpdate.AddressId.HasValue && clientToUpdate.Address != null)
                {
                    await _unitOfWork.AddressRepository.UpdateAsync(clientToUpdate.Address);
                }

                await _unitOfWork.ClientsRepository.UpdateAsync(clientToUpdate);
                return new ClientDto(clientToUpdate);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при обновлении клиента: {ex.Message}");
                throw new Exception(ex.Message);
            }
        }
    }
}
