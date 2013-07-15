using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Conference.Commands;
using Conference.Domain;
using Conference.Entity;
using ENode.Commanding;
using ENode.Domain;
using System.Threading;
using ENode.Infrastructure;

namespace Conference
{
    class Program
    {
        static void Main(string[] args)
        {
            new ENodeFrameworkUnitTestInitializer().Initialize();

            var memoryCache = ObjectContainer.Resolve<IMemoryCache>();
            var commandService = ObjectContainer.Resolve<ICommandService>();
            //初始化一个Conferecen 要求一个座位数量为5 的和一个座位数量为10
            var conferenceGuid = Guid.NewGuid();
            var orderId = Guid.NewGuid();

            var seatType = Guid.NewGuid();
            var addSeats = new AddSeats(conferenceGuid, seatType, 10);
            commandService.Execute(addSeats);
            var seatAvailability = memoryCache.Get<SeatsAvailability>(conferenceGuid.ToString());

            RegisterToConference conference =new RegisterToConference(conferenceGuid,orderId);


            conference.Seats.Add(new SeatQuantity(seatType, 3));

            commandService.Execute(conference);

            Thread.Sleep(1000);

            var order = memoryCache.Get<Order>(orderId.ToString());

            var seatAvailability2 = memoryCache.Get<SeatsAvailability>(conferenceGuid.ToString());
            Console.WriteLine("orderId:"+order);

            Console.WriteLine("press enter...");
            Console.ReadLine();


        }
    }
}
