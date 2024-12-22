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
        private readonly IUserDialogClients _userDialogClients;
        private readonly IUnitOfWork _unitOfWork;
        private IClientService _clientService => new ClientService(_unitOfWork);
        private ObservableCollection<Client> _clients;
        private CollectionViewSource _clientsViewSource;
        public ICollectionView ClientsView => _clientsViewSource?.View;

        #region Clients : ObservableCollection<Client> - Коллекция курьеров

        public ObservableCollection<Client> Clients
        {
            get => _clients;
            set
            {
                if (Set(ref _clients, value))
                {
                    _clientsViewSource = new CollectionViewSource
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

                    _clientsViewSource.Filter += OnClientsFilter;
                    _clientsViewSource.View.Refresh();

                    OnPropertyChanged(nameof(ClientsView));
                }
            }
        }
        #endregion

        #region ClientsFilter : string - Искомое слово

        private string _clientsFilter;
        public string ClientsFilter
        {
            get => _clientsFilter;
            set
            {
                if (Set(ref _clientsFilter, value))
                    _clientsViewSource.View.Refresh();

            }
        }
        #endregion


        #region SelectedClient : Client - Выбранный клиент
        private Client _selectedClient;
        public Client SelectedClient
        {
            get => _selectedClient;
            set
            {
                if (value is null)
                {
                    _selectedClient = value;
                    return;
                }
                _selectedClient = value;
                CachedSelectedClient = new Client()
                {
                    Id = value.Id,
                    FirstName = value.FirstName,
                    SecondName = value.SecondName,
                    PhoneNumber = value.PhoneNumber,
                    AddressId = value.AddressId,
                    Address = value.Address,    
                };
                Set(ref _selectedClient, value);
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
	        Clients = new ObservableCollection<Client>(await _unitOfWork.ClientsRepository.Items.ToArrayAsync());
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
                var clientToUpdate = p ?? CachedSelectedClient;
                await _clientService.UpdateClientAsync(clientToUpdate);
                SelectedClient = Clients.Find(x => x.Id == clientToUpdate.Id);
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
            var clientToRemove = p ?? SelectedClient;

            if (!_userDialogClients.ConfirmWarning($"Вы хотите удалить клиента {clientToRemove.FirstName}?", "Удаление клиента"))
                return;
            await _clientService.DeleteClientAsync(clientToRemove.Id);

            _clients.Remove(clientToRemove);

            if (ReferenceEquals(SelectedClient, clientToRemove))
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
            var newClient = new Client();
            if (!_userDialogClients.Edit(newClient))
                return;
            //TODO проверка полей адреса
            newClient = await _clientService.AddClientAsync(newClient.FirstName, newClient.SecondName, newClient.PhoneNumber, newClient.Address);
            await OnLoadDataCommandExecuted();
            SelectedClient = newClient;

        }

        #endregion

        #region Конструктор
        public ClientsViewModel(IUnitOfWork unitOfWork, IUserDialogClients userDialogClients)
        {
            _unitOfWork = unitOfWork;
            _userDialogClients = userDialogClients;

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
