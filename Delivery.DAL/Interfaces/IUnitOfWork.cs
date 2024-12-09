using System;
using System.Collections.Generic;
using System.Text;

namespace Delivery.DAL.Interfaces
{
	public interface IUnitOfWork
	{
		public IAddressesRepository AddressRepository { get; }
		public IClientsRepository ClientsRepository { get; }
		public ICouriersRepository CouriersRepository { get; }
		public ICourierStatusesRepository CourierStatusesRepository { get; }
		public ICourierTypesRepository CourierTypesRepository { get; }
		public IOrderLinesRepository OrderLinesRepository { get; }
		public IOrdersRepository OrdersRepository { get; }
		public IOrderStatusesRepository OrderStatusesRepository { get; }
		public IOrderTypesRepository OrderTypesRepository { get; }
		

	}
}
