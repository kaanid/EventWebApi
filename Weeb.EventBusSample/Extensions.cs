using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using Weeb.Event;

namespace Weeb.EventBusSample
{
    public static class Extensions
    {
        public static void AddEventBus(this IServiceCollection services)
        {
            services.AddSingleton<IEventBus, PassThroughEventBus>();
        }
    }
}
