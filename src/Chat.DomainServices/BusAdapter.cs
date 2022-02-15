using Chat.Domain;
using Chat.Domain.Abstraction;
using Hive.SeedWorks.Events;
using Hive.SeedWorks.Result;
using Microsoft.Extensions.Logging;
using System;
using Newtonsoft.Json;

namespace Chat.DomainServices
{
    internal class BusAdapter : IObserver<AggregateResult<IChat, IChatAnemicModel>>
    {
        private readonly IEventBus _eventBus;
        private readonly ILogger<BusAdapter> _logger;

        public BusAdapter(
            IEventBus eventBus,
            ILogger<BusAdapter> logger)
        {
            _eventBus = eventBus;
            _logger = logger;   
        }

        public void OnCompleted()
        {
            throw new NotImplementedException();
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public void OnNext(AggregateResult<IChat, IChatAnemicModel> value)
        {
            var message = JsonConvert.SerializeObject(value.Event.ChangedValueObjects);
            _logger.LogInformation($"DE! Command version:{value.Event.Command.Version}; vos: {message}");

            _eventBus.Publish(value.Event);
        }
    }
}
