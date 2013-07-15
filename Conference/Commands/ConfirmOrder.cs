using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ENode.Commanding;

namespace Conference.Commands
{
    [Serializable]
    public class ConfirmOrder:Command
    {
        public ConfirmOrder(Guid conferenceId,Guid orderId)
        {
            this.ConferenceId = conferenceId;
            this.OrderId = orderId;
        }

        public Guid ConferenceId { get; private set; }

        public Guid OrderId { get; private set; }
    }
}
