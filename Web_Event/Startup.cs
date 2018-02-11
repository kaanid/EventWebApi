using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NLog;
using NLog.Extensions.Logging;
using Web_Event.E;
using Weeb.Event;
using Weeb.EventBusSample;
using Weeb.EventDapperStore;
using Weeb.EventHanderContent;
using Weeb.RabbitMQEventBus;

namespace Web_Event
{
    public class Startup
    {
        private readonly Microsoft.Extensions.Logging.ILogger logger;
        private const string RMQ_HOSTNAME = "localhost";
        private const string RMQ_EXCHANGE = "EdaSample.Exchange";
        private const string RMQ_QUEUE = "EdaSample.Queue";

        public Startup(IConfiguration configuration,ILoggerFactory loggerFactory)
        {
            Configuration = configuration;
            logger = loggerFactory.CreateLogger<Startup>();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            logger.LogInformation($"正在对服务进行配置...");
            services.AddMvc();
            services.AddLogging(logbuilder=> {
                logbuilder
                    .AddConfiguration(Configuration.GetSection("Logging"))
                    .AddConsole()
                    .AddDebug()
                    .AddEventSourceLogger();
            });

            //services.AddTransient<IEventHandler, CustomerCreatedEventHandler>();
            //var eventHanlerExecutionContext = new EventHandlerExecutionContext(services,sc=>sc.BuildServiceProvider());
            //IEventHandlerExecutionContext

            services.AddEventHandlerContext();

            //services.AddEventBus();
            services.AddRabbitMQEventBus(RMQ_HOSTNAME, RMQ_EXCHANGE, RMQ_QUEUE);

            string connString=Configuration["Mysql:connectionString"];
            services.AddEventStore(connString);

            logger.LogInformation($"服务配置完成，已注册到Ioc容器！");


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env,ILoggerFactory loggerFactory)
        {
            var eventBus = app.ApplicationServices.GetRequiredService<IEventBus>();
            eventBus.Subscribe<CustomerCreatedEvent,CustomerCreatedEventHandler>();


            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            loggerFactory.AddNLog();



            app.UseMvc();

            logger.LogInformation($"正在对应用进行配置Configure...");
        }
    }
}
