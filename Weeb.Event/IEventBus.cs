using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Weeb.Event
{
    public interface IEventBus:IEventPublisher,IEventSubscriber
    {
    }
}
