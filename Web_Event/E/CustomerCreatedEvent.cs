using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Weeb.Event;

namespace Web_Event.E
{
    public class CustomerCreatedEvent:IEvent
    {
        public CustomerCreatedEvent(string customerName)
        {
            Id = Guid.NewGuid();
            TimeStamp = DateTime.UtcNow;
            CustomerName = customerName;
        }

        public Guid Id { get; }
        public DateTime TimeStamp { get; }

        public string CustomerName { get; }
    }
}
