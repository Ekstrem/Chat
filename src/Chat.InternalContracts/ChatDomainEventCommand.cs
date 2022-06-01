using Chat.Domain;
using Hive.SeedWorks.Events;

namespace Chat.InternalContracts
{
    public class ChatDomainEventCommand : BusDomainEventCommand<IChat>
    {
        public ChatDomainEventCommand(IDomainEvent<IChat> domainEvent)
            : base(domainEvent)
        {
        }
    }
}
