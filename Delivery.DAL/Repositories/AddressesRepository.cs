using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Delivery.DAL.Context;
using Delivery.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;
using Delivery.DAL.Models;

namespace Delivery.DAL.Repositories
{
	internal class AddressesRepository: IAddressesRepository

	{
		private readonly DeliveryDbContext _db;
		private readonly DbSet<Address> _Set;

		public bool AutoSaveChanges { get; set; } = true;

		public AddressesRepository (DeliveryDbContext db)
		{
			_db = db;
			_Set = db.Set<Address>();
		}

		public IQueryable<Address> Items => _Set;


		public Address Get(Guid id) => Items.SingleOrDefault(item => item.Id == id);

		public async Task<Address> GetAsync(Guid id, CancellationToken cancel = default) => await Items
		   .SingleOrDefaultAsync(item => item.Id == id, cancel)
		   .ConfigureAwait(false);

		public Address Add(Address item)
		{
			if (item is null) throw new ArgumentNullException(nameof(item));
			_db.Entry(item).State = EntityState.Added;
			if (AutoSaveChanges)
				_db.SaveChanges();
			return item;
		}

		public async Task<Address> AddAsync(Address item, CancellationToken cancel = default)
		{
			if (item is null) throw new ArgumentNullException(nameof(item));
			_db.Entry(item).State = EntityState.Added;
			if (AutoSaveChanges)
				await _db.SaveChangesAsync(cancel).ConfigureAwait(false);
			return item;
		}

		public void Update(Address item)
		{
			if (item is null) throw new ArgumentNullException(nameof(item));
			_db.Entry(item).State = EntityState.Modified;
			if (AutoSaveChanges)
				_db.SaveChanges();
		}

		public async Task UpdateAsync(Address item, CancellationToken cancel = default)
		{
			if (item is null) throw new ArgumentNullException(nameof(item));
			_db.Entry(item).State = EntityState.Modified;
			if (AutoSaveChanges)
				await _db.SaveChangesAsync(cancel).ConfigureAwait(false);
		}

		public void Remove(Guid id)
		{
			var item = _Set.Local.FirstOrDefault(i => i.Id == id) ?? new Address { Id = id };
			_db.Remove(item);
			if (AutoSaveChanges)
				_db.SaveChanges();
		}

		public async Task RemoveAsync(Guid id, CancellationToken cancel = default)
		{
			_db.Remove(new Address { Id = id });
			if (AutoSaveChanges)
				await _db.SaveChangesAsync(cancel).ConfigureAwait(false);
		}

		public async Task<Address> GetByAddressAsync (Address address)
		{
			return await _db.Addresses.FirstOrDefaultAsync(n =>
				n.City == address.City && n.Street == address.Street && n.HomeNumber == address.HomeNumber &&
				n.HomeNumber == address.HomeNumber);
		}
	}

}

