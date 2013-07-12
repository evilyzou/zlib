using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Conference.Entity;
using ENode.Eventing;

namespace Conference.Events
{
    [Serializable]
    public class OrderPlaced:Event
    {
        public OrderPlaced(Guid conferenceId,IEnumerable<SeatQuantity> seats,DateTime reservationAutoExpiration,string accessCode )
        {
            this.ConferenceId = conferenceId;
            this.Seats = seats;
            this.ReservationAutoExpiration = reservationAutoExpiration;
            this.AccessCode = accessCode;
        }
        public Guid ConferenceId { get; private set; }

        public IEnumerable<SeatQuantity> Seats { get; private set; }

        /// <summary>
        /// The expected expiration time if the reservation is not explicitly confirmed later.
        /// </summary>
        public DateTime ReservationAutoExpiration { get; private set; }

        public string AccessCode { get; private set; }
    }
}
