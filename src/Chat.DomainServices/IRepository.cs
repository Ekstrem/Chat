using Chat.Domain;
using Chat.Domain.Abstraction;
using DigiTFactory.Libraries.SeedWorks.TacticalPatterns;

namespace Chat.DomainServices
{
    public interface IRepository : IAnemicModelRepository<IChat, IChatAnemicModel>
    {
    }
}
