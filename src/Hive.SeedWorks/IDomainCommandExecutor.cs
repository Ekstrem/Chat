using Hive.SeedWorks.Definition;
using Hive.SeedWorks.Result;
using Hive.SeedWorks.TacticalPatterns;

namespace Hive.SeedWorks
{
    public interface IDomainCommandExecutor<TBoundedContext, TAggregate>
        where TBoundedContext: IBoundedContext
        where TAggregate: IAggregate<TBoundedContext>
    {
        AggregateResult<TBoundedContext, TAggregate> Handle(DomainOperationResultEnum command);
    }
}
