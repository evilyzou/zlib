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
        IEventHandler<OrderPlaced>

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


        private static List<SeatQuantity> ConvertItems(IEnumerable<OrderItem> items)
        {
            return items.Select(x => new SeatQuantity(x.SeatType, x.Quantity)).ToList();
        }

        public void Handle(OrderPlaced evnt)
        {
            this.conferenceId = evnt.ConferenceId;
            this.seats = evnt.Seats.ToList();
        }
    }
}
