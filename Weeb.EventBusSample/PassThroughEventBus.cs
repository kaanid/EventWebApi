using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Weeb.Event;

namespace Weeb.EventBusSample
{
    public class PassThroughEventBus:BaseEventBus
    {
        private readonly EventQueue eventQueue = new EventQueue();
       // private readonly IEnumerable<IEventHandler> _eventHandlers;
        private readonly ILogger<PassThroughEventBus> _log;

        private readonly IEventHandlerExecutionContext _context;

        public PassThroughEventBus(IEventHandlerExecutionContext context, ILogger<PassThroughEventBus> log)
            :base(context)
        {
            //_eventHandlers = eventHandlers;
            _context = context;
            _log = log;

            eventQueue.EventPushed += EventQueue_EventPushed;
        }

        private async void EventQueue_EventPushed(object sender,EventProcessedEventArgs e)
        {
            //(from eh in this._eventHandlers
            // where eh.CanHandle(e.Event)
            // select eh).ToList().ForEach(async eh => await eh.HandleAsync(e.Event));
            await _context.HandleEventAsync(e.Event);
        }

        private bool disposedValue = false;

        protected override void Dispose(bool disposing)
        {
            if(!disposedValue)
            {
                if(disposing)
                {
                    this.eventQueue.EventPushed -= EventQueue_EventPushed;
                    _log.LogInformation($"PassThroughEventBus已经被Dispose。Hash Code:{this.GetHashCode()}.");
                }
            }
            disposedValue = true;
            _log.LogInformation("PassThroughEventBus Dispose");
        }
        public override Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default) 
        {
            _log.LogInformation($"PassThroughEventBus PublishAsync {@event.Id}");
            return Task.Factory.StartNew(() => eventQueue.Push(@event));
        }

        public override void Subscribe<TEvent,TEventHandler>()
        {
            _log.LogInformation($"PassThroughEventBus Subscribe");
            //eventQueue.EventPushed += EventQueue_EventPushed;

            if(!_context.HandlerRegistered<TEvent,TEventHandler>())
            {
                _context.RegisterHandler<TEvent, TEventHandler>();
            }
        }
    }
}
