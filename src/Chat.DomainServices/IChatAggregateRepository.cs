using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chat.Domain;
using Chat.Domain.Abstraction;
using Hive.SeedWorks.TacticalPatterns;

namespace Chat.DomainServices
{
    public interface IChatAggregateRepository : IAnemicModelRepository<IChat, IChatAnemicModel> { }
}
