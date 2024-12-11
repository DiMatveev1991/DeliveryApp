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

		public async Task<Courier> AddCourierAsync(string fistName, string secondName, string phoneNumber, CourierStatus courierStatus)
		{

			try
			{
				var existCourierStatus = await _unitOfWork.CourierStatusesRepository.GetStatusAsync(courierStatus);
				var client = await _unitOfWork.CouriersRepository.AddAsync(new Courier()
				{
					FistName = fistName, SecondName = secondName, PhoneNumber = phoneNumber,
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
		try
		{
			var activeOrders = await _unitOfWork.CouriersRepository.GetActiveOrdersAsync(id);
			if (activeOrders.Any()) await _unitOfWork.CouriersRepository.RemoveAsync(id);
		}
		catch (Exception ex)
		{
			throw new Exception(ex.Message);
		}
	}

		public async Task<Courier> UpdateCourierAsync(Courier courier)
		{
			try
			{
				var existCourierStatus = await _unitOfWork.CourierStatusesRepository.GetStatusAsync(courier.CourierStatus);
			if (existCourierStatus.StatusName != "Выполняет заказ")
			{
				await _unitOfWork.CouriersRepository.UpdateAsync(courier);
			}
			return courier;
			}
			catch (Exception ex)
			{
				throw new Exception(ex.Message);
			}
		}
	}
}
