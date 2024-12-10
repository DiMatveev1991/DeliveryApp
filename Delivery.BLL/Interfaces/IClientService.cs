using System;
using System.Threading.Tasks;
using Delivery.DAL.Models;

namespace Delivery.BLL.Interfaces
{
	internal interface IClientService
	{
		Task<Client> AddClient(string fistName, string secondName, string phoneNumber, Address address);
		Task<Client> UpdateClient(Client client);
		Task DeleteClient(Guid id);

	}
}
