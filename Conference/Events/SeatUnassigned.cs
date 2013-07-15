using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ENode.Eventing;

namespace Conference.Events
{
    [Serializable]
    public class SeatUnassigned:Event
    {
        public SeatUnassigned(int position)
        {
            this.Position = position;
        }

        public int Position { get; private set; }
    }
}
