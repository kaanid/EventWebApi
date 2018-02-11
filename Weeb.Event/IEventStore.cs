using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Weeb.Event
{
    public interface IEventStore: IDisposable
    {
        Task SaveEventAsync<TEvent>(TEvent @event) where TEvent : IEvent;
    }
}
