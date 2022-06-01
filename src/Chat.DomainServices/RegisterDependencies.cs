using System;
using Autofac;
using Chat.Domain;
using Chat.Domain.Abstraction;
using Hive.SeedWorks.Monads;
using Hive.SeedWorks.Result;

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
                    .As<IChatAggregateProvider>())
                .Do(b => b
                    .RegisterType<BusAdapter>()
                    .As<IObserver<AggregateResult<IChat, IChatAnemicModel>>>());
    }
}
