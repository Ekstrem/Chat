using Chat.Domain;
using Chat.Domain.Abstraction;
using DigiTFactory.Libraries.SeedWorks.TacticalPatterns;

namespace Chat.DomainServices
{
    public interface IChatAggregateRepository : IAnemicModelRepository<IChat, IChatAnemicModel> { }
}
