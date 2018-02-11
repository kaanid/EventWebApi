using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Weeb.Event;

namespace Weeb.EventHanderContent
{
    public static class Extensions
    {
        public static void AddEventHandlerContext(this IServiceCollection services)
        {
            var eventHanderExecutionContext = new EventHandlerExecutionContext(services, sc => sc.BuildServiceProvider());
            services.AddSingleton<IEventHandlerExecutionContext>(eventHanderExecutionContext);
        }
    }
}
