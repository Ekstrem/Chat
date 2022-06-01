using Confluent.Kafka;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Polly;

namespace Chat.Infrastructure.Messaging.Kafka
{
    public class KafkaConsumerBackgroundService<TKey, TValue> : BackgroundService
    {
        private readonly IOptionsMonitor<KafkaConsumerOptions> _optionsMonitor;
        private readonly IKafkaConsumerFactory<TKey, TValue> _consumerFactory;
        private readonly IKafkaMessageHandler<TKey, TValue> _handler;

        public KafkaConsumerBackgroundService(IOptionsMonitor<KafkaConsumerOptions> optionsMonitor,
            IKafkaConsumerFactory<TKey, TValue> consumerFactory
            , IKafkaMessageHandler<TKey, TValue> handler)
        {
            _optionsMonitor = optionsMonitor;
            _consumerFactory = consumerFactory;
            _handler = handler;
        }

        protected override Task ExecuteAsync(CancellationToken cancellationToken)
        {
            return Task.Run(() => GetOverallExecutionPolicy()
                .ExecuteAsync(async ct => await Consume(ct), cancellationToken), cancellationToken);
        }

        protected virtual IAsyncPolicy GetOverallExecutionPolicy()
        {
            var policy = Policy.Handle<Exception>()
                .WaitAndRetryForeverAsync(
                    failedAttemptsCount =>
                        TimeSpan.FromSeconds(Math.Pow(2, failedAttemptsCount <= 6 ? failedAttemptsCount : 6)),
                    (e, failedAttemptsCount, wait) =>
                    {
                        // TODO: logging
                        Console.WriteLine(e);
                        Console.WriteLine(
                            $"Failure {failedAttemptsCount}. Retry in {wait.TotalSeconds} sec.");
                    });


            return policy;
        }

        private async Task Consume(CancellationToken cancellationToken)
        {
            var options = _optionsMonitor.Get(Options.DefaultName);

            var config = new ConsumerConfig(options.Config);
            using var consumer = CreateConsumer(config);
            {
                consumer.Subscribe(options.Topics);

                while (!cancellationToken.IsCancellationRequested)
                {
                    var cr = consumer.Consume(cancellationToken);

                   await _handler.Handle(cr.Message, cancellationToken);

                    if (config.EnableAutoOffsetStore is false)
                        consumer.StoreOffset(cr);
                }
            }
        }

        private IConsumer<TKey, TValue> CreateConsumer(ConsumerConfig config)
        {
            return _consumerFactory.Create(config);
        }
    }
}