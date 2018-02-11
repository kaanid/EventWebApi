using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;
using Weeb.Event;

namespace Weeb.RabbitMQEventBus
{
    public static class Extensions
    {
        public static void AddRabbitMQEventBus(this IServiceCollection services,string hostName,string rmq_Exchange,string rmq_queue)
        {
            var connectionFactory = new ConnectionFactory { HostName = hostName };
            services.AddSingleton<IEventBus>(sp=>new RabbitMQEventBus(
                sp.GetRequiredService<IEventHandlerExecutionContext>(),
                sp.GetRequiredService<ILogger<RabbitMQEventBus>>(),
                connectionFactory,
                rmq_Exchange,
                _queueName:rmq_queue
                ));
        }
    }
}
