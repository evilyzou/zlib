using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conference.Entity
{
    [Serializable]
    public class OrderItem
    {
        public OrderItem(Guid seatType, int quantity)
        {
            this.SeatType = seatType;
            this.Quantity = quantity;
        }

        public Guid SeatType { get; private set; }

        public int Quantity { get; private set; }
    }
}
