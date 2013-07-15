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
    public class SeatsAvailabilityHandler:ICommandHandler<AddSeats>,
        ICommandHandler<MakeSeatReservation>,
        ICommandHandler<CommitSeatReservation>
    {
        public void Handle(ICommandContext context, AddSeats command)
        {
            var seatAvailability = new SeatsAvailability(command.ConferenceId);
            context.Add(seatAvailability);

            seatAvailability.AddSeats(command.SeatType, command.Quantity);
        }

        public void Handle(ICommandContext context, MakeSeatReservation command)
        {
            var seatAvailability = context.Get<SeatsAvailability>(command.ConferenceId);
            seatAvailability.MakeReservation(command.ReservationId,command.Seats);
        }

        public void Handle(ICommandContext context, CommitSeatReservation command)
        {
            var seatAvailability = context.Get<SeatsAvailability>(command.ConferenceId);
            seatAvailability.CommitReservation(command.ReservationId);
        }
    }
}
