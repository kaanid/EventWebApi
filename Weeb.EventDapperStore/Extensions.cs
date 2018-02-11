using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using Weeb.Event;

namespace Weeb.EventDapperStore
{
    public static class Extensions
    {
        public static void AddEventStore(this IServiceCollection services,string connectionString)
        {
            services.AddTransient<IEventStore>(serviceProvider=>new DapperEventStore(connectionString, serviceProvider.GetService<ILogger<DapperEventStore>>()));
        }
    }
}
