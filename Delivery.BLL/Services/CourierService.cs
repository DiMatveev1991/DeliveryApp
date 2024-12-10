using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Delivery.BLL.Interfaces;
using Delivery.DAL.Interfaces;
using Delivery.DAL.Models;

namespace Delivery.BLL.Services
{
	internal class CourierService : ICourierService
	{

		private readonly IUnitOfWork _unitOfWork;

		public CourierService(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		public async Task<Courier> AddCourierAsync(string fistName, string secondName, string phoneNumber,
			CourierType courierType, CourierStatus courierStatus)
		{

			try
			{
				var existCourierType = await _unitOfWork.CourierTypesRepository.GetTypeAsync(courierType);
				existCourierType ??= await _unitOfWork.CourierTypesRepository.AddAsync(courierType);

				var existCourierStatus = await _unitOfWork.CourierStatusesRepository.GetStatusAsync(courierStatus);

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
	

	public async Task DeleteCourierAsync(Guid id)
	{
		var  activeOrders = await _unitOfWork.CouriersRepository.GetActiveOrdersAsync(id);
		if (activeOrders.Any()) await _unitOfWork.CouriersRepository.RemoveAsync(id);
	}

		public Task<Courier> UpdateCourierAsync(Courier courier)
		{
			throw new NotImplementedException();
		}
	}
}
