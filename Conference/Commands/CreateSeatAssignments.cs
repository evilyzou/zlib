using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Conference.Entity;
using ENode.Commanding;

namespace Conference.Commands
{
    [Serializable]
    public class CreateSeatAssignments:Command
    {
        public CreateSeatAssignments(Guid orderId,IEnumerable<SeatQuantity> seats )
        {
            this.OrderId = orderId;
            this.Seats = seats;
        }

        public Guid OrderId { get; private set; }
        public IEnumerable<SeatQuantity> Seats { get; private set; }
    }
}
