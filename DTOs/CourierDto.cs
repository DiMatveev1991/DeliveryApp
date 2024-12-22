using System;
using System.Collections.Generic;
using System.Text;
using Delivery.Models;

namespace Delivery.DTOs
{
    public class CourierDto
    {
        public CourierDto(Courier model)
        {
	        Id = model.Id;
	        FirstName = model.FirstName;
	        SecondName = model.SecondName;
	        PhoneNumber = model.PhoneNumber;
	        CourierStatusName = model.CourierStatus.StatusName;
	        CourierStatusId = model.CourierStatusId;

        }
        public Guid Id { get; set; }
        public string? FirstName { get; set; }
        public string? SecondName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? CourierStatusName { get; set; }
        public Guid? CourierStatusId { get; set; }

        public Courier ToModel()
        {
	        return new Courier()
	        {
		        Id = Id,
		        FirstName = FirstName,
		        SecondName = SecondName,
		        PhoneNumber = PhoneNumber,
				CourierStatus = new CourierStatus()
				{
					StatusName = CourierStatusName

				},
				CourierStatusId = CourierStatusId
	        };
        }
	}
}
