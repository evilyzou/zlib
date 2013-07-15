using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Conference.Entity;
using ENode.Commanding;

namespace Conference.Commands
{
    [Serializable]
    public class SeatsAvailabilityCommand:Command
    {
        public SeatsAvailabilityCommand(Guid conferenceId)
        {
            this.ConferenceId = conferenceId;
        }

        public Guid ConferenceId { get; private set; }

    }

    [Serializable]
    public class MakeSeatReservation :SeatsAvailabilityCommand
    {
        public MakeSeatReservation(Guid conferenceId,Guid reservationId,IEnumerable<SeatQuantity> seats):base(conferenceId)
        {
            this.Seats = seats.ToList();
            this.ReservationId = reservationId;
        }

        public Guid ReservationId { get; private set; }

        public List<SeatQuantity> Seats { get; private set; } 


    }

    [Serializable]
    public class AddSeats:SeatsAvailabilityCommand
    {
        public AddSeats(Guid conferenceId, Guid seatType, int quantity):base(conferenceId)
        {
            this.SeatType = seatType;
            this.Quantity = quantity;
        }

        public Guid SeatType { get; private set; }

        public int Quantity { get; private set; }
    }

    [Serializable]
    public class CommitSeatReservation: SeatsAvailabilityCommand
    {
        public CommitSeatReservation(Guid conferenceId,Guid reservationId):base(conferenceId)
        {
            this.ReservationId = reservationId;
        }

        public Guid ReservationId { get; private set; }
    }
}
