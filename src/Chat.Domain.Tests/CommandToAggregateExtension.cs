using DigiTFactory.Libraries.SeedWorks.Characteristics;
using System;
using DigiTFactory.Libraries.SeedWorks.Events;

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
