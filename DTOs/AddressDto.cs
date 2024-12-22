using Delivery.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Delivery.DTOs
{
    public class AddressDto
    {
        public AddressDto(Address model)
        {
            Id = model.Id;
            City = model.City;
            Region = model.Region;
            PostalCode = model.PostalCode;
            Street = model.Street;
            HomeNumber = model.HomeNumber;
            Corpus = model.Corpus;
            ApartmentNumber = model.ApartmentNumber;
        }
        public Guid Id { get; set; }
        public string? City { get; set; }
        public string? Region { get; set; }
        public string? PostalCode { get; set; }
        public string? Street { get; set; }
        public string? HomeNumber { get; set; }
        public string? Corpus { get; set; }
        public string? ApartmentNumber { get; set; }

        public Address ToModel()
        {

            return new Address()
            {
                Id = Id,
                City = City,
                Region = Region,
                PostalCode = PostalCode,
                Street = Street,
                HomeNumber = HomeNumber,
                Corpus = Corpus,
                ApartmentNumber = ApartmentNumber
            };
        }
    }
}
