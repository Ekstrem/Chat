using Confluent.Kafka;
using Microsoft.Extensions.Logging;

namespace Chat.Infrastructure.Messaging.Kafka
{
    public class KafkaConsumerFactory<TKey, TValue> : IKafkaConsumerFactory<TKey, TValue>
    {
        private readonly ILogger<IConsumer<TKey, TValue>> _logger;

        public KafkaConsumerFactory(ILogger<IConsumer<TKey, TValue>> logger) 
            => _logger = logger;

        public IConsumer<TKey, TValue> Create(ConsumerConfig config)
            => new ConsumerBuilder<TKey, TValue>(config)
                .SetLogHandler((_, logMessage) =>
                {
                    var logLevel = MapLogLevel(logMessage);
                    _logger.Log(logLevel, $"[{logMessage.Name}, {logMessage.Facility}]{logMessage.Message}");
                })
                .Build();

        private static LogLevel MapLogLevel(LogMessage logMessage)
            => logMessage.Level switch
            {
                SyslogLevel.Emergency or SyslogLevel.Alert or SyslogLevel.Critical => LogLevel.Critical,
                SyslogLevel.Error => LogLevel.Error,
                SyslogLevel.Warning => LogLevel.Warning,
                SyslogLevel.Notice or SyslogLevel.Info => LogLevel.Information,
                SyslogLevel.Debug => LogLevel.Debug,
                _ => LogLevel.Trace,
            };
    }
}