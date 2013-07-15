using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ENode.Eventing;

namespace Conference.Events
{
    [Serializable]
    public class SeatAssignmentsCreated:Event
    {
        public class SeatAssignmentInfo
        {
            public int Position { get; set; }
            public Guid SeatType { get; set; }
        }

        public SeatAssignmentsCreated(Guid orderId,IEnumerable<SeatAssignmentInfo> seats )
        {
            this.OrderId = orderId;
            this.Seats = seats;
        }

        public Guid OrderId { get; private set; }
        public IEnumerable<SeatAssignmentInfo> Seats { get; private set; }
    }
}
