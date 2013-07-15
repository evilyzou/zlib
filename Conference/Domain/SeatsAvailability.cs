using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Conference.Entity;
using Conference.Events;
using ENode.Domain;
using ENode.Eventing;

namespace Conference.Domain
{
    //座位聚合跟
    [Serializable]
    public class SeatsAvailability:AggregateRoot<Guid>,
        IEventHandler<AvailableSeatsChanged>,
        IEventHandler<SeatsReserved>,
        IEventHandler<SeatsReservationCommitted>
    {
        private readonly Dictionary<Guid, int> remainingSeats = new Dictionary<Guid, int>();
        private readonly Dictionary<Guid, List<SeatQuantity>> pendingReservations = new Dictionary<Guid, List<SeatQuantity>>();


        public SeatsAvailability(Guid conferenceId):base(conferenceId)
        {
            
        }

        public void AddSeats(Guid seatType, int quantity)
        {
            RaiseEvent(new AvailableSeatsChanged(new[] { new SeatQuantity(seatType, quantity) } ));
        }

        public void MakeReservation(Guid reservationId, IEnumerable<SeatQuantity> wantedSeats)
        {
            var wantedList = wantedSeats.ToList();
            if (wantedList.Any(x => !this.remainingSeats.ContainsKey(x.SeatType)))
            {
                throw new ArgumentOutOfRangeException("wantedSeats");
            }

            var difference = new Dictionary<Guid, SeatDifference>();

            foreach (var w in wantedList)
            {
                var item = GetOrAdd(difference, w.SeatType);
                item.Wanted = w.Quantity;
                item.Remaining = this.remainingSeats[w.SeatType];
            }

            List<SeatQuantity> existing;
            if (this.pendingReservations.TryGetValue(reservationId, out existing))
            {
                foreach (var e in existing)
                {
                    GetOrAdd(difference, e.SeatType).Existing = e.Quantity;
                }
            }

            var reservation = new SeatsReserved(reservationId,
                                                difference.Select(x => new SeatQuantity(x.Key, x.Value.Actual)).Where(
                                                    x => x.Quantity != 0).ToList(),
                                                difference.Select(x => new SeatQuantity(x.Key, -x.Value.DeltaSinceLast))
                                                    .Where(x => x.Quantity != 0).
                                                    ToList()
                );
            RaiseEvent(reservation);

        }

        public void CommitReservation(Guid reservationId)
        {
            if (this.pendingReservations.ContainsKey(reservationId))
            {
               RaiseEvent(new SeatsReservationCommitted(reservationId));
            }
        }

        public void Handle(AvailableSeatsChanged evnt)
        {
            foreach (var seat in evnt.Seats)
            {
                int newValue = seat.Quantity;
                int remaining;
                if (this.remainingSeats.TryGetValue(seat.SeatType, out remaining))
                {
                    newValue += remaining;
                }

                this.remainingSeats[seat.SeatType] = newValue;
            }
        }

        public void Handle(SeatsReserved evnt)
        {
            var details = evnt.ReservationDetails.ToList();
            if (details.Count > 0)
            {
                this.pendingReservations[evnt.ReservationId] = details;
            }
            else
            {
                this.pendingReservations.Remove(evnt.ReservationId);
            }

            foreach (var seat in evnt.AvailableSeatsChanged)
            {
                this.remainingSeats[seat.SeatType] = this.remainingSeats[seat.SeatType] + seat.Quantity;
            }
        }

        public void Handle(SeatsReservationCommitted e)
        {
            this.pendingReservations.Remove(e.ReservationId);
        }

        private class SeatDifference
        {
            public int Wanted { get; set; }
            public int Existing { get; set; }
            public int Remaining { get; set; }
            public int Actual
            {
                get { return Math.Min(this.Wanted, Math.Max(this.Remaining, 0) + this.Existing); }
            }
            public int DeltaSinceLast
            {
                get { return this.Actual - this.Existing; }
            }
        }

        private static TValue GetOrAdd<TKey, TValue>(Dictionary<TKey, TValue> dictionary, TKey key) where TValue : new()
        {
            TValue value;
            if (!dictionary.TryGetValue(key, out value))
            {
                value = new TValue();
                dictionary[key] = value;
            }

            return value;
        }

       
    }
}
