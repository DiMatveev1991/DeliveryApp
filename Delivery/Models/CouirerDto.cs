using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Delivery.DAL.Models;

namespace Delivery.WPF.Models
{
	internal class CourierDto
	{
		private Guid _courierId;
		private string _firstName;
		private string _secondName;
		private string _phoneNumber;
		private Guid _courierStatusId;
		private CourierStatus _courierStatus;
		public CourierDto () { }

		public CourierDto(Guid courierId, string firstName, string secondName, string phoneNumber, Guid courierStatusId, CourierStatus courierStatus)
		{ 
			_courierId = courierId;
			_firstName = firstName;
			_secondName = secondName;
			_phoneNumber = phoneNumber;
			_courierStatusId = courierStatusId;
			_courierStatus = courierStatus;
		}

	}
}
