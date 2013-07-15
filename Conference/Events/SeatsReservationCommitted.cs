using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ENode.Eventing;

namespace Conference.Events
{
    [Serializable]
    public class SeatsReservationCommitted:Event
    {
        public SeatsReservationCommitted(Guid reservationId)
        {
            this.ReservationId = reservationId;
        }

        public Guid ReservationId { get; private set; }
    }
}
