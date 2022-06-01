using Chat.Domain;
using Chat.Domain.Abstraction;
using Hive.SeedWorks.TacticalPatterns;

namespace Chat.DomainServices
{
    public interface IChatAggregateProvider : IAggregateProvider<IChat, IChatAggregate> 
    {
        IChatAggregate CreateAggregate();
    }
}
