using Delivery.DAL.Models;
using System.Threading.Tasks;
using System;

namespace Delivery.BLL.Interfaces
{
	internal interface ICourierService
	{
		Task<Courier> AddCourier(string fistName, string secondName, string phoneNumber, CourierType courierType, CourierStatus courierStatus);
		Task<Courier> UpdateCourier(Courier courier);
		Task<Courier> DeleteCourier(Guid id);
		//назначить заказ курьеру
		Task TakeOrderInProgress(Guid orderId, Guid courierId);
	}
}
