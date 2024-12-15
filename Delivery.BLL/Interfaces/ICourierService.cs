using Delivery.DAL.Models;
using System.Threading.Tasks;
using System;

namespace Delivery.BLL.Interfaces
{
	public interface ICourierService
	{
		Task<Courier> AddCourierAsync(string fistName, string secondName, string phoneNumber);
		Task<Courier> UpdateCourierAsync(Courier courier);
		Task DeleteCourierAsync(Guid id);
		
	
	}
}
