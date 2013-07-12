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
            //初始化一个Conferecen 要求一个座位数量为5 的和一个座位数量为10
            var conferenceGuid = Guid.NewGuid();
            var orderId = Guid.NewGuid();
            RegisterToConference conference =new RegisterToConference(conferenceGuid,orderId);
            

            var seatGuid1 = Guid.NewGuid();
            var seatGuid2 = Guid.NewGuid();
            conference.Seats.Add(new SeatQuantity(seatGuid1,10));
            conference.Seats.Add(new SeatQuantity(seatGuid2, 5));

            var commandService = ObjectContainer.Resolve<ICommandService>();
            commandService.Execute(conference);

            Thread.Sleep(1000);

            var order = memoryCache.Get<Order>(orderId.ToString());
            Console.WriteLine("orderId:"+order);

            Console.WriteLine("press enter...");
            Console.ReadLine();


        }
    }
}
