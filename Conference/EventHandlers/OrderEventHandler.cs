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
    public class OrderEventHandler:IEventHandler<OrderPlaced>,
        IEventHandler<SeatsReserved>,
        IEventHandler<OrderConfirmed>

    {
        private ICommandService commandService;
        public OrderEventHandler(ICommandService _commandService)
        {
            this.commandService = _commandService;
        }

        public void Handle(OrderPlaced evnt)
        {
            var makeReservation = new MakeSeatReservation(evnt.ConferenceId, evnt.Id, evnt.Seats);
            
            commandService.Send(makeReservation,(result) =>
                                                         {
                                                             if (result.Exception != null)
                                                             {
                                                                 Console.WriteLine("Exception:"+result.Exception);
                                                             }
                                                         });

        }

        public void Handle(SeatsReserved evnt)
        {
            var makeSeatsAsReserved = new MarkSeatsAsReserved(evnt.ReservationId, evnt.ReservationDetails,
                                                              new DateTime());
            commandService.Send(makeSeatsAsReserved,(result) =>
                                                        {
                                                            if (result.Exception != null)
                                                            {
                                                                Console.WriteLine("Exception:" + result.Exception);
                                                            }
                                                        });
        }


        public void Handle(OrderConfirmed evnt)
        {
            //产生新的Command
            var commitReservation = new CommitSeatReservation(evnt.ConferenceId, evnt.OrderId);
            commandService.Send(commitReservation, (result) =>
            {
                if (result.Exception != null)
                {
                    Console.WriteLine("Exception:" + result.Exception);
                }
            });
        }
    }
}
