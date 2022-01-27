using Hive.SeedWorks.Characteristics;
using System;
using Hive.SeedWorks.Events;

namespace Chat.Domain.Tests
{
    public static class CommandToAggregateExtension
    {
        public static ICommandToAggregate CreateCommandMetadata(this string commandName)
            => CommandToAggregate.Commit(
                Guid.NewGuid(),
                commandName,
                "test",
                DateTimeOffset.UtcNow.ToUnixTimeMilliseconds());
    }
}
