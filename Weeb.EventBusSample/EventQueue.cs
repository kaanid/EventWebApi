using System;
using System.Collections.Generic;
using System.Text;
using Weeb.Event;

namespace Weeb.EventBusSample
{
    internal sealed class EventQueue
    {
        public event EventHandler<EventProcessedEventArgs> EventPushed;

        public void Push(IEvent @event)
        {
            OnMessagePushed(new EventProcessedEventArgs(@event));
        }
        private void OnMessagePushed(EventProcessedEventArgs e) => this.EventPushed?.Invoke(this,e);
    }
}
