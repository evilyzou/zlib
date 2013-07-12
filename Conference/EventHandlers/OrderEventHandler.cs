using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Conference.Events;
using ENode.Commanding;
using ENode.Eventing;
using ENode.Infrastructure;

namespace Conference.EventHandlers
{
    [Component]
    public class OrderEventHandler:IEventHandler<OrderPlaced>
    {
        public void Handle(OrderPlaced evnt)
        {
            Console.WriteLine("处理OrderPlaced事件");
        }
    }
}
