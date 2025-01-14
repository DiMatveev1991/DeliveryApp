using System;
using System.Threading.Tasks;
using Delivery.DTOs;
using Delivery.Models;

namespace Delivery.BLL.Interfaces
{
    public interface IClientService
	{
		Task<ClientDto> AddClientAsync(string fistName, string secondName, string phoneNumber, AddressDto address);
		Task<ClientDto> UpdateClientAsync(ClientDto client);
		Task DeleteClientAsync(Guid id);

	}
}
