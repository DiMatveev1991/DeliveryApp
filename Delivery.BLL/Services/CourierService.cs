using Delivery.BLL.Interfaces;
using Delivery.DAL.Interfaces;
using Delivery.DAL.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Delivery.BLL.Services
{
	internal class CourierService : ICourierService
	{

		private readonly IUnitOfWork _unitOfWork;

		public CourierService(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		//TODO Не надо передавать статус заказа, надо сразу искать статус "Готов к работе" и ставить его
		public async Task<Courier> AddCourierAsync(string fistName, string secondName, string phoneNumber, CourierStatus courierStatus)
		{

			try
			{
				
				var courier = await _unitOfWork.CouriersRepository.AddAsync(new Courier()
				{
					FistName = fistName,
					SecondName = secondName,
					PhoneNumber = phoneNumber,
					CourierStatus = await _unitOfWork.CourierStatusesRepository.GetStatusAsync("Готов к выполнению заказа")
				});

				return courier;
			}

			catch (ArgumentException)
			{
				throw new ArgumentException("В статус курьера пришел не обрабатываемый тип");
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
				throw new Exception("Нет свободных курьеров");
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
				throw new Exception("Не удалось обновить, все курьеры на доставке");
			}
		}
	}
}
