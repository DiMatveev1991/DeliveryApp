using Delivery.BLL.Interfaces;
using Delivery.DAL.Interfaces;
using Delivery.DAL.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using Delivery.DTOs;
using Delivery.Models;
using ConstantProperty;


namespace Delivery.BLL.Services
{
	public class CourierService : ICourierService
	{
		public CourierStatusesConstant CourierStatusesConstant = new CourierStatusesConstant();

		private readonly IUnitOfWork _unitOfWork;

		public CourierService(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		public async Task<CourierDto> AddCourierAsync(string firstName, string secondName, string phoneNumber)
		{
			try
			{
				var courier = await _unitOfWork.CouriersRepository.AddAsync(new Courier()
				{
					FirstName = firstName,
					SecondName = secondName,
					PhoneNumber = phoneNumber,
					CourierStatus = await _unitOfWork.CourierStatusesRepository.GetStatusAsync(CourierStatusesConstant.ReadyToJobStatus)
				});

				return new CourierDto(courier);
			}
			catch (Exception e)
			{
				throw new Exception($"Произошла непредвиденная ошибка при добавлении курьера- {e.Message}", e);
			}
		}


		public async Task DeleteCourierAsync(Guid id)
		{
			try
			{
				var activeOrders = await _unitOfWork.CouriersRepository.GetActiveOrdersAsync(id);
				if (!activeOrders.Any())
				{
					await _unitOfWork.CouriersRepository.RemoveAsync(id);
				}
				else
				{
					throw new Exception("У данного курьера есть активные заказы, удаление невозможно");
				}
			}
			catch (Exception e)
			{
				throw new Exception($"Произошла непредвиденная ошибка при удалении курьер а- {e.Message}", e);
			}
		}

		public async Task<CourierDto> UpdateCourierAsync(CourierDto courier)
		{
			try
			{
				var existCourierStatus = await _unitOfWork.CourierStatusesRepository.GetStatusAsync(courier.CourierStatusName);
				if (existCourierStatus.StatusName == CourierStatusesConstant.OnJobStatus)
				{
					throw new Exception("Невозможно обновить данные курьера, так как он выполняет заказ");
				}

				return new CourierDto(await _unitOfWork.CouriersRepository.UpdateAsync(courier.ToModel()));
			}
			catch (Exception e)
			{
				throw new Exception($"Произошла непредвиденная ошибка при обновлении данных курьера- {e.Message}", e);
			}
		}
	}
}