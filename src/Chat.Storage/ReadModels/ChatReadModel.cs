using System;

namespace Chat.Storage.ReadModels
{
    public class ChatReadModel
    {
        public Guid Id { get; set; }

        public long Version { get; set; }

        public string Status { get; set; } = "Active";

        public string SubscriberName { get; set; } = string.Empty;

        public string OperatorName { get; set; } = string.Empty;

        public int MessageCount { get; set; }

        public string LastCommandName { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
