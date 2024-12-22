using Delivery.BLL.Services;
using Delivery.DAL.Context;
using Delivery.DAL.Interfaces;
using Delivery.DTOs;
using Delivery.Models;
using Microsoft.EntityFrameworkCore;

namespace DbInitialize
{
    public class DbInitializer
    {
        private readonly DeliveryDbContext _db;
        private readonly CourierService _courierService;
        private readonly ClientService _clientService;
        private readonly OrderService _orderService;
        private readonly IUnitOfWork _unitOfWork;
        OrderStatus[] orderStatuses = new OrderStatus[4];
        CourierStatus[] courierStatuses = new CourierStatus[2];
        Address[] addressArray = new Address[5];


        public DbInitializer(DeliveryDbContext db, IUnitOfWork unitOfWork)
        {
            _db = db;
            _unitOfWork = unitOfWork;
            _courierService = new CourierService(unitOfWork);
            _clientService = new ClientService(unitOfWork);
            _orderService = new OrderService(unitOfWork);
        }
        public async Task InitializeAsync()
        {
            Console.WriteLine("DB removing");
            await _db.Database.EnsureDeletedAsync().ConfigureAwait(false);
            Console.WriteLine("DB removed");
            await _db.Database.EnsureCreatedAsync().ConfigureAwait(true);
            Console.WriteLine("DB Created");
            await _db.Database.MigrateAsync();
            //if (! _db.OrderStatuses.Any()) return;
            //{
            Console.WriteLine("Init Started");
            await InitializeAddress();
            await InitializeStatusCourier();
            await InitializerCourier();
            await InitializeStatusOrder();
            await InitializerClients();
            await InitializeOrders();
            Console.WriteLine("Init Finished");

            await _db.DisposeAsync();
            //}
        }

        public async Task InitializeStatusOrder()
        {
            orderStatuses[0] = new OrderStatus() { StatusName = "Новая" };
            orderStatuses[1] = new OrderStatus() { StatusName = "Передано на выполнение" };
            orderStatuses[2] = new OrderStatus() { StatusName = "Выполнено" };
            orderStatuses[3] = new OrderStatus() { StatusName = "Отменена" };
            await _db.AddRangeAsync(orderStatuses);
            await _db.SaveChangesAsync();
        }

        public async Task InitializeStatusCourier()
        {
            courierStatuses[0] = new CourierStatus() { StatusName = "Выполняет заказ" };
            courierStatuses[1] = new CourierStatus() { StatusName = "Готов к выполнению заказа" };
            await _db.AddRangeAsync(courierStatuses);
            await _db.SaveChangesAsync();
        }

