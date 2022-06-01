using Chat.Domain;
using Chat.Domain.Abstraction;
using Hive.SeedWorks.Result;
using Microsoft.Extensions.Logging;
using System;
using Newtonsoft.Json;
using Chat.Infrastructure.Messaging.Kafka;

namespace Chat.DomainServices
{
    internal class BusAdapter : IObserver<AggregateResult<IChat, IChatAnemicModel>>
    {
        private readonly IExternalEventProducer _producer;
        private readonly ILogger<BusAdapter> _logger;

        public BusAdapter(
            IExternalEventProducer producer,
            ILogger<BusAdapter> logger)
        {
            _producer = producer;
            _logger = logger;   
        }

        public void OnCompleted() 
            => throw new NotImplementedException();

        public void OnError(Exception error) 
            => throw new NotImplementedException();

        public void OnNext(AggregateResult<IChat, IChatAnemicModel> value)
        {
            var message = JsonConvert.SerializeObject(value.ChangeValueObjects);
            _logger.LogInformation($"DE! Command version:{value.Event.Command.Version}; vos: {message}");

            _producer.Publish(value.Event);
        }
    }
}
