using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Conference.Entity;
using ENode.Eventing;

namespace Conference.Events
{
    [Serializable]
    public class SeatAssigned:Event
    {
        public SeatAssigned(int position,Guid seatType,PersonalInfo attendee)
        {
            this.Position = position;
            this.SeatType = seatType;
            this.Attendee = attendee;
        }

        public int Position { get; private set; }
        public Guid SeatType { get; private set; }
        public PersonalInfo Attendee { get; private set; }
    }
}
