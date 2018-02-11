using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Weeb.Event;

namespace Weeb.RabbitMQEventBus
{
    public class RabbitMQEventBus : BaseEventBus
    {
        private readonly IConnectionFactory connectionFactory;
        private readonly IConnection connection;
        private readonly IModel channel;
        private readonly string exchangeName;
        private readonly string exchangeType;
        private readonly string queueName;
        private readonly bool autoAck;
        private readonly ILogger logger;

        public RabbitMQEventBus(IEventHandlerExecutionContext _eventHandlerExecutionContext,
            ILogger<RabbitMQEventBus> _logger,
            IConnectionFactory _connectionFactory,
            string _exchangeName,
            string _exchangeType = ExchangeType.Fanout,
            string _queueName=null,
            bool _autoAck=false
            )
            :base(_eventHandlerExecutionContext)
        {
            connectionFactory = _connectionFactory;
            logger = _logger;
            connection = connectionFactory.CreateConnection();
            channel = connection.CreateModel();
            exchangeType = _exchangeType;
            exchangeName = _exchangeName;
            autoAck = _autoAck;

            channel.ExchangeDeclare(exchangeName, exchangeType);
            queueName = InitializeEventConsumer(_queueName);

            logger.LogInformation($"RabbitMQEventBus 构造函数调用完成* Hash Code:{this.GetHashCode()}.");
        }

        private string InitializeEventConsumer(string queue)
        {
            var localQueueName = queue;
            if(string.IsNullOrWhiteSpace(localQueueName))
            {
                localQueueName = channel.QueueDeclare().QueueName;
            }
            else
            {
                channel.QueueDeclare(localQueueName, true, false, false, null);
            }

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += async (model, eventArgument) =>
            {
                var eventBody = eventArgument.Body;
                var json = Encoding.UTF8.GetString(eventBody);

                var @event = (IEvent)JsonConvert.DeserializeObject(json, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All });
                await eventHandlerExecutionContext.HandleEventAsync(@event);
                if(!autoAck)
                {
                    //是否确认
                    channel.BasicAck(eventArgument.DeliveryTag, false);
                }
            };

            channel.BasicConsume(localQueueName, autoAck: autoAck, consumer: consumer);

            return localQueueName;
        }
        

        public override Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default(CancellationToken))
        {
            var json = JsonConvert.SerializeObject(@event, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All });
            var eventBody = Encoding.UTF8.GetBytes(json);
            channel.BasicPublish(exchangeName, @event.GetType().FullName, null, eventBody);

            return Task.CompletedTask;
        }

        public override void Subscribe<TEvent, TEventHandler>()
        {
            if(!eventHandlerExecutionContext.HandlerRegistered<TEvent,TEventHandler>())
            {
                eventHandlerExecutionContext.RegisterHandler<TEvent, TEventHandler>();
                channel.QueueBind(queueName, exchangeName, typeof(TEvent).FullName);
            }
        }

        private bool disposed = false;
        protected override void Dispose(bool disposing)
        {
            if(!disposed)
            {
                if(disposing)
                {
                    channel.Dispose();
                    connection.Dispose();
                    logger.LogInformation($"RabbitMQEventBus 已经被Dispose.Hash Code:{this.GetHashCode()}");
                }
                disposed = true;
                base.Dispose(disposing);
            }
        }
    }
}
