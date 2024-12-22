using Delivery.BLL.Interfaces;
using Delivery.DAL.Interfaces;
using Delivery.DAL.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using Delivery.DTOs;
using Delivery.Models;

namespace Delivery.BLL.Services
{
	public class CourierService : ICourierService
	{

		private readonly IUnitOfWork _unitOfWork;

		public CourierService(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		public async Task<CourierDto> AddCourierAsync(string fistName, string secondName, string phoneNumber)
		{

			try
			{
				
				var courier = await _unitOfWork.CouriersRepository.AddAsync(new Courier()
				{
					FirstName = fistName,
					SecondName = secondName,
					PhoneNumber = phoneNumber,
					CourierStatus = await _unitOfWork.CourierStatusesRepository.GetStatusAsync("Готов к выполнению заказа")
				});

				return new CourierDto(courier);
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
				if (!activeOrders.Any()) await _unitOfWork.CouriersRepository.RemoveAsync(id);
			}
			catch (Exception ex)
			{
				throw new Exception("Нет свободных курьеров");
			}
		}

		public async Task<CourierDto> UpdateCourierAsync(CourierDto courier)
		{
			try
			{
				var existCourierStatus = await _unitOfWork.CourierStatusesRepository.GetStatusAsync(courier.CourierStatusName);
				if (existCourierStatus.StatusName != "Выполняет заказ")
				{
					return new CourierDto(await _unitOfWork.CouriersRepository.UpdateAsync(courier.ToModel()));
				}

                throw new Exception("Курьер выполняет заказ");
            }
			catch (Exception ex)
			{
				throw new Exception("Не удалось обновить, все курьеры на доставке");
			}
		}
	}
}
