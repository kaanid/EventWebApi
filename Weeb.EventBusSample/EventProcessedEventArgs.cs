using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Weeb.Event;

namespace Weeb.EventBusSample
{
    public class EventProcessedEventArgs:EventArgs
    {
        public EventProcessedEventArgs(IEvent @event)
        {
            Event = @event;
        }
        public IEvent Event { set; get; }
    }
}
