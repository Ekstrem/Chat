using Chat.Api.BackgroundServices.EventBusConsumer;
using Chat.Api.Middlewares.Correlation;
using Chat.Infrastructure.Messaging.Kafka;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;

namespace Chat.Api
{
    public partial class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(x => 
            {
                x.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo 
                {
                    Version = "v1",
                    Title = "Chat v1 API",
                    Description = ""
                });
            });

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IExecutionContextAccessor, ExecutionContextAccessor>();

            services.Configure<KafkaConsumerOptions>(o =>
            {
                o.Config = new Dictionary<string, string>
                    {
                        {"bootstrap.servers", Configuration.GetSection("kafka:brokerList").Get<string>()}
                    }
                    .Concat(
                        Configuration.GetSection("kafka:consumers:readModel").Get<Dictionary<string, string>>())
                    .ToDictionary(x => x.Key, x => x.Value);
                o.Topics = new[] { Configuration.GetSection("kafka:topic").Get<string>() };
            });
            services.AddHostedService<EventBusConsumerService>();
            /*
            services.AddSingleton<EventMessageFactory>();
            services.AddSingleton<EventBusMessageHandler>();
            services.AddScoped<IEventBusConsumer, EventBusConsumer>();
            */

            services.AddControllers(options => { })
                .AddControllersAsServices();
        }
    }
}
