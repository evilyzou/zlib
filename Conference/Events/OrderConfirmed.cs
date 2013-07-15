using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Conference.Entity;
using ENode.Eventing;

namespace Conference.Events
{
    [Serializable]
    public class OrderConfirmed:Event
    {
        public OrderConfirmed(Guid conferenceId,Guid orderId,IEnumerable<SeatQuantity> seats )
        {
            this.ConferenceId = conferenceId;
            this.OrderId = orderId;
            this.Seats = seats;
        }
        public Guid ConferenceId { get; private set; }
        public Guid OrderId { get; private set; }

        public IEnumerable<SeatQuantity> Seats { get; private set; }
    }
}
