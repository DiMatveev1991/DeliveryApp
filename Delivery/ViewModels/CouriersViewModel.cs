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
using Microsoft.EntityFrameworkCore;

namespace Delivery.WPF.ViewModels
{
	internal class CouriersViewModel : ViewModel
	{
		private readonly ICourierService _CourierService;
		private readonly IUnitOfWork _UnitOfWork;
		private ObservableCollection<Courier> _Couriers;
		private CollectionViewSource _CouriersViewSource;
		public ICollectionView CouriersView => _CouriersViewSource.View;

		#region Couriers : ObservableCollection<Courier> - Коллекция курьеров

		public ObservableCollection<Courier> Couriers
		{
			get => _Couriers;
			set
			{
				if (Set(ref _Couriers, value))
				{
					_CouriersViewSource = new CollectionViewSource
					{
						Source = value,
						SortDescriptions =
						{
							new SortDescription(nameof(Delivery.DAL.Models.Courier.FirstName), ListSortDirection.Ascending)
						}
					};

					_CouriersViewSource.Filter += OnCouriersFilter;
					_CouriersViewSource.View.Refresh();

					OnPropertyChanged(nameof(CouriersView));
				}
			}
		}
		#endregion

		#region CouriersFilter : string - Искомое слово

		private string _CouriersFilter;
		public string CouriersFilter
		{
			get => _CouriersFilter;
			set
			{
				if (Set(ref _CouriersFilter, value))
					_CouriersViewSource.View.Refresh();
			}
		}
		#endregion
		
		
		#region SelectedCourier : Courier - Выбранный курьер
		private Courier _SelectedCourier;
		public Courier SelectedCourier { get => _SelectedCourier; set => Set(ref _SelectedCourier, value); }
		#endregion

		#region Command LoadDataCommand - Команда загрузки данных из репозитория

		private ICommand _LoadDataCommand;
		public ICommand LoadDataCommand => _LoadDataCommand
			??= new LambdaCommandAsync(OnLoadDataCommandExecuted, CanLoadDataCommandExecute);
		private bool CanLoadDataCommandExecute() => true;
		private async Task OnLoadDataCommandExecuted()
		{
			Couriers = new ObservableCollection<Courier>(await _UnitOfWork.CouriersRepository.Items.ToArrayAsync());
			OnPropertyChanged(nameof(Couriers));
		}
		#endregion

		#region Command UpdateCourierCommand  - команда измененияданных курьера в БД

		private ICommand _UpdateCourierCommand;
		public ICommand UpdateCourierCommand => _UpdateCourierCommand
			??= new LambdaCommandAsync<Courier>(OnUpdateCourierCommandExecuted, CanUpdateCourierCommandExecute);
		//Тут бы сделать валидацию вводимых данных, но как нибудь в другой раз
		private bool CanUpdateCourierCommandExecute(Courier p) => (p != null || SelectedCourier != null)&& SelectedCourier.CourierStatus.StatusName == "Готов к выполнению заказа";

		private async Task OnUpdateCourierCommandExecuted(Courier? p)
		{
			var courierToUpdate = p ?? SelectedCourier;
			await _CourierService.UpdateCourierAsync(courierToUpdate);
			if (ReferenceEquals(SelectedCourier, courierToUpdate))
				SelectedCourier = courierToUpdate;

		}
		
		#endregion

		#region Command RemoveCourierCommand : Courier - Удаление указанного курьера

		private ICommand _RemoveCourierCommand;

		public ICommand RemoveCourierCommand => _RemoveCourierCommand
			??= new LambdaCommandAsync<Courier>(OnRemoveCourierCommandExecuted, CanRemoveCourierCommandExecute);

		private bool CanRemoveCourierCommandExecute(Courier p) => (p != null || SelectedCourier != null) && SelectedCourier.CourierStatus.StatusName == "Готов к выполнению заказа";

		private async Task OnRemoveCourierCommandExecuted(Courier? p)
		{
			var courierToRemove = p ?? SelectedCourier;
			await _CourierService.DeleteCourierAsync(courierToRemove.Id);
			
			if (ReferenceEquals(SelectedCourier, courierToRemove))
				SelectedCourier = null;
			
		}

		#endregion
		#region Конструктор
		public CouriersViewModel(IUnitOfWork unitOfWork)
		{
			_CourierService = new CourierService(unitOfWork);
			_UnitOfWork = unitOfWork;
		}



		#endregion

		private void OnCouriersFilter(object Sender, FilterEventArgs E)
		{
			if (!(E.Item is Courier courier) || string.IsNullOrEmpty(CouriersFilter)) return;

			if (!courier.FirstName.Contains(CouriersFilter))
				E.Accepted = false;
		}



	}
}

