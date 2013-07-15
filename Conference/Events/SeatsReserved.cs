using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Conference.Entity;
using ENode.Eventing;

namespace Conference.Events
{
    [Serializable]
    public class SeatsReserved:Event
    {
        public SeatsReserved(Guid reservationId, IEnumerable<SeatQuantity> reservationDetails,IEnumerable<SeatQuantity> availableSeatsChanged )
        {
            this.ReservationId = reservationId;
            this.ReservationDetails = reservationDetails;
            this.AvailableSeatsChanged = availableSeatsChanged;
        }

        public Guid ReservationId { get; private set; }

        public IEnumerable<SeatQuantity> ReservationDetails { get; private set; }

        public IEnumerable<SeatQuantity> AvailableSeatsChanged { get; private set; }
    }
}
