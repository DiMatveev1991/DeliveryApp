using System;
using System.Threading.Tasks;
using Delivery.DAL.Models;

namespace Delivery.BLL.Interfaces
{
	internal interface IClientService
	{
		Task<Client> AddClientAsync(string fistName, string secondName, string phoneNumber, Address address);
		Task<Client> UpdateClientAsync(Client client);
		Task DeleteClientAsync(Guid id);

	}
}
