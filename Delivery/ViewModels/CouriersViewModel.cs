using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Delivery.BLL.Interfaces;
using Delivery.BLL.Services;
using Delivery.DAL.Interfaces;
using Delivery.DAL.Models;
using MathCore.WPF;
using MathCore.WPF.Commands;
using MathCore.WPF.ViewModels;
using static System.Reflection.Metadata.BlobBuilder;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using Delivery.DTOs;
using Delivery.Models;
using Microsoft.EntityFrameworkCore;
using Delivery.WPF.Services.Services.Interfaces;

namespace Delivery.WPF.ViewModels
{
	internal class CouriersViewModel : ViewModel
	{
        private readonly IUserDialogCouriers _userDialogCouriers;
        private ICourierService _сourierService => new CourierService(_unitOfWork);
		private readonly IUnitOfWork _unitOfWork;
		private ObservableCollection<Courier> _couriers;
		private CollectionViewSource _couriersViewSource;
		public ICollectionView CouriersView => _couriersViewSource?.View; 

		#region Couriers : ObservableCollection<Courier> - Коллекция курьеров

		public ObservableCollection<Courier> Couriers
		{
			get => _couriers;
			set
			{
				if (Set(ref _couriers, value))
				{
					_couriersViewSource = new CollectionViewSource
					{
						Source = value,
						SortDescriptions =
						{
							new SortDescription(nameof(Courier.FirstName), ListSortDirection.Ascending),
							new SortDescription(nameof(Courier.SecondName), ListSortDirection.Ascending),
							new SortDescription(nameof(Courier.PhoneNumber), ListSortDirection.Ascending),
							new SortDescription(nameof(Courier.CourierStatus.StatusName), ListSortDirection.Ascending)
						}
					};

					_couriersViewSource.Filter += OnCouriersFilter;
					_couriersViewSource.View.Refresh();

					OnPropertyChanged(nameof(CouriersView));
				}
			}
		}
		#endregion

		#region CouriersFilter : string - Искомое слово

		private string _couriersFilter;
		public string CouriersFilter
		{
			get => _couriersFilter;
			set
			{
				if (Set(ref _couriersFilter, value))
					_couriersViewSource.View.Refresh();

			}
		}
		#endregion

		#region SelectedCourier : Courier - Выбранный курьер
		private Courier _selectedCourier;
		public Courier SelectedCourier
		{
			get => _selectedCourier;
			set
            {
                if (value is null)
                {
                    _selectedCourier = value;
					return;
                }

				_selectedCourier = value;
				CachedSelectedCourier = new Courier()
				{
					Id = value.Id,
					FirstName = value.FirstName,
					SecondName = value.SecondName,
					PhoneNumber = value.PhoneNumber,
					CourierStatus = value.CourierStatus,
					CourierStatusId = value.CourierStatusId,
				};
				Set(ref _selectedCourier, value);
				_changedCommitted = false;
				_firstSelect = false;
			}
		}

		private bool _firstSelect = true;
		private bool _changedCommitted;
		private Courier _cachedSelectedCourier;
		public Courier CachedSelectedCourier { get => _cachedSelectedCourier; set => Set(ref _cachedSelectedCourier, value); }

		#endregion

		#region Command LoadDataCommand - Команда загрузки данных из репозитория

		private ICommand _loadDataCommand;
		public ICommand LoadDataCommand => _loadDataCommand
			??= new LambdaCommandAsync(OnLoadDataCommandExecuted, CanLoadDataCommandExecute);
		private bool CanLoadDataCommandExecute() => true;
		private async Task OnLoadDataCommandExecuted()
		{
			Couriers = new ObservableCollection<Courier>(await _unitOfWork.CouriersRepository.Items.ToArrayAsync());
			OnPropertyChanged(nameof(Couriers));
		}
		#endregion

		#region Command UpdateCourierCommand  - команда измененияданных курьера в БД

		private ICommand _updateCourierCommand;
		public ICommand UpdateCourierCommand => _updateCourierCommand
			??= new LambdaCommandAsync<Courier>(OnUpdateCourierCommandExecuted, CanUpdateCourierCommandExecute);
		//Тут бы сделать валидацию вводимых данных, но как нибудь в другой раз
		private bool CanUpdateCourierCommandExecute(Courier p) => (p != null || SelectedCourier != null)&& SelectedCourier.CourierStatus.StatusName == "Готов к выполнению заказа";

		private async Task OnUpdateCourierCommandExecuted(Courier? p)
		{
			try
			{
				var courierToUpdate = p ?? CachedSelectedCourier;
				await _сourierService.UpdateCourierAsync(new CourierDto(courierToUpdate));
				SelectedCourier = Couriers.Find(x => x.Id == courierToUpdate.Id);
				await OnLoadDataCommandExecuted();
				_changedCommitted = true;
			}
			catch (Exception ex)
			{
				throw new Exception(ex.Message);
			}
		}

		#endregion

		#region Command RemoveCourierCommand : Courier - Удаление указанного курьера
		private ICommand _removeCourierCommand;

		public ICommand RemoveCourierCommand => _removeCourierCommand
			??= new LambdaCommandAsync<Courier>(OnRemoveCourierCommandExecuted, CanRemoveCourierCommandExecute);

		private bool CanRemoveCourierCommandExecute(Courier p) => (p != null || SelectedCourier != null) && SelectedCourier.CourierStatus.StatusName == "Готов к выполнению заказа";

		private async Task OnRemoveCourierCommandExecuted(Courier? p)
		{
			var courierToRemove = p ?? SelectedCourier;
           
			if (!_userDialogCouriers.ConfirmWarning($"Вы хотите удалить курьера {courierToRemove.FirstName}?", "Удаление курьера"))
                return;
            await _сourierService.DeleteCourierAsync(courierToRemove.Id);

			_couriers.Remove(courierToRemove);

			if (ReferenceEquals(SelectedCourier, courierToRemove))
				SelectedCourier = null;
		}
        #endregion

        #region Command AddNewCourierCommand - Добавление нового курьера
        private ICommand _addNewCourierCommand;

        public ICommand AddNewCourierCommand => _addNewCourierCommand
            ??= new LambdaCommandAsync(OnAddNewCourierCommandExecuted, CanAddNewCourierCommandExecute);
        private bool CanAddNewCourierCommandExecute() => true;
        private async Task OnAddNewCourierCommandExecuted()
		{
			var newCourier = new Courier();
			if (!_userDialogCouriers.Edit(newCourier))
                return;
            newCourier = (await _сourierService.AddCourierAsync(newCourier.FirstName, newCourier.SecondName, newCourier.PhoneNumber)).ToModel();
			await OnLoadDataCommandExecuted();
                     SelectedCourier = newCourier;

        }

        #endregion

        #region Конструктор
        public CouriersViewModel(IUnitOfWork unitOfWork, IUserDialogCouriers userDialogCouriers)
		{
			_unitOfWork = unitOfWork;
			_userDialogCouriers = userDialogCouriers;

        }

		#endregion

		private void OnCouriersFilter(object sender, FilterEventArgs E)
		{
			if (!(E.Item is Courier courier) || string.IsNullOrEmpty(CouriersFilter)) return;

			if (!courier.PhoneNumber.Contains(CouriersFilter)&& 
			    !courier.FirstName.Contains(CouriersFilter) && 
			    !courier.SecondName.Contains(CouriersFilter) &&
			    !courier.CourierStatus.StatusName.Contains(CouriersFilter)
				)
				E.Accepted = false;
		}



	}
}

