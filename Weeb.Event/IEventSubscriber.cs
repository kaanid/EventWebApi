using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Weeb.Event
{
    public interface IEventSubscriber:IDisposable
    {
        void Subscribe<TEvent,TEventHandler>()
            where TEvent:IEvent
            where TEventHandler:IEventHandler<TEvent>;
    }
}
