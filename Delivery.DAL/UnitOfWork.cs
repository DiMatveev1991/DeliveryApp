using System;
using System.Collections.Generic;
using System.Text;
using Delivery.DAL.Interfaces;

namespace Delivery.DAL
{
	internal class UnitOfWork : IUnitOfWork
	{
		
		public IAddressesRepository _addressRepository;

		public IClientsRepository _clientsRepository;
		
		public ICouriersRepository _couriersRepository;

		public ICourierStatusesRepository _courierStatusesRepository;

		public IOrderLinesRepository _orderLinesRepository;

		public IOrdersRepository _ordersRepository;

		public IOrderStatusesRepository _orderStatusesRepository;

		public ICancelReasonRepository _cancelReasonRepository;
		public UnitOfWork (IAddressesRepository addressRepository,
			               ICouriersRepository couriersRepository, 
			               ICourierStatusesRepository courierStatusesRepository, 
						   IClientsRepository clientsRepository,
						   IOrderLinesRepository orderLinesRepository,
						   IOrdersRepository ordersRepository,
						   IOrderStatusesRepository orderStatusesRepository,
						   ICancelReasonRepository cancelReasonRepository
						   )
		{
			_addressRepository = addressRepository;
			_clientsRepository = clientsRepository;
			_orderLinesRepository = orderLinesRepository;
			_ordersRepository = ordersRepository;
			_orderStatusesRepository = orderStatusesRepository;
			_couriersRepository = couriersRepository;
			_courierStatusesRepository = courierStatusesRepository;
			_cancelReasonRepository = cancelReasonRepository;
		}
		public IAddressesRepository AddressRepository => _addressRepository;
		public IClientsRepository ClientsRepository => _clientsRepository;
		public ICouriersRepository CouriersRepository => _couriersRepository;
		public ICourierStatusesRepository CourierStatusesRepository => _courierStatusesRepository;
		public IOrderLinesRepository OrderLinesRepository => _orderLinesRepository;
		public IOrdersRepository OrdersRepository => _ordersRepository;
		public IOrderStatusesRepository OrderStatusesRepository => _orderStatusesRepository;
		public ICancelReasonRepository CancelReasonRepository => _cancelReasonRepository;

	}
}