        public async Task InitializerCourier()
        {
            var courier1 = await _courierService.AddCourierAsync("Dmitry", "Pupkin", "89295501278");
            var courier2 = await _courierService.AddCourierAsync("Dmitry2", "Pupkin2", "89295501279");
            var courier3 = await _courierService.AddCourierAsync("Dmitry3", "Pupkin3", "89295501289");
            var courier4 = await _courierService.AddCourierAsync("Dmitry4", "Pupkin4", "89295511289");
            var courier5 = await _courierService.AddCourierAsync("Dmitry5", "Pupkin", "89295501278");
            var courier6 = await _courierService.AddCourierAsync("Dmitry6", "Pupkin2", "89295501279");
            var courier7 = await _courierService.AddCourierAsync("Dmitry7", "Pupkin3", "89295501276");
            var courier8 = await _courierService.AddCourierAsync("Dmitry8", "Pupkin4", "89295511289");
            var courier9 = await _courierService.AddCourierAsync("Dmitry4", "Pupkin4", "89295511289");
            var courier10 = await _courierService.AddCourierAsync("Dmitry5", "Pupkin", "89295501278");
            var courier11 = await _courierService.AddCourierAsync("Dmitry6", "Pupkin2", "89295501279");
            var courier12 = await _courierService.AddCourierAsync("Dmitry7", "Pupkin3", "89295501276");
            var courier13 = await _courierService.AddCourierAsync("Dmitry8", "Pupkin4", "89295511289");
            var courier14 = await _courierService.AddCourierAsync("Dmitry", "Pupkin", "89295501278");
            var courier15 = await _courierService.AddCourierAsync("Dmitry2", "Pupkin2", "89295501279");
            var courier16 = await _courierService.AddCourierAsync("Dmitry3", "Pupkin3", "89295501289");
            var courier17 = await _courierService.AddCourierAsync("Dmitry4", "Pupkin4", "89295511289");
            var courier18 = await _courierService.AddCourierAsync("Dmitry5", "Pupkin", "89295501278");
            var courier19 = await _courierService.AddCourierAsync("Dmitry6", "Pupkin2", "89295501279");
            var courier20 = await _courierService.AddCourierAsync("Dmitry7", "Pupkin3", "89295501276");
            var courier21 = await _courierService.AddCourierAsync("Dmitry8", "Pupkin4", "89295511289");
            var courier22 = await _courierService.AddCourierAsync("Dmitry4", "Pupkin4", "89295511289");
            var courier23 = await _courierService.AddCourierAsync("Dmitry5", "Pupkin", "89295501278");
            var courier24 = await _courierService.AddCourierAsync("Dmitry6", "Pupkin2", "89295501279");
            var courier25 = await _courierService.AddCourierAsync("Dmitry7", "Pupkin3", "89295501276");
            var courier26 = await _courierService.AddCourierAsync("Dmitry8", "Pupkin4", "89295511289");
        }
        public async Task InitializeAddress()
        {
            addressArray[0] = await _unitOfWork.AddressRepository.AddAsync(new Address() { City = "Москва", Street = "Авангардная улица", HomeNumber = "3", PostalCode = "12345", Region = "Москва московская область", Corpus = "1", ApartmentNumber = "123" });
            addressArray[1] = await _unitOfWork.AddressRepository.AddAsync(new Address() { City = "Москва", Street = "Авиаконструктора Микояна, улица", HomeNumber = "33", PostalCode = "12347", Region = "Москва московская область", Corpus = "2", ApartmentNumber = "128" });
            addressArray[2] = await _unitOfWork.AddressRepository.AddAsync(new Address() { City = "Москва", Street = "Академика Семенихина, улица", HomeNumber = "330", PostalCode = "12346", Region = "Москва московская область", Corpus = "3", ApartmentNumber = "129" });
            addressArray[3] = await _unitOfWork.AddressRepository.AddAsync(new Address() { City = "Москва", Street = "Академика Понтрягина, улица", HomeNumber = "330", PostalCode = "12346", Region = "Москва московская область", ApartmentNumber = "129" });
            addressArray[4] = await _unitOfWork.AddressRepository.AddAsync(new Address() { City = "Москва", Street = "Академика Петрова, площадь", HomeNumber = "330", PostalCode = "12346", Region = "Москва московская область", ApartmentNumber = "129" });
            //await _db.AddRangeAsync(addressArray);
            //await _db.SaveChangesAsync();
            //_db.Entry(addressArray[0]).State = EntityState.Detached;
            //_db.Entry(addressArray[1]).State = EntityState.Detached;
            //_db.Entry(addressArray[2]).State = EntityState.Detached;
            //_db.Entry(addressArray[3]).State = EntityState.Detached;
            //_db.Entry(addressArray[4]).State = EntityState.Detached;
        }

        public async Task InitializerClients()
        {
            var client1 = await _clientService.AddClientAsync("Dmitry", "Pupkin1", "89295501278", new AddressDto(addressArray[0]));
            var client2 = await _clientService.AddClientAsync("Dmitry2", "Pupkin2", "89295501279", new AddressDto(addressArray[1]));
            var client3 = await _clientService.AddClientAsync("Dmitry3", "Pupkin3", "89295501289", new AddressDto(addressArray[2]));
            var client4 = await _clientService.AddClientAsync("Dmitry4", "Pupkin4", "89295511289", new AddressDto(addressArray[3]));
            var client5 = await _clientService.AddClientAsync("Dmitry5", "Pupkin0", "89295501278", new AddressDto(addressArray[4]));
        }

        public async Task InitializeOrders()
        {
            OrderLineDto[] orderLines = new OrderLineDto[2];
            orderLines[0] = new OrderLineDto() { ItemName = "Компьютер" };
            orderLines[1] = new OrderLineDto() { ItemName = "Lopata" };
            Client client = await _unitOfWork.ClientsRepository.Items.FirstOrDefaultAsync();
            Courier courier = await _unitOfWork.CouriersRepository.Items.FirstOrDefaultAsync();
              OrderDto order1 = await _orderService.AddOrderAsync(new OrderDto(){ClientId = client.Id, ClientName = client.FirstName, ClientPhone = client.PhoneNumber, FromAddress = new AddressDto(addressArray[0]), TargetAddress = new AddressDto(addressArray[1]), OrderLines = orderLines.ToList()});
        }
    }

}


