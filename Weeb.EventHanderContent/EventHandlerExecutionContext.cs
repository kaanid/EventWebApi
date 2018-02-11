using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Weeb.Event;

namespace Weeb.EventHanderContent
{
    public class EventHandlerExecutionContext: IEventHandlerExecutionContext
    {
        private readonly IServiceCollection _registry;
        private readonly Func<IServiceCollection, IServiceProvider> _serviceProviderFactory;

        private readonly ConcurrentDictionary<Type, List<Type>> _registrations = new ConcurrentDictionary<Type, List<Type>>();

        public EventHandlerExecutionContext(IServiceCollection registry, Func<IServiceCollection, IServiceProvider> serviceProviderFactory = null)
        {
            _registry = registry;
            _serviceProviderFactory = serviceProviderFactory;
        }

        public async Task HandleEventAsync(IEvent @event, CancellationToken cancellationToken = default(CancellationToken))
        {
            var eventType = @event.GetType();
            if(_registrations.TryGetValue(eventType,out List<Type> handlerTypes)&&handlerTypes?.Count>0)
            {
                var serviceProvider = _serviceProviderFactory(_registry);
                using (var childScope = serviceProvider.CreateScope())
                {
                    foreach (var handlerType in handlerTypes)
                    {
                        var handler = (IEventHandler)childScope.ServiceProvider.GetService(handlerType);
                        if(handler.CanHandle(@event))
                        {
                            await handler.HandleAsync(@event, cancellationToken);
                        }
                    }
                }
            }
        }

        public bool HandlerRegistered<TEvent, THandler>()
            where TEvent : IEvent
            where THandler : IEventHandler<TEvent>
        {
            return HandlerRegistered(typeof(TEvent), typeof(THandler));
        }

        public bool HandlerRegistered(Type eventType, Type handlerType)
        {
           if(_registrations.TryGetValue(eventType,out List<Type> handlerTypeList))
            {
                return handlerTypeList != null && handlerTypeList.Contains(handlerType);
            }
            return false;
        }

        public void RegisterHandler<TEvent, THandler>()
            where TEvent : IEvent
            where THandler : IEventHandler<TEvent>
        {
            RegisterHandler(typeof(TEvent),typeof(THandler));
        }

        public void RegisterHandler(Type eventType, Type handlerType)
        {
            if(_registrations.TryGetValue(eventType,out List<Type> list))
            {
                if (list != null)
                {
                    if(!list.Contains(eventType))
                    {
                        _registrations[eventType].Add(handlerType);
                    }
                    else
                    {
                        _registrations[eventType] = new List<Type> { handlerType};
                    }
                }
            }
            else
            {
                _registrations.TryAdd(eventType, new List<Type> { handlerType });
            }

            _registry.AddTransient(handlerType);
        }
    }
}
