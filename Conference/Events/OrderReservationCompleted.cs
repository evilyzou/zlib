using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Conference.Entity;
using ENode.Eventing;

namespace Conference.Events
{
    [Serializable]
    public class OrderReservationCompleted:Event
    {
        public OrderReservationCompleted(DateTime reservtionExpiration,IEnumerable<SeatQuantity> seats )
        {
            this.ReservationExpiration = reservtionExpiration;
            this.Seats = seats;
        }

        public DateTime ReservationExpiration { get; private set; }

        public IEnumerable<SeatQuantity> Seats { get;private set; }
    }
}
