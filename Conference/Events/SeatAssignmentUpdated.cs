using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Conference.Entity;
using ENode.Eventing;

namespace Conference.Events
{
    [Serializable]
    public class SeatAssignmentUpdated:Event
    {
        public SeatAssignmentUpdated(int position,PersonalInfo attendee)
        {
            this.Position = position;
            this.Attendee = attendee;
        }

        public int Position { get; private set; }
        public PersonalInfo Attendee { get; private set; }
    
    }
}
