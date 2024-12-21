using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using Delivery.BLL.Interfaces;
using Delivery.BLL.Services;
using Delivery.DAL.Interfaces;
using Delivery.DAL.Models;
using Delivery.WPF.Services.Services.Interfaces;
using MathCore.WPF.Commands;
using MathCore.WPF.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace Delivery.WPF.ViewModels
{
    internal class ClientsViewModel : ViewModel
    {
        private readonly IUserDialogClients _UserDialogClients;
        private readonly IUnitOfWork _UnitOfWork;
        private IClientService _ClientService => new ClientService(_UnitOfWork);
        private ObservableCollection<Client> _Clients;
        private CollectionViewSource _ClientsViewSource;
        public ICollectionView ClientsView => _ClientsViewSource?.View;

        #region Clients : ObservableCollection<Client> - Коллекция курьеров

        public ObservableCollection<Client> Clients
        {
            get => _Clients;
            set
            {
                if (Set(ref _Clients, value))
                {
                    _ClientsViewSource = new CollectionViewSource
                    {
                        Source = value,
                        SortDescriptions =
                        {
	                        new SortDescription(nameof(Delivery.DAL.Models.Client.FirstName), ListSortDirection.Ascending),
                            new SortDescription(nameof(Delivery.DAL.Models.Client.SecondName), ListSortDirection.Ascending),
                            new SortDescription(nameof(Delivery.DAL.Models.Client.PhoneNumber), ListSortDirection.Ascending),
                            new SortDescription(nameof(Delivery.DAL.Models.Client.Address.City), ListSortDirection.Ascending),
                            new SortDescription(nameof(Delivery.DAL.Models.Client.Address.Street), ListSortDirection.Ascending),
                            new SortDescription(nameof(Delivery.DAL.Models.Client.Address.HomeNumber), ListSortDirection.Ascending),
                            new SortDescription(nameof(Delivery.DAL.Models.Client.Address.ApartmentNumber), ListSortDirection.Ascending),
						}
                    };

                    _ClientsViewSource.Filter += OnClientsFilter;
                    _ClientsViewSource.View.Refresh();

                    OnPropertyChanged(nameof(ClientsView));
                }
            }
        }
        #endregion

        #region ClientsFilter : string - Искомое слово

        private string _ClientsFilter;
        public string ClientsFilter
        {
            get => _ClientsFilter;
            set
            {
                if (Set(ref _ClientsFilter, value))
                    _ClientsViewSource.View.Refresh();

            }
        }
        #endregion


        #region SelectedClient : Client - Выбранный курьер
        private Client _SelectedClient;
        public Client SelectedClient
        {
            get => _SelectedClient;
            set
            {
                if (value is null)
                {
                    _SelectedClient = value;
                    return;
                }
                //Если изменения не были сохранены в базе, то сбрасываем на значения из кеша
                if (!_changedCommitted && !_firstSelect)
                {
                    //Set(ref _SelectedClient, _cachedSelectedClient);
                }
                _SelectedClient = value;
                CachedSelectedClient = new Client()
                {
                    Id = value.Id,
                    FirstName = value.FirstName,
                    SecondName = value.SecondName,
                    PhoneNumber = value.PhoneNumber,
                    AddressId = value.AddressId,
                    Address = value.Address,    
                };
                Set(ref _SelectedClient, value);
                _changedCommitted = false;
                _firstSelect = false;
            }
        }

        private bool _firstSelect = true;
        private bool _changedCommitted;
        private Client _cachedSelectedClient;
        public Client CachedSelectedClient { get => _cachedSelectedClient; set => Set(ref _cachedSelectedClient, value); }

        #endregion

        #region Command LoadDataCommand - Команда загрузки данных из репозитория

        private ICommand _LoadDataCommand;
        public ICommand LoadDataCommand => _LoadDataCommand
            ??= new LambdaCommandAsync(OnLoadDataCommandExecuted, CanLoadDataCommandExecute);
        private bool CanLoadDataCommandExecute() => true;
        private async Task OnLoadDataCommandExecuted()
        {
	        Clients = new ObservableCollection<Client>(await _UnitOfWork.ClientsRepository.Items.ToArrayAsync());
            OnPropertyChanged(nameof(Clients));
        }
        #endregion

        #region Command UpdateClientCommand  - команда изменения данных клиента в БД

        private ICommand _UpdateClientCommand;
        public ICommand UpdateClientCommand => _UpdateClientCommand
            ??= new LambdaCommandAsync<Client>(OnUpdateClientCommandExecuted, CanUpdateClientCommandExecute);
        //Тут бы сделать валидацию вводимых данных, но как нибудь в другой раз
        private bool CanUpdateClientCommandExecute(Client p) => (p != null || SelectedClient != null);
        private async Task OnUpdateClientCommandExecuted(Client? p)
        {
            try
            {
                var ClientToUpdate = p ?? CachedSelectedClient;
                await _ClientService.UpdateClientAsync(ClientToUpdate);
                SelectedClient = Clients.Find(x => x.Id == ClientToUpdate.Id);
                await OnLoadDataCommandExecuted();
                _changedCommitted = true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        #endregion

        #region Command RemoveClientCommand : Client - Удаление указанного клиента
        private ICommand _RemoveClientCommand;

        public ICommand RemoveClientCommand => _RemoveClientCommand
            ??= new LambdaCommandAsync<Client>(OnRemoveClientCommandExecuted, CanRemoveClientCommandExecute);

        private bool CanRemoveClientCommandExecute(Client p) => (p != null || SelectedClient != null);

        private async Task OnRemoveClientCommandExecuted(Client? p)
        {
            var ClientToRemove = p ?? SelectedClient;

            if (!_UserDialogClients.ConfirmWarning($"Вы хотите удалить клиента {ClientToRemove.FirstName}?", "Удаление клиента"))
                return;
            await _ClientService.DeleteClientAsync(ClientToRemove.Id);

            _Clients.Remove(ClientToRemove);

            if (ReferenceEquals(SelectedClient, ClientToRemove))
                SelectedClient = null;
        }
        #endregion

        #region Command AddNewClientCommand - Добавление нового клиента
        private ICommand _AddNewClientCommand;

        public ICommand AddNewClientCommand => _AddNewClientCommand
            ??= new LambdaCommandAsync(OnAddNewClientCommandExecuted, CanAddNewClientCommandExecute);
        private bool CanAddNewClientCommandExecute() => true;

        private async Task OnAddNewClientCommandExecuted()
        {
            var new_Client = new Client();
            if (!_UserDialogClients.Edit(new_Client))
                return;
            //TODO проверка полей адреса
            new_Client = await _ClientService.AddClientAsync(new_Client.FirstName, new_Client.SecondName, new_Client.PhoneNumber, new_Client.Address);
            await OnLoadDataCommandExecuted();
            SelectedClient = new_Client;

        }

        #endregion

        #region Конструктор
        public ClientsViewModel(IUnitOfWork unitOfWork, IUserDialogClients userDialogClients)
        {
            _UnitOfWork = unitOfWork;
            _UserDialogClients = userDialogClients;

        }

        #endregion

        private void OnClientsFilter(object Sender, FilterEventArgs E)
        {
            if (!(E.Item is Client Client) || string.IsNullOrEmpty(ClientsFilter)) return;

            if (!Client.PhoneNumber.Contains(ClientsFilter) &&
                !Client.FirstName.Contains(ClientsFilter) &&
                !Client.SecondName.Contains(ClientsFilter) &&
                !Client.Address.HomeNumber.Contains(ClientsFilter) &&
                !Client.Address.ApartmentNumber.Contains(ClientsFilter) &&
                !Client.Address.City.Contains(ClientsFilter) &&
                !Client.Address.Street.Contains(ClientsFilter) 
				)
                E.Accepted = false;
        }




    }
}
