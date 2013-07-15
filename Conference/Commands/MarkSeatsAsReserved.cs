using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Conference.Entity;
using ENode.Commanding;

namespace Conference.Commands
{
    [Serializable]
    public class MarkSeatsAsReserved:Command
    {
        public MarkSeatsAsReserved(Guid orderId,IEnumerable<SeatQuantity> seats,DateTime expiration )
        {
            this.OrderId = orderId;
            this.Seats = seats.ToList();
            this.Expiration = expiration;
        }

        public Guid OrderId { get; set; }

        public List<SeatQuantity> Seats { get; set; }

        public DateTime Expiration { get; set; }
    }
}
