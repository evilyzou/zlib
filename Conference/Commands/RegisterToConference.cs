using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Conference.Entity;
using ENode.Commanding;

namespace Conference.Commands
{
    [Serializable]
    public class RegisterToConference:Command
    {
        public RegisterToConference(Guid conferenceId,Guid orderId)
        {
            this.ConferenceId = conferenceId;
            this.OrderId = orderId;
            this.Seats = new Collection<SeatQuantity>();
        }

        public Guid OrderId { get; private set; }

        public Guid ConferenceId { get; set; }

        public ICollection<SeatQuantity> Seats { get; set; }

    }
}
