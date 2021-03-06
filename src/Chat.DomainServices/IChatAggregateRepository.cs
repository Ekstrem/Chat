using Chat.Domain;
using Chat.Domain.Abstraction;
using Hive.SeedWorks.TacticalPatterns;

namespace Chat.DomainServices
{
    public interface IChatAggregateRepository : IAnemicModelRepository<IChat, IChatAnemicModel> { }
}
