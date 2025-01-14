using Delivery.Models;
using System;

namespace Delivery.DTOs
{
    public class ClientDto
    {
        public ClientDto(Client model)
        {
            Id = model.Id;
            AddressId = model.AddressId;
            if (model.Address != null) Address = new AddressDto(model.Address);
            FirstName = model.FirstName;
            SecondName = model.SecondName;
            PhoneNumber = model.PhoneNumber;

        }

        public Guid Id { get; set; }
        public Guid? AddressId { get; set; }
        public AddressDto? Address { get; set; }
        public string? FirstName { get; set; }
        public string? SecondName { get; set; }
        public string? PhoneNumber { get; set; }

        public Client ToModel()
        {
            return new Client()
            {
                Id = Id,
                AddressId = AddressId,
                FirstName = FirstName,
                SecondName = SecondName,
                PhoneNumber = PhoneNumber,
                Address = new Address()
                {
                    Id = Address.Id,
                    HomeNumber = Address.HomeNumber,
                    Street = Address.Street,
                    ApartmentNumber = Address.ApartmentNumber,
                    Corpus = Address.Corpus,
                    Region = Address.Region,
                    PostalCode = Address.PostalCode,
                    City = Address.City,
                }
            };
        }
    }
}
