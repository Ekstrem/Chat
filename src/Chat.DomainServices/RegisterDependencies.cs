using System;
using Autofac;
using Chat.Domain;
using Chat.Domain.Abstraction;
using Hive.SeedWorks;
using Hive.SeedWorks.Monads;
using Hive.SeedWorks.Result;
using Hive.SeedWorks.TacticalPatterns;

namespace Chat.DomainServices
{
    public class RegisterDependencies: Module
    {
        protected override void Load(ContainerBuilder builder)
            => RegisterDomainServices(builder);

        private static ContainerBuilder RegisterDomainServices(ContainerBuilder builder)
            => builder
                .Do(b => b
                    .RegisterType<AggregateProvider>()
                    .As<IAggregateProvider<IChat, IChatAnemicModel>>()
                    .As<IChatAggregateProvider>())
                .Do(b => b
                    .RegisterType<BusAdapter>()
                    .As<IObserver<AggregateResult<IChat, IChatAggregate>>>());
        //.Do(b =>b
        //        .RegisterType<DomainCommandExecutor>()
        //        .As<IDomainCommandExecutor<IChat, IChatAggregate>>());
    }
}
