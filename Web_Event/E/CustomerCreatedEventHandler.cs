using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Weeb.Event;

namespace Web_Event.E
{
    public class CustomerCreatedEventHandler : IEventHandler<CustomerCreatedEvent>
    {
        private readonly IEventStore _eventStore;
        private readonly ILogger<CustomerCreatedEventHandler> _log;
        public CustomerCreatedEventHandler(IEventStore eventStore, ILogger<CustomerCreatedEventHandler> log)
        {
            _eventStore = eventStore;
            _log = log;

            _log.LogInformation("CustomerCreatedEventHandler 构造完成!");
        }
        public bool CanHandle(IEvent @event)
        {
            return @event.GetType().Equals(typeof(CustomerCreatedEvent));
        }

        public Task<bool> HandleAsync(CustomerCreatedEvent @event, CancellationToken cancellationToken = default)
        {
            _eventStore.SaveEventAsync(@event);
            return Task.FromResult(true);
        }

        public Task<bool> HandleAsync(IEvent @event, CancellationToken cancellationToken = default)
        {
            return CanHandle(@event) ? HandleAsync((CustomerCreatedEvent)@event, cancellationToken) : Task.FromResult(false);
        }
    }
}
