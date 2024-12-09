using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Delivery.BLL.Interfaces;
using Delivery.DAL.Interfaces;
using Delivery.DAL.Models;

namespace Delivery.BLL.Services
{
	internal class CourierServices : ICourierService
	{

		private readonly IUnitOfWork _unitOfWork;

		public CourierServices (IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}
		public async Task<Courier> AddCourier(string fistName, string secondName, string phoneNumber, CourierType courierType)
		{
			var existCourierType = await _unitOfWork.CourierTypesRepository.GetByCourierType(courierType);
			existCourierType ??= await _unitOfWork.CourierTypesRepository.AddAsync(courierType);
			//проверить на совпадения статуса курьера в базе
			var client = await _unitOfWork.CouriersRepository.AddAsync(new Courier() 
			        { FistName = fistName, SecondName = secondName, PhoneNumber = phoneNumber, 
				      CourierType = await _unitOfWork.CourierTypesRepository.AddAsync(courierType), 
					  CourierStatus = await _unitOfWork.CourierStatusesRepository.AddAsync(new CourierStatus(){StatusName = "Готов выполнять заказы" })});
			return client;
		}

		public Task<Courier> DeleteCourier(Guid id)
		{
			throw new NotImplementedException();
		}

		public Task TakeOrderInProgress(Guid orderId, Guid courierId)
		{
			throw new NotImplementedException();
		}

		public Task<Courier> UpdateCourier(Courier courier)
		{
			throw new NotImplementedException();
		}
	}
}
