using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Conference.Commands;
using Conference.Domain;
using ENode.Commanding;
using ENode.Infrastructure;

namespace Conference.CommandHandlers
{
    [Component]
    public class SeatAssignmentsCommandHandler:ICommandHandler<CreateSeatAssignments>,
        ICommandHandler<AssignSeat>
    {
        public void Handle(ICommandContext context, CreateSeatAssignments command)
        {
            var seatAssignments = new SeatAssignments(command.OrderId, command.Seats);
            context.Add(seatAssignments);
        }

        public void Handle(ICommandContext context, AssignSeat command)
        {
            var seatAssignments = context.Get<SeatAssignments>(command.SeatAssignmentsId);
        }
    }
}
