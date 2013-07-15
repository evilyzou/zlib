using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Conference.Commands;
using Conference.Domain;
using Conference.Entity;
using ENode.Commanding;
using ENode.Infrastructure;

namespace Conference.CommandHandlers
{
    [Component]
    public class OrderCommandHandler:ICommandHandler<RegisterToConference>, //登记会议室,
        ICommandHandler<MarkSeatsAsReserved>,
        ICommandHandler<ConfirmOrder>
    {
        public void Handle(ICommandContext context, RegisterToConference command)
        {
            var items = command.Seats.Select(t => new OrderItem(t.SeatType, t.Quantity)).ToList();
            context.Add(new Order(command.OrderId,command.ConferenceId,items));
        }

        public void Handle(ICommandContext context, MarkSeatsAsReserved command)
        {
            var order = context.Get<Order>(command.OrderId);
            order.MarkAsReserved(command.Expiration,command.Seats);
        }

        public void Handle(ICommandContext context,ConfirmOrder confirmOrder)
        {
            var order = context.Get<Order>(confirmOrder.OrderId);
            order.Confirm(confirmOrder.ConferenceId,confirmOrder.OrderId);
        }
    }
}
