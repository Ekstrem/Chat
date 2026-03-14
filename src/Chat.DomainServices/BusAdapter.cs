using Chat.Domain;
using Chat.Domain.Abstraction;
using DigiTFactory.Libraries.SeedWorks.Events;
using DigiTFactory.Libraries.SeedWorks.Result;
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
            _logger.LogInformation("Поток доменных событий завершён.");
        }

        public void OnError(Exception error)
        {
            _logger.LogError(error, "Ошибка в потоке доменных событий.");
        }

        public void OnNext(AggregateResult<IChat, IChatAnemicModel> value)
        {
            var message = JsonConvert.SerializeObject(value.Event.ChangedValueObjects);
            _logger.LogInformation($"DE! Command version:{value.Event.Command.Version}; vos: {message}");

            _eventBus.Publish(value.Event);
        }
    }
}
