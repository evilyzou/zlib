using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Conference.Entity;
using ENode.Commanding;

namespace Conference.Commands
{
    [Serializable]
    public class AssignSeat:Command
    {
        public AssignSeat(Guid seatAssignmentsId,int position,PersonalInfo attendee)
        {
            this.SeatAssignmentsId = seatAssignmentsId;
            this.Position = position;
            this.Attendee = attendee;
        }


        public Guid SeatAssignmentsId { get; private set; }
        public int Position { get; private set; }
        public PersonalInfo Attendee { get; private set; }
    }
}
