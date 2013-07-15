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
    //订单聚合根
    [Serializable]
    public class Order : AggregateRoot<Guid>,
        IEventHandler<OrderPlaced>,
        IEventHandler<OrderPartiallyReserved>,
        IEventHandler<OrderReservationCompleted>
    {
        private static readonly TimeSpan ReservationAutoExpiration = TimeSpan.FromMinutes(15);


        private List<SeatQuantity> seats;
        private Guid conferenceId;
        private bool isConfirmed ;

        public Order():base(){}

        public Order(Guid id, Guid conferenceId, IEnumerable<OrderItem> items):base(id)
        {
            //产生一个OrderPlaced事件
            RaiseEvent(new OrderPlaced(conferenceId, ConvertItems(items), DateTime.UtcNow.Add(ReservationAutoExpiration),
                                       HandleGenerator.Generate(6)));
            //计算价格事件

        }

        public void MarkAsReserved(DateTime expirationDate, IEnumerable<SeatQuantity> reservedSeats)
        {
            if (this.isConfirmed)
                throw new InvalidOperationException("Cannot modify a confirmed order.");

            var reserved = reservedSeats.ToList();

            if (this.seats.Any(item => item.Quantity != 0 && !reserved.Any(seat => seat.SeatType == item.SeatType && seat.Quantity == item.Quantity)))
            {
                //部分成交
                RaiseEvent(new OrderPartiallyReserved (expirationDate,  reserved.ToArray() ));
            }
            else
            {
                RaiseEvent(new OrderReservationCompleted (expirationDate, reserved.ToArray() ));
            }
        }

        public void Confirm(Guid conferenceId,Guid orderId)
        {
            RaiseEvent(new OrderConfirmed(conferenceId,orderId,this.seats.AsReadOnly()));
        }


        private static List<SeatQuantity> ConvertItems(IEnumerable<OrderItem> items)
        {
            return items.Select(x => new SeatQuantity(x.SeatType, x.Quantity)).ToList();
        }

        public void Handle(OrderPlaced evnt)
        {
            this.conferenceId = evnt.ConferenceId;
            this.seats = evnt.Seats.ToList();
        }

        public void Handle(OrderPartiallyReserved e)
        {
            this.seats = e.Seats.ToList();
        }

        public void Handle(OrderReservationCompleted e)
        {
            this.seats = e.Seats.ToList();
        }

        public void Handler(OrderConfirmed e)
        {
            this.isConfirmed = true;
        }

       

    }
}
