using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Delivery.BLL.Interfaces;
using Delivery.DAL.Context;
using Delivery.DAL.Interfaces;
using Delivery.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Delivery.BLL.Services;
using System.ComponentModel.DataAnnotations;
using System.Windows.Documents;

namespace Delivery.WPF.Data
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
            Courier courier1 = await _courierService.AddCourierAsync("Dmitry", "Pupkin", "89295501278");
            Courier courier2 = await _courierService.AddCourierAsync("Dmitry2", "Pupkin2", "89295501279");
            Courier courier3 = await _courierService.AddCourierAsync("Dmitry3", "Pupkin3", "89295501289");
            Courier courier4 = await _courierService.AddCourierAsync("Dmitry4", "Pupkin4", "89295511289");
            Courier courier5 = await _courierService.AddCourierAsync("Dmitry5", "Pupkin", "89295501278");
            Courier courier6 = await _courierService.AddCourierAsync("Dmitry6", "Pupkin2", "89295501279");
            Courier courier7 = await _courierService.AddCourierAsync("Dmitry7", "Pupkin3", "89295501276");
            Courier courier8 = await _courierService.AddCourierAsync("Dmitry8", "Pupkin4", "89295511289");
            Courier courier9 = await _courierService.AddCourierAsync("Dmitry4", "Pupkin4", "89295511289");
            Courier courier10 = await _courierService.AddCourierAsync("Dmitry5", "Pupkin", "89295501278");
            Courier courier11 = await _courierService.AddCourierAsync("Dmitry6", "Pupkin2", "89295501279");
            Courier courier12 = await _courierService.AddCourierAsync("Dmitry7", "Pupkin3", "89295501276");
            Courier courier13 = await _courierService.AddCourierAsync("Dmitry8", "Pupkin4", "89295511289");
            Courier courier14 = await _courierService.AddCourierAsync("Dmitry", "Pupkin", "89295501278");
            Courier courier15 = await _courierService.AddCourierAsync("Dmitry2", "Pupkin2", "89295501279");
            Courier courier16 = await _courierService.AddCourierAsync("Dmitry3", "Pupkin3", "89295501289");
            Courier courier17 = await _courierService.AddCourierAsync("Dmitry4", "Pupkin4", "89295511289");
            Courier courier18 = await _courierService.AddCourierAsync("Dmitry5", "Pupkin", "89295501278");
            Courier courier19 = await _courierService.AddCourierAsync("Dmitry6", "Pupkin2", "89295501279");
            Courier courier20 = await _courierService.AddCourierAsync("Dmitry7", "Pupkin3", "89295501276");
            Courier courier21 = await _courierService.AddCourierAsync("Dmitry8", "Pupkin4", "89295511289");
            Courier courier22 = await _courierService.AddCourierAsync("Dmitry4", "Pupkin4", "89295511289");
            Courier courier23 = await _courierService.AddCourierAsync("Dmitry5", "Pupkin", "89295501278");
            Courier courier24 = await _courierService.AddCourierAsync("Dmitry6", "Pupkin2", "89295501279");
            Courier courier25 = await _courierService.AddCourierAsync("Dmitry7", "Pupkin3", "89295501276");
            Courier courier26 = await _courierService.AddCourierAsync("Dmitry8", "Pupkin4", "89295511289");
        }
        public async Task InitializeAddress()
        {
            addressArray[0] = new Address() { City = "Москва", Street = "Авангардная улица", HomeNumber = "3", PostalCode = "12345", Region = "Москва московская область", Corpus = "1", ApartmentNumber = "123" };
            addressArray[1] = new Address() { City = "Москва", Street = "Авиаконструктора Микояна, улица", HomeNumber = "33", PostalCode = "12347", Region = "Москва московская область", Corpus = "2", ApartmentNumber = "128" };
            addressArray[2] = new Address() { City = "Москва", Street = "Академика Семенихина, улица", HomeNumber = "330", PostalCode = "12346", Region = "Москва московская область", Corpus = "3", ApartmentNumber = "129" };
            addressArray[3] = new Address() { City = "Москва", Street = "Академика Понтрягина, улица", HomeNumber = "330", PostalCode = "12346", Region = "Москва московская область", ApartmentNumber = "129" };
            addressArray[4] = new Address() { City = "Москва", Street = "Академика Петрова, площадь", HomeNumber = "330", PostalCode = "12346", Region = "Москва московская область", ApartmentNumber = "129" };
            await _db.AddRangeAsync(addressArray);
            await _db.SaveChangesAsync();
        }

        public async Task InitializerClients()
        {
            Client client1 = await _clientService.AddClientAsync("Dmitry", "Pupkin1", "89295501278", addressArray[0]);
            Client client2 = await _clientService.AddClientAsync("Dmitry2", "Pupkin2", "89295501279", addressArray[1]);
            Client client3 = await _clientService.AddClientAsync("Dmitry3", "Pupkin3", "89295501289", addressArray[2]);
            Client client4 = await _clientService.AddClientAsync("Dmitry4", "Pupkin4", "89295511289", addressArray[3]);
            Client client5 = await _clientService.AddClientAsync("Dmitry5", "Pupkin0", "89295501278", addressArray[4]);
        }

        public async Task InitializeOrders()
        {
            OrderLine[] orderLines = new OrderLine[2];
            orderLines[0] = new OrderLine() { ItemName = "Компьютер" };
            orderLines[1] = new OrderLine() { ItemName = "Lopata" };

            Order order1 = await _orderService.AddOrderAsync(await _unitOfWork.ClientsRepository.Items.FirstOrDefaultAsync(), addressArray[0], addressArray[3], orderLines, DateTime.UtcNow.AddDays(5));
        }
    }

}


