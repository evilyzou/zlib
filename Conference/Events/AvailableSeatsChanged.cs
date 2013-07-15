using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Conference.Entity;
using ENode.Eventing;

namespace Conference.Events
{
    [Serializable]
    public class AvailableSeatsChanged:Event
    {
        public AvailableSeatsChanged(IEnumerable<SeatQuantity> seats )
        {
            this.Seats = seats;
        }

        public IEnumerable<SeatQuantity> Seats { get; private set; }

    }
}
