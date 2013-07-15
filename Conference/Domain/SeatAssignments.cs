using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Conference.Entity;
using Conference.Events;
using Conference.Utils;
using ENode.Domain;
using ENode.Eventing;

namespace Conference.Domain
{
    public class SeatAssignments:AggregateRoot<Guid>,
        IEventHandler<SeatAssignmentsCreated>,
        IEventHandler<SeatAssigned>,
        IEventHandler<SeatUnassigned>,
        IEventHandler<SeatAssignmentUpdated>
    {
        private class SeatAssignment
        {
            public SeatAssignment()
            {
                this.Attendee = new PersonalInfo();
            }

            public int Position { get; set; }

            public Guid SeatType { get; set; }

            public PersonalInfo Attendee { get; set; }
        }

        private Dictionary<int, SeatAssignment> seats = new Dictionary<int, SeatAssignment>();

        public SeatAssignments() : base() { }

        public SeatAssignments(Guid orderId, IEnumerable<SeatQuantity> seats):base(orderId)
            // Note that we don't use the order id as the assignments id
        {
            // Add as many assignments as seats there are.
            var i = 0;
            var all = new List<SeatAssignmentsCreated.SeatAssignmentInfo>();
            foreach (var seatQuantity in seats)
            {
                for (int j = 0; j < seatQuantity.Quantity; j++)
                {
                    all.Add(new SeatAssignmentsCreated.SeatAssignmentInfo { Position = i++, SeatType = seatQuantity.SeatType });
                }
            }

            RaiseEvent(new SeatAssignmentsCreated ( orderId, all ));
        }

        public void AssignSeat(int position, PersonalInfo attendee)
        {
            if (string.IsNullOrEmpty(attendee.Email))
                throw new ArgumentNullException("attendee.Email");

            SeatAssignment current;
            if (!this.seats.TryGetValue(position, out current))
                throw new ArgumentOutOfRangeException("position");

            if (!attendee.Email.Equals(current.Attendee.Email, StringComparison.InvariantCultureIgnoreCase))
            {
                if (current.Attendee.Email != null)
                {
                    RaiseEvent(new SeatUnassigned( position ));
                }

                RaiseEvent(new SeatAssigned(position,current.SeatType,attendee));
               
            }
            else if (!string.Equals(attendee.FirstName, current.Attendee.FirstName, StringComparison.InvariantCultureIgnoreCase)
                || !string.Equals(attendee.LastName, current.Attendee.LastName, StringComparison.InvariantCultureIgnoreCase))
            {
                RaiseEvent(new SeatAssignmentUpdated(position, attendee));
            }
        }


        public void Handle(SeatAssignmentsCreated e)
        {
            this.seats = e.Seats.ToDictionary(x => x.Position, x => new SeatAssignment { Position = x.Position, SeatType = x.SeatType });
        }

        public void Handle(SeatAssigned e)
        {
            this.seats[e.Position] = new SeatAssignment();
        }

        public void Handle(SeatUnassigned e)
        {
            this.seats[e.Position] = new SeatAssignment { SeatType = this.seats[e.Position].SeatType };
        }

        public void Handle(SeatAssignmentUpdated e)
        {
            this.seats[e.Position] = new SeatAssignment
                                         {
                                             // Seat type is also never received again from the client.
                                             SeatType = this.seats[e.Position].SeatType,
                                             // The email property is not received for updates, as those 
                                             // are for the same attendee essentially.
                                             Attendee = new PersonalInfo {Email = this.seats[e.Position].Attendee.Email}
                                         };

        }
    }
}
