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

		public CourierServices(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		public async Task<Courier> AddCourier(string fistName, string secondName, string phoneNumber,
			CourierType courierType, CourierStatus courierStatus)
		{

			try
			{
				var existCourierType = await _unitOfWork.CourierTypesRepository.GetByCourierType(courierType);
				existCourierType ??= await _unitOfWork.CourierTypesRepository.AddAsync(courierType);

				var existCourierStatus = await _unitOfWork.CourierStatusesRepository.GetByCourierStatus(courierStatus);

				var client = await _unitOfWork.CouriersRepository.AddAsync(new Courier()
				{
					FistName = fistName, SecondName = secondName, PhoneNumber = phoneNumber,
					CourierType = await _unitOfWork.CourierTypesRepository.AddAsync(existCourierType),
					CourierStatus = await _unitOfWork.CourierStatusesRepository.AddAsync(existCourierStatus)
				});
				
				return client;
			}
			
			catch (ArgumentException ex)
			{
				throw  new ArgumentException("В статус курьера пришел не обрабатываемый тип");
			}
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
