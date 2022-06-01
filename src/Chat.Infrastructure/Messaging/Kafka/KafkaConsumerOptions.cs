namespace Chat.Infrastructure.Messaging.Kafka
{
    public class KafkaConsumerOptions
    {
        public string[] Topics { get; set; } = Array.Empty<string>();
        public Dictionary<string, string> Config { get; set; } = new();
    }
}