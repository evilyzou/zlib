using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Conference.Commands;
using Conference.Events;
using ENode.Commanding;
using ENode.Eventing;
using ENode.Infrastructure;

namespace Conference.EventHandlers
{
    [Component]
    public class SeatsAssignmentsEventHandler:IEventHandler<OrderConfirmed>
    {
        private ICommandService commandService;
        public SeatsAssignmentsEventHandler(ICommandService _commandService)
        {
            this.commandService = _commandService;
        }

        public void Handle(OrderConfirmed evnt)
        {
            var createSeatsAssignments = new CreateSeatAssignments(evnt.OrderId, evnt.Seats);
            commandService.Send(createSeatsAssignments, (result) =>
            {
                if (result.Exception != null)
                {
                    Console.WriteLine("Exception:" + result.Exception);
                }
            });
        }
    }
}
