using Delivery.DAL.Models;
using System.Threading.Tasks;
using System;
using Delivery.DTOs;
using Delivery.Models;

namespace Delivery.BLL.Interfaces
{
	public interface ICourierService
	{
		Task<CourierDto> AddCourierAsync(string fistName, string secondName, string phoneNumber);
		Task<CourierDto> UpdateCourierAsync(CourierDto courier);
		Task DeleteCourierAsync(Guid id);
		
	
	}
}
